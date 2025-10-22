// Biến toàn cục để lưu ID/Name của Quy tắc đang được chọn
let currentRuleId = null;
let currentRuleName = '';

// --- HÀM HỖ TRỢ CHUNG ---

// Hàm gọi API và trả về JSON
async function fetchJson(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        }
    };
    if (data && (method === 'POST' || method === 'PUT')) {
        options.body = JSON.stringify(data);
    }

    const response = await fetch(url, options);
    if (response.ok) {
        return await response.json();
    } else {
        // Xử lý lỗi từ Server (Ví dụ: ModelState Invalid)
        try {
            const errorBody = await response.json();
            return errorBody;
        } catch {
            return { success: false, message: `Lỗi không xác định: ${response.status} ${response.statusText}` };
        }
    }
}

// Hàm tải HTML (Partial View)
async function fetchHtml(url) {
    const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
    if (!response.ok) {
        throw new Error(`Lỗi tải nội dung: ${response.statusText}`);
    }
    return await response.text();
}

// --- HÀM TẢI NỘI DUNG CHÍNH (3 CẤP) ---

async function loadPricingRules(page = 1) {
    try {
        const html = await fetchHtml(`/Admin/PricingRule/List?page=${page}`);
        document.getElementById("content-area").innerHTML = html;
        document.getElementById("breadcrumb").innerHTML = '<li class="breadcrumb-item active">💰 Quy tắc giá</li>';
        currentRuleId = null;
        currentRuleName = '';
    } catch (error) {
        console.error(error);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tải danh sách Quy tắc giá.</div>`;
    }
}

async function loadPricingDetails(ruleId, ruleName, page = 1) {
    try {
        const url = `/Admin/PricingDetail/List?ruleId=${ruleId}&page=${page}`;
        const html = await fetchHtml(url);
        document.getElementById("content-area").innerHTML = html;
        document.getElementById("breadcrumb").innerHTML = `
                    <li class="breadcrumb-item"><a href="#" onclick="loadPricingRules()">💰 Quy tắc giá</a></li>
                    <li class="breadcrumb-item active">${ruleName} - Chi tiết giá</li>
                `;
        currentRuleId = ruleId;
        currentRuleName = ruleName;
    } catch (error) {
        console.error(error);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tải Chi tiết giá.</div>`;
    }
}

async function loadPricingRuleDays(ruleId, ruleName, page = 1) {
    try {
        const url = `/Admin/PricingRuleDay/List?ruleId=${ruleId}&page=${page}`;
        const html = await fetchHtml(url);
        document.getElementById("content-area").innerHTML = html;
        document.getElementById("breadcrumb").innerHTML = `
                    <li class="breadcrumb-item"><a href="#" onclick="loadPricingRules()">💰 Quy tắc giá</a></li>
                    <li class="breadcrumb-item active">${ruleName} - Ngày áp dụng</li>
                `;
        currentRuleId = ruleId;
        currentRuleName = ruleName;
    } catch (error) {
        console.error(error);
        document.getElementById("content-area").innerHTML = `<div class="p-3 text-danger">Không thể tải Ngày áp dụng.</div>`;
    }
}

// --- LOGIC CRUD CHO PRICING RULE (PricingRuleController) ---

function openRuleModal(id = null, name = '', description = '', runtime = '', isActive = true) {
    const modalElement = document.getElementById('pricingRuleModal');
    const modal = new bootstrap.Modal(modalElement);

    // Đặt tiêu đề
    modalElement.querySelector('#ruleModalLabel').innerText = id ? 'Cập Nhật Quy Tắc Giá' : 'Tạo Mới Quy Tắc Giá';

    // Điền dữ liệu
    modalElement.querySelector('#ruleId').value = id || '';
    modalElement.querySelector('#ruleName').value = name;
    modalElement.querySelector('#ruleDescription').value = description;
    modalElement.querySelector('#ruleRuntime').value = runtime;
    modalElement.querySelector('#ruleIsActive').checked = isActive;

    // Xóa validation cũ (nếu có)
    modalElement.querySelector('#pricingRuleForm').classList.remove('was-validated');

    modal.show();
}

