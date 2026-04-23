
// Kod för sökning.
const el = document.getElementById("searchField");

el.addEventListener("input", async (e) => {

    const searchTerm = e.target.value;

    if (searchTerm.length > 2 || searchTerm === "") {

        const response = await fetch(`/User/Search?searchTerm=${searchTerm}`);
        const data = await response.json();
        
        console.log(data);

        const tbody = document.querySelector("#personTable tbody");

        tbody.innerHTML = data.map((item, index) => `
            <tr>
                <td>${index + 1}</td>
                <td>${item.user.name}</td>
                <td>${item.user.email}</td>
                <td>${item.user.phoneNumber}</td>
                <td>${item.roleName}</td>
                <td><a href="/User/Edit?id=${item.user.id}">Redigera</a></td>
            </tr>
        `).join('');
    }
});

//KOd för att synliggöra rensa-knappen.



const dropDownRole = document.getElementById("dropDownRole");

dropDownRole.addEventListener("change", e => {

    const clearButton = document.getElementById("clearButton");

    if (dropDownRole.value !== '') {


        clearButton.classList.remove("invisible");
    }


    else if (dropDownRole.value === "" || dropDownRole.value === "Land") {


        clearButton.classList.add("invisible");
    }


})

const openFilterModelButton = document.getElementById("openFilterModelButton");

openFilterModelButton.addEventListener("click", e => {

    const dropDownRole = document.getElementById("dropDownRole");


    if (dropDownRole.value !== '') {


        clearButton.classList.remove("invisible");
    }


    else if (dropDownRole.value === "" || dropDownRole.value === "Role") {


        clearButton.classList.add("invisible");
    }



})