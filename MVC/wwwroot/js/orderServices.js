import { updateOrderTotal } from "./orderPricing.js"

export const setupServices = () => {
    console.log("hi")
    const select = document.getElementById("SelectedCustomerId")
    console.log(select)
    select.addEventListener("change", customerChanged)
    document.getElementById("createOrderModal").addEventListener("hide.bs.modal", () => {
        document.activeElement?.blur();
    });
}

const customerChanged = async () => {
    const id = document.getElementById("SelectedCustomerId").value
    await setTullPrice(id);
    await setRabatt(id)
}

const setTullPrice = async (id) => {
    const res = await fetch(`/Order/GetCustomsRate?CustomerId=${id}`);
    const tullRate = await res.json();

    const tullElement = document.querySelector('[name="Customs"]')
    tullElement.value = tullRate.customsRate * 100
    updateOrderTotal();
}

const setRabatt = async (id) => {
    const res = await fetch(`/Order/GetCustomerById?id=${id}`);
    const customer = await res.json();

    const rabattElement = document.querySelector('[name="Discount"]')
    rabattElement.value = customer.discount
    updateOrderTotal();
}

