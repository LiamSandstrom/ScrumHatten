// ─── ADD ROW ───────────────────────────────────────────────
document.getElementById("addRow").addEventListener("click", () => {
    const template = document.getElementById("rowTemplate");
    const clone = template.content.cloneNode(true);
    document.getElementById("rows").prepend(clone);
    updateOrderTotal();
});

// ─── FAST ORDER ────────────────────────────────────────────
document.querySelector("#FastOrder")?.addEventListener("change", () => {
    updateOrderTotal();
});

// ─── REMOVE ROW ────────────────────────────────────────────
document.getElementById("rows").addEventListener("click", (e) => {
    const btn = e.target.closest(".remove-row");
    if (!btn) return;
    const row = btn.closest(".row-item");
    if (!row) return;
    row.remove();
    reindexRows();
    updateOrderTotal();
});

// ─── ROW CHANGES ───────────────────────────────────────────
document.getElementById("rows").addEventListener("change", async (e) => {
    const row = e.target.closest(".row-item");
    if (!row) return;

    if (e.target.classList.contains("type-dropdown")) {
        const type = e.target.value;
        const hatSelect = row.querySelector(".hat-dropdown");
        const typeErr = row.querySelector(".error-text");

        if (!type) {
            if (typeErr) typeErr.textContent = "Välj typ";
            return;
        }
        if (typeErr) typeErr.textContent = "";

        hatSelect.innerHTML = `<option>Laddar...</option>`;
        clearPreview(row);

        const res = await fetch(`/Order/GetHatsByType?type=${encodeURIComponent(type)}`);
        const data = await res.json();

        hatSelect.innerHTML = `<option value="">Välj hatt</option>`;
        for (const h of data) {
            const opt = document.createElement("option");
            opt.value = h.id;
            opt.textContent = h.name;
            opt.dataset.hat = JSON.stringify(h);
            hatSelect.appendChild(opt);
        }
    }

    if (e.target.classList.contains("hat-dropdown")) {
        const opt = e.target.selectedOptions[0];
        const hatErr = row.querySelector(".error-hat");

        if (!opt || !opt.dataset.hat) {
            if (hatErr) hatErr.textContent = "Välj hatt";
            return;
        }
        if (hatErr) hatErr.textContent = "";

        const hat = JSON.parse(opt.dataset.hat);
        if (!document.body.contains(row)) return;
        renderPreview(row, hat);
        reindexRows();
    }

    if (e.target.classList.contains("quantity-input")) {
        if (!document.body.contains(row)) return;
        updateTotal(row);
        updateOrderTotal();
        reindexRows();
    }
});

// ─── CLIENT VALIDATION (capture phase) ────────────────────
let rowValidationFailed = false;

document.getElementById("createOrderForm").addEventListener("submit", (e) => {
    const rows = document.querySelectorAll("#rows .row-item");
    const rowsError = document.getElementById("rowsError");
    rowValidationFailed = false;

    if (rows.length === 0) {
        rowsError.classList.remove("d-none");
        rowValidationFailed = true;
    } else {
        rowsError.classList.add("d-none");
    }

    rows.forEach(row => {
        const typeEl = row.querySelector(".type-dropdown");
        const hatEl = row.querySelector(".hat-dropdown");
        const typeErr = row.querySelector(".error-text");
        const hatErr = row.querySelector(".error-hat");

        if (!typeEl.value) {
            if (typeErr) typeErr.textContent = "Välj typ";
            rowValidationFailed = true;
        }
        if (!hatEl.value) {
            if (hatErr) hatErr.textContent = "Välj hatt";
            rowValidationFailed = true;
        }
    });

    const userId = document.querySelector("[name='SelectedUserId']");
    const customerId = document.querySelector("[name='SelectedCustomerId']");
    const date = document.querySelector("[name='DateToFinish']");

    if (!userId?.value) {
        document.getElementById("error-SelectedUserId").textContent = "Välj utförare";
        rowValidationFailed = true;
    }
    if (!customerId?.value) {
        document.getElementById("error-SelectedCustomerId").textContent = "Välj kund";
        rowValidationFailed = true;
    }
    if (!date?.value || new Date(date.value) < new Date(new Date().toDateString())) {
        document.getElementById("error-DateToFinish").textContent = "Datum krävs och kan inte vara i det förflutna";
        rowValidationFailed = true;
    }

    if (rowValidationFailed) {
        e.preventDefault();
        e.stopImmediatePropagation();
    }
}, true);

// ─── TRANSPORT ─────────────────────────────────────────────
document.querySelector("[name='TransportPrice']")?.addEventListener("input", () => {
    updateOrderTotal();
});

