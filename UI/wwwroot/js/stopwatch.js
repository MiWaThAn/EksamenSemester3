let startTime =0;
let elapsedTime = 0;
let timerId =null;

const display = document.getElementById("display");
const startBtn = document.getElementById("startBtn");
const stopBtn = document.getElementById("stopBtn");
const resetBtn = document.getElementById("resetBtn");

function formatTime(ms) {
	const totalTenths = Math.floor(ms / 100);
	const hours = Math.floor(totalTenths / 36000);
	const minutes = Math.floor((totalTenths % 36000) / 600);
	const seconds = Math.floor((totalTenths % 600) / 10);
	const tenths = totalTenths % 10;

return (
	String(hours).padStart(2, '0') + ":" +
	String(minutes).padStart(2, '0') + ":" +
	String(seconds).padStart(2, '0') + "." +
	tenths
);
}

function updateDisplay() {
	display.textContent = formatTime(elapsedTime);
}

startBtn.addEventListener("click", function () {
	if (timerId !== null) return; // Already running

	startTime = Date.now() - elapsedTime;

	timerId = setInterval(function () {
		elapsedTime = Date.now() - startTime;
		updateDisplay();
	}, 100);
});

stopBtn.addEventListener("click", function () {
	if (timerId === null) return; // Not running

	clearInterval(timerId);
	timerId = null;
});

resetBtn.addEventListener("click", function () {
	clearInterval(timerId;
		timerId = null;
		startTime = 0;
		elapsedTime = 0;
		updateDisplay();
});

updateDisplay();