document.addEventListener('DOMContentLoaded', function () {
    // Listen for 'submit' on ALL forms on the page
    document.querySelectorAll('form:not([data-skip-auto-spinner])').forEach(function (form) {
        form.addEventListener('submit', function (event) {

            // Find the submit button inside this specific form
            // This handles both <button> and <input type="submit">
            const button = form.querySelector('button[type="submit"], input[type="submit"], button:not([type])');

            if (!button) return;

            // If already loading, cancel the submission!
            if (button.classList.contains('is-loading')) {
                event.preventDefault();
                return false;
            }

            // Save original text (works for both <button> and <input>)
            const isInputTag = button.tagName === 'INPUT';
            const innerText = isInputTag ? button.value : button.innerText;

            // Apply loading state
            button.classList.add('is-loading');
            button.style.pointerEvents = 'none'; // Stops iOS double-taps

            if (isInputTag) {
                // <input type="submit"> can't have child elements like a spinner
                button.value = "⏳ Loading...";
            } else {
                // <button> can have a spinner
                button.innerText = "";
                const spinner = document.createElement('span');
                spinner.classList.add('spinner');
                spinner.style.display = 'inline-block';
                button.appendChild(spinner);
            }

            setTimeout(function () {
                button.classList.remove('is-loading');
                button.style.pointerEvents = 'auto';

                if (isInputTag) {
                    button.value = innerText;
                } else {
                    const existingSpinner = button.querySelector('.spinner');
                    if (existingSpinner) existingSpinner.remove();
                    button.innerText = innerText;
                }
            }, 3000);
        });
    });

    document.querySelectorAll('.faq details').forEach((details) => {
        const summary = details.querySelector('summary');
        const content = details.querySelector('.faq__wrap');
        if (!summary || !content) return;

        const DURATION = 280;
        const EASING = 'cubic-bezier(.4, 0, .2, 1)';

        content.style.overflow = 'hidden';
        if (!details.open) content.style.height = '0px';

        let currentAnim = null;
        let isAnimating = false;
        let ro = null;

        function open() {
            content.style.height = '0px';
            details.open = true;

            // Measure synchronously — fast path.
            const startHeight = content.scrollHeight;

            // Start the animation immediately.
            currentAnim?.cancel();
            currentAnim = content.animate(
                { height: ['0px', startHeight + 'px'] },
                { duration: DURATION, easing: EASING }
            );

            // Self-heal: if Firefox reflows after we start, catch the new
            // height and extend the animation's end value mid-flight.
            ro?.disconnect();
            ro = new ResizeObserver(() => {
                if (!currentAnim || currentAnim.playState !== 'running') return;
                const liveHeight = content.scrollHeight;
                const target = currentAnim.effect.getKeyframes().at(-1)?.height;
                const targetPx = parseFloat(target) || 0;
                if (liveHeight > targetPx + 0.5) {
                    currentAnim.effect.setKeyframes({
                        height: ['0px', liveHeight + 'px']
                    });
                }
            });
            ro.observe(content);

            currentAnim.onfinish = () => {
                ro?.disconnect();
                ro = null;
                content.style.height = 'auto';
                currentAnim = null;
                isAnimating = false;
            };
        }

        function close() {
            ro?.disconnect();
            ro = null;
            const startHeight = content.getBoundingClientRect().height;

            currentAnim?.cancel();
            currentAnim = content.animate(
                { height: [startHeight + 'px', '0px'] },
                { duration: DURATION, easing: EASING }
            );

            currentAnim.onfinish = () => {
                details.open = false;
                content.style.height = '0px';
                currentAnim = null;
                isAnimating = false;
            };
        }

        summary.addEventListener('click', (e) => {
            e.preventDefault();
            if (isAnimating) return;
            isAnimating = true;

            if (details.open) {
                close();
            } else {
                open();
            }
        });
    });

});

function FillPageId(id) {
    $("#Page").val(id);

    document.getElementById("search-form").submit();
}

