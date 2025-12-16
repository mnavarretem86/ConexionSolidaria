$(document).ready(function () {

    cargarRoles();
    cargarPersonas();

    $("#formCrear").submit(function (e) {
        e.preventDefault();

        $.post("/Usuario/Crear", $(this).serialize())
            .done(function (msg) {
                toastr.success(msg);
                $("#modalCrear").modal("hide");
                setTimeout(() => location.reload(), 800);
            })
            .fail(function (r) {
                toastr.error(r.responseText || "Error al crear usuario");
            });
    });

    $("#formEditar").submit(function (e) {
        e.preventDefault();

        $.post("/Usuario/Editar", $(this).serialize())
            .done(function (msg) {
                toastr.success(msg);
                $("#modalEditar").modal("hide");
                setTimeout(() => location.reload(), 800);
            })
            .fail(function (r) {
                toastr.error(r.responseText || "Error al actualizar usuario");
            });
    });
});

/* =========================
   MODAL EDITAR
========================= */
function abrirEditar(btn) {

    const form = document.getElementById("formEditar");
    form.UsuarioID.value = btn.dataset.usuarioid;

    cargarRoles(() => {
        form.RolID.value = btn.dataset.rolid;
    });

    new bootstrap.Modal(document.getElementById("modalEditar")).show();
}

/* =========================
   ROLES
========================= */
function cargarRoles(callback) {

    $.get("/Usuario/ObtenerRoles")
        .done(function (data) {

            ["#crearRol", "#editarRol"].forEach(id => {
                const select = $(id);
                if (!select.length) return;

                select.empty();
                select.append(`<option value="">Seleccione</option>`);

                data.forEach(r => {
                    // CORRECCIÓN de Casing: Usar camelCase (rolID, nombreRol)
                    select.append(
                        `<option value="${r.rolID}">${r.nombreRol}</option>`
                    );
                });
            });

            if (callback) callback();
        })
        .fail(() => toastr.error("No se pudieron cargar los roles"));
}

/* =========================
   PERSONAS SIN USUARIO
========================= */
function cargarPersonas() {

    // CORRECCIÓN DE RUTA: Apunta a /Usuario/PersonasSinUsuario
    $.get("/Usuario/PersonasSinUsuario")
        .done(function (data) {

            const select = $("#crearPersona");
            select.empty();
            select.append(`<option value="">Seleccione una persona</option>`);

            data.forEach(p => {
                // CORRECCIÓN de Casing: Usar camelCase (personaID, nombreCompleto)
                select.append(
                    `<option value="${p.personaID}">${p.nombreCompleto}</option>`
                );
            });

            // Añadir mensaje si la lista está vacía
            if (data.length === 0) {
                select.append(`<option value="" disabled>Todas las personas ya tienen usuario.</option>`);
            }

        })
        .fail(() => toastr.error("No se pudieron cargar las personas"));
}