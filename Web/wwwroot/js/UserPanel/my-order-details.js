(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const btn = document.getElementById('btn-pdf');
        if (!btn) return;

        const orderId = btn.dataset.orderId || 'order';

        btn.addEventListener('click', async function () {
            const invoice = document.getElementById('invoice-print');
            const overlay = document.getElementById('pdf-overlay');

            if (!invoice) {
                alert('قالب فاکتور یافت نشد.');
                return;
            }
            if (!window.html2canvas || !window.jspdf) {
                alert('کتابخانه‌های تولید PDF بارگذاری نشده‌اند.');
                return;
            }

            const originalHtml = btn.innerHTML;
            btn.disabled = true;
            btn.textContent = 'در حال آماده‌سازی…';

            // ── Show overlay with inline styles (bypasses any CSS dependency) ──
            if (overlay) {
                overlay.classList.add('is-visible');
                overlay.style.display = 'flex';
                overlay.style.position = 'fixed';
                overlay.style.top = '0';
                overlay.style.left = '0';
                overlay.style.right = '0';
                overlay.style.bottom = '0';
                overlay.style.zIndex = '99999';
                overlay.style.background = '#0b0f14';
                overlay.style.alignItems = 'center';
                overlay.style.justifyContent = 'center';
                overlay.setAttribute('aria-hidden', 'false');
            }

            document.body.classList.add('is-generating-pdf');

            try {
                // 1) Make sure the custom font is loaded before capture —
                //    otherwise html2canvas renders with Tahoma fallback.
                if (document.fonts && document.fonts.ready) {
                    await document.fonts.ready;
                }

                // 2) Let the overlay paint before the invoice moves on-screen,
                //    so the white A4 sheet never flashes.
                await new Promise(r => requestAnimationFrame(() => requestAnimationFrame(r)));

                invoice.classList.add('is-capturing');
                await new Promise(r => requestAnimationFrame(r));

                // 3) Capture. windowWidth/Height prevent clipping on narrow
                //    viewports (iPhone / small Android screens).
                const canvas = await window.html2canvas(invoice, {
                    scale: 2,
                    useCORS: true,
                    backgroundColor: '#ffffff',
                    logging: false,
                    windowWidth: Math.max(window.innerWidth, invoice.scrollWidth + 40),
                    windowHeight: Math.max(window.innerHeight, invoice.scrollHeight + 40)
                });

                // 4) Build the PDF.
                const { jsPDF } = window.jspdf;
                const pdf = new jsPDF({ orientation: 'p', unit: 'mm', format: 'a4' });

                const pageW = pdf.internal.pageSize.getWidth();
                const pageH = pdf.internal.pageSize.getHeight();
                const imgW = pageW;
                const imgH = (canvas.height * imgW) / canvas.width;

                if (imgH <= pageH) {
                    pdf.addImage(canvas, 'PNG', 0, 0, imgW, imgH);
                } else {
                    // Slice the tall canvas into page-sized chunks.
                    const pageCanvasH = Math.floor(canvas.width * (pageH / imgW));
                    let offset = 0;
                    let page = 0;

                    while (offset < canvas.height) {
                        const sliceH = Math.min(pageCanvasH, canvas.height - offset);

                        const slice = document.createElement('canvas');
                        slice.width = canvas.width;
                        slice.height = sliceH;
                        slice.getContext('2d').drawImage(
                            canvas,
                            0, offset, canvas.width, sliceH,
                            0, 0, canvas.width, sliceH
                        );

                        if (page > 0) pdf.addPage();
                        pdf.addImage(slice, 'PNG', 0, 0, imgW, (sliceH * imgW) / canvas.width);

                        offset += sliceH;
                        page++;
                    }
                }

                // 5) Download. Blob URL works on iOS Safari, Android Chrome,
                //    and desktop; `pdf.save()` alone can fail on older iOS.
                const blob = pdf.output('blob');
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `order-${orderId}.pdf`;
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(() => URL.revokeObjectURL(url), 2000);
            } catch (err) {
                console.error('PDF generation failed:', err);
                alert('تولید فاکتور ناموفق بود.');
            } finally {
                invoice.classList.remove('is-capturing');
                document.body.classList.remove('is-generating-pdf');

                if (overlay) {
                    overlay.classList.remove('is-visible');
                    overlay.style.display = 'none';
                    overlay.setAttribute('aria-hidden', 'true');
                }

                btn.disabled = false;
                btn.innerHTML = originalHtml;
            }
        });
    });
})();