

const el = document.getElementById("searchField");

el.addEventListener("input", async (e) => {

    const searchTerm = e.target.value;

    if (searchTerm.length > 2) {

        const response = await fetch('/Customer/Search?searchTerm=${searchTerm}');
        const data = response.json();

        console.log(data);
    }
});