(() => {
    const body = document.body;
    const addUrl = body.dataset.cartAddUrl;
    const removeUrl = body.dataset.cartRemoveUrl;
    const csrfToken = body.dataset.csrfToken || null;

    if (!addUrl || !removeUrl) {
        //console.warn('[cart-add] Missing data-cart-add-url / data-cart-remove-url on <body>.');
        return;
    }

    /* ─────────────────────────── Delegated click ─────────────────────────── */

    document.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-cart-toggle]');
        if (!btn || btn.disabled) return;
        e.preventDefault();

        const container = btn.closest('[data-cart-container]');
        if (!container) return;

        const productId = container.dataset.productId;
        if (!productId) return;

        const inCart = btn.dataset.state === 'in';
        const url = inCart ? removeUrl : addUrl;
        const label = btn.querySelector('.btn__label');

        // ── lock ──────────────────────────────────────────────
        btn.disabled = true;
        btn.classList.add('is-loading');

        try {
            const res = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(csrfToken ? { 'RequestVerificationToken': csrfToken } : {})
                },
                body: JSON.stringify({ productId: Number(productId) })
            });
            const data = await res.json().catch(() => ({}));

            // ── failure ──────────────────────────────────────
            if (!res.ok || !data.success) {
                btn.classList.remove('is-loading');
                btn.classList.add('is-error');
                setTimeout(() => {
                    btn.classList.remove('is-error');
                    btn.disabled = false;
                }, 500);
                notify('error', data.message || 'خطا در ارتباط با سرور');
                return;
            }

            // ── success: swap label + state ──────────────────
            const nowInCart = !!data.inCart;
            btn.dataset.state = nowInCart ? 'in' : 'out';
            label.textContent = nowInCart ? 'حذف از سبد' : 'افزودن';
            btn.classList.toggle('is-in-cart', nowInCart);

            // remove spinner → new text fades in
            btn.classList.remove('is-loading');

            // pulse button + flash card
            btn.classList.add('is-cart-flash');
            setTimeout(() => btn.classList.remove('is-cart-flash'), 720);
            setTimeout(() => { btn.disabled = false; }, 260);

            container.classList.remove('is-cart-flash');
            void container.offsetWidth;          // restart animation
            container.classList.add('is-cart-flash');
            setTimeout(() => container.classList.remove('is-cart-flash'), 950);

            // update the single source of truth — the sync listener does the rest
            if (typeof data.cartCount === 'number') {
                const input = document.getElementById('CartItemsCount');
                if (input && Number(input.value) !== data.cartCount) {
                    input.value = data.cartCount;
                    input.dispatchEvent(new Event('change', { bubbles: true }));
                }
            }

            // toast with backend message
            if (data.message) notify('success', data.message);

        } catch {
            btn.classList.remove('is-loading');
            btn.classList.add('is-error');
            setTimeout(() => {
                btn.classList.remove('is-error');
                btn.disabled = false;
            }, 500);
            notify('error', 'خطا در ارتباط با سرور');
        }
    });
})();

(() => {
    const input = document.getElementById('CartItemsCount');
    if (!input) return;

    function sync() {
        const count = Number(input.value) || 0;

        document.querySelectorAll('[data-cart-count-badge]').forEach(el => {
            el.textContent = count;
            el.hidden = count === 0;

            // Restart the bump animation even if it's already playing
            el.classList.remove('is-bumped');
            void el.offsetWidth;
            el.classList.add('is-bumped');
            setTimeout(() => el.classList.remove('is-bumped'), 420);
        });
    }

    input.addEventListener('change', sync);
})();

// Wrapper so the button still works if Toast isn't loaded on this page
function notify(type, msg) {
    if (!msg) return;
    if (window.Toast && typeof Toast[type] === 'function') {
        Toast[type](msg, { duration: 5000 });
    } else {
        // Fallback: never lose the message silently
        console.warn(`[cart-add] Toast.${type}:`, msg);
    }
}