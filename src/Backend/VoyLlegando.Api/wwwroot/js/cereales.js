// =======================================================
// VOYLLEGANDO
// CEREALES
// =======================================================

let cereales = [];

let cerealSeleccionado = null;


// =======================================================
// INICIO
// =======================================================

async function iniciar()
{
    const perfil =
        await validarSesion("L");

    if (!perfil)
        return;

    await cargarCereales();
}


iniciar();


// =======================================================
// VOLVER
// =======================================================

function volver()
{
    window.location.href =
        "/logistica.html";
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


    elemento.textContent =
        texto;


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
        lista.innerHTML = "";


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


    lista.innerHTML = "";


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


    document
        .getElementById(
            "idCereal"
        )
        .value = "";


    document
        .getElementById(
            "idCereal"
        )
        .disabled = false;


    document
        .getElementById(
            "nombre"
        )
        .value = "";


    document
        .getElementById(
            "habilitado"
        )
        .checked = true;


    document
        .getElementById(
            "tituloCereal"
        )
        .textContent =
            "Nuevo Cereal";


    document
        .getElementById(
            "btnDeshabilitar"
        )
        .classList.add(
            "oculto"
        );


    mostrarCereales();


    document
        .getElementById(
            "idCereal"
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


        document
            .getElementById(
                "idCereal"
            )
            .value =
                cereal.idCereal;


        // El ID no puede cambiarse una vez creado.
        document
            .getElementById(
                "idCereal"
            )
            .disabled = true;


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


        // Mostramos Deshabilitar solamente
        // cuando actualmente está habilitado.
        const boton =
            document.getElementById(
                "btnDeshabilitar"
            );


        if (cereal.habilitado)
        {
            boton.classList.remove(
                "oculto"
            );
        }
        else
        {
            boton.classList.add(
                "oculto"
            );
        }


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


    const habilitado =
        document
            .getElementById(
                "habilitado"
            )
            .checked;


    const idCereal =
        Number(idTexto);


    if (
        !Number.isInteger(idCereal) ||
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


    if (nombre.length > 100)
    {
        mensaje(
            "El nombre no puede superar los 100 caracteres.",
            true
        );

        return;
    }


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
// ESCAPAR HTML
// =======================================================

function escapar(valor)
{
    const elemento =
        document.createElement(
            "div"
        );


    elemento.textContent =
        valor ?? "";


    return elemento.innerHTML;
}