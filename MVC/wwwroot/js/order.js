import { loadData } from "./orderApi.js";
import { initRowListeners } from "./orderRows.js";
import { initPricingListeners } from "./orderPricing.js";
import { initStepNav } from "./orderUI.js";

(async () => {
    await loadData();
    initRowListeners();
    initPricingListeners();
    initStepNav();
})();




// ------------------- ORDER PREVIEW ------------------------
document.addEventListener('DOMContentLoaded', function() {
    const modalEl = document.getElementById('orderDetailModal');
    let orderModal = modalEl ? new bootstrap.Modal(modalEl) : null;

    // Öppna modal vid klick på orderkort
    document.querySelectorAll('.kanban-col').forEach(column => {
        column.addEventListener('click', function(e) {
            const card = e.target.closest('.order-card');
            if (card && orderModal) {
                const orderId = card.getAttribute('data-id');
                orderModal.show();
                fetchOrderDetails(orderId);
            }
        });
    });

    document.getElementById('btnPrintOrder')?.addEventListener('click', function() {
        generateOrderPDF();
    });

    // "Ta mig an"-knappen
    document.getElementById('btnAssignToMe')?.addEventListener('click', function() {
        const orderId = document.getElementById('detailOrderId').value;
        if (orderId) assignOrderToMe(orderId);
    });

    document.getElementById('btnReleaseOrder')?.addEventListener('click', function() {
        const orderId = document.getElementById('detailOrderId').value;
        if (orderId) releaseOrder(orderId);
    });
});

// ─── ASYNC API CALLS ───────────────────────────────────────
async function fetchOrderDetails(id) {
    try {
        const response = await fetch(`/Order/Orders/${id}`);
        const order = await response.json();


        document.getElementById('detailOrderIdDisplay').textContent = id.slice(-5);
        document.getElementById('detailOrderId').value = id;


        if (order.customer) {
            document.getElementById('detailCustomerName').textContent = order.customer.name || "Namn saknas";
            document.getElementById('detailCustomerEmail').innerHTML = `<i class="bi bi-envelope"></i> ${order.customer.email || '-'}`;
            document.getElementById('detailCustomerPhone').innerHTML = `<i class="bi bi-telephone"></i> ${order.customer.phoneNumber || '-'}`;


            document.getElementById('detailCustomerAdress').textContent = order.customer.adress || "";
            document.getElementById('detailCustomerZip').textContent = order.customer.zipCode || "";
            document.getElementById('detailCustomerCity').textContent = order.customer.city || "";
            document.getElementById('detailCustomerCountry').textContent = order.customer.country || "";
        } else {
            document.getElementById('detailCustomerName').textContent = "Ingen kund kopplad";
            document.getElementById('detailCustomerEmail').textContent = "";
            document.getElementById('detailCustomerPhone').textContent = "";
            document.getElementById('detailCustomerAdress').textContent = "Adressuppgifter saknas";
            document.getElementById('detailCustomerZip').textContent = "";
            document.getElementById('detailCustomerCity').textContent = "";
            document.getElementById('detailCustomerCountry').textContent = "";
        }

        // --- HATTAR ---
        const list = document.getElementById('detailHatsList');
        list.innerHTML = order.hats?.map(h =>
            `<li class="list-group-item d-flex justify-content-between">
                ${h.name} <span>${h.quantity || 1} st</span>
            </li>`
        ).join('') || "Inga hattar valda";

        // --- PRIS ---

        const transportEl = document.getElementById('detailTransportPrice');
        if (transportEl) {
            transportEl.textContent = `${(order.transportPrice || 0).toLocaleString('sv-SE')} kr`;
        }

        document.getElementById('detailTotalPrice').textContent = `${(order.finalPrice || 0).toLocaleString('sv-SE')} kr`;

        // --- KNAPP-LOGIK ---
        const btn = document.getElementById('btnAssignToMe');
        const btnRelease = document.getElementById('btnReleaseOrder');
        const isTaken = order.makerId && order.makerId !== "00000000-0000-0000-0000-000000000000";

        if (isTaken) {
            // 1. DÖLJ "Ta mig an" istället för att bara göra den grå
            btn.classList.add('d-none');

            // 2. Visa "Släpp"-knappen
            if (btnRelease) {
                btnRelease.classList.remove('d-none');
                btnRelease.innerHTML = `<i class="bi bi-arrow-left-circle"></i> Släpp (${order.makerName || 'order'})`;
            }
        } else {
            // 1. VISA "Ta mig an" igen
            btn.classList.remove('d-none');
            btn.innerHTML = `<i class="bi bi-person-plus"></i> Ta mig an`;
            btn.disabled = false;

            // Se till att den har rätt färg (grön)
            btn.classList.remove('btn-outline-secondary');
            btn.classList.add('btn-success');

            // 2. DÖLJ "Släpp"-knappen
            if (btnRelease) btnRelease.classList.add('d-none');
        }

    } catch (err) {
        console.error("Fel vid hämtning av orderdetaljer:", err);
    }
}
async function assignOrderToMe(orderId) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const response = await fetch(`/Order/AssignToMe?orderId=${orderId}`, {
        method: 'POST',
        headers: { 'RequestVerificationToken': token }
    });
    if (response.ok) location.reload();
}

