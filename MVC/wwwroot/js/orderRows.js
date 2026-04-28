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
        if (sel) sel.name = `Rows[${rowIdx}].Materials[${mIdx}].MaterialId`;
        if (qty) qty.name = `Rows[${rowIdx}].Materials[${mIdx}].Amount`;
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
    const sizeSelect = rowDiv.querySelector(".row-size");
    for (const [id, hat] of allHats) {
        const opt = document.createElement("option");
        opt.value = id;
        opt.textContent = hat.name;
        hatSelect.appendChild(opt);
    }
    hatSelect.addEventListener("change", () => {
        const hat = allHats.get(hatSelect.value);
        if (hat) renderPreview(rowDiv, hat);
    });
    if (sizeSelect) {
        sizeSelect.addEventListener("change", () => {
            const hat = allHats.get(hatSelect.value);
            if (hat) renderPreview(rowDiv, hat);
        });
    }
};

const normalizeSizeLabel = (value) =>
    value
        .toString()
        .trim()
        .toLowerCase()
        .replace(/–/g, "-")
        .replace(/\s+/g, " ")
        .replace(/\s*-\s*/g, "-")
        .replace(/år/, "y")
        .replace(/månader/, "m");

const sizePatterns = {
    "0-6m": /0\s*[-–]\s*6.*månad/i,
    "6-12m": /6\s*[-–]\s*12.*månad/i,
    "1-2y": /1\s*[-–]\s*2.*år/i,
    "2-4y": /2\s*[-–]\s*4.*år/i,
    "5-7y": /5\s*[-–]\s*7.*år/i,
    "8-12y": /8\s*[-–]\s*12.*år/i,
    "S": /\bS\b/i,
    "M": /\bM\b/i,
    "L": /\bL\b/i,
    "XL": /\bXL\b/i,
};

const getSizeStock = (hat, selectedSize) => {
    if (!selectedSize) return null;

    const sizes = Array.isArray(hat.sizes)
        ? hat.sizes
        : Array.isArray(hat.Sizes)
            ? hat.Sizes
            : [];

    const pattern = sizePatterns[selectedSize];

    if (pattern) {
        const sizeObj = sizes.find(s => {
            const rawLabel = s.label ?? s.Label ?? "";
            return pattern.test(rawLabel);
        });
        return sizeObj?.quantity ?? sizeObj?.Quantity ?? null;
    }

    // Fallback to exact match if no pattern
    const normalizedSelected = normalizeSizeLabel(selectedSize);
    const sizeObj = sizes.find(s => {
        const rawLabel = s.label ?? s.Label ?? "";
        const normalizedLabel = normalizeSizeLabel(rawLabel);
        return normalizedLabel === normalizedSelected;
    });

    return sizeObj?.quantity ?? sizeObj?.Quantity ?? null;
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
    const sizeSelect = row.querySelector(".row-size");
    const selectedSize = sizeSelect?.value;

    const defaultStock = hat.quantity ?? 0;
    const sizeStock = getSizeStock(hat, selectedSize);
    const stock = selectedSize
        ? (sizeStock !== null ? sizeStock : 0)
        : defaultStock;

    console.log(`Hat: ${hat.name}, SelectedSize: ${selectedSize}, SizeStock: ${sizeStock}, Stock: ${stock}, DefaultStock: ${defaultStock}`);

    const el = row.querySelector(".hat-quantity");
    el.textContent = selectedSize
        ? `I lager (${selectedSize}): ${stock}`
        : `I lager: ${stock}`;

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

    for (const [id, mat] of allMaterials) {
        const option = document.createElement("option");
        option.value = id;
        option.textContent = mat.name;
        select.appendChild(option);
    }

    const unitSpan = row.querySelector(".input-group-text");
    const updateUnit = () => {
        const mat = allMaterials.get(select.value);
        if (unitSpan) unitSpan.textContent = mat?.unit ?? "";
    };
    updateUnit();
    select.addEventListener("change", updateUnit);

    reIndexRows();
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
