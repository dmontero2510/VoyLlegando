// =======================================================
// VOYLLEGANDO
// CEREALES
// SYSTEM ADMINISTRATOR
// =======================================================

let cereales = [];

let cerealSeleccionado = null;


// =======================================================
// INICIO
// =======================================================

async function iniciar()
{
    const perfil =
        await validarSesion(
            "S"
        );


    if (!perfil)
        return;


    const nombreUsuario =
        document.getElementById(
            "nombreUsuario"
        );


    if (nombreUsuario)
    {
        nombreUsuario.textContent =
            perfil.nombre ||
            "Administrador";
    }


    await cargarCereales();
}


iniciar();


// =======================================================
// VOLVER
// =======================================================

function volver()
{
    window.location.href =
        "/tablas.html";
}


// =======================================================
// MENSAJES
// =======================================================

function mensaje(
    texto,
    esError = false
)
{
    const elemento =
        document.getElementById(
            "mensaje"
        );


    if (!elemento)
        return;


    elemento.textContent =
        texto || "";


    elemento.className =
        esError
            ? "mensaje error"
            : "mensaje";
}


// =======================================================
// CARGAR CEREALES
// =======================================================

async function cargarCereales()
{
    const lista =
        document.getElementById(
            "listaCereales"
        );


    lista.innerHTML =
        "Cargando...";


    try
    {
        cereales =
            await API.get(
                "/api/Cereales"
            );


        mostrarCereales();
    }
    catch (error)
    {
        lista.innerHTML =
            "";


        mensaje(
            error.message,
            true
        );
    }
}


// =======================================================
// MOSTRAR CEREALES
// =======================================================

function mostrarCereales()
{
    const lista =
        document.getElementById(
            "listaCereales"
        );


    lista.innerHTML =
        "";


    if (
        !cereales ||
        cereales.length === 0
    )
    {
        lista.innerHTML =
            "No hay cereales cargados.";

        return;
    }


    cereales.forEach(
        cereal =>
        {
            const item =
                document.createElement(
                    "div"
                );


            item.className =
                "cereal-item";


            if (
                cerealSeleccionado &&
                cerealSeleccionado.idCereal ===
                    cereal.idCereal
            )
            {
                item.classList.add(
                    "seleccionado"
                );
            }


            const estado =
                cereal.habilitado
                    ? "Habilitado"
                    : "Deshabilitado";


            const claseEstado =
                cereal.habilitado
                    ? "estado-habilitado"
                    : "estado-deshabilitado";


            item.innerHTML =
                `
                <div class="cereal-nombre">

                    ${escapar(cereal.nombre)}

                </div>

                <div class="cereal-detalle">

                    Código:
                    ${cereal.idCereal}

                    <br>

                    <span class="${claseEstado}">
                        ${estado}
                    </span>

                </div>
                `;


            item.onclick =
                () =>
                    seleccionarCereal(
                        cereal.idCereal
                    );


            lista.appendChild(
                item
            );
        }
    );
}


// =======================================================
// NUEVO CEREAL
// =======================================================

function nuevoCereal()
{
    cerealSeleccionado =
        null;


    let proximoCodigo =
        1;


    if (
        cereales &&
        cereales.length > 0
    )
    {
        const maximo =
            Math.max(
                ...cereales.map(
                    cereal =>
                        Number(
                            cereal.idCereal
                        ) || 0
                )
            );


        proximoCodigo =
            maximo + 1;
    }


    const idCereal =
        document.getElementById(
            "idCereal"
        );


    idCereal.value =
        proximoCodigo;

    idCereal.disabled =
        false;


    document
        .getElementById(
            "nombre"
        )
        .value =
            "";


    document
        .getElementById(
            "habilitado"
        )
        .checked =
            true;


    document
        .getElementById(
            "tituloCereal"
        )
        .textContent =
            "Nuevo Cereal";


    const botonEstado =
        document.getElementById(
            "btnEstado"
        );


    botonEstado.classList.add(
        "oculto"
    );


    mostrarCereales();


    document
        .getElementById(
            "nombre"
        )
        .focus();
}

// =======================================================
// SELECCIONAR CEREAL
// =======================================================

async function seleccionarCereal(
    idCereal
)
{
    try
    {
        const cereal =
            await API.get(
                `/api/Cereales/${idCereal}`
            );


        cerealSeleccionado =
            cereal;


        const inputId =
            document.getElementById(
                "idCereal"
            );


        inputId.value =
            cereal.idCereal;


        // El código no puede cambiarse
        // una vez creado.
        inputId.disabled =
            true;


        document
            .getElementById(
                "nombre"
            )
            .value =
                cereal.nombre || "";


        document
            .getElementById(
                "habilitado"
            )
            .checked =
                cereal.habilitado;


        document
            .getElementById(
                "tituloCereal"
            )
            .textContent =
                `Cereal #${cereal.idCereal}`;


        actualizarBotonEstado(
            cereal
        );


        mostrarCereales();
    }
    catch (error)
    {
        mensaje(
            error.message,
            true
        );
    }
}


