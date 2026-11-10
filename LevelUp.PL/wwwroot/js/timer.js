/* =====================================================================
   timer.js · عدادات صفحة الأجهزة (Device/Index) بس
   - الكارت الشغال (.device-card[data-elapsed]):
       data-elapsed          ثواني الجلسة كلها لحظة رسم الصفحة
       data-segment-elapsed  ثواني الجزء الحالي (من آخر تحويل) لحظة رسم الصفحة
       data-base-cost        سعر الأجزاء اللي خلصت قبل آخر تحويل (ج.م)
       data-rate             سعر ساعة الجزء الحالي (ج.م)
     العداد = data-elapsed + الوقت من فتح الصفحة
     السعر  = data-base-cost + سعر (data-segment-elapsed + الوقت من فتح الصفحة)
              والحساب بالدقيقة: أي جزء من دقيقة بيتحسب دقيقة.
   - الكارت المستني الحساب: data-stopped="true" والعداد والسعر ثابتين.
   - الكارت الفاضي: لما "هيلعب إيه؟" يتغير، زرار [data-start-label] وسطر [data-mode-rate]
     بيتغيروا على حسب data-label و data-rate بتوع الـ option المختار.
   مفيش فلترة هنا: الفلترة في السيرفر (asp-route-filter).
   ===================================================================== */
(function () {
  'use strict';

  function pad(n) { return String(n).padStart(2, '0'); }

  function clock(sec) {
    sec = Math.max(0, Math.floor(sec));
    return pad(Math.floor(sec / 3600)) + ':' + pad(Math.floor(sec % 3600 / 60)) + ':' + pad(sec % 60);
  }

  function hoursMinutes(sec) {
    var m = Math.floor(Math.max(0, sec) / 60);
    return Math.floor(m / 60) + ':' + pad(m % 60);
  }

  function playCost(sec, rate) {
    return Math.round(Math.ceil(Math.max(0, sec) / 60) * rate / 60);
  }

  function money(n) { return Math.round(n).toLocaleString('en-US') + ' ج.م'; }

  /* ===== 1) العداد ===== */
  var openedAt = Date.now();
  var cards = [].slice.call(document.querySelectorAll('.device-card[data-elapsed]'))
    .filter(function (c) { return c.getAttribute('data-stopped') !== 'true'; });

  function tick() {
    var gone = (Date.now() - openedAt) / 1000;
    cards.forEach(function (card) {
      var total = Number(card.getAttribute('data-elapsed')) + gone;
      var segment = Number(card.getAttribute('data-segment-elapsed')) + gone;
      var base = Number(card.getAttribute('data-base-cost')) || 0;
      var rate = Number(card.getAttribute('data-rate')) || 0;

      var timer = card.querySelector('.timer');
      if (timer) timer.textContent = clock(total);

      var cost = card.querySelector('[data-cost]');
      if (cost) cost.textContent = money(base + playCost(segment, rate));

      var live = card.querySelector('[data-segment-live]');
      if (live) live.textContent = hoursMinutes(segment);
    });
  }

  if (cards.length) {
    tick();
    setInterval(tick, 1000);
  }

  /* ===== 2) الكارت الفاضي: لما "هيلعب إيه؟" يتغير =====
     السيرفر بيرسم الكلام على حسب النمط الافتراضي للغرفة، وده بيغيّره بس لما الاختيار يتغير.
     كل option عليه data-label و data-rate. */
  document.addEventListener('change', function (e) {
    var select = e.target;
    if (!select.matches || !select.matches('.modepick select')) return;
    var card = select.closest('.device-card');
    var option = select.options[select.selectedIndex];
    if (!card || !option) return;
    var label = option.getAttribute('data-label');
    var rate = option.getAttribute('data-rate');

    var start = card.querySelector('[data-start-label]');
    if (start) start.textContent = 'بدء ' + label;

    var modeRate = card.querySelector('[data-mode-rate]');
    if (modeRate) modeRate.textContent = label + ' ' + rate + ' ج.م/ساعة';
  });
})();
