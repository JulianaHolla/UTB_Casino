// Spins the reels for a moment after a bet, then settles on the server result.
(function () {
  var box = document.getElementById("reels");
  if (!box || box.dataset.spin !== "true") return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

  var symbols = ["🍒", "🍋", "🔔", "⭐", "7️⃣"];
  var reels = Array.prototype.slice.call(box.querySelectorAll(".reel"));

  reels.forEach(function (reel, i) {
    var stopAt = Date.now() + 600 + i * 350;
    var timer = setInterval(function () {
      if (Date.now() >= stopAt) {
        clearInterval(timer);
        reel.textContent = reel.dataset.final;
      } else {
        reel.textContent = symbols[Math.floor(Math.random() * symbols.length)];
      }
    }, 70);
  });
})();
