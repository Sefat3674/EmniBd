(function () {
    var header = document.getElementById('elyraHeader');
    if (!header) return;

    function onScroll() {
        header.classList.toggle('is-scrolled', window.scrollY > 8);
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
})();
