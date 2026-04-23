import { validateStep1 } from "./orderValidation.js";
import { updateOrderTotal } from "./orderPricing.js";

let stepIndex = 1;
const maxIndex = 2;

export const initStepNav = () => {
    document.getElementById("prevBtn").addEventListener("click", () => {
        if (stepIndex === 1) return;
        setStep(stepIndex - 1);
        stepIndex--;
        const nextBtn = document.getElementById("nextBtn");
        nextBtn.type = "button";
        nextBtn.textContent = "Nästa";
    });

    document.getElementById("nextBtn").addEventListener("click", () => {
        if (stepIndex >= maxIndex) return;
        if (stepIndex === 1 && !validateStep1()) return;
        setStep(stepIndex + 1);
        updateOrderTotal();
        stepIndex++;
        setTimeout(() => {
            const nextBtn = document.getElementById("nextBtn");
            nextBtn.type = "submit";
            nextBtn.textContent = "Skicka";
        }, 0);
    });
};

const setStep = (index) => {
    document.getElementById(`step${stepIndex}`).style.display = "none";
    document.getElementById(`step${index}`).style.display = "block";
};
