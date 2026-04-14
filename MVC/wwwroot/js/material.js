function saveMaterial() {
    const materialData = {
        Name: document.getElementById('matName').value,
        Unit: document.getElementById('matUnit').value,
        PricePerUnit: parseFloat(document.getElementById('matPrice').value),
        Quantity: parseFloat(document.getElementById('matAmount').value) || 0
    };

    fetch('/Material/Create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(materialData)
    })
    .then(response => {
        if (response.ok) {
            alert('Materialet har sparats!');
            location.reload();
        } else {
            alert("Gick inte att spara materialet.");
        }

    })
    .catch(error => console.error('Error:', error));

}

function updateStock(id) {
const inputField = document.getElementById(`restock-${id}`);
const addedAmount = parseFloat(inputField.value);

if (isNaN(addedAmount) || addedAmount <= 0 ) {

    alert("Vänligen ange en giltig mängd.");
    return;
}
fetch(`/Material/Restock/${id}`,{

    method: 'PATCH',
    headers: { 'Content-Type': 'application/json'},
    body: JSON.stringify(addedAmount)
})
.then(response => {

    if (response.ok) {

        location.reload(); 

    }else{

        alert("Kunde inte uppdatera lagret.");

    }

    });

}

async function saveAllRestocks() {
    const inputs = document.querySelectorAll('input[id^="restock-"]');
    const updatePromises = [];

    inputs.forEach(input => {
        const id = input.id.replace('restock-', '');
        const addedAmount = parseFloat(input.value);

        if (!isNaN(addedAmount) && addedAmount > 0) {
            updatePromises.push(updateStockSingle(id, addedAmount));

        }
    });

    if (updatePromises.length === 0) {
        alert("Inga ändringar att spara.");
        return;
    }

    try {
        await Promise.all(updatePromises);
        alert("Lagret har uppdaterats!");
        location.reload();
        } catch (error) {
            alert("Kunde inte uppdatera lagret.");
            console.error(error);
        }
    }

    function updateStockSingle(id, addedAmount) {
    return fetch(`/Material/Restock/${id}`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(addedAmount)
    }).then(response => {
        if (!response.ok) {
            throw new Error(`Kunde inte uppdatera material med ID: ${id}`);
        }
    });
}
