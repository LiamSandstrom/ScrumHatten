import { allHats, allMaterials } from "./orderApi.js";
import { updateOrderTotal } from "./orderPricing.js";

export let rowIndex = 0;

export const reIndexRows = () => {
    const rows = document.querySelectorAll(".row-item");
    let i = 0;
    for (const row of rows) {
        setRowNames(row, i);
        setMaterialRowNames(row.querySelector(".material-list"), i);
        i++;
    }
    rowIndex = i;
};

export const setRowNames = (rowItem, i) => {
    const map = [
        [".hat-dropdown", `Rows[${i}].HatId`],
        [".quantity-input", `Rows[${i}].Quantity`],
        [".custom-description", `Rows[${i}].CustomDescription`],
        [".row-size", `Rows[${i}].Size`],
        [".custom-image", `Rows[${i}].CustomImage`],
        [".custom-price", `Rows[${i}].CustomPrice`],
    ];
    for (const [sel, name] of map) {
        const el = rowItem.querySelector(sel);
        if (el) el.name = name;
    }
};

export const setMaterialRowNames = (materialList, rowIdx) => {
    if (!materialList) return;
    const rows = materialList.querySelectorAll(".material-row");
    let mIdx = 0;
    for (const row of rows) {
        const sel = row.querySelector(".form-control");
        const qty = row.querySelector(".material-quantity");
        if (sel) sel.name = `Rows[${rowIdx}].Materials[${mIdx}].Id`;
        if (qty) qty.name = `Rows[${rowIdx}].Materials[${mIdx}].Quantity`;
        mIdx++;
    }
};

export const addRow = () => {
    const hatDiv = document.getElementById("rows");
    if (!hatDiv) return;

    const clone = document.getElementById("rowTemplate").content.cloneNode(true);
    const rowItem = clone.querySelector(".row-item");
    setRowNames(rowItem, rowIndex);
    hatDiv.prepend(clone);

    const addedRow = hatDiv.firstElementChild;
    populateHatDropdown(addedRow);
    rowIndex++;
};

export const addCustomRow = () => {
    const hatDiv = document.getElementById("rows");
    const clone = document.getElementById("customRowTemplate").content.cloneNode(true);
    const rowItem = clone.querySelector(".row-item");
    setRowNames(rowItem, rowIndex);
    makeRowCustom(rowItem);
    hatDiv.prepend(clone);

    const addedRow = hatDiv.firstElementChild;
    const fileInput = addedRow.querySelector(".custom-image");
    const preview = addedRow.querySelector(".custom-preview");
    fileInput.addEventListener("change", () => {
        const file = fileInput.files[0];
        const placeholder = addedRow.querySelector(".custom-preview-placeholder");
        if (file) {
            preview.src = URL.createObjectURL(file);
            preview.classList.remove("d-none");
            placeholder.classList.add("d-none");
        } else {
            preview.src = "";
            preview.classList.add("d-none");
            placeholder.classList.remove("d-none");
        }
    });

    rowIndex++;
};

const populateHatDropdown = (rowDiv) => {
    const hatSelect = rowDiv.querySelector(".hat-dropdown");
    for (const [id, hat] of allHats) {
        const opt = document.createElement("option");
        opt.value = id;
        opt.textContent = hat.name;
        hatSelect.appendChild(opt);
    }
    hatSelect.addEventListener("change", () => {
        renderPreview(rowDiv, allHats.get(hatSelect.value));
    });
};

export const renderPreview = (row, hat) => {
    row.dataset.price = hat.price || 0;

    const img = row.querySelector(".hat-image");
    if (hat.imageUrl?.trim()) {
        img.src = hat.imageUrl;
        img.classList.remove("d-none");
    } else {
        img.src = "";
        img.classList.add("d-none");
    }

    row.querySelector(".edit-btn")?.classList.remove("d-none");
    row.querySelector(".hat-name").textContent = hat.name || "";
    row.querySelector(".hat-description").textContent = hat.description || "";
    row.querySelector(".hat-price").textContent = `Pris: ${hat.price ?? "-"}`;

    const qtyInput = row.querySelector(".quantity-input");
    const stock = hat.quantity ?? 0;
    const el = row.querySelector(".hat-quantity");
    el.textContent = `I lager: ${stock}`;

    const overStock = parseInt(qtyInput.value || "0") > stock;
    el.classList.toggle("text-danger", overStock);
    el.classList.toggle("fw-bold", overStock);
    el.classList.toggle("text-muted", !overStock);
};

export const makeRowCustom = (row) => {
    const materialList = row.querySelector(".material-list");
    if (!materialList) return;

    if (!row.querySelector(".add-material-btn")) {
        const addBtn = document.createElement("button");
        addBtn.type = "button";
        addBtn.className = "btn btn-sm btn-outline-secondary mt-2 add-material-btn";
        addBtn.style.cssText = "max-width:150px;margin-left:10px";
        addBtn.textContent = "Lägg till material";
        addBtn.addEventListener("click", () => addMaterialRow(materialList));
        materialList.after(addBtn);
    }
};

const addMaterialRow = (materialList) => {
    const template = materialList.querySelector("template");
    if (!template) return;

    materialList.appendChild(template.content.cloneNode(true));

    const row = materialList.lastElementChild;
    const select = row.querySelector(".materials-select");

    row.querySelector(".remove-material").addEventListener("click", () => {
        row.remove();
        reIndexRows();
    });

    const parentRow = materialList.closest(".row-item");
    const rIndex = [...document.querySelectorAll(".row-item")].indexOf(parentRow);
    setMaterialRowNames(materialList, rIndex);

    for (const [id, name] of allMaterials) {
        const option = document.createElement("option");
        option.value = id;
        option.textContent = name;
        select.appendChild(option);
    }
};

export const initRowListeners = () => {
    const rowsEl = document.getElementById("rows");

    rowsEl.addEventListener("click", (e) => {
        const editBtn = e.target.closest(".edit-btn");
        if (editBtn) {
            const row = editBtn.closest(".row-item");
            if (row) makeRowCustom(row);
        }

        const removeBtn = e.target.closest(".remove-row");
        if (removeBtn) {
            const row = removeBtn.closest(".row-item");
            if (row) {
                row.remove();
                updateOrderTotal();
                reIndexRows();
            }
        }
    });

    document.getElementById("addHatBtn").addEventListener("click", addRow);
    document.getElementById("addCustomHatBtn").addEventListener("click", addCustomRow);
};
