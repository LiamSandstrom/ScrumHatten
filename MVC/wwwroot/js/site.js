class NotifyUI {
    #element
    #timeout
    #mainElement
    constructor() {
        this.#element = null;
        this.#timeout = null;
        this.#mainElement = document.querySelector(".mainContainer");
    }
    showSuccess(message) {
        this.createElement(message, "notify-success notification", "/icons/Check.svg")
    }
    showError(message) {
        this.createElement(message, "notify-error notification", "/icons/Error.svg")
    }
    createElement(message, cssClass, imgUrl) {
        if (this.#element) {
            this.#element.remove();
            this.#element = null
            clearTimeout(this.#timeout)
        }
        const div = document.createElement("div");
        div.classList = cssClass
        const icon = document.createElement("img")
        icon.src = imgUrl
        div.appendChild(icon)
        const text = document.createElement("p")
        text.textContent = message
        div.appendChild(text)
        this.#mainElement.appendChild(div)
        this.#element = div;
        this.#timeout = setTimeout(() => this.#element.style.opacity = 0, 3000)
    }
}

async function apiFetch(url, options = {}) {
    const ui = new NotifyUI()
    const response = await fetch(url, options);
    const contentType = response.headers.get('content-type');
    if (contentType?.includes('application/json')) {
        const data = await response.json();

        clearErrorText();

        if (data?.notify) {
            data.success
                ? ui.showSuccess(data.message)
                : ui.showError(data.message);
        }

        if (data?.errors) {
            for (let [key, message] of Object.entries(data.errors)) {
                let safeKey = key
                    .replace(/\./g, "-")
                    .replace(/\[/g, "-")
                    .replace(/\]/g, "");
                if (key.toLowerCase() === "password") safeKey = "Password";
                const errElement = document.getElementById(`error-${safeKey}`);
                if (errElement) {
                    errElement.textContent = message;
                } else {
                    const formError = document.getElementById("formError");
                    if (formError) {
                        formError.textContent = message;
                        formError.classList.remove("d-none");
                    }
                }
            }
        }

        if (data?.redirectUrl) {
            const delay = data.notify ? 1000 : 0;
            setTimeout(() => {
                if (data.redirectUrl === "refresh") {
                    if (data.notify && data.message) {
                        sessionStorage.setItem("pendingNotify", JSON.stringify({
                            message: data.message,
                            success: data.success
                        }));
                    }
                    location.reload();
                } else {
                    window.location.href = data.redirectUrl;
                }
            }, delay);
        }

        response.parsedData = data;
    }
    return response;
}

function clearErrorText() {
    const errTexts = document.querySelectorAll(".error-text");
    for (const t of errTexts) {
        t.textContent = "";
    }

    const formError = document.getElementById("formError");
    if (formError) {
        formError.classList.add("d-none");
        formError.textContent = "";
    }
}

handleFormSubmit = (formId) => {
    const form = document.getElementById(formId);
    if (!form || form.dataset.bound) return;
    form.dataset.bound = "true";
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        await apiFetch(form.action, {
            method: "POST",
            body: new FormData(form)
        });
    });
}

document.addEventListener("DOMContentLoaded", () => {
    const pending = sessionStorage.getItem("pendingNotify");
    if (!pending) return;
    sessionStorage.removeItem("pendingNotify");
    const ui = new NotifyUI();
    const { message, success } = JSON.parse(pending);
    success ? ui.showSuccess(message) : ui.showError(message);
});
