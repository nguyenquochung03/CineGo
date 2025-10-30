const monthRow = document.getElementById('month-row');
const dayRow = document.getElementById('day-row');
const regionList = document.getElementById("region-list");
const cityList = document.getElementById("city-list");
const cinemaList = document.getElementById("cinema-list");
const movieList = document.getElementById("movie-list");

let selectedRegion = null;
let selectedCity = null;
let selectedCinema = null;
const today = new Date();
let selectedDate = today;

// ====== Helper ======
function formatWeekday(date) {
    return date.toLocaleDateString('vi-VN', { weekday: 'short' });
}

function formatMonth(date, isToday) {
    if (isToday || date.getDate() === 1) {
        return `Tháng ${date.getMonth() + 1} Năm ${date.getFullYear()}`;
    } else return '';
}

function formatTime(timeStr) {
    const [h, m] = timeStr.split(':');
    return `${h}:${m}`;
}

// ====== Calendar ======
function renderCalendar() {
    monthRow.innerHTML = '';
    dayRow.innerHTML = '';

    for (let i = 0; i < 7; i++) {
        const d = new Date(today);
        d.setDate(today.getDate() + i);

        // Month
        const monthCell = document.createElement('div');
        monthCell.className = 'month-cell';
        monthCell.textContent = formatMonth(d, i === 0);
        monthRow.appendChild(monthCell);

        // Day
        const dayCell = document.createElement('div');
        dayCell.className = 'day-cell';
        if (d.toDateString() === selectedDate.toDateString()) dayCell.classList.add('selected');

        const weekday = document.createElement('div');
        weekday.className = 'weekday';
        weekday.textContent = formatWeekday(d);

        const dayNumber = document.createElement('div');
        dayNumber.className = 'day-number';
        dayNumber.textContent = d.getDate();

        dayCell.appendChild(weekday);
        dayCell.appendChild(dayNumber);

        dayCell.addEventListener('click', () => {
            selectedDate = d;
            renderCalendar();

            const dateSpan = document.querySelector('#selected-date span');
            dateSpan.textContent = d.toLocaleDateString('vi-VN', { weekday: 'long', day: 'numeric', month: 'numeric', year: 'numeric' });

            if (selectedCinema) loadMovies();
        });

        dayRow.appendChild(dayCell);
    }
}

renderCalendar();

// ====== Region → City → Cinema ======
regionList.querySelectorAll(".region-item").forEach(item => {
    item.addEventListener("click", async () => {
        regionList.querySelectorAll(".region-item").forEach(r => r.classList.remove("active"));
        item.classList.add("active");
        selectedRegion = item.dataset.id;

        const res = await fetch(`/Booking/GetCitiesByRegion?regionId=${selectedRegion}`);
        const cities = await res.json();
        renderCities(cities, true);
    });
});

async function renderCities(cities, autoSelectFirst = false) {
    cityList.innerHTML = "";
    cinemaList.innerHTML = "";

    if (!cities?.length) {
        cityList.innerHTML = `<p class="no-city-msg">Không có thành phố nào trong vùng này</p>`;
        return;
    }

    cities.forEach((c, i) => {
        const div = document.createElement("div");
        div.classList.add("city-item");
        div.textContent = c.name;

        div.addEventListener("click", async () => {
            document.querySelectorAll(".city-item").forEach(ci => ci.classList.remove("active"));
            div.classList.add("active");
            selectedCity = c.id;

            const res = await fetch(`/Booking/GetCinemasByCity?cityId=${selectedCity}`);
            const cinemas = await res.json();
            renderCinemas(cinemas);
        });

        cityList.appendChild(div);
    });

    if (autoSelectFirst && cities.length > 0) {
        const firstCity = cityList.querySelector(".city-item");
        firstCity.classList.add("active");
        selectedCity = cities[0].id;

        const res = await fetch(`/Booking/GetCinemasByCity?cityId=${selectedCity}`);
        const cinemas = await res.json();
        renderCinemas(cinemas);
    }
}

