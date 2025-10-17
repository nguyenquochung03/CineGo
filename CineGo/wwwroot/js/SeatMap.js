let currentSeat = null;

function handleSeatClick(element) {
    currentSeat = element;
    const row = element.dataset.row;
    const column = element.dataset.column;
    const type = element.dataset.type || "Standard";

    let label = element.dataset.label;
    if (!label) {
        label = String.fromCharCode(64 + parseInt(row)) + column;
    }

    document.getElementById("seatRow").value = row;
    document.getElementById("seatColumn").value = column;
    document.getElementById("seatType").value = type;
    document.getElementById("seatLabel").value = label;

    document.querySelector("#seatModal .btn-danger").style.display = label ? "inline-block" : "none";

    const modal = new bootstrap.Modal(document.getElementById("seatModal"));
    modal.show();
}

async function saveSeat() {
    const row = parseInt(document.getElementById("seatRow").value);
    const column = parseInt(document.getElementById("seatColumn").value);
    const type = document.getElementById("seatType").value;
    const label = document.getElementById("seatLabel").value;

    const theaterId = window.currentTheaterId;
    let seatId = currentSeat.dataset.label ? parseInt(currentSeat.dataset.id) || 0 : 0;

    const response = await fetch(seatId ? `/Admin/Seat/Update/${seatId}` : `/Admin/Seat/Create`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Id: seatId,
            Row: row,
            Column: column,
            Type: type,
            Label: label,
            TheaterId: theaterId
        })
    });

    const data = await response.json();
    if (data.success) {
        currentSeat.dataset.type = type;
        currentSeat.dataset.label = label;
        currentSeat.classList.remove("bg-secondary", "bg-danger", "bg-warning");
        currentSeat.classList.add(type.toLowerCase() === "vip" ? "bg-warning" : "bg-danger");
        currentSeat.textContent = label;

        if (!currentSeat.dataset.id || currentSeat.dataset.id === "0") {
            currentSeat.dataset.id = data.data?.id || 0;
        }

        bootstrap.Modal.getInstance(document.getElementById("seatModal")).hide();
    }
    else {
        alert(data.message);
    }
}

async function deleteSeatModal() {
    const seatId = parseInt(currentSeat.dataset.id);
    if (!seatId || seatId === 0) return;

    const response = await fetch(`/Admin/Seat/Delete/${seatId}`, { method: "POST" });
    const data = await response.json();
    if (data.success) {
        currentSeat.dataset.type = "";
        currentSeat.dataset.label = "";
        currentSeat.dataset.id = "0";
        currentSeat.classList.remove("bg-danger", "bg-warning");
        currentSeat.classList.add("bg-secondary");
        bootstrap.Modal.getInstance(document.getElementById("seatModal")).hide();
        currentSeat.textContent = "";
    } else {
        alert(data.message);
    }
}

function initSeatMap() {
    const seats = document.querySelectorAll(".seat");
    seats.forEach(seat => {
        seat.addEventListener("click", () => handleSeatClick(seat));
    });
}