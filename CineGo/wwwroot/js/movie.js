let currentPage = 1;
let currentTab = 'nowShowing';
const initialPageSize = 8;
const loadMorePageSize = 8;

document.addEventListener('DOMContentLoaded', function () {
    loadMovies(initialPageSize);

    const btnNowShowing = document.getElementById('btnNowShowing');
    const btnComingSoon = document.getElementById('btnComingSoon');
    const btnLoadMore = document.getElementById('btnLoadMore');

    const movieContainer = document.getElementById('movieContainer');

    if (!btnNowShowing || !btnComingSoon || !btnLoadMore || !movieContainer) {
        console.error('Movie elements not found in DOM!');
        return;
    }

    btnNowShowing.addEventListener('click', function () {
        currentTab = 'nowShowing';
        currentPage = 1;
        movieContainer.innerHTML = '';
        btnLoadMore.style.display = 'none';
        btnNowShowing.classList.add('active');
        btnComingSoon.classList.remove('active');
        loadMovies(initialPageSize);
    });

    btnComingSoon.addEventListener('click', function () {
        currentTab = 'comingSoon';
        currentPage = 1;
        movieContainer.innerHTML = '';
        btnLoadMore.style.display = 'none';
        btnComingSoon.classList.add('active');
        btnNowShowing.classList.remove('active');
        loadMovies(initialPageSize);
    });

    btnLoadMore.addEventListener('click', function () {
        currentPage++;
        loadMovies(loadMorePageSize);
    });
});

document.addEventListener('click', function (e) {
    if (e.target.classList.contains('btn-detail')) {
        const movieId = e.target.getAttribute('data-id');
        if (movieId) {
            window.location.href = `/MovieDetail/Detail/${movieId}`;
        }
    }

    if (e.target.classList.contains('btn-ticket')) {
        window.location.href = `/Booking/Index`;
    }
});

function loadMovies(pageSize) {
    showLoading();
    let url = currentTab === 'nowShowing' ? '/Movie/NowShowing' : '/Movie/ComingSoon';

    fetch(`${url}?page=${currentPage}&pageSize=${pageSize}`)
        .then(res => res.json())
        .then(res => {
            const container = document.getElementById('movieContainer');

            if (res.success && res.data.items.length > 0) {
                res.data.items.forEach(movie => {
                    let firstPoster = movie.posters.length > 0 ? movie.posters[0].url : '/images/default-movie.png';
                    let releaseDate = new Date(movie.releaseDate).toLocaleDateString('vi-VN');

                    //let html = `
                    //    <div class="movie-card">
                    //        <div class="poster-wrapper">
                    //            <img src="${firstPoster}" alt="${movie.title}" />
                    //            <div class="movie-overlay">
                    //                ${currentTab === 'nowShowing'
                    //        ? `<button class="btn-movie-action btn-ticket">Đặt vé</button>`
                    //        : ''
                    //                 }
                    //                <button class="btn-movie-action btn-detail" data-id="${movie.id}">Chi tiết</button>
                    //            </div>
                    //        </div>

                    //        <div class="movie-title" title="${movie.title}">
                    //            <span class="movie-age age-${movie.ageLimit}">${movie.ageLimit}</span>
                    //            ${movie.title}
                    //        </div>
                    //        <div class="movie-info">${movie.runtime} phút | ${releaseDate}</div>
                    //    </div>
                    //`;
                    //container.insertAdjacentHTML('beforeend', html);
                });

                if (currentPage < res.data.totalPages) {
                    document.getElementById('btnLoadMore').style.display = 'inline-block';
                } else {
                    document.getElementById('btnLoadMore').style.display = 'none';
                }
            } else {
                if (currentPage === 1) {
                    container.innerHTML = '<p class="no-movies">Chưa có phim nào.</p>';
                }
                document.getElementById('btnLoadMore').style.display = 'none';
            }
        })
        .catch(err => {
            console.error('Error loading movies:', err);
        })
        .finally(() => {
            hideLoading();
        });
}