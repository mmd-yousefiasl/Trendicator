document.addEventListener('DOMContentLoaded', function () {
    // Listen for 'submit' on ALL forms on the page
    document.querySelectorAll('form').forEach(function (form) {
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