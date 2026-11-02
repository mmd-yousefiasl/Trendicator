document.addEventListener('DOMContentLoaded', () => {
    const btn = document.getElementById('load-more-reviews');
    const container = document.getElementById('reviews-container');
    const pageInput = document.getElementById('reviews-page');
    if (!btn || !container || !pageInput) return;

    btn.addEventListener('click', async () => {
        const nextPage = parseInt(pageInput.value, 10) + 1;
        const original = btn.textContent;

        btn.disabled = true;
        btn.textContent = 'در حال بارگذاری…';

        try {
            const url = `/product/comments?productId=${encodeURIComponent(btn.dataset.productId)}&page=${nextPage}`;
            const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!res.ok) throw new Error(res.status);

            container.insertAdjacentHTML('beforeend', await res.text());
            pageInput.value = nextPage;

            if (res.headers.get('X-Has-More') !== 'true') {
                document.getElementById('load-more-wrapper')?.remove();
            }
        } catch (err) {
            console.error('load more reviews failed:', err);
        } finally {
            if (document.body.contains(btn)) {
                btn.disabled = false;
                btn.textContent = original;
            }
        }
    });
});

(function () {
    if (window.__copyLinkBound) return;
    window.__copyLinkBound = true;

    function onCopied() {
        if (window.Toast && typeof Toast.info === 'function') {
            Toast.info('لینک محصول کپی شد', { duration: 5000 });
        }
    }

    function legacyCopy(text) {
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.setAttribute('readonly', '');
        ta.style.position = 'fixed';
        ta.style.top = '-1000px';
        ta.style.opacity = '0';
        document.body.appendChild(ta);
        ta.select();
        try { document.execCommand('copy'); onCopied(); }
        catch { /* silent */ }
        finally { ta.remove(); }
    }

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-copy-link]');
        if (!btn) return;

        const url = btn.getAttribute('data-url');
        if (!url) return;

        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(url)
                .then(onCopied)
                .catch(() => legacyCopy(url));
        } else {
            legacyCopy(url);
        }
    });
})();