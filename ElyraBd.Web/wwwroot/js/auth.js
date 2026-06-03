(function () {
    document.querySelectorAll('[data-toggle-password]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = document.querySelector(btn.getAttribute('data-target'));
            if (!input) return;

            var isPassword = input.type === 'password';
            input.type = isPassword ? 'text' : 'password';

            var icon = btn.querySelector('i');
            if (icon) {
                icon.classList.toggle('bi-eye', !isPassword);
                icon.classList.toggle('bi-eye-slash', isPassword);
            }
        });
    });
})();
