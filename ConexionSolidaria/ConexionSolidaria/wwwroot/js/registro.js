document.addEventListener("DOMContentLoaded", function () {

    const form = document.getElementById("formRegistro");

    if (!form) return;

    form.addEventListener("submit", async function (e) {

        e.preventDefault();

        const formData = new FormData(form);

        try {

            const response = await fetch("/Registro/Registrar", {
                method: "POST",
                body: formData
            });

            const result = await response.text();

            if (response.ok) {

                Swal.fire({
                    icon: 'success',
                    title: 'Registro exitoso',
                    text: result
                }).then(() => {
                    window.location.href = "/";
                });

            } else {

                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: result
                });

            }

        } catch (error) {

            Swal.fire({
                icon: 'error',
                title: 'Error inesperado',
                text: 'Ocurrió un problema al registrar.'
            });

        }

    });

});