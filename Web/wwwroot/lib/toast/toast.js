(() => {
    const containerId = 'toast-container';
    const LEAVE_MS = 300;
    let container;

    const ICONS = {
        success: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6 9 17l-5-5"/></svg>`,
        error: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9"/><path d="m15 9-6 6M9 9l6 6"/></svg>`,
        warning: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0Z"/><path d="M12 9v4M12 17h.01"/></svg>`,
        info: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9"/><path d="M12 16v-4M12 8h.01"/></svg>`,
        loading: `<span class="toast-spinner"></span>`
    };

    function ensureContainer() {
        if (container) return container;
        container = document.getElementById(containerId);
        if (!container) {
            container = document.createElement('div');
            container.id = containerId;
            document.body.appendChild(container);
        }
        return container;
    }

    // FLIP: record positions, run `mutate`, animate siblings back
    function flipSiblings(elements, mutate) {
        const first = elements.map(el => el.getBoundingClientRect().top);
        mutate();
        const last = elements.map(el => el.getBoundingClientRect().top);

        elements.forEach((el, i) => {
            const dy = first[i] - last[i];
            if (dy === 0) return;

            el.style.transition = 'none';
            el.style.translate = `0 ${dy}px`;
            el.getBoundingClientRect(); // force reflow
            el.style.transition = `translate ${LEAVE_MS}ms ease`;
            el.style.translate = '0 0';

            el.addEventListener('transitionend', () => {
                el.style.transition = '';
                el.style.translate = '';
            }, { once: true });
        });
    }

    function dismiss(el) {
        if (!el || el.dataset.leaving) return;
        el.dataset.leaving = '1';

        const parent = el.parentElement;
        if (!parent) return;

        const siblings = [...parent.children].filter(c => c !== el && !c.dataset.leaving);

        flipSiblings(siblings, () => {
            const h = el.offsetHeight;
            el.style.height = h + 'px';
            el.getBoundingClientRect();

            el.style.height = '0px';
            el.style.marginBottom = '0px';
            el.style.paddingTop = '0px';
            el.style.paddingBottom = '0px';
            el.style.borderWidth = '0px';
            el.style.opacity = '0';
            el.style.transform = 'var(--toast-enter-from)';
            el.style.pointerEvents = 'none';
        });

        setTimeout(() => el.remove(), LEAVE_MS + 50);
    }

    function show(message, opts = {}) {
        const { type = 'info', duration = 4000, onClick } = opts;

        const c = ensureContainer();
        const el = document.createElement('div');
        el.className = `toast toast-${type}`;
        if (duration > 0) el.style.setProperty('--toast-duration', duration + 'ms');

        el.innerHTML = `
      <div class="toast-icon">${ICONS[type] ?? ICONS.info}</div>
      <div class="toast-content">
        <div class="toast-message"></div>
      </div>
      ${type !== 'loading' && duration > 0 ? `<div class="toast-progress"></div>` : ''}
    `;

        el.querySelector('.toast-message').textContent = message;

        el.addEventListener('click', () => {
            if (typeof onClick === 'function') onClick(el);
            dismiss(el);
        });

        // FLIP siblings up while the new toast takes its slot
        const siblings = [...c.children].filter(x => !x.dataset.leaving);
        flipSiblings(siblings, () => c.appendChild(el));

        // Trigger the entrance transition after layout is settled
        void el.offsetWidth;
        el.classList.add('toast-visible');

        if (type !== 'loading' && duration > 0) {
            setTimeout(() => dismiss(el), duration);
        }

        return { el, close: () => dismiss(el) };
    }

    const Toast = {
        show,
        success: (msg, opts) => show(msg, { ...opts, type: 'success' }),
        error: (msg, opts) => show(msg, { ...opts, type: 'error' }),
        warning: (msg, opts) => show(msg, { ...opts, type: 'warning' }),
        info: (msg, opts) => show(msg, { ...opts, type: 'info' }),
        loading: (msg, opts) => show(msg, { ...opts, type: 'loading', duration: 0 }),
        dismissAll: () => document.querySelectorAll('.toast').forEach(dismiss),
        clear: () => document.querySelectorAll('.toast').forEach(t => t.remove())
    };

    window.Toast = Toast;
})();