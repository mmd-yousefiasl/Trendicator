/* ─────────────── Shared: flash message that survives reload ─────────────── */
/* Exposed on window so it's safe even if this partial is rendered twice.     */
(() => {
    if (window.setPageFlash) return;

    window.PAGE_FLASH_KEY = 'cart-flash';

    window.setPageFlash = function (type, msg) {
        if (!msg) return;
        try {
            sessionStorage.setItem(window.PAGE_FLASH_KEY, JSON.stringify({
                type: type || 'success',
                msg,
                at: Date.now()
            }));
        } catch { /* private mode / quota — silently skip */ }
    };
})();

/* ────────────────────── Coupon: apply / remove ────────────────────── */
(() => {
    const summary = document.querySelector('.order-summary');
    if (!summary) return;

    // ── Config comes from the DOM, set by Razor ──────────────────
    const applyUrl = summary.dataset.applyUrl;
    const removeUrl = summary.dataset.removeUrl;
    const csrfToken = summary.dataset.csrfToken || null;

    const postHeaders = () => {
        const h = { 'Content-Type': 'application/json' };
        if (csrfToken) h['RequestVerificationToken'] = csrfToken;
        return h;
    };

    /* ─────────────────────────── Events ─────────────────────────── */

    summary.addEventListener('click', (e) => {
        const open = e.target.closest('[data-open-coupon]');
        const apply = e.target.closest('[data-coupon-apply]');
        const remove = e.target.closest('.coupon-applied__remove');

        if (open) { e.preventDefault(); toggleForm(); return; }
        if (apply) { e.preventDefault(); applyCoupon(apply); return; }
        if (remove) { e.preventDefault(); removeCoupon(); return; }
    });

    summary.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && e.target.matches('[data-coupon-input]')) {
            e.preventDefault();
            summary.querySelector('[data-coupon-apply]')?.click();
        }
    });

    summary.addEventListener('input', (e) => {
        if (e.target.matches('[data-coupon-input]')) clearError();
    });

    /* ─────────────────────────── Helpers ─────────────────────────── */

    function toggleForm() {
        const form = summary.querySelector('[data-coupon-form]');
        if (!form) return;
        form.classList.toggle('is-open');
        if (!form.classList.contains('is-open')) clearError();
    }

    function clearError() {
        const box = summary.querySelector('[data-coupon-error]');
        const inp = summary.querySelector('[data-coupon-input]');
        if (box) box.hidden = true;
        inp?.classList.remove('is-error');
    }

    function showError(msg) {
        const form = summary.querySelector('[data-coupon-form]');
        const box = summary.querySelector('[data-coupon-error]');
        const txt = summary.querySelector('[data-coupon-error-text]');
        const inp = summary.querySelector('[data-coupon-input]');
        if (!box) return;

        form?.classList.add('is-open');
        txt.textContent = msg;
        box.hidden = false;

        if (inp) {
            inp.classList.remove('is-error');
            void inp.offsetWidth;
            inp.classList.add('is-error');
            inp.focus();
        }
    }

    function setLoading(on) {
        summary.classList.toggle('is-loading', on);
    }

    /* ─────────────────────────── Actions ─────────────────────────── */

    async function applyCoupon(btn) {
        const input = summary.querySelector('[data-coupon-input]');
        const code = input?.value.trim();

        if (!code) { showError('لطفاً کد تخفیف را وارد کنید.'); return; }

        clearError();
        btn.disabled = true;
        setLoading(true);

        try {
            const res = await fetch(applyUrl, {
                method: 'POST',
                headers: postHeaders(),
                body: JSON.stringify({ code })
            });
            const data = await res.json().catch(() => ({}));

            if (!res.ok || !data.success) {
                setLoading(false);
                btn.disabled = false;
                showError(data.message || 'کد تخفیف معتبر نیست یا منقضی شده است.');
                return;
            }

            // success → stash message, reload; the server re-renders the summary
            setPageFlash('success', data.message || 'کد تخفیف با موفقیت اعمال شد.');
            location.reload();

        } catch {
            setLoading(false);
            btn.disabled = false;
            showError('خطا در ارتباط با سرور. لطفاً دوباره تلاش کنید.');
        }
    }

    async function removeCoupon() {
        setLoading(true);
        try {
            const res = await fetch(removeUrl, {
                method: 'POST',
                headers: postHeaders()
            });
            const data = await res.json().catch(() => ({}));

            if (res.ok && data.success) {
                setPageFlash('success', data.message || 'کد تخفیف حذف شد.');
                location.reload();
            } else {
                setLoading(false);
            }
        } catch {
            setLoading(false);
        }
    }
})();

