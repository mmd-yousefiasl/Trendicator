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