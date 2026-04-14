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

        if (data?.notify) {
            data.success
                ? ui.showSuccess(data.message)
                : ui.showError(data.message);
        }

        if (data?.redirectUrl) {
            const delay = data.notify ? 1000 : 0;
            setTimeout(() => {
                data.redirectUrl === "refresh"
                    ? location.reload()
                    : window.location.href = data.redirectUrl;
            }, delay);
        }

        response.parsedData = data;
    }

    return response;
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