async function releaseOrder(orderId) {

    if (!confirm("Är du säker på att du vill släppa denna order?")) return;

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    try {
        const response = await fetch(`/Order/ReleaseOrder?orderId=${orderId}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': token }
        });

        if (response.ok) {
            location.reload();
        } else {
            alert("Kunde inte släppa ordern.");
        }
    } catch (err) {
        console.error("Fel:", err);
    }
}

// Initiera modalerna
const editModal = new bootstrap.Modal(document.getElementById('orderEditModal'));

// 1. När man klickar "Redigera" i Detail-modalen
document.getElementById('btnEditOrder')?.addEventListener('click', function() {
    const orderId = document.getElementById('detailOrderId').value;
    openEditModal(orderId);
});

// 2. Öppna Edit-modal och fyll med data
async function openEditModal(id) {
    try {
        const response = await fetch(`/Order/Orders/${id}`);
        const order = await response.json();

        // Nu mappar vi mot de små bokstäverna vi skickar från Controllern
        document.getElementById('editHiddenOrderId').value = order.id;
        document.getElementById('editOrderDisplayId').textContent = order.id.slice(-5);

        document.getElementById('editTransportPrice').value = order.transportPrice || 0;
        document.getElementById('editTimeToMake').value = order.timeToMake || 0;

        if (order.dateToFinish) {
            const date = new Date(order.dateToFinish);
            document.getElementById('editDateToFinish').value = date.toISOString().split('T')[0];
        }

        document.getElementById('editSelectedUserId').value = order.makerId || "";
        document.getElementById('editSelectedCustomerId').value = order.customerId || "";
        document.getElementById('editFastOrder').checked = order.fastOrder || false;

        editModal.show();
    } catch (error) {
        console.error("Kunde inte ladda ordern:", error);
    }
}

// 3. Spara ändringar
document.getElementById('editOrderForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const formData = new FormData(this);
    const orderId = document.getElementById('editHiddenOrderId').value;

    // Om checkboxen inte är ikryssad skickas den inte med FormData som "false", 
    // så vi ser till att vi har ett värde för den.
    const data = Object.fromEntries(formData.entries());
    data.FastOrder = document.getElementById('editFastOrder').checked;

    try {
        const response = await fetch(`/Order/UpdateBasicInfo/${orderId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            editModal.hide();
            location.reload(); // Uppdatera Kanban för att se ändringar (t.ex. ny kund/pris)
        } else {
            alert("Kunde inte spara ändringarna.");
        }
    } catch (error) {
        console.error("Fel vid sparande:", error);
    }
});

function generateOrderPDF() {
    const orderId = document.getElementById('detailOrderId').value;
    if (!orderId) {
        alert("Kunde inte hitta order-ID.");
        return;
    }

    const url = `/Order/PrintShippingDocument/${orderId}`;
    window.open(url, '_blank');
}
