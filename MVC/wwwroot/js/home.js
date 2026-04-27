document.addEventListener('DOMContentLoaded', function () {
    fetch('/Home/LowInventoryAlerts')
        .then(res => res.json())
        .then(payload => {
            console.log('Fetched payload:', payload); // Inspektera här i konsolen/debuggern

            // Stöd för både direkt array eller inbäddad array i t.ex. payload.data
            const materials = Array.isArray(payload) ? payload : (payload.data ?? payload.materials ?? []);

            const list = document.getElementById('lowInventoryList');
            if (!materials || materials.length === 0) {
                list.innerHTML = '<li class="list-group-item">Inget material ligger under beställningspunkten.</li>';
                return;
            }

            list.innerHTML = materials.map(e => {
                // Välj rätt fältnamn (stöd för olika casing/namn)
                const qty = e.Quantity ?? e.quantity ?? e.Amount ?? e.amount ?? '0';
                const name = e.Name ?? e.name ?? e.MaterialName ?? e.materialName ?? 'Okänt';

                return `
                <li class="list-group-item d-flex align-items-center">
                <span class="text-dark">${name} </span>
                 <span class="badge me-2" style="background-color: #DE1313; color: white;"> ${qty}</span>
  
                 </li>
                `;
            }).join('');
        })
        .catch(err => {
            console.error('Fetch error:', err);
        });
});