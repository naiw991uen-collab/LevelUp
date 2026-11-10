/* =====================================================================
   theme.js · الوضع الداكن / الفاتح
   لازم يتحمّل في <head> من غير defer ولا async، وبعد ملفات الـ CSS:
     <script src="../js/theme.js"></script>
   الجزء الأول بيطبق الثيم المحفوظ على <html> فوراً قبل ما الصفحة تترسم
   (عشان متحصلش ومضة بين الثيمين)، والجزء التاني بيربط زرار #themeToggle
   بعد ما الصفحة تحمّل.
   ===================================================================== */
(function () {
  'use strict';

  var KEY = 'levelup-theme';
  var root = document.documentElement;

  function read() {
    try { return localStorage.getItem(KEY) === 'light' ? 'light' : 'dark'; }
    catch (e) { return 'dark'; }
  }

  function apply(theme) {
    root.setAttribute('data-theme', theme);
  }

  /* ===== 1) يتنفذ فوراً في <head> ===== */
  apply(read());

  /* ===== 2) زرار التبديل ===== */
  function current() {
    return root.getAttribute('data-theme') === 'light' ? 'light' : 'dark';
  }

  function syncButton() {
    var btn = document.getElementById('themeToggle');
    if (!btn) return;
    var text = current() === 'dark' ? 'تفعيل الوضع الفاتح' : 'تفعيل الوضع الداكن';
    btn.setAttribute('aria-label', text);
    btn.setAttribute('title', text);
  }

  function toggle() {
    var next = current() === 'dark' ? 'light' : 'dark';
    apply(next);
    try { localStorage.setItem(KEY, next); } catch (e) { }
    syncButton();
  }

  document.addEventListener('DOMContentLoaded', function () {
    syncButton();
    document.addEventListener('click', function (e) {
      if (e.target.closest && e.target.closest('#themeToggle')) toggle();
    });
  });

  /* لو الثيم اتغير من تاب تانية، طبّقه هنا كمان */
  window.addEventListener('storage', function (e) {
    if (e.key !== KEY) return;
    apply(e.newValue === 'light' ? 'light' : 'dark');
    syncButton();
  });
})();
