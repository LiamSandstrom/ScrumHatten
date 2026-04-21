
let stepIndex = 1;
const maxIndex = 2;
const allHats = new Map();
const rowIndex = 0;
const allMaterials = new Map();

const getAllMaterials = async () => {
    const res = await fetch("/Order/GetAllMaterials");
    const data = await res.json();
    for (const m of data) {
        allMaterials.set(m.id, m.name);
    }
}

const getAllHats = async () => {
    const res = await fetch("/Order/GetAllHats")
    const data = await res.json()

    for (const d of data) {
        const { id, ...rest } = d
        allHats.set(id, rest)
    }

    console.log(allHats)
}
(async () => {
    await getAllHats()
    await getAllMaterials();
})()

//klicka knapp => 
//add Row
//name="Hats[${index}].HatId" name="Hats[0].HatId" auto binds to backend viewmodel
//Errors: id="error-Hats-${index}-HatId"    

// get data of row

// ─── Row Stuff ────────────────────────────────────────
document.getElementById("addHatBtn").addEventListener("click", () => {
    addRow();
})

const addRow = () => {
    const hatDiv = document.getElementById("rows")
    if (!hatDiv) {
        console.log("cant find hat div")
        return
    }

    const template = document.getElementById("rowTemplate");
    const clone = template.content.cloneNode(true);
    const rowItem = clone.querySelector('.row-item');

    hatDiv.prepend(clone);
    populateRows(rowItem)
}

const populateRows = (rowDiv) => {
    const hatSelect = rowDiv.querySelector('.hat-dropdown');
    hatSelect.innerHTML = `<optgroup label="Custom"><option value="-1">Skräddarsydd Hatt</option></optgroup> <optgroup id="lager-opt" label="Lager"></optgroup>  `;
    const lagerGroup = hatSelect.querySelector("#lager-opt");
    for (const h of allHats) {
        const opt = document.createElement("option");
        opt.value = h[0];
        opt.textContent = h[1].name;
        lagerGroup.appendChild(opt);
    }

    hatSelect.addEventListener("change", () => {
        const selectedId = hatSelect.value
        console.log("selected id:", selectedId)
        console.log(allHats.get(selectedId))
        renderPreview(rowDiv, allHats.get(selectedId))
    })
}

// ─── RENDER PREVIEW ────────────────────────────────────────
function renderPreview(row, hat) {
    row.dataset.price = hat.price || 0;

    const img = row.querySelector(".hat-image");
    if (hat.imageUrl && hat.imageUrl.trim() !== "") {
        img.src = hat.imageUrl;
        img.classList.remove("d-none");
    } else {
        img.src = "";
        img.classList.add("d-none");
    }

    const editBtn = row.querySelector(".edit-btn")
    editBtn.classList.remove("d-none")
    row.querySelector(".hat-name").textContent = hat.name || "";
    row.querySelector(".hat-description").textContent = hat.description || "";
    row.querySelector(".hat-price").textContent = `Pris: ${hat.price ?? "-"}`;

    const qtyInput = row.querySelector(".quantity-input");
    const selectedQty = parseInt(qtyInput.value || "0");
    const stock = hat.quantity ?? 0;

    const el = row.querySelector(".hat-quantity");
    el.textContent = `I lager: ${stock}`;

    if (selectedQty > stock) {
        el.classList.remove("text-muted");
        el.classList.add("text-danger", "fw-bold");
    } else {
        el.classList.remove("text-danger", "fw-bold");
        el.classList.add("text-muted");
    }
}

document.getElementById("rows").addEventListener("click", (e) => {
    const btn = e.target.closest(".edit-btn");
    if (!btn) return;
    const row = btn.closest(".row-item");
    if (!row) return;
    makeRowCustom(row)
});

const makeRowCustom = (row) => {
    const materialList = row.querySelector(".material-list");
    if (!materialList) return;

    // Init index counter on element itself
    if (materialList.dataset.index === undefined) {
        materialList.dataset.index = materialList.querySelectorAll(".material-row").length;
    }

    materialList.querySelector(".add-material-btn")?.remove(); // avoid duplicates

    const addBtn = document.createElement("button");
    addBtn.type = "button";
    addBtn.className = "btn btn-sm btn-outline-secondary mt-2 add-material-btn";
    addBtn.textContent = "Lägg till material";
    addBtn.addEventListener("click", () => addMaterialRow(materialList));
    materialList.after(addBtn);
}

