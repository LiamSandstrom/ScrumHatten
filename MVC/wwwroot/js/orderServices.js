export const setupServices = () => {
    console.log("hi")
    const select = document.getElementById("SelectedCustomerId")
    console.log(select)
    select.addEventListener("change", customerChanged)
}

const customerChanged = async () => {
    const id = document.getElementById("SelectedCustomerId").value
    console.log(id)
    await setTullPrice(id);
}

const setTullPrice = async (id) => {
    const res = await fetch(`/Order/GetCustomsRate?CustomerId=${id}`);
    const tullRate = await res.json();
    console.log(tullRate);
}


const setRabatt = async () => {

}
