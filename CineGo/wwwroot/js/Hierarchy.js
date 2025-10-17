document.addEventListener("DOMContentLoaded", () => {
    loadRegions();
});

// ===== FETCH HTML UTILITY =====
async function fetchHtml(url) {
    const res = await fetch(url);
    if (!res.ok) throw new Error("Failed: " + url);
    return await res.text();
}

// ===== GLOBAL STATE =====
window.currentRegionId = null;
window.currentRegionName = '';
window.currentCityId = null;
window.currentCityName = '';
window.currentCinemaId = null;
window.currentCinemaName = '';
window.currentTheaterId = null;
window.currentTheaterName = '';

// ===== LOAD DYNAMIC CONTENT =====
async function loadRegions(page = 1) {
    const html = await fetchHtml(`/Admin/Region/Index?page=${page}`);
    document.getElementById("content-area").innerHTML = html;
    document.getElementById("breadcrumb").innerHTML = '<li class="breadcrumb-item active">📍 Khu vực</li>';
}

async function loadCities(regionId, regionName, page = 1) {
    const html = await fetchHtml(`/Admin/City/Index?regionId=${regionId}&page=${page}`);
    document.getElementById("content-area").innerHTML = html;

    window.currentRegionId = regionId;
    window.currentRegionName = regionName;

    document.getElementById("breadcrumb").innerHTML = `
        <li class="breadcrumb-item"><a href="#" onclick="loadRegions()">📍 Khu vực: ${regionName}</a></li>
        <li class="breadcrumb-item active">🌆 Thành phố</li>`;
}

async function loadCinemas(cityId, cityName, page = 1) {
    const html = await fetchHtml(`/Admin/Cinema/Index?cityId=${cityId}&page=${page}`);
    document.getElementById("content-area").innerHTML = html;

    window.currentCityId = cityId;
    window.currentCityName = cityName;

    document.getElementById("breadcrumb").innerHTML = `
        <li class="breadcrumb-item"><a href="#" onclick="loadRegions()">📍 Khu vực: ${window.currentRegionName}</a></li>
        <li class="breadcrumb-item"><a href="#" onclick="loadCities(${window.currentRegionId},'${window.currentRegionName}')">🌆 Thành phố: ${cityName}</a></li>
        <li class="breadcrumb-item active">🎬 Rạp</li>`;
}

async function loadTheaters(cinemaId, cinemaName, page = 1) {
    const html = await fetchHtml(`/Admin/Theater/Index?cinemaId=${cinemaId}&page=${page}`);
    document.getElementById("content-area").innerHTML = html;

    window.currentCinemaId = cinemaId;
    window.currentCinemaName = cinemaName;

    document.getElementById("breadcrumb").innerHTML = `
        <li class="breadcrumb-item"><a href="#" onclick="loadRegions()">📍 Khu vực: ${window.currentRegionName}</a></li>
        <li class="breadcrumb-item"><a href="#" onclick="loadCities(${window.currentRegionId},'${window.currentRegionName}')">🌆 Thành phố: ${window.currentCityName}</a></li>
        <li class="breadcrumb-item"><a href="#" onclick="loadCinemas(${window.currentCityId},'${window.currentCityName}')">🎬 Rạp: ${cinemaName}</a></li>
        <li class="breadcrumb-item active">🎭 Phòng chiếu</li>`;
}

async function loadSeats(theaterId, theaterName) {
    const html = await fetchHtml(`/Admin/Seat/Index?theaterId=${theaterId}`);
    document.getElementById("content-area").innerHTML = html;

    window.currentTheaterId = theaterId;
    window.currentTheaterName = theaterName;

    document.getElementById("breadcrumb").innerHTML = `
        <li class="breadcrumb-item"><a href="#" onclick="loadRegions()">📍 Khu vực: ${window.currentRegionName}</a></li>
        <li class="breadcrumb-item"><a href="#" onclick="loadCities(${window.currentRegionId},'${window.currentRegionName}')">🌆 Thành phố: ${window.currentCityName}</a></li>
        <li class="breadcrumb-item"><a href="#" onclick="loadCinemas(${window.currentCityId},'${window.currentCityName}')">🎬 Rạp: ${window.currentCinemaName}</a></li>
        <li class="breadcrumb-item"><a href="#" onclick="loadTheaters(${window.currentCinemaId},'${window.currentCinemaName}')">🎭 Phòng: ${theaterName}</a></li>
        <li class="breadcrumb-item active">💺 Chỗ ngồi</li>`;
}

