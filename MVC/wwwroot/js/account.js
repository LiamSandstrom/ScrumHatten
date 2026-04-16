(() => {
    window.addErrorListener = (formId) => {

        document.getElementById(formId).addEventListener("submit", async function(e) {
            e.preventDefault();

            const formData = new FormData(this);

            try {
                const response = await fetch(this.action, {
                    method: "POST",
                    body: formData
                });

                const result = await response.json();
                clearErrorText()
                if (result.success) {
                    if (result.redirectUrl) window.location.href = result.redirectUrl;
                } else {

                    for (let [key, message] of Object.entries(result.errors)) {

                        if (key == "password") key = "Password";

                        const errElement = document.getElementById(`error-${key}`);
                        errElement.textContent = message;
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
        }
    }

    addErrorListener("registerForm")
})();