/* ─────────────────── Cart page: remove a single line ─────────────────── */
(() => {
    const body = document.body;
    const removeUrl = body.dataset.cartRemoveUrl;
    const csrfToken =
        body.dataset.csrfToken ||
        document.querySelector('[data-csrf-token]')?.dataset.csrfToken ||
        null;

    if (!removeUrl) {
        console.warn('[cart-remove] Missing data-cart-remove-url on <body>.');
        return;
    }

    document.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-cart-remove]');
        if (!btn || btn.disabled) return;
        e.preventDefault();

        const line = btn.closest('[data-cart-line]');
        if (!line) return;

        const productId = line.dataset.productId;
        if (!productId) return;

        // themed confirm (fallback to native if Trendicator didn't load)
        const confirmed = window.Trendicator?.confirm
            ? await Trendicator.confirm({
                title: 'حذف از سبد خرید',
                message: 'این محصول از سبد خرید شما حذف شود؟',
                okText: 'حذف',
                cancelText: 'انصراف',
                danger: true
            })
            : confirm('این محصول از سبد خرید حذف شود؟');

        if (!confirmed) return;

        btn.disabled = true;
        btn.classList.add('is-loading');

        try {
            const res = await fetch(removeUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(csrfToken ? { 'RequestVerificationToken': csrfToken } : {})
                },
                body: JSON.stringify({ productId: Number(productId) })
            });

            const data = await res.json().catch(() => ({}));

            if (!res.ok || !data.success) {
                btn.disabled = false;
                btn.classList.remove('is-loading');
                notify('error', data.message || 'خطا در حذف محصول از سبد خرید');
                return;
            }

            // stash the message — it'll be shown after the page re-renders
            setPageFlash('success', data.message);

            location.reload();
        } catch {
            btn.disabled = false;
            btn.classList.remove('is-loading');
            notify('error', 'خطا در ارتباط با سرور');
        }
    });
})();

/* ─────────────────── Flash toast (survives reload) ─────────────────── */
(() => {
    let raw;
    try { raw = sessionStorage.getItem('cart-flash'); } catch { return; }
    if (!raw) return;

    try { sessionStorage.removeItem('cart-flash'); } catch { }

    let flash;
    try { flash = JSON.parse(raw); } catch { return; }

    // discard if it's older than ~5s (user navigated away & came back)
    if (!flash?.msg || Date.now() - (flash.at || 0) > 5000) return;

    const fire = () => (window.notify || window.Trendicator?.toast || (() => { }))(
        flash.type || 'success',
        flash.msg
    );

    // Toast region lives in <body>; DOMContentLoaded is the safe point.
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', fire, { once: true });
    } else {
        // already parsed (script at bottom of body) — fire right away,
        // but give the new DOM a beat to settle so the animation reads clearly
        setTimeout(fire, 250);
    }
})();

(() => {
    const form = document.querySelector('[data-checkout-form]');
    if (!form) return;

    const overlay = document.querySelector('[data-page-overlay]');
    const btn = form.querySelector('button[type="submit"]');

    // Same fallback chain used by the remove handler
    const notify = window.notify || window.Trendicator?.toast || console.warn;

    const showOverlay = () => {
        overlay.hidden = false;
        overlay.setAttribute('aria-hidden', 'false');
        document.body.classList.add('is-overlay-open');
    };

    const hideOverlay = () => {
        overlay.hidden = true;
        overlay.setAttribute('aria-hidden', 'true');
        document.body.classList.remove('is-overlay-open');
    };

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        showOverlay();
        btn.disabled = true;

        try {
            const res = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                credentials: 'same-origin'
            });

            if (!res.ok) throw new Error('network');

            const data = await res.json();

            if (data.success && data.url) {
                // Overlay stays up — page is leaving.
                window.location.replace(data.url);
                return;
            }

            hideOverlay();
            btn.disabled = false;
            notify('error', data.message || 'خطایی رخ داد. لطفاً دوباره تلاش کنید.');

        } catch {
            hideOverlay();
            btn.disabled = false;
            notify('error', 'اتصال به سرور برقرار نشد. لطفاً دوباره تلاش کنید.');
        }
    });

    // bfcache — Back from bank restores DOM; reset everything.
    window.addEventListener('pageshow', (e) => {
        if (!e.persisted) return;
        hideOverlay();
        btn.disabled = false;
    });
})();