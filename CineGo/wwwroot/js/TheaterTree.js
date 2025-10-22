// base url: adjust if your area-prefix is different
const baseUrl = '/Admin/TheaterTree';

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

async function postJson(url, data) {
    const res = await fetch(url, {
        method: 'POST',
        credentials: 'same-origin',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    if (!res.ok) throw new Error('Network error');
    return res.json();
}

// --- LOAD THEATER TREE ---
async function loadTheaterTree(showtimeId, showtimeTitle) {
    try {
        const html = await fetchHtml(`/Admin/TheaterTree/LoadTreeView?showtimeId=${showtimeId}&showtimeTitle=${encodeURIComponent(showtimeTitle)}`);
        document.getElementById("content-area").innerHTML = html;
        document.getElementById("breadcrumb").innerHTML = `
            <li class="breadcrumb-item">
                <a href="#" onclick="loadShowtimes()">🎬 Suất chiếu</a>
            </li>
            <li class="breadcrumb-item active">${showtimeTitle} - Rạp</li>
        `;

        if (showtimeId) {
            const container = document.getElementById("theaterTreeContainer");
            const res = await fetch(`${baseUrl}/GetRegions?showtimeId=${showtimeId}`);
            const regions = await res.json();
            renderRegions(container, regions, showtimeId);
        }
        window.currentShowtimeId = showtimeId;
        window.currentShowtimeTitle = showtimeTitle;
    } catch (err) {
        console.error(err);
        document.getElementById("content-area").innerHTML =
            `<div class="p-3 text-danger">Không thể tải cây rạp chiếu.</div>`;
    }
}

// Buttons
document.addEventListener('click', function (e) {
    if (e.target && e.target.id === 'btnCollapseTree') {
        loadTheaterTree(window.currentShowtimeId, window.currentShowtimeTitle);
    }
    if (e.target && e.target.id === 'btnExpandTree') {
        loadFullTree(window.currentShowtimeId);
    }
});

function getShowtimeId() {
    const c = document.getElementById('theaterTreeContainer');
    return c ? Number(c.dataset.showtimeId) : 0;
}

// Render helpers
function renderRegions(container, regions, showtimeId, expandAll = false) {
    container.innerHTML = '';
    if (!regions || regions.length === 0) {
        container.innerHTML = `<div class="text-muted">Không có region nào.</div>`;
        return;
    }

    const ul = document.createElement('div');
    regions.forEach(r => {
        const node = createRegionNode(r, showtimeId, expandAll);
        ul.appendChild(node);
    });
    container.appendChild(ul);
}

function renderCities(container, cities, showtimeId) {
    container.innerHTML = '';
    if (!cities || !cities.length) {
        container.innerHTML = `<div class="text-muted">Không có city.</div>`;
        return;
    }
    cities.forEach(c => {
        const node = createCityNode(c, showtimeId);
        container.appendChild(node);
    });
}

function renderCinemas(container, cinemas, showtimeId) {
    container.innerHTML = '';
    if (!cinemas || !cinemas.length) {
        container.innerHTML = `<div class="text-muted">Không có cinema.</div>`;
        return;
    }
    cinemas.forEach(ci => {
        const node = createCinemaNode(ci, showtimeId);
        container.appendChild(node);
    });
}

function createRegionNode(regionDto, showtimeId, expand = false) {
    const wrapper = document.createElement('div');
    wrapper.className = 'tree-node tree-region';

    const row = document.createElement('div');
    row.className = 'node-row';

    const checkbox = document.createElement('input');
    checkbox.type = 'checkbox';
    checkbox.checked = regionDto.selectedCount > 0 && regionDto.selectedCount === regionDto.totalCount;
    checkbox.indeterminate = regionDto.selectedCount > 0 && regionDto.selectedCount < regionDto.totalCount;
    checkbox.dataset.nodeType = 'region';
    checkbox.dataset.nodeId = regionDto.id;
    row.appendChild(checkbox);

    const name = document.createElement('div');
    name.className = 'node-name';
    name.innerHTML = `🎞️ ${regionDto.name}`;
    row.appendChild(name);

    const meta = document.createElement('div');
    meta.className = 'node-meta';
    meta.textContent = `(${regionDto.selectedCount}/${regionDto.totalCount})`;
    row.appendChild(meta);

    wrapper.appendChild(row);

    const children = document.createElement('div');
    children.className = 'tree-children';
    children.style.display = expand ? 'block' : 'none';
    wrapper.appendChild(children);

    name.addEventListener('click', async (ev) => {
        ev.stopPropagation();
        if (children.style.display === 'none') {
            if (!children.dataset.loaded) {
                const cities = await fetchJson(`${baseUrl}/GetCities?regionId=${regionDto.id}&showtimeId=${getShowtimeId()}`);
                renderCities(children, cities, getShowtimeId());
                children.dataset.loaded = '1';
            }
            children.style.display = 'block';
        } else {
            children.style.display = 'none';
        }
    });

    checkbox.addEventListener('change', function (ev) {
        ev.stopPropagation();
        onNodeCheckboxChange('region', regionDto.id, this.checked);
    });

    return wrapper;
}


function createCityNode(cityDto, showtimeId) {
    const wrapper = document.createElement('div');
    wrapper.className = 'tree-node tree-city';

    const row = document.createElement('div');
    row.className = 'node-row';

    const checkbox = document.createElement('input');
    checkbox.type = 'checkbox';
    checkbox.checked = cityDto.selectedCount > 0 && cityDto.selectedCount === cityDto.totalCount;
    checkbox.indeterminate = cityDto.selectedCount > 0 && cityDto.selectedCount < cityDto.totalCount;
    checkbox.dataset.nodeType = 'city';
    checkbox.dataset.nodeId = cityDto.id;
    row.appendChild(checkbox);

    const name = document.createElement('div');
    name.className = 'node-name';
    name.innerHTML = `🏙️ ${cityDto.name}`;
    row.appendChild(name);

    const meta = document.createElement('div');
    meta.className = 'node-meta';
    meta.textContent = `(${cityDto.selectedCount}/${cityDto.totalCount})`;
    row.appendChild(meta);

    wrapper.appendChild(row);

    const children = document.createElement('div');
    children.className = 'tree-children';
    wrapper.appendChild(children);

    name.addEventListener('click', async (ev) => {
        ev.stopPropagation();
        if (children.style.display === 'none' || !children.style.display) {
            if (!children.dataset.loaded) {
                const cinemas = await fetchJson(`${baseUrl}/GetCinemas?cityId=${cityDto.id}&showtimeId=${getShowtimeId()}`);
                renderCinemas(children, cinemas, getShowtimeId());
                children.dataset.loaded = '1';
            }
            children.style.display = 'block';
        } else {
            children.style.display = 'none';
        }
    });

    checkbox.addEventListener('change', function (ev) {
        ev.stopPropagation();
        onNodeCheckboxChange('city', cityDto.id, this.checked);
    });

    return wrapper;
}


function createCinemaNode(cinemaDto, showtimeId) {
    const wrapper = document.createElement('div');
    wrapper.className = 'tree-node tree-cinema';

    const row = document.createElement('div');
    row.className = 'node-row';

    const checkbox = document.createElement('input');
    checkbox.type = 'checkbox';
    checkbox.checked = cinemaDto.selectedCount > 0 && cinemaDto.selectedCount === cinemaDto.totalCount;
    checkbox.indeterminate = cinemaDto.selectedCount > 0 && cinemaDto.selectedCount < cinemaDto.totalCount;
    checkbox.dataset.nodeType = 'cinema';
    checkbox.dataset.nodeId = cinemaDto.id;
    row.appendChild(checkbox);

    const name = document.createElement('div');
    name.className = 'node-name';
    name.innerHTML = `🎦 ${cinemaDto.name}`;
    row.appendChild(name);

    const meta = document.createElement('div');
    meta.className = 'node-meta';
    meta.textContent = `(${cinemaDto.selectedCount}/${cinemaDto.totalCount})`;
    row.appendChild(meta);

    wrapper.appendChild(row);

    const children = document.createElement('div');
    children.className = 'tree-children';
    wrapper.appendChild(children);

    name.addEventListener('click', async (ev) => {
        ev.stopPropagation();
        if (children.style.display === 'none' || !children.style.display) {
            if (!children.dataset.loaded) {
                const theaters = await fetchJson(`${baseUrl}/GetTheaters?cinemaId=${cinemaDto.id}&showtimeId=${getShowtimeId()}`);
                renderTheaters(children, theaters, getShowtimeId());
                children.dataset.loaded = '1';
            }
            children.style.display = 'block';
        } else {
            children.style.display = 'none';
        }
    });

    checkbox.addEventListener('change', function (ev) {
        ev.stopPropagation();
        onNodeCheckboxChange('cinema', cinemaDto.id, this.checked);
    });

    return wrapper;
}


function renderTheaters(container, theaters, showtimeId) {
    container.innerHTML = '';
    if (!theaters || !theaters.length) {
        container.innerHTML = `<div class="text-muted">Không có phòng chiếu.</div>`;
        return;
    }

    theaters.forEach(t => {
        const row = document.createElement('div');
        row.className = 'node-row tree-theater';

        const checkbox = document.createElement('input');
        checkbox.type = 'checkbox';
        checkbox.checked = !!t.isSelected;
        checkbox.dataset.nodeType = 'theater';
        checkbox.dataset.nodeId = t.id;
        row.appendChild(checkbox);

        const name = document.createElement('div');
        name.className = 'node-name';
        name.innerHTML = `🎭 ${t.name}`;
        row.appendChild(name);

        const meta = document.createElement('div');
        meta.className = 'node-meta';
        meta.textContent = t.isSelected ? '(1/1)' : '(0/1)';
        row.appendChild(meta);

        container.appendChild(row);

        checkbox.addEventListener('change', function (ev) {
            ev.stopPropagation();
            onNodeCheckboxChange('theater', t.id, this.checked);
        });
    });
}

// Handles checking/unchecking a node:
async function onNodeCheckboxChange(nodeType, nodeId, isChecked) {
    const showtimeId = getShowtimeId();
    if (!showtimeId) return;

    // get all theater ids under this node
    const ids = await postJson(`${baseUrl}/GetTheaterIdsForNode`, { NodeType: nodeType, NodeId: nodeId, ShowtimeId: showtimeId });

    if (!ids || !ids.length) {
        console.warn('No theater ids found for node', nodeType, nodeId);
        return;
    }

    // perform add or remove
    const endpoint = isChecked ? `${baseUrl}/AddTheaters` : `${baseUrl}/RemoveTheaters`;
    const resp = await postJson(endpoint, { ShowtimeId: showtimeId, TheaterIds: ids });

    // showfeedback (simple)
    if (resp && resp.success) {
        // update UI: refresh counts for visible nodes by reloading the current visible subtree(s).
        refreshVisibleSubtrees();
    } else {
        alert('Thực hiện không thành công: ' + (resp && resp.message ? resp.message : 'Không rõ lỗi'));
        // revert checkbox states by re-rendering visible parts
        refreshVisibleSubtrees();
    }
}

// find all nodes that are currently visible and refresh them by re-querying their parent lists
async function refreshVisibleSubtrees() {
    const showtimeId = getShowtimeId();
    if (!showtimeId) return;

    try {
        const res = await fetch(`${baseUrl}/GetFullTree?showtimeId=${showtimeId}`);
        const stats = await res.json();

        updateVisibleNodeStatsFromFullTree(stats);
    } catch (err) {
        console.error("refreshVisibleSubtrees error:", err);
    }
}

function updateVisibleNodeStatsFromFullTree(fullTree) {
    if (!fullTree) return;

    fullTree.forEach(region => {
        const regionEl = document.querySelector(`input[data-node-type="region"][data-node-id="${region.id}"]`);
        if (regionEl) {
            const total = region.cities.flatMap(c => c.cinemas).flatMap(ci => ci.theaters).length;
            const selected = region.cities.flatMap(c => c.cinemas)
                .flatMap(ci => ci.theaters)
                .filter(t => t.isSelected).length;
            updateNodeMeta(regionEl, selected, total);
        }

        region.cities.forEach(city => {
            const cityEl = document.querySelector(`input[data-node-type="city"][data-node-id="${city.id}"]`);
            if (cityEl) {
                const total = city.cinemas.flatMap(ci => ci.theaters).length;
                const selected = city.cinemas.flatMap(ci => ci.theaters)
                    .filter(t => t.isSelected).length;
                updateNodeMeta(cityEl, selected, total);
            }

            city.cinemas.forEach(cinema => {
                const cinemaEl = document.querySelector(`input[data-node-type="cinema"][data-node-id="${cinema.id}"]`);
                if (cinemaEl) {
                    const total = cinema.theaters.length;
                    const selected = cinema.theaters.filter(t => t.isSelected).length;
                    updateNodeMeta(cinemaEl, selected, total);
                }

                cinema.theaters.forEach(theater => {
                    const tEl = document.querySelector(`input[data-node-type="theater"][data-node-id="${theater.id}"]`);
                    if (tEl) {
                        tEl.checked = !!theater.isSelected;
                        const meta = tEl.closest(".node-row")?.querySelector(".node-meta");
                        if (meta) meta.textContent = theater.isSelected ? "(1/1)" : "(0/1)";
                    }
                });
            });
        });
    });
}

function updateNodeMeta(checkbox, selectedCount, totalCount) {
    checkbox.checked = selectedCount > 0 && selectedCount === totalCount;
    checkbox.indeterminate = selectedCount > 0 && selectedCount < totalCount;
    const meta = checkbox.closest(".node-row")?.querySelector(".node-meta");
    if (meta) meta.textContent = `(${selectedCount}/${totalCount})`;
}