// =======================================================
// ACTUALIZAR BOTON ESTADO
// =======================================================

function actualizarBotonEstado(
    cereal
)
{
    const boton =
        document.getElementById(
            "btnEstado"
        );


    if (!cereal)
    {
        boton.classList.add(
            "oculto"
        );

        return;
    }


    boton.classList.remove(
        "oculto"
    );


    if (cereal.habilitado)
    {
        boton.textContent =
            "Deshabilitar";

        boton.classList.remove(
            "btn-principal"
        );

        boton.classList.add(
            "btn-peligro"
        );
    }
    else
    {
        boton.textContent =
            "Habilitar";

        boton.classList.remove(
            "btn-peligro"
        );

        boton.classList.add(
            "btn-principal"
        );
    }
}


// =======================================================
// GUARDAR CEREAL
// =======================================================

async function guardarCereal()
{
    const idTexto =
        document
            .getElementById(
                "idCereal"
            )
            .value;


    const nombre =
        document
            .getElementById(
                "nombre"
            )
            .value
            .trim();


    const idCereal =
        Number(
            idTexto
        );


    if (
        !Number.isInteger(
            idCereal
        ) ||
        idCereal <= 0
    )
    {
        mensaje(
            "Ingrese un código de cereal válido.",
            true
        );

        return;
    }


    if (!nombre)
    {
        mensaje(
            "Ingrese el nombre del cereal.",
            true
        );

        return;
    }


    if (
        nombre.length > 100
    )
    {
        mensaje(
            "El nombre no puede superar los 100 caracteres.",
            true
        );

        return;
    }


    // ---------------------------------------------------
    // ESTADO
    //
    // Nuevo:
    // siempre nace habilitado.
    //
    // Edición:
    // conserva el estado actual.
    // ---------------------------------------------------

    const habilitado =
        cerealSeleccionado
            ? cerealSeleccionado.habilitado
            : true;


    const datos =
    {
        idCereal:
            idCereal,

        nombre:
            nombre,

        habilitado:
            habilitado
    };


    try
    {
        let respuesta;


        if (cerealSeleccionado)
        {
            respuesta =
                await API.put(
                    `/api/Cereales/${cerealSeleccionado.idCereal}`,
                    datos
                );
        }
        else
        {
            respuesta =
                await API.post(
                    "/api/Cereales",
                    datos
                );
        }


        mensaje(
            respuesta.mensaje
        );


        await cargarCereales();


        await seleccionarCereal(
            idCereal
        );
    }
    catch (error)
    {
        mensaje(
            error.message,
            true
        );
    }
}


// =======================================================
// CAMBIAR ESTADO
// =======================================================

async function cambiarEstadoCereal()
{
    if (!cerealSeleccionado)
        return;


    if (cerealSeleccionado.habilitado)
    {
        await deshabilitarCereal();
    }
    else
    {
        await habilitarCereal();
    }
}


// =======================================================
// DESHABILITAR CEREAL
// =======================================================

async function deshabilitarCereal()
{
    if (!cerealSeleccionado)
        return;


    if (
        !confirm(
            `¿Deshabilitar el cereal "${cerealSeleccionado.nombre}"?`
        )
    )
    {
        return;
    }


    try
    {
        const id =
            cerealSeleccionado.idCereal;


        const respuesta =
            await API.delete(
                `/api/Cereales/${id}`
            );


        mensaje(
            respuesta.mensaje
        );


        await cargarCereales();


        await seleccionarCereal(
            id
        );
    }
    catch (error)
    {
        mensaje(
            error.message,
            true
        );
    }
}


// =======================================================
// HABILITAR CEREAL
// =======================================================

async function habilitarCereal()
{
    if (!cerealSeleccionado)
        return;


    if (
        !confirm(
            `¿Habilitar el cereal "${cerealSeleccionado.nombre}"?`
        )
    )
    {
        return;
    }


    try
    {
        const id =
            cerealSeleccionado.idCereal;


        const datos =
        {
            idCereal:
                id,

            nombre:
                cerealSeleccionado.nombre,

            habilitado:
                true
        };


        const respuesta =
            await API.put(
                `/api/Cereales/${id}`,
                datos
            );


        mensaje(
            respuesta.mensaje
        );


        await cargarCereales();


        await seleccionarCereal(
            id
        );
    }
    catch (error)
    {
        mensaje(
            error.message,
            true
        );
    }
}


// =======================================================
// ESCAPAR HTML
// =======================================================

function escapar(
    valor
)
{
    const elemento =
        document.createElement(
            "div"
        );


    elemento.textContent =
        valor ?? "";


    return elemento.innerHTML;
}