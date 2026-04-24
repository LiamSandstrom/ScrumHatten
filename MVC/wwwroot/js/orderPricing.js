export const getTransportCost = () =>
    parseFloat(document.querySelector("[name='TransportPrice']")?.value || "0");

export const getDiscountFraction = () =>
    parseFloat(document.querySelector("[name='Discount']")?.value / 100 || "0");

export const getCustomsFraction = () =>
    parseFloat(document.querySelector("[name='Customs']")?.value / 100 || "0");

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
    const customs = getCustomsFraction();

    const discounted = subtotal * (1 - discount);  // discount on subtotal
    const vat = discounted * 0.25;                  // moms only on discounted product price
    const afterVat = discounted + vat;
    const isFast = document.getElementById("fastOrder").checked;
    const fastAmount = isFast ? afterVat * 0.20 : 0;
    const afterFast = afterVat + fastAmount;
    const customsAmount = afterFast * customs;      // tull after fastorder
    const total = afterFast + customsAmount + transport; // transport last

    document.getElementById("orderTotal").textContent = `${total.toFixed(2)} kr`;
    renderBreakdown(subtotal, discounted, vat, transport, isFast, fastAmount, customsAmount, customs, rowSummaries);
};

const renderBreakdown = (originalSubtotal, discounted, vat, transport, isFast, fastAmount, customsAmount, customsFraction, rows) => {
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
                    <span class="fw-bold">${discounted.toFixed(2)} kr</span>
                </span>
            </div>`;
    } else {
        html += `
            <div class="d-flex justify-content-between">
                <span>Orderpris</span>
                <span>${discounted.toFixed(2)} kr</span>
            </div>`;
    }

    html += `
        <div class="d-flex justify-content-between">
            <span>Moms (25%)</span>
            <span>
                <span class="">${(vat).toFixed(2)} kr</span>
            </span>
        </div>`;

    if (isFast) {
        html += `
            <div class="d-flex justify-content-between text-primary">
                <span>Snabborder (+20%)</span>
                <span>${fastAmount.toFixed(2)} kr</span>
            </div>`;
    }

    if (customsFraction > 0) {
        html += `
            <div class="d-flex justify-content-between">
                <span>Tull (${(customsFraction * 100).toFixed(0)}%)</span>
                <span>${customsAmount.toFixed(2)} kr</span>
            </div>`;
    }

    html += `
        <div class="d-flex justify-content-between">
            <span>Transport</span>
            <span>${transport.toFixed(2)} kr</span>
        </div>`;

    document.getElementById("orderBreakdown").innerHTML = html;
};

export const initPricingListeners = () => {
    document.querySelector("[name='TransportPrice']")?.addEventListener("input", updateOrderTotal);
    document.querySelector("[name='Discount']")?.addEventListener("input", updateOrderTotal);
    document.getElementById("fastOrder").addEventListener("input", updateOrderTotal);
    document.querySelector("[name='Customs']")?.addEventListener("input", updateOrderTotal);
};
