// ===============================
// ABRIR MODAL EDITAR
// ===============================
function abrirEditar(btn) {
    const form = document.getElementById("formEditar");

    form.PersonaID.value = btn.dataset.id;
    form.PrimerNombre.value = btn.dataset.primernombre || "";
    form.SegundoNombre.value = btn.dataset.segundonombre || "";
    form.PrimerApellido.value = btn.dataset.primerapellido || "";
    form.SegundoApellido.value = btn.dataset.segundoapellido || "";
    form.DNI.value = btn.dataset.dni || "";
    form.Genero.value = btn.dataset.genero || "";
    form.FechaNacimiento.value = btn.dataset.fechanacimiento || "";
    form.Email.value = btn.dataset.email || "";
    form.Telefono.value = btn.dataset.telefono || "";
    form.Direccion.value = btn.dataset.direccion || "";

    new bootstrap.Modal(document.getElementById("modalEditar")).show();
}

// ===============================
// CREAR PERSONA
// ===============================
$(document).ready(function () {

    $("#formCrear").on("submit", function (e) {
        e.preventDefault();

        $.post("/Persona/Crear", $(this).serialize())
            .done(msg => {
                toastr.success(msg);
                bootstrap.Modal.getInstance(
                    document.getElementById("modalCrear")
                )?.hide();

                setTimeout(() => location.reload(), 800);
            })
            .fail(r => {
                toastr.error(r.responseText || "Error al crear la persona");
            });
    });

    // ===============================
    // EDITAR PERSONA
    // ===============================
    $("#formEditar").on("submit", function (e) {
        e.preventDefault();

        $.post("/Persona/Editar", $(this).serialize())
            .done(msg => {
                toastr.success(msg);
                bootstrap.Modal.getInstance(
                    document.getElementById("modalEditar")
                )?.hide();

                setTimeout(() => location.reload(), 800);
            })
            .fail(r => {
                toastr.error(r.responseText || "Error al actualizar la persona");
            });
    });

});
