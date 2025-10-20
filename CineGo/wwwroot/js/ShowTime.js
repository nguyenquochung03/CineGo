// --- AUTO LOAD ---
document.addEventListener('DOMContentLoaded', () => loadShowtimes());

// Khi modal showtimeModal được mở
const showtimeModalEl = document.getElementById('showtimeModal');

showtimeModalEl.addEventListener('shown.bs.modal', () => {
    const firstInput = showtimeModalEl.querySelector('#movieSearch');
    if (firstInput) firstInput.focus();
});

// --- Khi modal ShowtimePrice được mở ---
const showtimePriceModalEl = document.getElementById('showtimePriceModal');
showtimePriceModalEl.addEventListener('shown.bs.modal', () => {
    const firstInput = showtimePriceModalEl.querySelector('#ticketType');
    if (firstInput) firstInput.focus();
});

let currentShowtimeId = null;
let currentShowtimeTitle = "";

// --- FETCH ---
async function fetchJson(url, method = 'GET', data = null) {
    const options = { method, headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' } };
    if (data && (method === 'POST' || method === 'PUT')) options.body = JSON.stringify(data);
    const res = await fetch(url, options);
    if (!res.ok) try { return await res.json(); } catch { return { success: false, message: res.statusText }; }
    return await res.json();
}

async function fetchHtml(url) {
    const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
    if (!res.ok) throw new Error(res.statusText);
    return await res.text();
}

// --- LOAD SHOWTIME ---
async function loadShowtimes(page = 1) {
    try {
        const html = await fetchHtml(`/Admin/Showtime/List?page=${page}`);
        document.getElementById("content-area").innerHTML = html;
        document.getElementById("breadcrumb").innerHTML = '<li class="breadcrumb-item active">🎬 Suất chiếu</li>';

        // --- Re-init autocomplete cho filterMovie ---
        const filterMovieEl = document.getElementById('filterMovie');
        if (filterMovieEl) {
            initAutocomplete(
                filterMovieEl,
                document.getElementById('filterMovieId'),
                document.getElementById('filterMovieSuggestions')
            );
        }
    } catch (err) {
        console.error(err);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tải danh sách suất chiếu.</div>`;
    }
}

// --- MODAL SHOWTIME ---
function openShowtimeModal(id = null, movieId = '', movieTitle = '', date = '', start = '', end = '', format = '') {
    const modalEl = document.getElementById('showtimeModal');
    const modal = new bootstrap.Modal(modalEl);
    modalEl.querySelector('#showtimeModalLabel').innerText = id ? 'Cập nhật Suất Chiếu' : 'Tạo Suất Chiếu';
    modalEl.querySelector('#showtimeId').value = id || '';
    modalEl.querySelector('#movieId').value = movieId || '';
    modalEl.querySelector('#movieSearch').value = movieTitle || '';
    modalEl.querySelector('#showtimeDate').value = date || '';
    modalEl.querySelector('#startTime').value = start || '';
    modalEl.querySelector('#endTime').value = end || '';
    modalEl.querySelector('#format').value = format || '';

    // Reset trạng thái validation
    const form = modalEl.querySelector('#showtimeForm');
    form.classList.remove('was-validated');

    modalEl.querySelectorAll('.form-control').forEach(input =>
        input.classList.remove('is-invalid')
    );

    modalEl.querySelectorAll('.invalid-feedback').forEach(el => {
        el.style.display = 'none';
    });

    // Xóa gợi ý autocomplete cũ (nếu có)
    const suggestions = modalEl.querySelector('#movieSuggestions');
    if (suggestions) suggestions.innerHTML = '';

    modal.show();
}

// --- SAVE SHOWTIME ---
document.getElementById('showtimeSaveBtn').addEventListener('click', async () => {
    const form = document.getElementById('showtimeForm');
    let hasError = false;

    // Reset tất cả lỗi trước
    document.querySelectorAll('.invalid-feedback').forEach(el => el.style.display = 'none');
    document.querySelectorAll('.form-control').forEach(el => el.classList.remove('is-invalid'));

    const movieId = document.getElementById('movieId').value;
    const dateValue = document.getElementById('showtimeDate').value;
    const startTime = document.getElementById('startTime').value;
    const endTime = document.getElementById('endTime').value;

    // Kiểm tra required fields
    if (!movieId) {
        const input = document.getElementById('movieSearch');
        const errorDiv = document.getElementById('movieSearchError') || document.createElement('div');
        errorDiv.id = 'movieSearchError';
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = 'Vui lòng chọn phim.';
        input.classList.add('is-invalid');
        if (!errorDiv.parentElement) input.parentElement.appendChild(errorDiv);
        errorDiv.style.display = 'block';
        hasError = true;
    }

    if (!dateValue) {
        const input = document.getElementById('showtimeDate');
        const errorDiv = document.getElementById('showtimeDateError');
        input.classList.add('is-invalid');
        errorDiv.textContent = 'Vui lòng chọn ngày chiếu.';
        errorDiv.style.display = 'block';
        hasError = true;
    } else {
        const selectedDate = new Date(dateValue);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        if (selectedDate < today) {
            const input = document.getElementById('showtimeDate');
            const errorDiv = document.getElementById('showtimeDateError');
            input.classList.add('is-invalid');
            errorDiv.textContent = 'Ngày chiếu không thể là quá khứ.';
            errorDiv.style.display = 'block';
            hasError = true;
        }
    }

    if (!startTime) {
        const input = document.getElementById('startTime');
        const errorDiv = document.getElementById('startTimeError');
        input.classList.add('is-invalid');
        errorDiv.textContent = 'Vui lòng nhập giờ bắt đầu.';
        errorDiv.style.display = 'block';
        hasError = true;
    }

    if (!endTime) {
        const input = document.getElementById('endTime');
        const errorDiv = document.getElementById('endTimeError');
        input.classList.add('is-invalid');
        errorDiv.textContent = 'Vui lòng nhập giờ kết thúc.';
        errorDiv.style.display = 'block';
        hasError = true;
    }

    if (startTime && endTime && startTime >= endTime) {
        const startInput = document.getElementById('startTime');
        const endInput = document.getElementById('endTime');
        const startError = document.getElementById('startTimeError');
        const endError = document.getElementById('endTimeError');
        startInput.classList.add('is-invalid');
        endInput.classList.add('is-invalid');
        startError.textContent = 'Giờ bắt đầu phải nhỏ hơn giờ kết thúc.';
        endError.textContent = 'Giờ bắt đầu phải nhỏ hơn giờ kết thúc.';
        startError.style.display = 'block';
        endError.style.display = 'block';
        hasError = true;
    }

    if (hasError) return;

    // --- Nếu pass tất cả validation client, gửi request ---
    const id = document.getElementById('showtimeId').value;
    const data = {
        Id: id || 0,
        MovieId: movieId,
        Date: dateValue,
        StartTime: startTime,
        EndTime: endTime,
        Format: document.getElementById('format').value
    };

    const method = id ? 'PUT' : 'POST';
    const result = await fetchJson('/Admin/Showtime/' + (id ? 'Update' : 'Create'), method, data);

    if (result.success) {
        bootstrap.Modal.getInstance(document.getElementById('showtimeModal')).hide();
        loadShowtimes();
    } else {
        alert(result.message || 'Lỗi xử lý');
    }
});

// --- DELETE SHOWTIME ---
async function deleteShowtime(id) {
    if (!confirm('Bạn có chắc muốn xóa suất chiếu này?')) return;
    const result = await fetchJson(`/Admin/Showtime/Delete/${id}`, 'DELETE');
    if (result.success) loadShowtimes();
    else alert(result.message || 'Xóa thất bại');
}

// --- LOAD SHOWTIME PRICE ---
async function loadShowtimePrices(showtimeId, showtimeTitle, page = 1) {
    try {
        const html = await fetchHtml(`/Admin/ShowtimePrice/GetByShowtime?showtimeId=${showtimeId}&page=${page}`);
        document.getElementById("content-area").innerHTML = html;
        document.getElementById("breadcrumb").innerHTML = `
            <li class="breadcrumb-item"><a href="#" onclick="loadShowtimes()">🎬 Suất chiếu</a></li>
            <li class="breadcrumb-item active">${showtimeTitle} - Giá vé</li>
        `;
        currentShowtimeId = showtimeId;
        currentShowtimeTitle = showtimeTitle;
    } catch (err) {
        console.error(err);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tải giá suất chiếu.</div>`;
    }
}

// --- MODAL SHOWTIME PRICE ---
function openShowtimePriceModal(id = null, ticketType = '', seatType = '', price = 0) {
    const modalEl = document.getElementById('showtimePriceModal');
    const modal = new bootstrap.Modal(modalEl);
    modalEl.querySelector('#priceId').value = id || '';
    modalEl.querySelector('#priceShowtimeId').value = currentShowtimeId;
    modalEl.querySelector('#ticketType').value = ticketType;
    modalEl.querySelector('#seatType').value = seatType;
    modalEl.querySelector('#price').value = price;
    modalEl.querySelector('#showtimePriceForm').classList.remove('was-validated');
    modalEl.querySelector('#showtimePriceModalLabel').innerText = id ? 'Cập nhật Giá vé' : 'Thêm Giá vé';
    modal.show();
}

// --- SAVE SHOWTIME PRICE ---
document.getElementById('priceSaveBtn').addEventListener('click', async () => {
    const form = document.getElementById('showtimePriceForm');
    if (!form.checkValidity()) { form.classList.add('was-validated'); return; }

    const data = {
        Id: document.getElementById('priceId').value || 0,
        ShowtimeId: currentShowtimeId,
        TicketType: document.getElementById('ticketType').value,
        SeatType: document.getElementById('seatType').value,
        Price: parseFloat(document.getElementById('price').value)
    };

    const method = data.Id ? 'PUT' : 'POST';
    const result = await fetchJson('/Admin/ShowtimePrice/' + (data.Id ? 'Update' : 'Create'), method, data);
    if (result.success) {
        bootstrap.Modal.getInstance(document.getElementById('showtimePriceModal')).hide();
        loadShowtimePrices(currentShowtimeId, currentShowtimeTitle);
    } else alert(result.message || 'Lỗi xử lý');
});

// --- DELETE SHOWTIME PRICE ---
async function deleteShowtimePrice(id) {
    if (!confirm('Bạn có chắc muốn xóa giá vé này?')) return;
    const result = await fetchJson(`/Admin/ShowtimePrice/Delete/${id}`, 'DELETE');
    if (result.success) loadShowtimePrices(currentShowtimeId, currentShowtimeTitle);
    else alert(result.message || 'Xóa thất bại');
}

// --- SEARCH SHOWTIME ---
async function searchByDate() {
    const date = document.getElementById('filterDate').value;
    if (!date) return alert("Vui lòng chọn ngày.");

    try {
        const html = await fetchHtml(`/Admin/Showtime/GetByDate?date=${date}`);
        document.getElementById("content-area").innerHTML = html;
    } catch (err) {
        console.error(err);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tìm kiếm suất chiếu theo ngày.</div>`;
    }
}

async function searchByDateTimeRange() {
    const date = document.getElementById('filterDate').value;
    const start = document.getElementById('filterStart').value;
    const end = document.getElementById('filterEnd').value;
    if (!date || !start || !end) return alert("Vui lòng chọn ngày và giờ bắt đầu/kết thúc.");

    try {
        // Chuyển start/end sang format TimeSpan "hh:mm:ss"
        const startTs = start + ":00";
        const endTs = end + ":00";

        const html = await fetchHtml(
            `/Admin/Showtime/GetByDateTimeRange?date=${date}&start=${startTs}&end=${endTs}`
        );
        document.getElementById("content-area").innerHTML = html;
    } catch (err) {
        console.error(err);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tìm kiếm suất chiếu theo giờ.</div>`;
    }
}

async function searchByMovieName() {
    const movieName = document.getElementById('filterMovie').value.trim();
    if (!movieName) return alert("Vui lòng nhập tên phim.");

    try {
        const html = await fetchHtml(`/Admin/Showtime/GetByMovieName?movieName=${encodeURIComponent(movieName)}`);
        document.getElementById("content-area").innerHTML = html;
    } catch (err) {
        console.error(err);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tìm kiếm suất chiếu theo tên phim.</div>`;
    }
}

function applySearch() {
    const date = document.getElementById('filterDate').value;
    const start = document.getElementById('filterStart').value;
    const end = document.getElementById('filterEnd').value;
    const movieName = document.getElementById('filterMovie').value.trim();

    // Nếu nhập movie name
    if (movieName) {
        searchByMovieName();
        return;
    }

    // Nếu nhập ngày và giờ
    if (date && start && end) {
        searchByDateTimeRange();
        return;
    }

    // Nếu chỉ nhập ngày
    if (date) {
        searchByDate();
        return;
    }

    return;
}


// --- HÀM CHUNG CHO AUTOCOMPLETE ---
function initAutocomplete(inputEl, hiddenInputEl, suggestionsDivEl) {
    if (!inputEl || !hiddenInputEl || !suggestionsDivEl) return; // kiểm tra tồn tại

    let selectedIndex = -1;
    let currentSuggestions = [];

    // --- Fetch danh sách phim ---
    async function loadSuggestions(query) {
        if (!query) {
            suggestionsDivEl.innerHTML = '';
            hiddenInputEl.value = '';
            currentSuggestions = [];
            return;
        }

        try {
            const res = await fetch(`/Admin/Movie/Autocomplete?title=${encodeURIComponent(query)}`);
            const json = await res.json();

            suggestionsDivEl.innerHTML = '';
            currentSuggestions = [];

            if (json.success && json.data.length > 0) {
                json.data.forEach((movie, index) => {
                    const item = document.createElement('button');
                    item.type = 'button';
                    item.className = 'list-group-item list-group-item-action';
                    item.textContent = movie.title;
                    item.dataset.id = movie.id;

                    item.addEventListener('click', () => selectMovie(index));
                    suggestionsDivEl.appendChild(item);
                    currentSuggestions.push(item);
                });
            } else {
                const noItem = document.createElement('div');
                noItem.className = 'list-group-item';
                noItem.textContent = 'Không tìm thấy phim';
                suggestionsDivEl.appendChild(noItem);
            }
        } catch (err) {
            console.error(err);
        }
    }

    // --- Khi chọn phim ---
    function selectMovie(index) {
        const movie = currentSuggestions[index];
        if (!movie) return;

        inputEl.value = movie.textContent;
        hiddenInputEl.value = movie.dataset.id;
        suggestionsDivEl.innerHTML = '';
        selectedIndex = -1;
    }

    // --- Tô sáng phim đang chọn bằng phím ---
    function highlightSuggestion() {
        currentSuggestions.forEach((el, idx) => {
            el.classList.toggle('active', idx === selectedIndex);
        });
    }

    // --- Sự kiện nhập ---
    inputEl.addEventListener('input', () => {
        const query = inputEl.value.trim();
        loadSuggestions(query);
    });

    // --- Sự kiện bàn phím ---
    inputEl.addEventListener('keydown', (e) => {
        const len = currentSuggestions.length;
        if (!len) return;

        if (e.key === 'ArrowDown') {
            e.preventDefault();
            selectedIndex = (selectedIndex + 1) % len;
            highlightSuggestion();
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            selectedIndex = (selectedIndex - 1 + len) % len;
            highlightSuggestion();
        } else if (e.key === 'Enter') {
            e.preventDefault();
            if (selectedIndex >= 0) selectMovie(selectedIndex);
        }
    });

    // --- Click ra ngoài thì ẩn dropdown ---
    document.addEventListener('click', (e) => {
        if (!suggestionsDivEl.contains(e.target) && e.target !== inputEl) {
            suggestionsDivEl.innerHTML = '';
            selectedIndex = -1;
        }
    });
}

// --- KHỞI TẠO AUTOCOMPLETE CHO MODAL ---
const movieSearchEl = document.getElementById('movieSearch');
if (movieSearchEl) {
    initAutocomplete(
        movieSearchEl,
        document.getElementById('movieId'),
        document.getElementById('movieSuggestions')
    );
}

// --- KHỞI TẠO AUTOCOMPLETE CHO Ô LỌC PHIM NGOÀI DANH SÁCH ---
const filterMovieEl = document.getElementById('filterMovie');
if (filterMovieEl) {
    initAutocomplete(
        filterMovieEl,
        document.getElementById('filterMovieId'),
        document.getElementById('filterMovieSuggestions')
    );
}