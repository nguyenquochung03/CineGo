document.addEventListener('DOMContentLoaded', function () {
    const stars = document.querySelectorAll('.star-select');
    const btnSubmit = document.getElementById('btnSubmitReview');
    let selectedRating = 0;

    stars.forEach(star => {
        star.addEventListener('click', function () {
            selectedRating = parseInt(this.dataset.value);
            stars.forEach(s => s.classList.remove('active'));
            for (let i = 0; i < selectedRating; i++) {
                stars[i].classList.add('active');
            }
        });
    });

    btnSubmit.addEventListener('click', function () {
        const content = document.getElementById('reviewContent').value.trim();
        if (!selectedRating || !content) {
            alert("Vui lòng chọn số sao và nhập nội dung đánh giá!");
            return;
        }
        alert(`Đánh giá thành công!\n⭐ ${selectedRating} sao\nNội dung: ${content}`);
        document.getElementById('reviewContent').value = '';
        stars.forEach(s => s.classList.remove('active'));
        selectedRating = 0;
    });
});
