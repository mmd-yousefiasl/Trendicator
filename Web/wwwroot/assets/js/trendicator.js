/* ==========================================================================
   Trendicator — main.js
   All non-login page code. Split from trendicator.js without changing logic.
   Hooks: [data-theme-toggle] [data-nav-toggle]
          [data-chart] [data-tabs] [data-count]
          [data-filter] [data-comment-form]
   ========================================================================== */
(function () {
    "use strict";

    /* ---------- Helpers ---------- */
    const $ = (sel, ctx) => (ctx || document).querySelector(sel);
    const $$ = (sel, ctx) => Array.from((ctx || document).querySelectorAll(sel));

    const FA_DIGITS = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
    function toFa(n) { return String(n).replace(/\d/g, d => FA_DIGITS[+d]); }
    function money(n) { return toFa(Number(n).toLocaleString("en-US")); } // ۱۲۳٬۴۵۶

    // Expose for .NET-rendered scripts
    window.Trendicator = { toFa, money };

    /* ---------- Toast ---------- */
    function toast(msg, type) {
        let region = $(".toast-region");
        if (!region) {
            region = document.createElement("div");
            region.className = "toast-region";
            document.body.appendChild(region);
        }
        const el = document.createElement("div");
        el.className = "toast" + (type ? " toast--" + type : "");
        el.setAttribute("role", "status");
        el.textContent = msg;
        region.appendChild(el);
        setTimeout(() => { el.style.opacity = "0"; setTimeout(() => el.remove(), 300); }, 2600);
    }

    /* Promise-based confirm dialog → resolves true / false */
    window.Trendicator.confirm = function (opts) {
        opts = Object.assign({
            title: 'تأیید',
            message: '',
            okText: 'تأیید',
            cancelText: 'انصراف',
            danger: false
        }, opts || {});

        return new Promise(resolve => {
            const dlg = document.createElement('dialog');
            dlg.className = 'confirm';
            dlg.innerHTML = `
            <div class="confirm__body">
                <h3 class="confirm__title"></h3>
                <p class="confirm__msg"></p>
            </div>
            <div class="confirm__actions">
                <button type="button" class="btn btn--ghost btn--sm" data-cancel></button>
                <button type="button"
                        class="btn btn--sm ${opts.danger ? 'btn--danger-solid' : 'btn--primary'}"
                        data-ok></button>
            </div>`;

            dlg.querySelector('.confirm__title').textContent = opts.title;
            dlg.querySelector('.confirm__msg').textContent = opts.message;
            dlg.querySelector('[data-cancel]').textContent = opts.cancelText;
            dlg.querySelector('[data-ok]').textContent = opts.okText;

            let result = false;

            dlg.querySelector('[data-ok]').addEventListener('click', () => {
                result = true;
                dlg.close();
            });
            dlg.querySelector('[data-cancel]').addEventListener('click', () => dlg.close());

            // Backdrop click → cancel (ignore clicks that land inside the card)
            dlg.addEventListener('click', e => {
                if (e.target === dlg) dlg.close();
            });

            // Escape / close() both land here
            dlg.addEventListener('close', () => {
                dlg.remove();
                resolve(result);
            });

            document.body.appendChild(dlg);
            dlg.showModal();

            // Safer default: focus Cancel so Enter / accidental taps don't destroy anything
            dlg.querySelector('[data-cancel]').focus();
        });
    };

    window.Trendicator.toast = toast;

    /* ---------- Theme-aware images ---------- */
    function applyThemeImages(theme) {
        $$("img[data-src-light]").forEach(img => {
            const want = theme === "light" ? img.dataset.srcLight : img.dataset.srcDark;
            if (want && img.getAttribute("src") !== want) img.setAttribute("src", want);
        });
    }

    /* ---------- Theme toggle ---------- */
    const THEME_KEY = "trendicator-theme";
    function applyTheme(t) {
        document.documentElement.setAttribute("data-theme", t);
        try { localStorage.setItem(THEME_KEY, t); } catch (e) { /* private mode */ }
        $$('meta[name="theme-color"]').forEach(m => m.setAttribute("content", t === "light" ? "#f6f8fa" : "#0b0f14"));
        applyThemeImages(t);
    }
    function initTheme() {
        let saved = null;
        try { saved = localStorage.getItem(THEME_KEY); } catch (e) { }
        const theme = saved || "dark";
        applyTheme(theme);
        $$("[data-theme-toggle]").forEach(btn =>
            btn.addEventListener("click", () =>
                applyTheme(document.documentElement.getAttribute("data-theme") === "dark" ? "light" : "dark")
            )
        );
    }

    /* ---------- Mobile nav ---------- */
    function initNav() {
        const toggle = $("[data-nav-toggle]");
        const nav = $(".main-nav");
        if (!toggle || !nav) return;

        function setOpen(open) {
            nav.classList.toggle("is-open", open);
            toggle.setAttribute("aria-expanded", String(open));
            document.body.style.overflow = open ? "hidden" : ""; // lock scroll while the drawer is open
        }

        toggle.addEventListener("click", () => setOpen(!nav.classList.contains("is-open")));
        // Close on outside click / Escape / link click
        document.addEventListener("click", e => {
            if (nav.classList.contains("is-open") && !nav.contains(e.target) && !toggle.contains(e.target)) setOpen(false);
        });
        document.addEventListener("keydown", e => {
            if (e.key === "Escape" && nav.classList.contains("is-open")) setOpen(false);
        });
        $$("a", nav).forEach(a => a.addEventListener("click", () => setOpen(false)));
    }

    /* ---------- Charts: deterministic SVG candlesticks ---------- */
    function mulberry32(seed) {
        return function () {
            seed |= 0; seed = (seed + 0x6D2B79F5) | 0;
            let t = Math.imul(seed ^ (seed >>> 15), 1 | seed);
            t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
            return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
        };
    }

    function candles(seed, n) {
        const rnd = mulberry32(seed);
        const out = [];
        let price = 50;
        for (let i = 0; i < n; i++) {
            const drift = (rnd() - 0.42) * 8;          // slight upward bias
            const open = price;
            const close = Math.max(8, open + drift);
            const high = Math.max(open, close) + rnd() * 3.5;
            const low = Math.min(open, close) - rnd() * 3.5;
            out.push({ open, close, high, low });
            price = close;
        }
        return out;
    }

    function buildChart(el) {
        const seed = parseInt(el.dataset.chart, 10) || 1;
        const count = parseInt(el.dataset.chartCount, 10) || 26;
        const W = 300, H = 170, P = 6;
        const data = candles(seed, count);
        const max = Math.max(...data.map(d => d.high));
        const min = Math.min(...data.map(d => d.low));
        const range = max - min || 1;
        const step = (W - P * 2) / count;
        const cw = Math.min(step * 0.55, 9);

        let s = "<svg viewBox='0 0 " + W + " " + H + "' preserveAspectRatio='none' role='img' aria-hidden='true'>";
        // grid
        for (let g = 1; g < 4; g++) {
            const y = (H / 4) * g;
            s += "<line x1='0' y1='" + y + "' x2='" + W + "' y2='" + y + "' stroke='var(--line-soft)' stroke-width='1'/>";
        }
        data.forEach((d, i) => {
            const x = P + i * step + step / 2;
            const up = d.close >= d.open;
            const color = up ? "var(--up)" : "var(--down)";
            const yH = P + (1 - (d.high - min) / range) * (H - P * 2);
            const yL = P + (1 - (d.low - min) / range) * (H - P * 2);
            const yO = P + (1 - (d.open - min) / range) * (H - P * 2);
            const yC = P + (1 - (d.close - min) / range) * (H - P * 2);
            const top = Math.min(yO, yC), h = Math.max(2, Math.abs(yC - yO));
            s += "<line x1='" + x + "' y1='" + yH + "' x2='" + x + "' y2='" + yL + "' stroke='" + color + "' stroke-width='1.4'/>";
            s += "<rect x='" + (x - cw / 2) + "' y='" + top + "' width='" + cw + "' height='" + h + "' rx='1' fill='" + color + "'/>";
        });
        s += "</svg>";
        el.innerHTML = s;
    }

    function initCharts() { $$("[data-chart]").forEach(buildChart); }

    /* ---------- Blog comments (client-side demo) ---------- */
    function initComments() {
        const form = $("[data-comment-form]");
        if (!form) return;
        const list = $("[data-comment-list]");
        const countEl = $("[data-comment-count]");
        form.addEventListener("submit", e => {
            e.preventDefault();
            const name = form.elements.name.value.trim();
            const body = form.elements.body.value.trim();
            if (!name || !body) return;

            const art = document.createElement("article");
            art.className = "comment";
            const initial = document.createElement("div");
            initial.className = "comment__avatar";
            initial.setAttribute("aria-hidden", "true");
            initial.textContent = name.charAt(0);
            const bodyEl = document.createElement("div");
            bodyEl.className = "comment__body";
            const head = document.createElement("div");
            head.className = "comment__head";
            const b = document.createElement("b");
            b.textContent = name;
            const time = document.createElement("span");
            time.className = "comment__time";
            time.textContent = "همین حالا";
            const p = document.createElement("p");
            p.textContent = body;
            head.append(b, time);
            bodyEl.append(head, p);
            art.append(initial, bodyEl);
            list.prepend(art);

            if (countEl) countEl.textContent = toFa(parseInt(toEn(countEl.textContent), 10) + 1);
            form.reset();
            toast("دیدگاه شما ثبت شد و پس از بررسی نمایش داده می‌شود", "ok");
        });
    }
    function toEn(s) { return String(s).replace(/[۰-۹]/g, d => FA_DIGITS.indexOf(d)); }

    /* ---------- Tabs ---------- */
    function initTabs() {
        $$("[data-tabs]").forEach(wrap => {
            const tabs = $$("[role='tab']", wrap);
            tabs.forEach(tab => tab.addEventListener("click", () => {
                tabs.forEach(t => {
                    t.setAttribute("aria-selected", t === tab);
                    const p = document.getElementById(t.getAttribute("aria-controls"));
                    if (p) p.hidden = t !== tab;
                });
            }));
        });
    }

    /* ---------- Shop filters (client-side demo) ---------- */
    function initFilters() {
        const grid = $("[data-filter-grid]");
        if (!grid) return;
        const cards = $$("[data-filter-tags]", grid);
        $$("[data-filter]").forEach(el => el.addEventListener("click", e => {
            if (el.tagName === "A") e.preventDefault();
            const tag = el.dataset.filter;
            $$("[data-filter]").forEach(x => x.classList.toggle("is-active", x === el));
            cards.forEach(c => {
                const tags = (c.dataset.filterTags || "").split(",");
                c.hidden = tag !== "*" && !tags.includes(tag);
            });
        }));
        const search = $("[data-filter-search]");
        if (search) search.addEventListener("input", () => {
            const q = search.value.trim();
            cards.forEach(c => { c.hidden = q && !(c.dataset.productName || "").includes(q); });
        });
    }

    /* ---------- Count-up stats ---------- */
    function initCounters() {
        const els = $$("[data-count]");
        if (!els.length) return;
        const io = new IntersectionObserver(entries => {
            entries.forEach(en => {
                if (!en.isIntersecting) return;
                const el = en.target;
                io.unobserve(el);
                const target = parseInt(el.dataset.count, 10) || 0;
                const t0 = performance.now(), dur = 900;
                (function tick(t) {
                    const p = Math.min((t - t0) / dur, 1);
                    el.textContent = money(Math.round(target * (1 - Math.pow(1 - p, 3))));
                    if (p < 1) requestAnimationFrame(tick);
                })(t0);
            });
        }, { threshold: .4 });
        els.forEach(el => io.observe(el));
    }

    /* ---------- Collapsible regions (panel nav on mobile, shop filters) ---------- */
    function initCollapsibles() {
        $$("[data-collapse-toggle]").forEach(btn => {
            const target = $(btn.dataset.collapseToggle);
            if (!target) return;
            btn.addEventListener("click", () => {
                const open = target.classList.toggle("is-open");
                btn.setAttribute("aria-expanded", String(open));
            });
        });
    }

    /* ---------- Boot ---------- */
    document.addEventListener("DOMContentLoaded", () => {
        initTheme(); initNav(); initCharts();
        initTabs(); initFilters(); initCounters();
        initCollapsibles();
        initComments();
    });
})();

