export const validateStep1 = () => {
    const rows = document.querySelectorAll(".row-item");
    let valid = true;

    const rowsError = document.getElementById("error-rows");
    if (rows.length === 0) {
        if (rowsError) rowsError.textContent = "Lägg till minst en hatt";
        return false;
    }
    if (rowsError) rowsError.textContent = "";

    for (const row of rows) {
        const isCustom = !!row.querySelector(".custom-price");

        valid = validateField(
            row.querySelector(".quantity-input"),
            row.querySelector(".error-qty"),
            el => parseInt(el.value) >= 1,
            "Antal måste vara minst 1"
        ) && valid;

        valid = validateField(
            row.querySelector(".row-size"),
            row.querySelector(".error-size"),
            el => !!el.value,
            "Välj storlek"
        ) && valid;

        if (isCustom) {
            valid = validateField(
                row.querySelector(".custom-price"),
                row.querySelector(".error-price"),
                el => !!el.value && parseFloat(el.value) > 0,
                "Pris måste vara mer än 0"
            ) && valid;

            const materialsError = row.querySelector(".error-materials");
            if (row.querySelectorAll(".material-row").length === 0) {
                if (materialsError) materialsError.textContent = "Lägg till minst ett material";
                valid = false;
            } else {
                if (materialsError) materialsError.textContent = "";
            }
        } else {
            valid = validateField(
                row.querySelector(".hat-dropdown"),
                row.querySelector(".error-hat"),
                el => !!el.value,
                "Välj en hatt"
            ) && valid;
        }

        for (const mRow of row.querySelectorAll(".material-row")) {
            valid = validateField(
                mRow.querySelector(".material-quantity"),
                mRow.querySelector(".error-material-qty"),
                el => parseInt(el.value) >= 1,
                "Antal måste vara minst 1"
            ) && valid;
        }
    }

    return valid;
};

const validateField = (el, errEl, check, msg) => {
    if (!el) return true;
    const ok = check(el);
    el.classList.toggle("is-invalid", !ok);
    if (errEl) errEl.textContent = ok ? "" : msg;
    return ok;
};
