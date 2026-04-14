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

function editMaterial(id) {
    fetch(`/Material/details/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Kunde inte hämta materialet");
            }
            return response.json();
        })
        .then(data => {
            // Fyll i fälten i modalen
            document.getElementById('editMatId').value = data.id;
            document.getElementById('editMatName').value = data.name;
            document.getElementById('editMatUnit').value = data.unit;
            document.getElementById('editMatPrice').value = data.pricePerUnit;
            // Om du har ett fält för antal i edit-modalen:
            if(document.getElementById('editMatQuantity')) {
                document.getElementById('editMatQuantity').value = data.quantity;
            }

            // Öppna modalen med Bootstrap
            var editModal = new bootstrap.Modal(document.getElementById('editMaterialModal'));
            editModal.show();
        })
        .catch(error => {
            console.error('Fel:', error);
            alert("Kunde inte ladda materialets detaljer.");
        });
}

function updateMaterial() {
    const data = {
        id: document.getElementById('editMatId').value,
        name: document.getElementById('editMatName').value,
        unit: document.getElementById('editMatUnit').value,
        pricePerUnit: parseFloat(document.getElementById('editMatPrice').value),
        quantity: parseFloat(document.getElementById('editMatQuantity')?.value || 0)
    };

    fetch('/Material/Update', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    .then(response => {
        if (response.ok) {
            location.reload();
        } else {
            alert("Något gick fel vid sparandet.");
        }
    });
}

function deleteMaterial() {
    const id = document.getElementById('editMatId').value;
    const name = document.getElementById('editMatName').value;

    if (confirm(`Är du säker på att du vill ta bort "${name}"?`)) {
        fetch(`/Material/Delete/${id}`, { 
            method: 'DELETE' 
        })
        .then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert("Kunde inte ta bort materialet.");
            }
        });
    }
}