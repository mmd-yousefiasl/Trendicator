(function () {
    "use strict";

    const $$ = (sel, ctx) => Array.from((ctx || document).querySelectorAll(sel));
    const toFa = window.Trendicator ? window.Trendicator.toFa : String;

    /* ---------- OTP boxes: auto-advance, backspace, paste ---------- */
    function initOtp() {
        $$("[data-otp]").forEach(group => {
            const inputs = $$("input", group);
            const form = group.closest("form");
            const span = form ? form.querySelector('[data-valmsg-for="LoginCode"]') : null;

            function clearMessage() {
                if (span && span.textContent) {
                    span.textContent = "";
                    span.className = "field-validation-valid";
                }
            }

            function showMessage(msg) {
                if (span) {
                    span.textContent = msg;
                    span.className = "field-validation-error";
                }
            }

            inputs.forEach((input, i) => {
                input.addEventListener("input", () => {
                    input.value = input.value.replace(/[^\d]/g, "").slice(-1);
                    clearMessage();
                    if (input.value && i < inputs.length - 1) inputs[i + 1].focus();
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
                    text.slice(0, inputs.length).split("").forEach((d, j) => {
                        if (inputs[j]) inputs[j].value = d;
                    });
                    clearMessage();
                    inputs[Math.min(text.length, inputs.length) - 1].focus();
                });

                input.addEventListener("focus", () => input.select());
            });

            if (form) {
                form.addEventListener("submit", e => {
                    const empty = inputs.some(x => !x.value);
                    if (empty) {
                        e.preventDefault();
                        showMessage("کد تایید ضروری می باشد.");
                        inputs.find(x => !x.value).focus();
                    } else {
                        clearMessage();
                    }
                });
            }
        });
    }

    /* ---------- Countdown timers (resend OTP) ---------- */
    function initCountdowns() {
        $$("[data-countdown]").forEach(el => {
            const total = parseInt(el.dataset.countdown, 10) || 0;
            const timeEl = el.querySelector("[data-countdown-time]") || el;
            const resend = el.parentElement && el.parentElement.querySelector("[data-resend]");

            let timer = null;
            const pad = n => String(n).padStart(2, "0");

            function render(s) {
                timeEl.textContent = toFa(Math.floor(s / 60) + ":" + pad(s % 60));
            }

            function showResend() {
                el.classList.add("hidden");
                if (resend) resend.classList.remove("hidden");
            }

            function start() {
                clearInterval(timer);

                // If there's no time left, just show the resend button.
                if (total <= 0) {
                    showResend();
                    return;
                }

                let left = total;
                el.classList.remove("hidden");
                if (resend) resend.classList.add("hidden");
                render(left);

                timer = setInterval(() => {
                    left -= 1;
                    if (left <= 0) {
                        clearInterval(timer);
                        showResend();
                        return;
                    }
                    render(left);
                }, 1000);
            }

            if (resend) resend.addEventListener("click", start);
            start();
        });
    }

    /* ---------- Boot ---------- */
    function boot() {
        initOtp();
        initCountdowns();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();