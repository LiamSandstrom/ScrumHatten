export const getTransportCost = () =>
    parseFloat(document.querySelector("[name='TransportPrice']")?.value || "0");

export const getDiscountFraction = () =>
    parseFloat(document.querySelector("[name='Discount']")?.value / 100 || "0");

export const updateOrderTotal = () => {
    let subtotal = 0;
    const rowSummaries = [];

    document.querySelectorAll(".row-item").forEach(row => {
        const isCustom = !!row.querySelector(".custom-price");
        const qty = parseInt(row.querySelector(".quantity-input")?.value || "0");
        let price = 0, name = "";

        if (isCustom) {
            price = parseFloat(row.querySelector(".custom-price")?.value || "0");
            name = row.querySelector(".custom-description")?.value?.trim() || "Skräddarsydd hatt";
        } else {
            price = parseFloat(row.dataset.price || "0");
            const dd = row.querySelector(".hat-dropdown");
            name = dd?.options[dd.selectedIndex]?.text || "Hatt";
        }

        const rowTotal = price * qty;
        subtotal += rowTotal;
        if (name && qty > 0) rowSummaries.push({ name, qty, price, rowTotal });
    });

    const transport = getTransportCost();
    const discount = getDiscountFraction();
    const discounted = subtotal * (1 - discount);
    const isFast = document.getElementById("fastOrder").checked;
    const fastAmount = isFast ? (discounted + transport) * 0.20 : 0;
    const afterFast = discounted + transport + fastAmount;
    const vat = afterFast * 0.25;
    const total = afterFast + vat;

    document.getElementById("orderTotal").textContent = `${total.toFixed(2)} kr`;
    renderBreakdown(subtotal, discounted, transport, isFast, fastAmount, vat, rowSummaries);
};

const renderBreakdown = (originalSubtotal, subtotal, transport, isFast, fastAmount, vat, rows) => {
    const discount = getDiscountFraction();
    let html = "";

    if (rows.length > 0) {
        html += `<div class="mb-3">`;
        for (const r of rows) {
            html += `
                <div class="d-flex justify-content-between align-items-center py-1 border-bottom">
                    <div>
                        <span class="fw-semibold">${r.name}</span>
                        <span class="text-muted small ms-2">× ${r.qty}</span>
                    </div>
                    <span>${r.rowTotal.toFixed(2)} kr</span>
                </div>`;
        }
        html += `</div>`;
    }

    if (discount > 0) {
        html += `
            <div class="d-flex justify-content-between">
                <span>Orderpris</span>
                <span>
                    <span class="text-muted text-decoration-line-through me-2">${originalSubtotal.toFixed(2)} kr</span>
                    <span class="fw-bold">${subtotal.toFixed(2)} kr</span>
                </span>
            </div>`;
    } else {
        html += `
            <div class="d-flex justify-content-between">
                <span>Orderpris</span>
                <span>${subtotal.toFixed(2)} kr</span>
            </div>`;
    }

    html += `
        <div class="d-flex justify-content-between">
            <span>Transport</span>
            <span>${transport.toFixed(2)} kr</span>
        </div>`;

    if (isFast) {
        html += `
            <div class="d-flex justify-content-between text-primary">
                <span>Snabborder (+20%)</span>
                <span>${fastAmount.toFixed(2)} kr</span>
            </div>`;
    }

    html += `
        <div class="d-flex justify-content-between">
            <span>Moms (25%)</span>
            <span>${vat.toFixed(2)} kr</span>
        </div>`;

    document.getElementById("orderBreakdown").innerHTML = html;
};

export const initPricingListeners = () => {
    document.querySelector("[name='TransportPrice']")?.addEventListener("input", updateOrderTotal);
    document.querySelector("[name='Discount']")?.addEventListener("input", updateOrderTotal);
    document.getElementById("fastOrder").addEventListener("input", updateOrderTotal);
};
