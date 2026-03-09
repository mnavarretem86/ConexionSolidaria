document.addEventListener("DOMContentLoaded", function () {

    if (window.mensajeExito && window.mensajeExito !== '') {
        Swal.fire({
            icon: 'success',
            title: '¡Logrado!',
            text: window.mensajeExito,
            confirmButtonColor: '#0d6efd',
            timer: 3000
        });
    }

    if (window.mensajeError && window.mensajeError !== '') {
        Swal.fire({
            icon: 'error',
            title: 'Ups...',
            text: window.mensajeError,
            confirmButtonColor: '#dc3545'
        });
    }

    const botonesConfirmar = document.querySelectorAll(".btn-confirmar");

    botonesConfirmar.forEach(btn => {

        btn.addEventListener("click", function (e) {

            e.preventDefault();

            const form = this.closest("form");
            const fila = this.closest("tr");

            const nombreEvento = fila.querySelector("td span")
                ? fila.querySelector("td span").innerText.trim()
                : "este evento";

            Swal.fire({
                title: '¿Confirmas tu inscripción?',
                html: `Estás por anotarte al evento: <br><strong class="text-primary">${nombreEvento}</strong>`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#198754',
                cancelButtonColor: '#6c757d',
                confirmButtonText: '<i class="fas fa-check me-1"></i> Sí, inscribirme',
                cancelButtonText: 'No, cancelar',
                reverseButtons: true
            }).then((result) => {

                if (result.isConfirmed) {

                    Swal.fire({
                        title: 'Procesando...',
                        allowOutsideClick: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });

                    form.submit();
                }

            });

        });

    });

});