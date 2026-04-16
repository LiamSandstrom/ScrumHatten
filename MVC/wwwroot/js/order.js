document.getElementById("addRow").addEventListener("click", () => {
    const template = document.getElementById("rowTemplate");
    const clone = template.content.cloneNode(true);
    document.getElementById("rows").prepend(clone);
    updateOrderTotal();
});

document.querySelector("#FastOrder")?.addEventListener("change", () => {
    updateOrderTotal();
});


document.getElementById("rows").addEventListener("click", (e) => {
    const btn = e.target.closest(".remove-row");
    if (!btn) return;

    const row = btn.closest(".row-item");
    if (!row) return;

    row.remove();
    updateOrderTotal();
});


document.getElementById("rows").addEventListener("change", async (e) => {
    const row = e.target.closest(".row-item");
    if (!row) return;

    if (e.target.classList.contains("type-dropdown")) {
        const type = e.target.value;
        if (!type) return;

        const hatSelect = row.querySelector(".hat-dropdown");

        hatSelect.innerHTML = `<option>Loading...</option>`;
        clearPreview(row);

        const res = await fetch(`/Order/GetHatsByType?type=${encodeURIComponent(type)}`);
        const data = await res.json();

        hatSelect.innerHTML = `<option value="">Select hat</option>`;

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
        if (!opt || !opt.dataset.hat) return;

        const hat = JSON.parse(opt.dataset.hat);

        if (!document.body.contains(row)) return;

        renderPreview(row, hat);
    }

    if (e.target.classList.contains("quantity-input")) {
        if (!document.body.contains(row)) return;
        updateTotal(row);
        updateOrderTotal();
    }
});


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


function updateTotal(row) {
    if (!row.dataset.price) return;
    const price = parseFloat(row.dataset.price || "0");
    const qty = parseInt(row.querySelector(".quantity-input").value || "0");

    const total = price * qty;

    const el = row.querySelector(".hat-total");
    el.textContent = isNaN(total) ? "" : `Total: ${total.toFixed(2)} kr`;

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

function getTransportCost() {
    const el = document.querySelector("[name='TransportPrice']");
    return parseFloat(el?.value || "0");
}

document.querySelector("[name='TransportPrice']")?.addEventListener("input", () => {
    updateOrderTotal();
});

function updateOrderTotal() {
    let subtotal = 0;

    document.querySelectorAll(".row-item").forEach(row => {
        const price = parseFloat(row.dataset.price || "0");
        const qty = parseInt(row.querySelector(".quantity-input")?.value || "0");

        subtotal += price * qty;
    });

    const transport = getTransportCost();
    const isFastOrder = document.querySelector("#FastOrder")?.checked;

    let fastOrderAmount = 0;

    let base = subtotal + transport;

    if (isFastOrder) {
        fastOrderAmount = base * 0.20;
    }

    const afterFast = base + fastOrderAmount;
    const vat = afterFast * 0.25;
    const total = afterFast + vat;

    document.getElementById("orderTotal").textContent =
        `${total.toFixed(2)} kr`;

    renderBreakdown(subtotal, transport, fastOrderAmount, vat);
}

function renderBreakdown(subtotal, transport, fastOrder, vat) {
    const el = document.getElementById("orderBreakdown");

    let html = `
        <div>Orderpris: ${subtotal.toFixed(2)} kr</div>
        <div>Transport: ${transport.toFixed(2)} kr</div>
    `;

    if (fastOrder > 0) {
        html += `<div>Snabborder (+20%): ${fastOrder.toFixed(2)} kr</div>`;
    }

    html += `
        <div>Moms (25%): ${vat.toFixed(2)} kr</div>
    `;

    el.innerHTML = html;
}

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
