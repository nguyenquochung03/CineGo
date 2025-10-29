let countdownSeconds = 300; // 5 phút = 300s
let timer;

const resendLink = document.getElementById("resendLink");
const countdown = document.getElementById("countdown");

function startCountdown() {
    resendLink.classList.add("disabled");
    updateCountdownText();

    timer = setInterval(() => {
        countdownSeconds--;
        updateCountdownText();

        if (countdownSeconds <= 0) {
            clearInterval(timer);
            countdown.textContent = "";
            resendLink.classList.remove("disabled");
            resendLink.textContent = "Nhận lại mã";
        }
    }, 1000);
}

function updateCountdownText() {
    let minutes = Math.floor(countdownSeconds / 60);
    let seconds = countdownSeconds % 60;
    countdown.textContent = `(${minutes}:${seconds.toString().padStart(2, '0')})`;
}

function submitResend(e) {
    e.preventDefault();
    if (resendLink.classList.contains("disabled")) return;

    document.getElementById("resendForm").submit();
    countdownSeconds = 300; // reset 5 phút
    startCountdown();
}

// Auto start countdown khi vừa mở form
window.onload = startCountdown;