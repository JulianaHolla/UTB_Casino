// Number field only for single-number bets; the ball ticks through numbers before settling.
(function () {
  var type = document.getElementById("BetType");
  var group = document.getElementById("number-group");
  if (type && window.selectedBetType) type.value = window.selectedBetType;
  function sync() { if (type && group) group.style.display = type.value === "number" ? "" : "none"; }
  if (type) { type.addEventListener("change", sync); sync(); }

  var ball = document.getElementById("roulette-result");
  if (!ball || window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  var final = ball.dataset.number, stopAt = Date.now() + 1200;
  var timer = setInterval(function () {
    if (Date.now() >= stopAt) { clearInterval(timer); ball.textContent = final; }
    else ball.textContent = Math.floor(Math.random() * 37);
  }, 60);
})();
