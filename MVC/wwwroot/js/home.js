document.addEventListener('DOMContentLoaded', function () {
    fetch('/Home/LowInventoryAlerts')
        .then(res => res.json())
        .then(payload => {
            console.log('Fetched payload:', payload); 

            // Stöd för både direkt array eller inbäddad array i t.ex. payload.data
            const materials = Array.isArray(payload) ? payload : (payload.data ?? payload.materials ?? []);

            const list = document.getElementById('lowInventoryList');
            if (!materials || materials.length === 0) {
                list.innerHTML = '<li class="list-group-item">Inget material ligger under beställningspunkten.</li>';
                return;
            }

            list.innerHTML = materials.map(e => {
                const qty = e.Quantity ?? e.quantity ?? '0';
                const name = e.Name ?? e.name ?? 'Okänt';
                const unit = e.Unit ?? e.unit ?? 'Okänt';

                return `
                <li class="list-group-item d-flex align-items-center">
                <span class="text-dark me-2">${name} </span>
                 <span class="me-2"> ${qty}${unit} <i class="bi bi-exclamation-triangle"></i></span> 
  
                 </li>
                `;
            }).join('');
        })
        .catch(err => {
            console.error('Fetch error:', err);
        });
});


async function messageWidget() {

    const response = await fetch("message/unreadmessages");
    const data = await response.json();

    const table = document.getElementById("messagesList");

    table.innerHTML = data.map((item, index) => {

        const maxLength = 50; 

        const trimmedContent = item.content.length > maxLength
            ? item.content.substring(0, maxLength) + "..."
            : item.content;

        return `
    <li class="list-group-item d-flex align-items-center">
    <span class="text-dark me-2">${item.senderName}</span>
    <span class="text-dark me-2">${trimmedContent}</span>


    `}).join("");

    

} 

document.addEventListener("DOMContentLoaded", messageWidget);