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