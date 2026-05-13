let startTime = 0;
let elapsedTime = 0;
let timerId = null;

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
    const display = document.getElementById("display");
    display.textContent = formatTime(elapsedTime);
}

export function startStopwatch() {
    if (timerId !== null) return;
    startTime = Date.now() - elapsedTime;
    timerId = setInterval(function () {
        elapsedTime = Date.now() - startTime;
        updateDisplay();
    }, 100);
}

export function stopStopwatch() {
    if (timerId === null) return;
    clearInterval(timerId);
    timerId = null;
}

export function resetStopwatch() {
    clearInterval(timerId);
    timerId = null;
    startTime = 0;
    elapsedTime = 0;
    updateDisplay();
}

export function initializeStopwatch() {
    updateDisplay();
}