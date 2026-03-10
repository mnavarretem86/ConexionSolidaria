document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("cambiarPasswordForm");

    form?.addEventListener("submit", async (e) => {
        e.preventDefault();

        const nueva = document.getElementById("nuevaPassword").value;
        const confirmar = document.getElementById("confirmarPassword").value;
        const actual = document.getElementById("passwordActual")?.value || "";

        if (nueva !== confirmar)
            return Swal.fire("Error", "Las contraseñas no coinciden", "error");

        try {
            const res = await fetch("/Auth/CambiarPassword", {
                method: "POST",
                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                body: new URLSearchParams({ nuevaPassword: nueva, passwordActual: actual })
            });

            const data = await res.json();

            if (data.success) {
                Swal.fire({ icon: "success", title: "Contraseña actualizada", timer: 2000, showConfirmButton: false })
                    .then(() => window.location.href = "/");
            } else {
                Swal.fire("Error", data.message || "Error al cambiar contraseña", "error");
            }
        } catch (err) {
            Swal.fire("Error", "Error de conexión", "error");
        }
    });
});