/* ==========================================================================
   Trendicator — trendicator.js
   Vanilla JS · no dependencies · data-attribute driven
   Hooks: [data-theme-toggle] [data-nav-toggle] [data-add-to-cart]
          [data-cart-render] [data-chart] [data-tabs] [data-count]
          [data-filter] [data-toast-demo] [data-comment-form]
   ========================================================================== */
(function () {
  "use strict";

  /* ---------- Helpers ---------- */
  const $  = (sel, ctx) => (ctx || document).querySelector(sel);
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
  window.Trendicator.toast = toast;

  /* ---------- Theme-aware images ----------
     <img> elements carry data-src-light with the light-theme WebP;
     the default src is the dark one. Swaps on toggle and on init. */
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
  // Prevent flash: inline snippet in <head> also does this; here we sync buttons
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
  // Seeded PRNG so every render of the same seed looks identical (SSR-friendly)
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

  /* ---------- Cart (localStorage) ---------- */
  const CART_KEY = "trendicator-cart";
  function cartGet() {
    try { return JSON.parse(localStorage.getItem(CART_KEY)) || []; }
    catch (e) { return []; }
  }
  function cartSet(items) {
    try { localStorage.setItem(CART_KEY, JSON.stringify(items)); } catch (e) { }
    updateCartBadge();
  }
  function updateCartBadge() {
    const n = cartGet().reduce((s, i) => s + (i.qty || 1), 0);
    $$("[data-cart-count]").forEach(el => {
      el.textContent = toFa(n);
      el.hidden = n === 0;
    });
  }
  function addToCart(item) {
    const items = cartGet();
    const found = items.find(i => i.id === item.id);
    if (found) { found.qty = (found.qty || 1) + 1; }
    else { items.push(Object.assign({ qty: 1 }, item)); }
    cartSet(items);
    toast("«" + item.name + "» به سبد خرید اضافه شد", "ok");
  }

  function initCart() {
    $$("[data-add-to-cart]").forEach(btn =>
      btn.addEventListener("click", () => {
        const el = btn.closest("[data-product]") || document;
        addToCart({
          id: btn.dataset.id || el.dataset.productId || "item",
          name: btn.dataset.name || el.dataset.productName || "محصول",
          price: parseInt(btn.dataset.price || el.dataset.productPrice, 10) || 0,
        });
      })
    );
    updateCartBadge();
    renderCartPage();
  }

  function renderCartPage() {
    const box = $("[data-cart-render]");
    if (!box) return;
    const items = cartGet();
    const emptyTpl = $("[data-cart-empty]");
    const list = $("[data-cart-list]", box);

    if (!items.length) {
      if (list) list.innerHTML = "";
      if (emptyTpl) emptyTpl.hidden = false;
      const pay = $("[data-cart-pay]");
      if (pay) pay.disabled = true;
      return;
    }
    if (emptyTpl) emptyTpl.hidden = true;

    if (list) {
      list.innerHTML = items.map(it => (
        "<div class='cart-line' data-line='" + it.id + "'>" +
          "<div class='cart-line__thumb'><svg viewBox='0 0 60 36'><rect x='8' y='12' width='6' height='14' fill='var(--up)'/><rect x='20' y='8' width='6' height='18' fill='var(--down)'/><rect x='32' y='14' width='6' height='10' fill='var(--up)'/><rect x='44' y='6' width='6' height='16' fill='var(--up)'/></svg></div>" +
          "<div class='cart-line__body'>" +
            "<div class='cart-line__title'>" + it.name + "</div>" +
            "<div class='cart-line__meta'>لایسنس دائمی · دانلود فوری</div>" +
          "</div>" +
          "<div class='price num'><small>تومان</small> " + money(it.price * (it.qty || 1)) + "</div>" +
          "<button class='btn btn--danger btn--sm' data-remove='" + it.id + "'>حذف</button>" +
        "</div>"
      )).join("");

      $$("[data-remove]", list).forEach(b => b.addEventListener("click", () => {
        cartSet(cartGet().filter(i => i.id !== b.dataset.remove));
        renderCartPage();
        toast("محصول از سبد حذف شد");
      }));
    }

    const total = items.reduce((s, i) => s + i.price * (i.qty || 1), 0);
    $$("[data-cart-total]").forEach(el => { el.textContent = money(total); });
    const pay = $("[data-cart-pay]");
    if (pay) pay.disabled = false;
    // mirror total to checkout summary if present
    $$("[data-checkout-total]").forEach(el => { el.textContent = money(total); });
  }

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
  // Pair: button[data-collapse-toggle="#target"] + target element.
  // If the button lives inside the target itself, the class toggles on the
  // target and the button stays visible (used by the shop filter panel).
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

  /* ---------- Demo toast triggers ---------- */
  function initDemos() {
    $$("[data-toast-demo]").forEach(b => b.addEventListener("click", () =>
      toast(b.dataset.toastDemo || "این یک پیام نمونه است", b.dataset.toastType)
    ));
  }

  /* ---------- Password visibility toggle ---------- */
  function initPasswordToggles() {
    $$("[data-pass-toggle]").forEach(btn => btn.addEventListener("click", () => {
      const input = btn.parentElement.querySelector("input");
      if (!input) return;
      const show = input.type === "password";
      input.type = show ? "text" : "password";
      btn.setAttribute("aria-label", show ? "مخفی کردن رمز" : "نمایش رمز");
      const eyeOn = btn.querySelector("[data-eye-on]");
      const eyeOff = btn.querySelector("[data-eye-off]");
      if (eyeOn) eyeOn.hidden = !show;
      if (eyeOff) eyeOff.hidden = show;
    }));
  }

  /* ---------- OTP boxes: auto-advance, backspace, paste ---------- */
  function initOtp() {
    $$("[data-otp]").forEach(group => {
      const inputs = $$("input", group);
      const form = group.closest("form");
      const hidden = form ? form.querySelector("[data-otp-value]") : null;

      function sync() {
        if (hidden) hidden.value = inputs.map(i => i.value).join("");
      }

      inputs.forEach((input, i) => {
        input.addEventListener("input", () => {
          input.value = input.value.replace(/[^\d]/g, "").slice(-1);
          sync();
          if (input.value && i < inputs.length - 1) inputs[i + 1].focus();
          if (inputs.every(x => x.value)) {
            inputs.forEach(x => x.blur());
            if (form) form.requestSubmit ? form.requestSubmit() : null;
          }
        });
        input.addEventListener("keydown", e => {
          if (e.key === "Backspace" && !input.value && i > 0) inputs[i - 1].focus();
          if (e.key === "ArrowLeft" && i < inputs.length - 1) inputs[i + 1].focus();
          if (e.key === "ArrowRight" && i > 0) inputs[i - 1].focus();
        });
        input.addEventListener("paste", e => {
          const text = (e.clipboardData || window.clipboardData).getData("text").replace(/\D/g, "");
          if (!text) return;
          e.preventDefault();
          text.slice(0, inputs.length).split("").forEach((d, j) => { if (inputs[j]) inputs[j].value = d; });
          sync();
          inputs[Math.min(text.length, inputs.length) - 1].focus();
        });
        input.addEventListener("focus", () => input.select());
      });
    });
  }

  /* ---------- Countdown timers (resend OTP etc.) ---------- */
  function initCountdowns() {
    $$("[data-countdown]").forEach(el => {
      const total = parseInt(el.dataset.countdown, 10) || 120;
      const btn = el.querySelector("[data-resend]") || el;
      let left = total, timer = null;

      const btnLabel = btn.textContent;

      function fmt(s) {
        const m = Math.floor(s / 60), r = s % 60;
        return toFa(m + ":" + String(r).padStart(2, "0"));
      }

      function render() {
        const span = el.querySelector("[data-countdown-time]") || el;
        span.textContent = fmt(left);
      }

      function stop() {
        clearInterval(timer); timer = null;
        btn.disabled = false;
        btn.textContent = btnLabel;
      }

      function start() {
        left = total;
        btn.disabled = true;
        render();
        timer = setInterval(() => {
          left--;
          render();
          if (left <= 0) stop();
        }, 1000);
      }

      if (btn !== el) btn.addEventListener("click", start);
      start();
    });
  }

  /* ---------- Boot ---------- */
  document.addEventListener("DOMContentLoaded", () => {
    initTheme(); initNav(); initCharts(); initCart();
    initTabs(); initFilters(); initCounters(); initDemos();
    initPasswordToggles(); initOtp(); initCountdowns(); initCollapsibles();
    initComments();
  });
})();
