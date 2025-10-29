const monthRow = document.getElementById('month-row');
const dayRow = document.getElementById('day-row');

// Region → City → Cinema logic
const regionList = document.getElementById("region-list");
const cityList = document.getElementById("city-list");
const cinemaList = document.getElementById("cinema-list");

let selectedRegion = null;
let selectedCity = null;
let selectedCinema = null;

const today = new Date();
let selectedDate = today;

function formatWeekday(date) {
    return date.toLocaleDateString('vi-VN', { weekday: 'short' });
}

function formatMonth(date, isToday) {
    if (isToday || date.getDate() === 1) {
        return `Tháng ${date.getMonth() + 1} Năm ${date.getFullYear()}`;
    } else {
        return '';
    }
}

function renderCalendar() {
    monthRow.innerHTML = '';
    dayRow.innerHTML = '';

    for (let i = 0; i < 7; i++) {
        const d = new Date(today);
        d.setDate(today.getDate() + i);

        // Month row
        const monthCell = document.createElement('div');
        monthCell.className = 'month-cell';
        monthCell.textContent = formatMonth(d, i === 0);
        monthRow.appendChild(monthCell);

        // Day row
        const dayCell = document.createElement('div');
        dayCell.className = 'day-cell';
        if (d.toDateString() === selectedDate.toDateString()) {
            dayCell.classList.add('selected');
        }

        const weekday = document.createElement('div');
        weekday.className = 'weekday';
        weekday.textContent = formatWeekday(d);

        const dayNumber = document.createElement('div');
        dayNumber.className = 'day-number';
        dayNumber.textContent = d.getDate();

        dayCell.appendChild(weekday);
        dayCell.appendChild(dayNumber);

        // click chọn ngày
        dayCell.addEventListener('click', () => {
            selectedDate = d;
            renderCalendar();

            if (typeof loadMovies === "function" && selectedCinema) loadMovies();

            // cập nhật hiển thị ngày đã chọn
            const dateSpan = document.querySelector('#selected-date span');
            dateSpan.textContent = d.toLocaleDateString('vi-VN', { weekday: 'long', day: 'numeric', month: 'numeric', year: 'numeric' });
        });

        dayRow.appendChild(dayCell);
    }
}

// lần đầu render
renderCalendar();

// Khi click chọn region
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

    if (!cities || cities.length === 0) {
        cityList.innerHTML = `<p class="no-data-message">Không có thành phố nào trong vùng này</p>`;
        return;
    }

    cities.forEach((c, index) => {
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

    // 🔥 Nếu có city và được phép auto select → chọn city đầu tiên luôn
    if (autoSelectFirst && cities.length > 0) {
        const firstCity = cityList.querySelector(".city-item");
        if (firstCity) {
            firstCity.classList.add("active");
            selectedCity = cities[0].id;

            const res = await fetch(`/Booking/GetCinemasByCity?cityId=${selectedCity}`);
            const cinemas = await res.json();
            renderCinemas(cinemas);
        }
    }
}

function renderCinemas(cinemas) {
    cinemaList.innerHTML = "";

    if (!cinemas || cinemas.length === 0) {
        cinemaList.innerHTML = `<p class="no-data-message">Không có rạp nào trong thành phố này</p>`;
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

            // cập nhật hiển thị rạp đã chọn
            const cinemaSpan = document.querySelector('#selected-cinema span');
            cinemaSpan.textContent = c.name;

            if (typeof loadMovies === "function") loadMovies();
        });

        cinemaList.appendChild(div);
    });
}

// 🧠 Khi trang load → tự chọn region đầu tiên và load city + cinema đầu tiên
document.addEventListener("DOMContentLoaded", async () => {
    // Movie list mặc định khi chưa chọn rạp
    const movieList = document.getElementById("movie-list");
    movieList.innerHTML = `
        <p class="text-center text-muted fst-italic">
            Không có lịch chiếu phù hợp. Vui lòng chọn rạp.
        </p>
    `;

    // Tự chọn region đầu tiên và load city + cinema đầu tiên
    const firstRegion = regionList.querySelector(".region-item");
    if (firstRegion) {
        firstRegion.classList.add("active");
        selectedRegion = firstRegion.dataset.id;

        const res = await fetch(`/Booking/GetCitiesByRegion?regionId=${selectedRegion}`);
        const cities = await res.json();
        renderCities(cities, true);
    }

    // Cập nhật hiển thị ngày đã chọn
    const dateSpan = document.querySelector('#selected-date span');
    dateSpan.textContent = selectedDate.toLocaleDateString('vi-VN', {
        weekday: 'long',
        day: 'numeric',
        month: 'numeric',
        year: 'numeric'
    });
});


async function renderMovies(movies) {
    const movieList = document.getElementById("movie-list");
    movieList.innerHTML = "";

    // Nếu chưa chọn rạp
    if (!selectedCinema) {
        movieList.innerHTML = `
            <p class="text-center text-muted fst-italic">
                Không có lịch chiếu phù hợp. Vui lòng chọn rạp.
            </p>
        `;
        return;
    }

    for (const m of movies) {
        const div = document.createElement("div");
        div.classList.add("movie-item", "mb-4");

        div.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h6 class="fw-bold">${m.ageLimit}+ ${m.title}</h6>
                <a href="/Movie/Detail/${m.slug}" class="movie-detail-link">
                    <i class="fas fa-chevron-right"></i>
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
                <div>${s.startTime}</div>
                <div class="text-muted small">${seat.bookedSeats}/${seat.totalSeats} ghế</div>
            `;
            showtimeContainer.appendChild(stDiv);
        }
    }

    // Nếu có rạp nhưng không có phim hoặc lịch chiếu
    if (movies.length === 0) {
        movieList.innerHTML = `
            <p class="text-center text-muted fst-italic">
                Không có lịch chiếu phù hợp tại rạp này.
            </p>
        `;
    }
}
