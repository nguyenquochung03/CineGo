document.addEventListener('DOMContentLoaded', function () {
    const textarea = document.getElementById('reviewContent');
    const charCount = document.getElementById('charCount');
    const maxLength = 220;

    textarea.addEventListener('input', function () {
        let length = textarea.value.length;

        // Giới hạn không cho nhập quá maxLength
        if (length > maxLength) {
            textarea.value = textarea.value.substring(0, maxLength);
            length = maxLength;
        }

        // Cập nhật số ký tự
        charCount.textContent = `${length} / ${maxLength} Ký Tự`;

        // Đổi màu cảnh báo khi gần đạt giới hạn
        if (length > maxLength * 0.9) {
            charCount.classList.remove('text-muted');
            charCount.classList.add('text-danger');
        } else {
            charCount.classList.remove('text-danger');
            charCount.classList.add('text-muted');
        }
    });
});

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".rating-group").forEach(group => {
        const stars = group.querySelectorAll(".rating-stars i");
        const scoreSpan = group.querySelector(".rating-score");
        let selectedValue = 0;

        stars.forEach((star, index) => {
            star.addEventListener("mousemove", (e) => {
                const rect = star.getBoundingClientRect();
                const offsetX = e.clientX - rect.left;
                const isHalf = offsetX < rect.width / 2;
                clearHover();
                highlightTemp(index, isHalf);
                scoreSpan.textContent = isHalf
                    ? `${(index * 2) + 1} điểm`
                    : `${(index + 1) * 2} điểm`;
            });

            star.addEventListener("mouseleave", () => {
                clearHover();
                highlightSelected(selectedValue);
                scoreSpan.textContent = selectedValue
                    ? `${selectedValue} điểm`
                    : "0 điểm";
            });

            star.addEventListener("click", (e) => {
                const rect = star.getBoundingClientRect();
                const offsetX = e.clientX - rect.left;
                const isHalf = offsetX < rect.width / 2;
                selectedValue = isHalf ? (index * 2) + 1 : (index + 1) * 2;
                highlightSelected(selectedValue);
                scoreSpan.textContent = `${selectedValue} điểm`;
            });
        });

        function clearHover() {
            stars.forEach(s => s.className = "bi bi-star");
        }

        function highlightTemp(index, isHalf) {
            stars.forEach((s, i) => {
                s.className = "bi bi-star";
                if (i < index) s.className = "bi bi-star-fill";
            });
            if (isHalf) stars[index].className = "bi bi-star-half";
            else stars[index].className = "bi bi-star-fill";
        }

        function highlightSelected(value) {
            stars.forEach((s, i) => {
                const starValue = (i + 1) * 2;
                s.className = starValue <= value ? "bi bi-star-fill" : "bi bi-star";
            });
            if (value % 2 === 1) {
                const index = Math.floor(value / 2);
                stars[index].className = "bi bi-star-half";
            }
        }
    });
});

document.querySelectorAll('.like-button').forEach(btn => {
    btn.addEventListener('click', () => {
        const icon = btn.querySelector('.like-icon');
        const liked = btn.getAttribute('data-liked') === 'true';
        btn.setAttribute('data-liked', !liked);
        icon.textContent = liked ? '🤍' : '❤️';
    });
});

