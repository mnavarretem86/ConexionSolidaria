document.addEventListener("DOMContentLoaded", function () {

    if (window.mensajeExito && window.mensajeExito !== '') {
        Swal.fire({
            icon: 'success',
            title: '¡Hecho!',
            text: window.mensajeExito,
            confirmButtonColor: '#198754'
        });
    }

    if (window.mensajeError && window.mensajeError !== '') {
        Swal.fire({
            icon: 'error',
            title: 'Atención',
            text: window.mensajeError,
            confirmButtonColor: '#d33'
        });
    }
    const botonesDesinscripcion = document.querySelectorAll(".btn-confirmar-des");

    botonesDesinscripcion.forEach(btn => {
        btn.addEventListener("click", function (e) {
            e.preventDefault();

            const form = this.closest("form");
            const fila = this.closest("tr");
            const elNombre = fila.querySelector(".text-event-title");
            const nombreEvento = elNombre ? elNombre.innerText.trim() : "este evento";

            Swal.fire({
                title: '¿Deseas cancelar tu inscripción?',
                html: `Se te dará de baja del evento:<br><strong class="text-danger">${nombreEvento}</strong>`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d',
                confirmButtonText: '<i class="fas fa-trash-alt me-1"></i> Sí, desinscribirme',
                cancelButtonText: 'No, mantener',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: 'Procesando baja...',
                        allowOutsideClick: false,
                        didOpen: () => { Swal.showLoading(); }
                    });
                    form.submit();
                }
            });
        });
    });
});