const addMaterialRow = (materialList) => {
    const template = materialList.querySelector("template");
    if (!template) return;

    const index = parseInt(materialList.dataset.index);
    const clone = template.content.cloneNode(true);
    const matRow = clone.querySelector(".material-row");

    matRow.querySelector("input[name='Materials.Index']").value = index;

    const select = matRow.querySelector("select");
    select.name = `Materials[${index}].MaterialId`;
    select.innerHTML = [...allMaterials.entries()]
        .map(([id, name]) => `<option value="${id}">${name}</option>`)
        .join("");

    matRow.querySelector("input[type='number']").name = `Materials[${index}].Amount`;

    materialList.appendChild(clone);
    materialList.dataset.index = index + 1;
}









// ─── Swap step ────────────────────────────────────────
document.getElementById("prevBtn").addEventListener("click", () => {
    if (stepIndex == 1) return

    setStepIndex(stepIndex - 1)
    stepIndex--

});

document.getElementById("nextBtn").addEventListener("click", () => {
    if (stepIndex >= maxIndex) return

    setStepIndex(stepIndex + 1)
    stepIndex++
});

const setStepIndex = (index) => {
    const oldStep = document.getElementById(`step${stepIndex}`);
    const newStep = document.getElementById(`step${index}`);
    oldStep.style.display = "none"
    newStep.style.display = "block"
}








// ─── CLIENT VALIDATION (capture phase) ────────────────────
document.getElementById("createOrderForm").addEventListener("submit", (e) => {
    const userId = document.querySelector("[name='SelectedUserId']");
    const customerId = document.querySelector("[name='SelectedCustomerId']");
    const date = document.querySelector("[name='DateToFinish']");

    if (!userId?.value) {
        document.getElementById("error-SelectedUserId").textContent = "Välj utförare";
        rowValidationFailed = true;
    }
    if (!customerId?.value) {
        document.getElementById("error-SelectedCustomerId").textContent = "Välj kund";
        rowValidationFailed = true;
    }
    if (!date?.value || new Date(date.value) < new Date(new Date().toDateString())) {
        document.getElementById("error-DateToFinish").textContent = "Datum krävs och kan inte vara i det förflutna";
        rowValidationFailed = true;
    }

    if (rowValidationFailed) {
        e.preventDefault();
        e.stopImmediatePropagation();
    }
}, true);


// ─── TRANSPORT ─────────────────────────────────────────────
document.querySelector("[name='TransportPrice']")?.addEventListener("input", () => {
    updateOrderTotal();
});

document.getElementById("fastOrder").addEventListener("input", () => {
    updateOrderTotal();
});

// ─── ORDER TOTAL ───────────────────────────────────────────
function getTransportCost() {
    return parseFloat(document.querySelector("[name='TransportPrice']")?.value || "0");
}

document.getElementById("rows").addEventListener("click", (e) => {
    const btn = e.target.closest(".remove-row");
    if (!btn) return;
    const row = btn.closest(".row-item");
    if (!row) return;
    row.remove();
    updateOrderTotal();
});

function updateOrderTotal() {
    let subtotal = 0;

    const transport = getTransportCost();
    const isFastOrder = document.getElementById("fastOrder").checked;
    const fastOrderAmount = isFastOrder ? (subtotal + transport) * 0.20 : 0;
    const afterFast = subtotal + transport + fastOrderAmount;
    const vat = afterFast * 0.25;
    const total = afterFast + vat;

    document.getElementById("orderTotal").textContent = `${total.toFixed(2)} kr`;
    renderBreakdown(subtotal, transport, isFastOrder, fastOrderAmount, vat);
}

function renderBreakdown(subtotal, transport, isFastOrder, fastOrderAmount, vat) {
    let html = `
        <div>Orderpris: ${subtotal.toFixed(2)} kr</div>
        <div>Transport: ${transport.toFixed(2)} kr</div>
    `;
    if (isFastOrder) html += `<div>Snabborder (+20%): ${fastOrderAmount.toFixed(2)} kr</div>`;
    html += `<div>Moms (25%): ${vat.toFixed(2)} kr</div>`;
    document.getElementById("orderBreakdown").innerHTML = html;
}
