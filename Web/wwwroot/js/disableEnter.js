document.addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        event.preventDefault(); // stops form submission or default action
        return false;           // extra safeguard
    }
});