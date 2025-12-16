document.addEventListener("DOMContentLoaded", function () {

    const loginForm = document.getElementById("loginForm");
    if (!loginForm) return;

    loginForm.addEventListener("submit", function (e) {
        e.preventDefault();

        fetch("/Auth/Login", {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: new URLSearchParams({
                email: document.getElementById("loginEmail").value,
                contrasena: document.getElementById("loginPassword").value
            })
        })
            .then(res => res.json())
            .then(data => {
                const msg = document.getElementById("loginMessage");

                if (data.success) {
                    msg.className = "alert alert-success";
                    msg.innerText = "Login exitoso";
                    msg.classList.remove("d-none");

                    setTimeout(() => location.reload(), 1000);
                } else {
                    msg.className = "alert alert-danger";
                    msg.innerText = data.message;
                    msg.classList.remove("d-none");
                }
            })
            .catch(err => {
                console.error(err);
            });
    });
});