document.getElementById('ruleSaveBtn').addEventListener('click', async () => {
    const form = document.getElementById('pricingRuleForm');

    // Kích hoạt HTML5 Validation (Client-side)
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }
    form.classList.remove('was-validated');

    const id = document.getElementById('ruleId').value;
    const name = document.getElementById('ruleName').value;
    const description = document.getElementById('ruleDescription').value;
    const runtime = document.getElementById('ruleRuntime').value;
    const isActive = document.getElementById('ruleIsActive').checked;
    const page = document.querySelector('.pagination .active a')?.dataset.page || 1;

    const dto = {
        Id: id ? parseInt(id) : 0,
        Name: name,
        Description: description,
        Runtime: parseInt(runtime),
        IsActive: isActive
    };

    const url = id ? `/Admin/PricingRule/Update/${id}` : `/Admin/PricingRule/Create`;
    const response = await fetchJson(url, 'POST', dto);

    if (response.success) {
        // Thành công: Đóng modal và tải lại danh sách
        bootstrap.Modal.getInstance(document.getElementById('pricingRuleModal')).hide();
        await loadPricingRules(page);
    } else {
        // Lỗi: Ghi vào console
        console.error("Lỗi CRUD PricingRule:", response.message);
    }
});

// Hàm deletePricingRule
async function deletePricingRule(id) {
    if (!confirm("Bạn có chắc chắn muốn XÓA Quy tắc giá này không? Hành động này sẽ xóa tất cả Chi tiết giá và Ngày áp dụng liên quan.")) return;

    const response = await fetchJson(`/Admin/PricingRule/Delete/${id}`, 'POST');

    if (response.success) {
        // Thành công: Tải lại danh sách
        await loadPricingRules();
    } else {
        // Lỗi: Ghi vào console
        console.error("Lỗi Delete PricingRule:", response.message);
    }
}


// --- LOGIC CRUD CHO PRICING DETAIL (PricingDetailController) ---

function openDetailModal(id = null, ticketType = '', seatType = '', basePrice = '') {
    if (!currentRuleId) return console.error("Lỗi: Vui lòng chọn một Quy tắc giá trước.");

    const modalElement = document.getElementById('pricingDetailModal');
    const modal = new bootstrap.Modal(modalElement);

    modalElement.querySelector('#detailModalLabel').innerText = id ? 'Cập Nhật Chi Tiết Giá' : `Thêm Chi Tiết Giá cho: ${currentRuleName}`;
    modalElement.querySelector('#detailId').value = id || '';
    modalElement.querySelector('#detailTicketType').value = ticketType;
    modalElement.querySelector('#detailSeatType').value = seatType;
    modalElement.querySelector('#detailBasePrice').value = basePrice;
    modalElement.querySelector('#detailPricingRuleId').value = currentRuleId;

    modalElement.querySelector('#pricingDetailForm').classList.remove('was-validated');

    modal.show();
}

document.getElementById('detailSaveBtn').addEventListener('click', async () => {
    const form = document.getElementById('pricingDetailForm');
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }
    form.classList.remove('was-validated');

    const id = document.getElementById('detailId').value;
    const ruleId = parseInt(document.getElementById('detailPricingRuleId').value);
    const ticketType = document.getElementById('detailTicketType').value;
    const seatType = document.getElementById('detailSeatType').value;
    const basePrice = document.getElementById('detailBasePrice').value;
    const page = document.querySelector('.pagination .active a')?.dataset.page || 1;

    const dto = {
        Id: id ? parseInt(id) : 0,
        PricingRuleId: ruleId,
        TicketType: ticketType,
        SeatType: seatType,
        BasePrice: parseFloat(basePrice)
    };

    const url = id ? `/Admin/PricingDetail/Update/${id}` : `/Admin/PricingDetail/Create`;
    const response = await fetchJson(url, 'POST', dto);

    if (response.success) {
        // Thành công: Đóng modal và tải lại danh sách
        bootstrap.Modal.getInstance(document.getElementById('pricingDetailModal')).hide();
        await loadPricingDetails(ruleId, currentRuleName, page);
    } else {
        // Lỗi: Ghi vào console
        console.error("Lỗi CRUD PricingDetail:", response.message);
    }
});

