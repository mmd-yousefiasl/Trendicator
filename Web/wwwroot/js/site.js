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

});