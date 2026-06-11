

; (function () {
    'use strict';
    if (window.__invAdminThemeLoaded) return;
    window.__invAdminThemeLoaded = true;
    const THEME_KEY = 'inv-admin-theme';
    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
        if (window._salesChart) updateChartTheme(window._salesChart, theme);
        if (window._categoryChart) updateChartTheme(window._categoryChart, theme);
    }

    function toggleTheme() {
        const current = document.documentElement.getAttribute('data-theme') || 'light';
        applyTheme(current === 'light' ? 'dark' : 'light');
    }

    (function () {
        const saved = localStorage.getItem(THEME_KEY);
        if (saved) {
            applyTheme(saved);
        } else {
            const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
            applyTheme(prefersDark ? 'dark' : 'light');
        }
    })();


    function toggleSidebar() {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('sidebarOverlay');
        const isOpen = sidebar && sidebar.classList.contains('open');

        if (sidebar) sidebar.classList.toggle('open', !isOpen);
        if (overlay) overlay.classList.toggle('open', !isOpen);
        document.body.style.overflow = isOpen ? '' : 'hidden';
    }

    document.addEventListener('DOMContentLoaded', function () {
        var path = window.location.pathname.toLowerCase();
        document.querySelectorAll('.nav-item').forEach(function (link) {
            var href = (link.getAttribute('href') || '').toLowerCase();
            if (href && href !== '/' && path.startsWith(href)) {
                link.classList.add('active');
            } else if (href === '/' && path === '/') {
                link.classList.add('active');
            }
        });
    });
    window.toggleTheme = toggleTheme;
    window.toggleSidebar = toggleSidebar;

})(); // end IIFE