// Hàm deletePricingDetail
async function deletePricingDetail(id) {
    if (!confirm("Bạn có chắc chắn muốn XÓA Chi tiết giá này không?")) return;

    const response = await fetchJson(`/Admin/PricingDetail/Delete/${id}`, 'POST');

    if (response.success) {
        // Thành công: Tải lại danh sách
        await loadPricingDetails(currentRuleId, currentRuleName);
    } else {
        // Lỗi: Ghi vào console
        console.error("Lỗi Delete PricingDetail:", response.message);
    }
}


// --- LOGIC CRUD CHO PRICING RULE DAY (PricingRuleDayController) ---

function openRuleDayModal(id = null, dayName = '') {
    if (!currentRuleId) return console.error("Lỗi: Vui lòng chọn một Quy tắc giá trước.");

    const modalElement = document.getElementById('pricingRuleDayModal');
    const modal = new bootstrap.Modal(modalElement);

    modalElement.querySelector('#dayModalLabel').innerText = id ? 'Cập Nhật Ngày Áp Dụng' : `Thêm Ngày Áp Dụng cho: ${currentRuleName}`;
    modalElement.querySelector('#dayId').value = id || '';
    modalElement.querySelector('#dayName').value = dayName;
    modalElement.querySelector('#dayPricingRuleId').value = currentRuleId;

    modalElement.querySelector('#pricingRuleDayForm').classList.remove('was-validated');

    modal.show();
}

document.getElementById('daySaveBtn').addEventListener('click', async () => {
    const form = document.getElementById('pricingRuleDayForm');
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }
    form.classList.remove('was-validated');

    const id = document.getElementById('dayId').value;
    const ruleId = parseInt(document.getElementById('dayPricingRuleId').value);
    const dayName = document.getElementById('dayName').value;
    const page = document.querySelector('.pagination .active a')?.dataset.page || 1;

    const dto = {
        Id: id ? parseInt(id) : 0,
        PricingRuleId: ruleId,
        DayName: dayName
    };

    const url = id ? `/Admin/PricingRuleDay/Update/${id}` : `/Admin/PricingRuleDay/Create`;
    const response = await fetchJson(url, 'POST', dto);

    if (response.success) {
        // Thành công: Đóng modal và tải lại danh sách
        bootstrap.Modal.getInstance(document.getElementById('pricingRuleDayModal')).hide();
        await loadPricingRuleDays(ruleId, currentRuleName, page);
    } else {
        // Lỗi: Ghi vào console
        console.error("Lỗi CRUD PricingRuleDay:", response.message);
    }
});

async function deletePricingRuleDay(id) {
    if (!confirm("Bạn có chắc chắn muốn XÓA Ngày áp dụng này không?")) return;

    const response = await fetchJson(`/Admin/PricingRuleDay/Delete/${id}`, 'POST');

    if (response.success) {
        // Thành công: Tải lại danh sách
        await loadPricingRuleDays(currentRuleId, currentRuleName);
    } else {
        // Lỗi: Ghi vào console
        console.error("Lỗi Delete PricingRuleDay:", response.message);
    }
}

// Khởi tạo: Tải danh sách Quy tắc giá khi trang load
document.addEventListener('DOMContentLoaded', () => {
    loadPricingRules();
});

$('#pricingDetailModal').on('shown.bs.modal', function () {
    $(this).find('input:visible:first').focus();
});

$('#pricingRuleDayModal').on('shown.bs.modal', function () {
    $(this).find('input:visible:first').focus();
});

$('#pricingRuleModal').on('shown.bs.modal', function () {
    $(this).find('input:visible:first').focus();
});