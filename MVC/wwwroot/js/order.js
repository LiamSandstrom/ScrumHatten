
let stepIndex = 1;
const maxIndex = 2;

document.getElementById("prevBtn").addEventListener("click", () => {
    if (stepIndex == 1) return

    setStepIndex(stepIndex - 1)
    stepIndex--

});

document.getElementById("nextBtn").addEventListener("click", () => {
    if (stepIndex >= maxIndex) return

    setStepIndex(stepIndex + 1)
    stepIndex++
});

const setStepIndex = (index) => {
    const oldStep = document.getElementById(`step${stepIndex}`);
    const newStep = document.getElementById(`step${index}`);
    oldStep.style.display = "none"
    newStep.style.display = "block"
}



// ─── CLIENT VALIDATION (capture phase) ────────────────────
document.getElementById("createOrderForm").addEventListener("submit", (e) => {
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

document.getElementById("fastOrder").addEventListener("input", () => {
    console.log("hi")
    updateOrderTotal();
});

// ─── ORDER TOTAL ───────────────────────────────────────────
function getTransportCost() {
    return parseFloat(document.querySelector("[name='TransportPrice']")?.value || "0");
}

function updateOrderTotal() {
    let subtotal = 0;

    const transport = getTransportCost();
    const isFastOrder = document.getElementById("fastOrder").checked;
    const fastOrderAmount = isFastOrder ? (subtotal + transport) * 0.20 : 0;
    const afterFast = subtotal + transport + fastOrderAmount;
    const vat = afterFast * 0.25;
    const total = afterFast + vat;

    document.getElementById("orderTotal").textContent = `${total.toFixed(2)} kr`;
    renderBreakdown(subtotal, transport, isFastOrder, fastOrderAmount, vat);
}

function renderBreakdown(subtotal, transport, isFastOrder, fastOrderAmount, vat) {
    let html = `
        <div>Orderpris: ${subtotal.toFixed(2)} kr</div>
        <div>Transport: ${transport.toFixed(2)} kr</div>
    `;
    if (isFastOrder) html += `<div>Snabborder (+20%): ${fastOrderAmount.toFixed(2)} kr</div>`;
    html += `<div>Moms (25%): ${vat.toFixed(2)} kr</div>`;
    document.getElementById("orderBreakdown").innerHTML = html;
}