function renderCinemas(cinemas) {
    cinemaList.innerHTML = "";

    if (!cinemas?.length) {
        cinemaList.innerHTML = `<p class="no-cinema-msg">Không có rạp nào trong thành phố này</p>`;
        return;
    }

    cinemas.forEach(c => {
        const div = document.createElement("div");
        div.classList.add("cinema-card");
        div.textContent = c.name;

        div.addEventListener("click", () => {
            document.querySelectorAll(".cinema-card").forEach(ci => ci.classList.remove("active"));
            div.classList.add("active");
            selectedCinema = c.id;

            const cinemaSpan = document.querySelector('#selected-cinema span');
            cinemaSpan.textContent = c.name;

            loadMovies();
        });

        cinemaList.appendChild(div);
    });
}

// ====== On load ======
document.addEventListener("DOMContentLoaded", async () => {
    movieList.innerHTML = `<p class="no-schedule-msg">Không có lịch chiếu phù hợp. Vui lòng chọn rạp.</p>`;

    const firstRegion = regionList.querySelector(".region-item");
    if (firstRegion) {
        firstRegion.classList.add("active");
        selectedRegion = firstRegion.dataset.id;

        const res = await fetch(`/Booking/GetCitiesByRegion?regionId=${selectedRegion}`);
        const cities = await res.json();
        renderCities(cities, true);
    }

    const dateSpan = document.querySelector('#selected-date span');
    dateSpan.textContent = selectedDate.toLocaleDateString('vi-VN', { weekday: 'long', day: 'numeric', month: 'numeric', year: 'numeric' });
});

// ====== Load Movies ======
async function loadMovies() {
    if (!selectedCinema) {
        movieList.innerHTML = `<p class="no-schedule-msg">Không có lịch chiếu phù hợp. Vui lòng chọn rạp.</p>`;
        return;
    }

    movieList.innerHTML = `
        <div class="loading-msg text-center py-4">
            <i class="fas fa-spinner fa-spin me-2"></i> Đang tải danh sách phim...
        </div>
    `;

    try {
        const res = await fetch(`/Booking/GetMoviesByCinemaAndDate?cinemaId=${selectedCinema}&date=${selectedDate.toISOString()}`);
        const movies = await res.json();
        await renderMovies(movies);
    } catch (err) {
        console.error("Lỗi khi load movies:", err);
        movieList.innerHTML = `<p class="text-center text-danger fst-italic">Đã xảy ra lỗi khi tải phim. Vui lòng thử lại sau.</p>`;
    }
}

async function renderMovies(movies) {
    movieList.innerHTML = "";

    if (!selectedCinema) {
        movieList.innerHTML = `<p class="no-schedule-msg">Không có lịch chiếu phù hợp. Vui lòng chọn rạp.</p>`;
        return;
    }

    if (!movies.length) {
        movieList.innerHTML = `<p class="no-schedule-cinema-msg">Không có lịch chiếu phù hợp tại rạp này.</p>`;
        return;
    }

    for (const m of movies) {
        const div = document.createElement("div");
        div.classList.add("movie-item", "mb-4");

        div.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h6 class="fw-bold">${m.ageLimit}+ ${m.title}</h6>
                <a href="/Movie/Detail/${m.slug}" class="movie-detail-link">
                    <i class="fas fa-circle-play"></i>
                </a>
            </div>
            <div class="showtime-list d-flex flex-wrap gap-2" id="showtime-${m.id}"></div>
        `;

        movieList.appendChild(div);

        const res = await fetch(`/Booking/GetShowtimesByMovieAndDate?movieId=${m.id}&date=${selectedDate.toISOString()}`);
        const showtimes = await res.json();

        const showtimeContainer = document.getElementById(`showtime-${m.id}`);
        for (const s of showtimes) {
            const seatRes = await fetch(`/Booking/GetSeatStatus?showtimeId=${s.id}`);
            const seat = await seatRes.json();

            const stDiv = document.createElement("div");
            stDiv.classList.add("showtime-card", "p-2", "border", "rounded");
            stDiv.innerHTML = `
                <div class="small fw-bold">${s.theaters[0].theaterName}</div>
                <div>${formatTime(s.startTime)}</div>
                <div class="text-muted small">${seat.bookedSeats}/${seat.totalSeats} ghế</div>
            `;

            showtimeContainer.appendChild(stDiv);
        }
    }
}
