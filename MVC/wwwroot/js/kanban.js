document.addEventListener('DOMContentLoaded', function () {
    const columnIds = ['pending', 'inprogress', 'completed', 'delivered'];

    columnIds.forEach(id => {
        const el = document.getElementById(id);
        if (!el) return; // Säkerhetscheck om kolumnen inte finns på sidan

        new Sortable(el, {
            group: 'kanban',
            animation: 150,
            ghostClass: 'bg-light',
            onEnd: function (evt) {
                const orderId = evt.item.getAttribute('data-id');
                const newStatus = evt.to.id;
                
                console.log(`Flyttade Order ${orderId} till status: ${newStatus}`);

                updateOrderStatus(orderId, newStatus);
            }
        });
    });

    function updateOrderStatus(id, status) {
        // Vi använder Fetch API för att prata med din Controller
        fetch('/Order/UpdateStatus', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `id=${id}&status=${status}`
        })
        .then(response => {
            if (response.ok) {
                console.log("Status uppdaterad i databasen!");
            } else {
                console.error("Kunde inte uppdatera status.");
            }
        })
        .catch(error => console.error('Fel:', error));
    }
});