// ─── REINDEX ───────────────────────────────────────────────
function reindexRows() {
    document.querySelectorAll('#rows .row-item').forEach((row, i) => {
        row.querySelectorAll('input[type=hidden]').forEach(el => el.remove());

        const hatId = row.querySelector('.hat-dropdown')?.value;
        const qty = row.querySelector('.quantity-input')?.value;
        if (!hatId) return;

        const h1 = document.createElement('input');
        h1.type = 'hidden';
        h1.name = `Rows[${i}].HatId`;
        h1.value = hatId;

        const h2 = document.createElement('input');
        h2.type = 'hidden';
        h2.name = `Rows[${i}].Quantity`;
        h2.value = qty;

        row.appendChild(h1);
        row.appendChild(h2);
    });
}

// ─── RENDER PREVIEW ────────────────────────────────────────
function renderPreview(row, hat) {
    row.dataset.price = hat.price || 0;

    const img = row.querySelector(".hat-image");
    if (hat.imageUrl && hat.imageUrl.trim() !== "") {
        img.src = hat.imageUrl;
        img.classList.remove("d-none");
    } else {
        img.src = "";
        img.classList.add("d-none");
    }

    row.querySelector(".hat-name").textContent = hat.name || "";
    row.querySelector(".hat-description").textContent = hat.description || "";
    row.querySelector(".hat-price").textContent = `Pris: ${hat.price ?? "-"}`;

    const qtyInput = row.querySelector(".quantity-input");
    const selectedQty = parseInt(qtyInput.value || "0");
    const stock = hat.quantity ?? 0;

    const el = row.querySelector(".hat-quantity");
    el.textContent = `I lager: ${stock}`;

    if (selectedQty > stock) {
        el.classList.remove("text-muted");
        el.classList.add("text-danger", "fw-bold");
    } else {
        el.classList.remove("text-danger", "fw-bold");
        el.classList.add("text-muted");
    }

    updateTotal(row);
    updateOrderTotal();
}

// ─── UPDATE ROW TOTAL ──────────────────────────────────────
function updateTotal(row) {
    if (!row.dataset.price) return;
    const price = parseFloat(row.dataset.price || "0");
    const qty = parseInt(row.querySelector(".quantity-input").value || "0");
    const total = price * qty;

    row.querySelector(".hat-total").textContent = isNaN(total) ? "" : `Total: ${total.toFixed(2)} kr`;

    const stock = parseInt(row.querySelector(".hat-quantity")?.textContent.replace(/\D/g, "") || "0");
    const elQty = row.querySelector(".hat-quantity");

    if (qty > stock) {
        elQty.classList.remove("text-muted");
        elQty.classList.add("text-danger", "fw-bold");
    } else {
        elQty.classList.remove("text-danger", "fw-bold");
        elQty.classList.add("text-muted");
    }
}

// ─── ORDER TOTAL ───────────────────────────────────────────
function getTransportCost() {
    return parseFloat(document.querySelector("[name='TransportPrice']")?.value || "0");
}

function updateOrderTotal() {
    let subtotal = 0;
    document.querySelectorAll(".row-item").forEach(row => {
        const price = parseFloat(row.dataset.price || "0");
        const qty = parseInt(row.querySelector(".quantity-input")?.value || "0");
        subtotal += price * qty;
    });

    const transport = getTransportCost();
    const isFastOrder = document.querySelector("#FastOrder")?.checked;
    const fastOrderAmount = isFastOrder ? (subtotal + transport) * 0.20 : 0;
    const afterFast = subtotal + transport + fastOrderAmount;
    const vat = afterFast * 0.25;
    const total = afterFast + vat;

    document.getElementById("orderTotal").textContent = `${total.toFixed(2)} kr`;
    renderBreakdown(subtotal, transport, fastOrderAmount, vat);
}

function renderBreakdown(subtotal, transport, fastOrder, vat) {
    let html = `
        <div>Orderpris: ${subtotal.toFixed(2)} kr</div>
        <div>Transport: ${transport.toFixed(2)} kr</div>
    `;
    if (fastOrder > 0) html += `<div>Snabborder (+20%): ${fastOrder.toFixed(2)} kr</div>`;
    html += `<div>Moms (25%): ${vat.toFixed(2)} kr</div>`;
    document.getElementById("orderBreakdown").innerHTML = html;
}

// ─── CLEAR PREVIEW ─────────────────────────────────────────
function clearPreview(row) {
    const img = row.querySelector(".hat-image");
    img.src = "";
    img.classList.add("d-none");
    row.querySelector(".hat-name").textContent = "";
    row.querySelector(".hat-description").textContent = "";
    row.querySelector(".hat-price").textContent = "";
    row.querySelector(".hat-quantity").textContent = "";
    row.querySelector(".hat-total").textContent = "";
    row.dataset.price = "";
}
