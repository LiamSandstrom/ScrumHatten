
// Kod för sökning.
const el = document.getElementById("searchField");

el.addEventListener("input", async (e) => {

    const searchTerm = e.target.value;

    if (searchTerm.length > 2 || searchTerm === "") {

        const response = await fetch(`/Customer/Search?searchTerm=${searchTerm}`);
        const data = await response.json();

        console.log(data);

        const tbody = document.querySelector("#personTable tbody");

        tbody.innerHTML = data.map((item, index) => `
            <tr>
                <th>${index + 1}</th>
                <td>
                <a href="Customer/Edit?id=${item.id}">${item.name} <i class="bi bi-pencil-square"></i>
                </a>
                </td>
                <td>${item.email}</td>
                <td>${item.phoneNumber}</td>
                <td>${item.adress}</td>
                <td>${item.zipCode}</td>
                <td>${item.city}</td>
                <td>${item.country}</td>
                <td>${item.discount}</td>
                <td>
                <a href="mailto:${item.email}" class="btn btn-sm btn-outline-secondary" title="Skicka mail">
                <i class="bi bi-envelope-fill"></i>
                </a>
                </td>
                <td>
                <a href="/Customer?id=${item.id}"><i class="bi bi-clock-history"></i></a>
                </td>
            </tr>
        `).join('');
    }
});

 //KOd för att synliggöra rensa-knappen.

const cityDropDown = document.getElementById("dropdowncity");


cityDropDown.addEventListener("change", e => {

    const clearButton = document.getElementById("clearButton");

    if (cityDropDown.value !== '') {

      
        clearButton.classList.remove("invisible");
    }


    else if (cityDropDown.value === "" || cityDropDown.value === "Stad") {


        clearButton.classList.add("invisible");
    }

    
})

const dropDownCountry = document.getElementById("dropDownCountry");



dropDownCountry.addEventListener("change", e => {

    const clearButton = document.getElementById("clearButton");

    if (dropDownCountry.value !== '') {


        clearButton.classList.remove("invisible");
    }


    else if (dropDownCountry.value === "" || dropDownCountry.value === "Land") {


        clearButton.classList.add("invisible");
    }


})

const openFilterModelButton = document.getElementById("openFilterModelButton");

openFilterModelButton.addEventListener("click", e => {

    const dropDownCountry = document.getElementById("dropDownCountry");
    const cityDropDown = document.getElementById("dropdowncity");

    if (dropDownCountry.value !== '' || cityDropDown.value !== '') {


            clearButton.classList.remove("invisible");
        }


        else if (dropDownCountry.value === "" || dropDownCountry.value === "Land") {


            clearButton.classList.add("invisible");
        }


   
})
