(() => {
    window.addErrorListener = (formId) => {

        const form = document.getElementById(formId)
        if (!form) return;

        document.getElementById(formId).addEventListener("submit", async function(e) {
            e.preventDefault();

            const form = this;
            const formData = new FormData(form);

            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    body: formData
                });

                const result = await response.json();

                clearErrorText();

                if (result.success) {
                    if (result.redirectUrl) {
                        window.location.href = result.redirectUrl;
                    } else {
                        // fallback (t.ex modal)
                        const modal = document.getElementById("createOrderModal");
                        if (modal) {
                            bootstrap.Modal.getInstance(modal)?.hide();
                            location.reload();
                        }
                    }
                } else {
                    for (let [key, message] of Object.entries(result.errors)) {

                        let safeKey = key
                            .replace(/\./g, "-")
                            .replace(/\[/g, "-")
                            .replace(/\]/g, "");

                        if (key.toLowerCase() === "password") safeKey = "Password";

                        const errElement = document.getElementById(`error-${safeKey}`);

                        if (errElement) {
                            errElement.textContent = message;
                        } else {
                            // fallback global error
                            const formError = document.getElementById("formError");
                            if (formError) {
                                formError.textContent = message;
                                formError.classList.remove("d-none");
                            }
                        }
                    }
                }
            } catch (error) {
                console.log(error);
            }
        });

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
    };

    // init forms
    addErrorListener("registerForm");
})();