// ===== MODAL UTILITY =====
function showModal(modalId, title, data = {}) {
    const modalEl = document.getElementById(modalId);
    modalEl.querySelector(".modal-title").innerText = title;

    for (const key in data) {
        const el = modalEl.querySelector(`#${key}`);
        if (el) el.value = data[key];
    }

    const errorEl = modalEl.querySelector(".text-danger");
    if (errorEl) errorEl.innerText = '';

    new bootstrap.Modal(modalEl).show();
}

function resetForm(ids) { ids.forEach(id => { const el = document.getElementById(id); if (el) el.value = ''; }); }
function resetError(ids) { ids.forEach(id => { const el = document.getElementById(id); if (el) el.innerText = ''; }); }

// ===== CRUD REGION =====
function addRegion() { showModal('regionModal', 'Thêm khu vực'); }
function editRegion(id, name) { showModal('regionModal', 'Sửa khu vực', { regionId: id, regionName: name }); }

document.getElementById('regionSaveBtn').addEventListener('click', async () => {
    const id = document.getElementById('regionId').value;
    const name = document.getElementById('regionName').value.trim();
    const errorEl = document.getElementById('regionError');
    errorEl.innerText = "";

    if (!name) { errorEl.innerText = "Tên khu vực không được trống!"; return; }

    const url = id ? `/Admin/Region/Update/${id}` : `/Admin/Region/Create`;
    const res = await fetch(url, {
        method: 'POST',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Name: name })
    });
    const data = await res.json();

    if (data.success) {
        bootstrap.Modal.getInstance(document.getElementById('regionModal')).hide();
        loadRegions();
        resetForm(['regionId', 'regionName']);
        resetError(['regionError']);
    } else { errorEl.innerText = data.message; }
});

async function deleteRegion(id) {
    if (confirm("Xóa khu vực?")) {
        const res = await fetch(`/Admin/Region/Delete/${id}`, { method: 'POST' });
        const data = await res.json();
        if (data.success) loadRegions(); else alert(data.message);
    }
}

// ===== CRUD CITY =====
function addCity(regionId) { showModal('cityModal', 'Thêm thành phố', { cityRegionId: regionId }); }
function editCity(id, name, regionId) { showModal('cityModal', 'Sửa thành phố', { cityId: id, cityName: name, cityRegionId: regionId }); }

document.getElementById('citySaveBtn').addEventListener('click', async () => {
    const id = document.getElementById('cityId').value;
    const regionId = document.getElementById('cityRegionId').value;
    const name = document.getElementById('cityName').value.trim();
    const errorEl = document.getElementById('cityError');
    errorEl.innerText = "";

    if (!name) { errorEl.innerText = "Tên thành phố không được trống!"; return; }

    const url = id ? `/Admin/City/Update/${id}` : `/Admin/City/Create`;
    const res = await fetch(url, {
        method: 'POST',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Name: name, RegionId: parseInt(regionId) })
    });
    const data = await res.json();

    if (data.success) {
        bootstrap.Modal.getInstance(document.getElementById('cityModal')).hide();
        loadCities(regionId, window.currentRegionName);
        resetForm(['cityId', 'cityName', 'cityRegionId']);
        resetError(['cityError']);
    } else { errorEl.innerText = data.message; }
});

