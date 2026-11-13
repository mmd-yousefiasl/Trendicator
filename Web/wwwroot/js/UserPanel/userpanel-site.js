(function () {
    var nav = document.getElementById('panel-nav');
    if (!nav) return;

    var normalize = function (path) {
        path = path.replace(/\/+$/, '');
        return (path === '' ? '/' : path).toLowerCase();
    };

    var currentPath = normalize(window.location.pathname);
    var links = nav.querySelectorAll('a[href]');
    var best = null;
    var bestLen = -1;

    links.forEach(function (a) {
        var url;
        try { url = new URL(a.href, window.location.origin); } catch (e) { return; }
        if (url.origin !== window.location.origin) return;

        var linkPath = normalize(url.pathname);
        var matches = linkPath === currentPath || currentPath.indexOf(linkPath + '/') === 0;

        if (matches && linkPath.length > bestLen) {
            best = a;
            bestLen = linkPath.length;
        }
    });

    if (best) {
        best.classList.add('is-active');
        best.setAttribute('aria-current', 'page');
    }
})();

document.addEventListener('click', (e) => {
    const form = e.target.closest('form[data-confirm-delete]');
    if (!form || form.disabled) return;
    e.preventDefault();
    ConfirmDelete(form);
});

async function ConfirmDelete(element) {
    if (element.dataset.confirming === 'true') return;
    element.dataset.confirming = 'true';

    const confirmed = window.Trendicator?.confirm
        ? await Trendicator.confirm({
            title: 'آیا از انجام این عملیات مطمئن هستید؟',
            message: element.dataset.confirmMessage || 'این عملیات قابل بازگشت نیست.',
            okText: 'آره، مطمئنم',
            cancelText: 'نه، منصرف شدم',
            danger: true
        })
        : confirm('آیا از انجام این عملیات مطمئن هستید؟');

    element.dataset.confirming = 'false';
    if (!confirmed) return;

    // requestSubmit fires the 'submit' event → your spinner handler runs.
    // element.submit() would NOT fire it.
    if (typeof element.requestSubmit === 'function') {
        element.requestSubmit();
    } else {
        element.submit();
    }
}

(function () {
    var input = document.getElementById('Attachment');
    var zone = document.getElementById('fileDrop');
    var name = document.getElementById('fileDropName');
    var hint = document.getElementById('fileDropHint');
    if (!input || !zone) return;

    zone.addEventListener('mouseenter', function () {
        if (!zone.dataset.hasFile) {
            zone.style.borderColor = 'var(--brand)';
            zone.style.background = 'var(--brand-soft)';
        }
    });
    zone.addEventListener('mouseleave', function () {
        if (!zone.dataset.hasFile) {
            zone.style.borderColor = 'var(--line)';
            zone.style.background = 'var(--bg-soft)';
        }
    });

    input.addEventListener('change', function () {
        if (this.files && this.files.length > 0) {
            var f = this.files[0];

            zone.dataset.hasFile = '1';
            zone.style.borderStyle = 'solid';
            zone.style.borderColor = 'var(--brand)';
            zone.style.background = 'var(--brand-soft)';

            name.textContent = f.name;
            name.style.color = 'var(--brand)';

            var kb = f.size / 1024;
            hint.textContent = (kb >= 1024
                ? (kb / 1024).toFixed(1) + ' مگابایت'
                : kb.toFixed(0) + ' کیلوبایت');
        } else {
            zone.dataset.hasFile = '';
            zone.style.borderStyle = 'dashed';
            zone.style.borderColor = 'var(--line)';
            zone.style.background = 'var(--bg-soft)';

            name.textContent = 'فایلی انتخاب نشده — برای انتخاب کلیک کنید';
            name.style.color = 'var(--text-2)';
            hint.textContent = 'PNG, JPG, JPEG, WEBP — حداکثر ۵۰۰ کیلوبایت';
        }
    });
})();

(function () {
    "use strict";

    var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    var LOAD_DELAY = reduceMotion ? 0 : 650; // ms of fake "fetching"

    document.addEventListener("click", function (e) {
        var btn = e.target.closest("[data-balance-toggle]");
        if (!btn) return;

        var card = btn.closest("[data-balance]");
        if (!card || card.dataset.busy === "1") return;

        var mask = card.querySelector("[data-balance-mask]");
        var spinner = card.querySelector("[data-balance-spinner]");
        var amount = card.querySelector("[data-balance-amount]");
        var isShown = btn.getAttribute("aria-pressed") === "true";

        // ── Hide: instant, no delay ──
        if (isShown) {
            btn.setAttribute("aria-pressed", "false");
            btn.setAttribute("aria-label", "نمایش موجودی");
            amount.classList.remove("is-revealed");
            amount.hidden = true;
            spinner.hidden = true;
            mask.hidden = false;
            return;
        }

        // ── Show: spinner → amount ──
        card.dataset.busy = "1";
        btn.disabled = true;
        mask.hidden = true;
        amount.hidden = true;
        spinner.hidden = false;

        window.setTimeout(function () {
            spinner.hidden = true;
            amount.hidden = false;

            // replay the reveal animation even on repeat clicks
            amount.classList.remove("is-revealed");
            void amount.offsetWidth;
            amount.classList.add("is-revealed");

            btn.disabled = false;
            btn.setAttribute("aria-pressed", "true");
            btn.setAttribute("aria-label", "مخفی کردن موجودی");
            delete card.dataset.busy;
        }, LOAD_DELAY);
    });
})();