import { loadData } from "./orderApi.js";
import { initRowListeners } from "./orderRows.js";
import { initPricingListeners } from "./orderPricing.js";
import { initStepNav } from "./orderUI.js";

(async () => {
    await loadData();
    initRowListeners();
    initPricingListeners();
    initStepNav();
})();
