

const el = document.getElementById("searchField");

el.addEventListener("input", async (e) => {

    const searchTerm = e.target.value;

    if (searchTerm.length > 3 || searchTerm === "") {

        const response = await fetch(`/Customer/Search?searchTerm=${searchTerm}`);
        const data = await response.json();

        console.log(data);

        const tbody = document.querySelector("#personTable tbody");

        tbody.innerHTML = data.map((item, index) => `
            <tr>
                <td>${index + 1}</td>
                <td>${item.name}</td>
                <td>${item.email}</td>
                <td>${item.phoneNumber}</td>
                <td>${item.adress}</td>
                <td>${item.zipCode}</td>
                <td>${item.city}</td>
                <td>${item.country}</td>
                <td><a href="/Customer/Edit?id=${item.id}">Redigera</a></td>
            </tr>
        `).join('');






    }
});