async function deleteCity(id) {
    if (confirm("Xóa thành phố?")) {
        const res = await fetch(`/Admin/City/Delete/${id}`, { method: 'POST' });
        const data = await res.json();
        if (data.success) loadCities(window.currentRegionId, window.currentRegionName); else alert(data.message);
    }
}

// ===== CRUD CINEMA =====
function addCinema(cityId) { showModal('cinemaModal', 'Thêm rạp', { cinemaCityId: cityId }); }
function editCinema(id, name, address, cityId) { showModal('cinemaModal', 'Sửa rạp', { cinemaId: id, cinemaName: name, cinemaAddress: address, cinemaCityId: cityId }); }

document.getElementById('cinemaSaveBtn').addEventListener('click', async () => {
    const id = document.getElementById('cinemaId').value;
    const cityId = document.getElementById('cinemaCityId').value;
    const name = document.getElementById('cinemaName').value.trim();
    const address = document.getElementById('cinemaAddress').value.trim();
    const errorEl = document.getElementById('cinemaError');
    errorEl.innerText = "";

    if (!name) { errorEl.innerText = "Tên rạp không được trống!"; return; }

    const url = id ? `/Admin/Cinema/Update/${id}` : `/Admin/Cinema/Create`;
    const res = await fetch(url, {
        method: 'POST',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Name: name, Address: address, CityId: cityId })
    });
    const data = await res.json();

    if (data.success) {
        bootstrap.Modal.getInstance(document.getElementById('cinemaModal')).hide();
        loadCinemas(cityId, window.currentCityName);
        resetForm(['cinemaId', 'cinemaName', 'cinemaAddress', 'cinemaCityId']);
        resetError(['cinemaError']);
    } else { errorEl.innerText = data.message; }
});

async function deleteCinema(id, cityId) {
    if (confirm("Xóa rạp?")) {
        const res = await fetch(`/Admin/Cinema/Delete/${id}`, { method: 'POST' });
        const data = await res.json();
        if (data.success) loadCinemas(cityId, window.currentCityName); else alert(data.message);
    }
}

// ===== CRUD THEATER =====
function addTheater(cinemaId) { showModal('theaterModal', 'Thêm phòng', { theaterCinemaId: cinemaId }); }
function editTheater(id, name, rows, cols, cinemaId) { showModal('theaterModal', 'Sửa phòng', { theaterId: id, theaterName: name, theaterRows: rows, theaterCols: cols, theaterCinemaId: cinemaId }); }

document.getElementById('theaterSaveBtn').addEventListener('click', async () => {
    const id = document.getElementById('theaterId').value;
    const cinemaId = document.getElementById('theaterCinemaId').value;
    const name = document.getElementById('theaterName').value.trim();
    const rows = parseInt(document.getElementById('theaterRows').value);
    const cols = parseInt(document.getElementById('theaterCols').value);
    const errorEl = document.getElementById('theaterError');
    errorEl.innerText = "";

    if (!name || !rows || !cols) { errorEl.innerText = "Điền đầy đủ thông tin!"; return; }

    const url = id ? `/Admin/Theater/Update/${id}` : `/Admin/Theater/Create`;

    const res = await fetch(url, {
        method: 'POST',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Name: name, Rows: rows, Columns: cols, CinemaId: cinemaId })
    });
    const data = await res.json();

    if (data.success) {
        bootstrap.Modal.getInstance(document.getElementById('theaterModal')).hide();
        loadTheaters(cinemaId, window.currentCinemaName);
        resetForm(['theaterId', 'theaterName', 'theaterRows', 'theaterCols', 'theaterCinemaId']);
        resetError(['theaterError']);
    } else { errorEl.innerText = data.message; }
});

async function deleteTheater(id, cinemaId) {
    if (confirm("Xóa phòng chiếu?")) {
        const res = await fetch(`/Admin/Theater/Delete/${id}`, { method: 'POST' });
        const data = await res.json();
        if (data.success) loadTheaters(cinemaId, window.currentCinemaName); else alert(data.message);
    }
}