(function () {
    "use strict";

    var menu = document.querySelector(".cat-nav");
    if (!menu) return;

    var summary = menu.querySelector("summary");
    var tabs = Array.prototype.slice.call(menu.querySelectorAll(".cat-nav__tab"));
    var panes = Array.prototype.slice.call(menu.querySelectorAll(".cat-nav__pane"));

    function activate(tab) {
        var id = tab.getAttribute("data-cat");
        tabs.forEach(function (t) {
            var on = t === tab;
            t.classList.toggle("is-active", on);
            t.setAttribute("aria-expanded", on ? "true" : "false");
        });
        panes.forEach(function (p) {
            p.classList.toggle("is-active", p.getAttribute("data-pane") === id);
        });
    }

    tabs.forEach(function (tab) {
        tab.addEventListener("click", function () {
            activate(tab);
            if (window.matchMedia("(max-width: 860px)").matches) {
                tab.scrollIntoView({ inline: "center", block: "nearest", behavior: "smooth" });
            }
        });
    });

    /* Reset to the first tab every time the menu opens */
    menu.addEventListener("toggle", function () {
        if (menu.open && tabs.length) activate(tabs[0]);
    });

    /* Click outside → close */
    document.addEventListener("click", function (e) {
        if (menu.open && !menu.contains(e.target)) menu.open = false;
    });

    /* Esc → close + focus back to the trigger */
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && menu.open) {
            menu.open = false;
            summary.focus();
        }
    });
})();