// =======================================================
// VOYLLEGANDO
// PRODUCTORES Y CAMPOS
// =======================================================

let productores = [];

let productorSeleccionado = null;

let mapaCampo = null;

let marcadorCampo = null;

// Marcadores de referencia de los demás campos
// pertenecientes al productor seleccionado.
let marcadoresCamposProductor = [];


// =======================================================
// INICIO
// =======================================================

async function iniciar()
{
    const perfil =
        await validarSesion("L");

    if (!perfil)
        return;


    document
        .getElementById("nombreUsuario")
        .textContent =
            perfil.nombre;


    inicializarMapa();

    await cargarTiposIva();

    await cargarProductores();
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
// TIPOS DE IVA
// =======================================================

async function cargarTiposIva()
{
    const combo =
        document.getElementById("iva");


    combo.innerHTML =
        '<option value="">Seleccione...</option>';


    try
    {
        const tipos =
            await API.get(
                "/api/TiposIva"
            );


        tipos.forEach(
            tipo =>
            {
                const option =
                    document.createElement(
                        "option"
                    );


                option.value =
                    tipo.idIva;


                option.textContent =
                    tipo.descripcion;


                combo.appendChild(
                    option
                );
            }
        );
    }
    catch (error)
    {
        mensaje(
            "No se pudieron cargar los tipos de IVA: " +
            error.message,
            true
        );
    }
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
// CARGAR PRODUCTORES
// =======================================================

async function cargarProductores()
{
    const lista =
        document.getElementById(
            "listaProductores"
        );


    lista.innerHTML =
        "Cargando...";


    try
    {
        productores =
            await API.get(
                "/api/Productores"
            );


        mostrarProductores();
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
// MOSTRAR PRODUCTORES
// =======================================================

function mostrarProductores()
{
    const lista =
        document.getElementById(
            "listaProductores"
        );


    lista.innerHTML = "";


    if (
        !productores ||
        productores.length === 0
    )
    {
        lista.innerHTML =
            "No hay productores.";

        return;
    }


    productores.forEach(
        productor =>
        {
            const item =
                document.createElement(
                    "div"
                );


            item.className =
                "productor-item";


            if (
                productorSeleccionado &&
                productorSeleccionado.idProductor ===
                productor.idProductor
            )
            {
                item.classList.add(
                    "seleccionado"
                );
            }


            const estado =
                productor.habilitado
                    ? "Habilitado"
                    : "Deshabilitado";


            item.innerHTML =
                `
                <div class="productor-nombre">
                    ${escapar(productor.nombre)}
                </div>

                <div class="productor-detalle">

                    CUIT:
                    ${escapar(productor.cuit)}

                    <br>

                    ${estado}

                </div>
                `;


            item.onclick =
                () =>
                    seleccionarProductor(
                        productor.idProductor
                    );


            lista.appendChild(
                item
            );
        }
    );
}


// =======================================================
// NUEVO PRODUCTOR
// =======================================================

function nuevoProductor()
{
    productorSeleccionado =
        null;


    document
        .getElementById(
            "idProductor"
        )
        .value = "";


    document
        .getElementById(
            "nombre"
        )
        .value = "";


    document
        .getElementById(
            "domicilio"
        )
        .value = "";


    document
        .getElementById(
            "cuit"
        )
        .value = "";


    document
        .getElementById(
            "iva"
        )
        .value = "";


    document
        .getElementById(
            "habilitado"
        )
        .checked = true;


    document
        .getElementById(
            "tituloProductor"
        )
        .textContent =
            "Nuevo Productor";


    document
        .getElementById(
            "btnDeshabilitar"
        )
        .classList.add(
            "oculto"
        );


    document
        .getElementById(
            "seccionCampos"
        )
        .classList.add(
            "oculto"
        );


    cancelarCampo();

    limpiarMarcadoresCamposProductor();

    mostrarProductores();


    document
        .getElementById(
            "nombre"
        )
        .focus();
}


// =======================================================
// SELECCIONAR PRODUCTOR
// =======================================================

async function seleccionarProductor(
    idProductor
)
{
    try
    {
        const productor =
            await API.get(
                `/api/Productores/${idProductor}`
            );


        productorSeleccionado =
            productor;


        const btnNuevoCampo =
            document.getElementById(
                "btnNuevoCampo"
            );


        const aviso =
            document.getElementById(
                "avisoProductorDeshabilitado"
            );


        if (productor.habilitado)
        {
            btnNuevoCampo.disabled =
                false;


            aviso.classList.add(
                "oculto"
            );
        }
        else
        {
            btnNuevoCampo.disabled =
                true;


            aviso.classList.remove(
                "oculto"
            );
        }


        document
            .getElementById(
                "idProductor"
            )
            .value =
                productor.idProductor;


        document
            .getElementById(
                "nombre"
            )
            .value =
                productor.nombre || "";


        document
            .getElementById(
                "domicilio"
            )
            .value =
                productor.domicilio || "";


        document
            .getElementById(
                "cuit"
            )
            .value =
                productor.cuit || "";


        document
            .getElementById(
                "iva"
            )
            .value =
                productor.iva || "";


        document
            .getElementById(
                "habilitado"
            )
            .checked =
                productor.habilitado;


        document
            .getElementById(
                "tituloProductor"
            )
            .textContent =
                `Productor #${productor.idProductor}`;


        document
            .getElementById(
                "btnDeshabilitar"
            )
            .classList.remove(
                "oculto"
            );


        document
            .getElementById(
                "nombreProductorCampos"
            )
            .textContent =
                productor.nombre;


        document
            .getElementById(
                "seccionCampos"
            )
            .classList.remove(
                "oculto"
            );


        cancelarCampo();

        mostrarProductores();

        await cargarCampos();
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
// GUARDAR PRODUCTOR
// =======================================================

async function guardarProductor()
{
    const id =
        document
            .getElementById(
                "idProductor"
            )
            .value;


    const datos =
    {
        nombre:
            document
                .getElementById(
                    "nombre"
                )
                .value
                .trim(),

        domicilio:
            document
                .getElementById(
                    "domicilio"
                )
                .value
                .trim(),

        iva:
            document
                .getElementById(
                    "iva"
                )
                .value
                .trim(),

        cuit:
            document
                .getElementById(
                    "cuit"
                )
                .value
                .trim(),

        habilitado:
            document
                .getElementById(
                    "habilitado"
                )
                .checked
    };


    if (!datos.nombre)
    {
        mensaje(
            "Ingrese el nombre del productor.",
            true
        );

        return;
    }


    if (!datos.cuit)
    {
        mensaje(
            "Ingrese el CUIT.",
            true
        );

        return;
    }


    try
    {
        let respuesta;


        if (id)
        {
            respuesta =
                await API.put(
                    `/api/Productores/${id}`,
                    datos
                );


            mensaje(
                respuesta.mensaje
            );


            await cargarProductores();


            await seleccionarProductor(
                Number(id)
            );
        }
        else
        {
            respuesta =
                await API.post(
                    "/api/Productores",
                    datos
                );


            mensaje(
                respuesta.mensaje
            );


            await cargarProductores();


            await seleccionarProductor(
                respuesta.idProductor
            );
        }
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
// DESHABILITAR PRODUCTOR
// =======================================================

async function deshabilitarProductor()
{
    if (!productorSeleccionado)
        return;


    if (
        !confirm(
            `¿Deshabilitar a ${productorSeleccionado.nombre}?`
        )
    )
    {
        return;
    }


    try
    {
        const respuesta =
            await API.delete(
                `/api/Productores/${productorSeleccionado.idProductor}`
            );


        mensaje(
            respuesta.mensaje
        );


        await cargarProductores();


        await seleccionarProductor(
            productorSeleccionado.idProductor
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
// CAMPOS
// =======================================================

async function cargarCampos()
{
    if (!productorSeleccionado)
        return;


    const lista =
        document.getElementById(
            "listaCampos"
        );


    lista.innerHTML =
        "Cargando campos...";


    try
    {
        const campos =
            await API.get(
                `/api/Campos/productor/${productorSeleccionado.idProductor}`
            );


        lista.innerHTML = "";


        if (
            !campos ||
            campos.length === 0
        )
        {
            lista.innerHTML =
                "El productor no tiene campos.";

            return;
        }


        campos.forEach(
            campo =>
            {
                const item =
                    document.createElement(
                        "div"
                    );


                item.className =
                    "campo-item";


                let coordenadas =
                    "Sin ubicación";


                if (
                    campo.latitud != null &&
                    campo.longitud != null
                )
                {
                    coordenadas =
                        `${campo.latitud}, ${campo.longitud}`;
                }


                item.innerHTML =
                    `
                    <div class="campo-nombre">

                        ${escapar(campo.descripCampo)}

                    </div>

                    <div class="campo-coordenadas">

                        📍 ${coordenadas}

                    </div>

                    <div class="campo-botones">

                        <button
                            type="button"
                            class="btn btn-secundario btn-chico"
                            onclick="editarCampo(${campo.idCampo})">

                            Editar

                        </button>

                    </div>
                    `;


                lista.appendChild(
                    item
                );
            }
        );
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
// NUEVO CAMPO
// =======================================================

async function nuevoCampo()
{
    if (!productorSeleccionado)
    {
        mensaje(
            "Primero seleccione un productor.",
            true
        );

        return;
    }


    if (!productorSeleccionado.habilitado)
    {
        mensaje(
            "El productor está deshabilitado. No se pueden agregar campos.",
            true
        );

        return;
    }


    document
        .getElementById(
            "seccionCampoEditar"
        )
        .classList.remove(
            "oculto"
        );


    document
        .getElementById(
            "tituloCampo"
        )
        .textContent =
            "Nuevo Campo";


    document
        .getElementById(
            "idCampo"
        )
        .value = "";


    document
        .getElementById(
            "descripCampo"
        )
        .value = "";


    limpiarUbicacion();


    document
        .getElementById(
            "btnEliminarCampo"
        )
        .classList.add(
            "oculto"
        );


    setTimeout(
        async () =>
        {
            mapaCampo.invalidateSize();

            await mostrarCamposProductorEnMapa(
                null,
                true
            );
        },
        100
    );


    document
        .getElementById(
            "descripCampo"
        )
        .focus();
}


// =======================================================
// EDITAR CAMPO
// =======================================================

async function editarCampo(
    idCampo
)
{
    try
    {
        const campo =
            await API.get(
                `/api/Campos/${idCampo}`
            );


        document
            .getElementById(
                "seccionCampoEditar"
            )
            .classList.remove(
                "oculto"
            );


        document
            .getElementById(
                "tituloCampo"
            )
            .textContent =
                `Campo #${campo.idCampo}`;


        document
            .getElementById(
                "idCampo"
            )
            .value =
                campo.idCampo;


        document
            .getElementById(
                "descripCampo"
            )
            .value =
                campo.descripCampo;


        document
            .getElementById(
                "btnEliminarCampo"
            )
            .classList.remove(
                "oculto"
            );


        setTimeout(
            async () =>
            {
                mapaCampo.invalidateSize();


                // Mostramos como referencia todos los demás
                // campos del mismo productor.
                await mostrarCamposProductorEnMapa(
                    campo.idCampo,
                    false
                );


                if (
                    campo.latitud != null &&
                    campo.longitud != null
                )
                {
                    establecerUbicacion(
                        campo.latitud,
                        campo.longitud,
                        true
                    );
                }
                else
                {
                    limpiarUbicacion();

                    encuadrarCamposProductor();
                }
            },
            100
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
// GUARDAR CAMPO
// =======================================================

async function guardarCampo()
{
    if (!productorSeleccionado)
        return;


    const idCampo =
        document
            .getElementById(
                "idCampo"
            )
            .value;


    const descripcion =
        document
            .getElementById(
                "descripCampo"
            )
            .value
            .trim();


    if (!descripcion)
    {
        mensaje(
            "Ingrese la descripción del campo.",
            true
        );

        return;
    }


    const latitudTexto =
        document
            .getElementById(
                "latitud"
            )
            .value;


    const longitudTexto =
        document
            .getElementById(
                "longitud"
            )
            .value;


    const datos =
    {
        idProductor:
            productorSeleccionado.idProductor,

        descripCampo:
            descripcion,

        latitud:
            latitudTexto
                ? Number(latitudTexto)
                : null,

        longitud:
            longitudTexto
                ? Number(longitudTexto)
                : null
    };


    try
    {
        let respuesta;


        if (idCampo)
        {
            respuesta =
                await API.put(
                    `/api/Campos/${idCampo}`,
                    datos
                );
        }
        else
        {
            respuesta =
                await API.post(
                    "/api/Campos",
                    datos
                );
        }


        mensaje(
            respuesta.mensaje
        );


        cancelarCampo();

        await cargarCampos();
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
// ELIMINAR CAMPO
// =======================================================

async function eliminarCampo()
{
    const idCampo =
        document
            .getElementById(
                "idCampo"
            )
            .value;


    if (!idCampo)
        return;


    if (
        !confirm(
            "¿Eliminar este campo?"
        )
    )
    {
        return;
    }


    try
    {
        const respuesta =
            await API.delete(
                `/api/Campos/${idCampo}`
            );


        mensaje(
            respuesta.mensaje
        );


        cancelarCampo();

        await cargarCampos();
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
// CANCELAR CAMPO
// =======================================================

function cancelarCampo()
{
    document
        .getElementById(
            "seccionCampoEditar"
        )
        .classList.add(
            "oculto"
        );


    document
        .getElementById(
            "idCampo"
        )
        .value = "";


    document
        .getElementById(
            "descripCampo"
        )
        .value = "";


    limpiarUbicacion();

    limpiarMarcadoresCamposProductor();
}


// =======================================================
// MAPA
// =======================================================

function inicializarMapa()
{
    mapaCampo =
        L.map(
            "mapaCampo"
        );


    L.tileLayer(
        "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
        {
            maxZoom: 19,

            attribution:
                '&copy; OpenStreetMap'
        }
    )
    .addTo(
        mapaCampo
    );


    // Centro inicial aproximado de Argentina

    mapaCampo.setView(
        [-35.5, -63.0],
        6
    );


    mapaCampo.on(
        "click",
        event =>
        {
            establecerUbicacion(
                event.latlng.lat,
                event.latlng.lng,
                false
            );
        }
    );
}


// =======================================================
// CAMPOS DEL PRODUCTOR EN EL MAPA
// =======================================================

async function mostrarCamposProductorEnMapa(
    idCampoExcluir = null,
    encuadrar = true
)
{
    limpiarMarcadoresCamposProductor();


    if (
        !productorSeleccionado ||
        !mapaCampo
    )
    {
        return;
    }


    try
    {
        const campos =
            await API.get(
                `/api/Campos/productor/${productorSeleccionado.idProductor}`
            );


        if (
            !campos ||
            campos.length === 0
        )
        {
            if (encuadrar)
            {
                mapaCampo.setView(
                    [-35.5, -63.0],
                    6
                );
            }

            return;
        }


        campos.forEach(
            campo =>
            {
                // En edición no mostramos el mismo campo
                // como punto de referencia porque ese campo
                // tendrá el marcador editable.

                if (
                    idCampoExcluir != null &&
                    Number(campo.idCampo) ===
                    Number(idCampoExcluir)
                )
                {
                    return;
                }


                if (
                    campo.latitud == null ||
                    campo.longitud == null
                )
                {
                    return;
                }


                const lat =
                    Number(campo.latitud);


                const lon =
                    Number(campo.longitud);


                if (
                    !Number.isFinite(lat) ||
                    !Number.isFinite(lon)
                )
                {
                    return;
                }


                const marcador =
                    L.marker(
                        [lat, lon],
                        {
                            draggable: false
                        }
                    )
                    .addTo(
                        mapaCampo
                    );


                // Nombre siempre visible sobre el punto.
                marcador.bindTooltip(
                    escapar(campo.descripCampo),
                    {
                        permanent: true,
                        direction: "top",
                        offset: [0, -8],
                        opacity: 0.90
                    }
                );


                // También dejamos un popup por si se toca
                // el marcador desde el celular.
                marcador.bindPopup(
                    `<strong>${escapar(campo.descripCampo)}</strong>`
                );


                marcadoresCamposProductor.push(
                    marcador
                );
            }
        );


        if (encuadrar)
        {
            encuadrarCamposProductor();
        }
    }
    catch (error)
    {
        console.error(
            "No se pudieron mostrar los campos en el mapa:",
            error
        );
    }
}


// =======================================================
// ENCUADRAR CAMPOS DEL PRODUCTOR
// =======================================================

function encuadrarCamposProductor()
{
    if (
        !mapaCampo ||
        marcadoresCamposProductor.length === 0
    )
    {
        mapaCampo.setView(
            [-35.5, -63.0],
            6
        );

        return;
    }


    if (
        marcadoresCamposProductor.length === 1
    )
    {
        const posicion =
            marcadoresCamposProductor[0]
                .getLatLng();


        mapaCampo.setView(
            posicion,
            15
        );

        return;
    }


    const grupo =
        L.featureGroup(
            marcadoresCamposProductor
        );


    mapaCampo.fitBounds(
        grupo.getBounds(),
        {
            padding: [40, 40],
            maxZoom: 15
        }
    );
}


// =======================================================
// LIMPIAR PUNTOS DE REFERENCIA
// =======================================================

function limpiarMarcadoresCamposProductor()
{
    if (!mapaCampo)
    {
        marcadoresCamposProductor = [];

        return;
    }


    marcadoresCamposProductor.forEach(
        marcador =>
        {
            if (
                mapaCampo.hasLayer(
                    marcador
                )
            )
            {
                mapaCampo.removeLayer(
                    marcador
                );
            }
        }
    );


    marcadoresCamposProductor = [];
}


// =======================================================
// ESTABLECER UBICACION DEL CAMPO ACTUAL
// =======================================================

function establecerUbicacion(
    latitud,
    longitud,
    centrar
)
{
    const lat =
        Number(latitud);


    const lon =
        Number(longitud);


    if (
        !Number.isFinite(lat) ||
        !Number.isFinite(lon)
    )
    {
        return;
    }


    document
        .getElementById(
            "latitud"
        )
        .value =
            lat.toFixed(8);


    document
        .getElementById(
            "longitud"
        )
        .value =
            lon.toFixed(8);


    if (!marcadorCampo)
    {
        marcadorCampo =
            L.marker(
                [lat, lon],
                {
                    draggable: true
                }
            )
            .addTo(
                mapaCampo
            );


        marcadorCampo.on(
            "dragend",
            event =>
            {
                const posicion =
                    event.target
                        .getLatLng();


                establecerUbicacion(
                    posicion.lat,
                    posicion.lng,
                    false
                );
            }
        );
    }
    else
    {
        marcadorCampo
            .setLatLng(
                [lat, lon]
            );
    }


    if (centrar)
    {
        mapaCampo.setView(
            [lat, lon],
            15
        );
    }
}


// =======================================================
// QUITAR UBICACION DEL CAMPO ACTUAL
// =======================================================

function limpiarUbicacion()
{
    document
        .getElementById(
            "latitud"
        )
        .value = "";


    document
        .getElementById(
            "longitud"
        )
        .value = "";


    if (
        mapaCampo &&
        marcadorCampo
    )
    {
        mapaCampo.removeLayer(
            marcadorCampo
        );


        marcadorCampo =
            null;
    }
}


// =======================================================
// MI UBICACION
// =======================================================

function usarMiUbicacion()
{
    if (!navigator.geolocation)
    {
        mensaje(
            "El navegador no permite obtener la ubicación.",
            true
        );

        return;
    }


    mensaje(
        "Obteniendo ubicación..."
    );


    navigator.geolocation
        .getCurrentPosition(

            position =>
            {
                establecerUbicacion(
                    position.coords.latitude,
                    position.coords.longitude,
                    true
                );


                mensaje(
                    "Ubicación seleccionada."
                );
            },

            () =>
            {
                mensaje(
                    "No se pudo obtener su ubicación.",
                    true
                );
            },

            {
                enableHighAccuracy:
                    true,

                timeout:
                    15000,

                maximumAge:
                    0
            }
        );
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