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