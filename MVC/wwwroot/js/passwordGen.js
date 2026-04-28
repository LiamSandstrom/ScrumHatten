
document.addEventListener("DOMContentLoaded", () => {
    const useButton = document.getElementById("useButton");
    useButton.disabled = true;

    document.getElementById("passwordButton").addEventListener("click", async () => {
        const res = await fetch('/Account/GeneratePassword');
        const data = await res.json();
        document.getElementById('PasswordGen').value = data.password;
        useButton.disabled = false;
    });

    document.getElementById("useButton").addEventListener("click", () => {
        const passwordValue = document.getElementById('PasswordGen').value;
        document.getElementById('Password').value = passwordValue;
        document.getElementById('Password2').value = passwordValue;
    });
});

document.getElementById("passwordButton").addEventListener("click", async () => {
    const res = await fetch('/Account/GeneratePassword');
    const data = await res.json();
    document.getElementById('PasswordGen').value = data.password;
});

document.getElementById("useButton").addEventListener("click", async () => {
    const passwordValue = document.getElementById('PasswordGen').value;
    document.getElementById('Password').value = passwordValue;
    document.getElementById('Password2').value = passwordValue
})
