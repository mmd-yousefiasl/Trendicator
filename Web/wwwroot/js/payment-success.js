(() => {
    const canvas = document.querySelector('[data-confetti]');
    if (!canvas) return;

    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
        canvas.remove();
        return;
    }

    /* ═══════════════════════════════════════════════════════════
       ⚙️  PROFILES — auto-selected by device capability
       ═══════════════════════════════════════════════════════════ */
    const PROFILES = {
        high: { count: 50, dpr: 2, fps: 60 },
        mid: { count: 35, dpr: 1.5, fps: 30 },
        low: { count: 18, dpr: 1, fps: 20 },
    };

    const detectProfile = () => {
        const cores = navigator.hardwareConcurrency || 0;
        const mem = navigator.deviceMemory || 0;

        if ((cores > 0 && cores <= 2) || (mem > 0 && mem <= 2)) return 'low';
        if (cores > 0 && cores <= 4) return 'mid';
        if (mem > 0 && mem <= 4) return 'mid';
        return 'high';
    };

    const profileName = detectProfile();
    const profile = PROFILES[profileName];
    const frameInterval = 1000 / profile.fps;
    window.__confettiProfile = profileName;  // debug: check console
    /* ═══════════════════════════════════════════════════════════ */

    /* ═══════════════════════════════════════════════════════════
       🎨  VISUAL SETTINGS
       ═══════════════════════════════════════════════════════════ */
    const CONFIG = {
        widthMin: 7, widthMax: 13,
        heightMin: 11, heightMax: 20,
        vyMin: 0.6, vyMax: 1.6,
        gravity: 0.01,
        vxRange: 0.6,
        swaySpeedMin: 0.015, swaySpeedMax: 0.035,
        swayAmpMin: 0.2, swayAmpMax: 0.6,
        vrotRange: 0.15,
        flutterSpeedMin: 0.04, flutterSpeedMax: 0.08,
        colors: ['#10b981', '#34d399', '#fbbf24', '#f59e0b', '#60a5fa', '#f472b6'],
    };
    /* ═══════════════════════════════════════════════════════════ */

    const ctx = canvas.getContext('2d');
    const rand = (a, b) => a + Math.random() * (b - a);
    const randSigned = (r) => (Math.random() - 0.5) * r;
    const pick = (arr) => arr[(Math.random() * arr.length) | 0];

    let papers = [];
    let rafId = null;
    let running = true;
    let lastFrame = 0;
    let dpr = 1;

    // dt reference — 60fps is our "1.0x" baseline.
    const NOMINAL_DT = 1000 / 60;
    const DT_CAP = 4;          // cap at 4x — prevents teleport after a stall

    const resize = () => {
        dpr = Math.min(window.devicePixelRatio || 1, profile.dpr);

        const w = window.innerWidth || document.documentElement.clientWidth || 1;
        const h = window.innerHeight || document.documentElement.clientHeight || 1;

        canvas.width = w * dpr;
        canvas.height = h * dpr;
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        canvas._w = w;
        canvas._h = h;
    };

    const spawn = (y = 0) => ({
        x: Math.random() * canvas._w,
        y,
        w: rand(CONFIG.widthMin, CONFIG.widthMax),
        h: rand(CONFIG.heightMin, CONFIG.heightMax),
        vy: rand(CONFIG.vyMin, CONFIG.vyMax),
        vx: randSigned(CONFIG.vxRange),
        rot: Math.random() * Math.PI * 2,
        vrot: randSigned(CONFIG.vrotRange),
        sway: Math.random() * Math.PI * 2,
        swaySpeed: rand(CONFIG.swaySpeedMin, CONFIG.swaySpeedMax),
        swayAmp: rand(CONFIG.swayAmpMin, CONFIG.swayAmpMax),
        flutter: Math.random() * Math.PI * 2,
        flutterSpeed: rand(CONFIG.flutterSpeedMin, CONFIG.flutterSpeedMax),
        color: pick(CONFIG.colors),
    });

    const recycle = (p) => {
        p.y = -20 - Math.random() * 60;
        p.x = Math.random() * canvas._w;
        p.vy = rand(CONFIG.vyMin, CONFIG.vyMax);
        p.rot = Math.random() * Math.PI * 2;
        p.sway = Math.random() * Math.PI * 2;
        p.flutter = Math.random() * Math.PI * 2;
        p.color = pick(CONFIG.colors);
    };

    const frame = (now) => {
        if (!running) return;
        rafId = requestAnimationFrame(frame);

        // FPS throttle — skip if we're ahead of schedule.
        if (now - lastFrame < frameInterval - 1) return;

        const dtMs = now - lastFrame;
        const dt = Math.min(dtMs / NOMINAL_DT, DT_CAP);   // time-based physics
        lastFrame = now;

        // Reset transform before clearRect (transform persists from last paper).
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        ctx.clearRect(0, 0, canvas._w, canvas._h);

        // ── Pass 1: physics (all papers, once) ──
        for (let i = 0; i < papers.length; i++) {
            const p = papers[i];
            p.vy += CONFIG.gravity * dt;
            p.x += (p.vx + Math.sin(p.sway) * p.swayAmp) * dt;
            p.y += p.vy * dt;
            p.sway += p.swaySpeed * dt;
            p.rot += p.vrot * dt;
            p.flutter += p.flutterSpeed * dt;

            if (p.y > canvas._h + 30) recycle(p);
            if (p.x < -30) p.x = canvas._w + 20;
            else if (p.x > canvas._w + 30) p.x = -20;
        }

        // ── Pass 2: draw, batched by color (6 fillStyle sets, not 50) ──
        for (let c = 0; c < CONFIG.colors.length; c++) {
            const color = CONFIG.colors[c];
            let fillSet = false;

            for (let i = 0; i < papers.length; i++) {
                const p = papers[i];
                if (p.color !== color) continue;

                if (!fillSet) { ctx.fillStyle = color; fillSet = true; }

                const squash = Math.cos(p.flutter);
                const cos = Math.cos(p.rot);
                const sin = Math.sin(p.rot);

                // Single matrix write — replaces save/translate/rotate/scale/restore.
                // Matrix = DPR × Translate(px, py) × Rotate(θ) × Scale(squash, 1)
                ctx.setTransform(
                    dpr * cos * squash, dpr * sin * squash,
                    -dpr * sin, dpr * cos,
                    dpr * p.x, dpr * p.y
                );
                ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h);
            }
        }
    };

    resize();
    requestAnimationFrame(resize);   // second pass catches late layout on Android

    papers = Array.from({ length: profile.count }, (_, i) =>
        i < profile.count / 2
            ? spawn(Math.random() * canvas._h)
            : spawn(-Math.random() * canvas._h)
    );

    lastFrame = performance.now();
    rafId = requestAnimationFrame(frame);

    let resizeTO;
    const scheduleResize = () => {
        clearTimeout(resizeTO);
        resizeTO = setTimeout(resize, 150);
    };
    window.addEventListener('resize', scheduleResize);
    window.addEventListener('orientationchange', () => setTimeout(resize, 200));

    document.addEventListener('visibilitychange', () => {
        if (document.hidden) {
            running = false;
            if (rafId) { cancelAnimationFrame(rafId); rafId = null; }
        } else if (!running) {
            running = true;
            lastFrame = performance.now();
            rafId = requestAnimationFrame(frame);
        }
    });

    window.addEventListener('pagehide', () => {
        running = false;
        if (rafId) cancelAnimationFrame(rafId);
    });
})();