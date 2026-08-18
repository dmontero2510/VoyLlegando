// =======================================================
// VOYLLEGANDO
// PLANTAS Y DESTINOS
// =======================================================

let plantas = [];

let plantaSeleccionada = null;

let mapaDestino = null;

let marcadorDestino = null;

// Marcadores de referencia de los demás destinos
// pertenecientes a la planta seleccionada.
let marcadoresDestinosPlanta = [];


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
        .getElementById(
            "nombreUsuario"
        )
        .textContent =
            perfil.nombre;


    inicializarMapa();


    await cargarTiposIva();


    await cargarPlantas();
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
// TIPOS DE IVA
// =======================================================

async function cargarTiposIva()
{
    const combo =
        document.getElementById(
            "iva"
        );


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
// CARGAR PLANTAS
// =======================================================

async function cargarPlantas()
{
    const lista =
        document.getElementById(
            "listaPlantas"
        );


    lista.innerHTML =
        "Cargando...";


    try
    {
        plantas =
            await API.get(
                "/api/Plantas"
            );


        mostrarPlantas();
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
// MOSTRAR PLANTAS
// =======================================================

function mostrarPlantas()
{
    const lista =
        document.getElementById(
            "listaPlantas"
        );


    lista.innerHTML = "";


    if (
        !plantas ||
        plantas.length === 0
    )
    {
        lista.innerHTML =
            "No hay plantas.";

        return;
    }


    plantas.forEach(
        planta =>
        {
            const item =
                document.createElement(
                    "div"
                );


            item.className =
                "planta-item";


            if (
                plantaSeleccionada &&
                plantaSeleccionada.idPlanta ===
                planta.idPlanta
            )
            {
                item.classList.add(
                    "seleccionado"
                );
            }


            const estado =
                planta.habilitado
                    ? "Habilitada"
                    : "Deshabilitada";


            item.innerHTML =
                `
                <div class="planta-nombre">

                    ${escapar(planta.nombre)}

                </div>

                <div class="planta-detalle">

                    CUIT:
                    ${escapar(planta.cuit)}

                    <br>

                    ${estado}

                </div>
                `;


            item.onclick =
                () =>
                    seleccionarPlanta(
                        planta.idPlanta
                    );


            lista.appendChild(
                item
            );
        }
    );
}


// =======================================================
// NUEVA PLANTA
// =======================================================

function nuevaPlanta()
{
    plantaSeleccionada =
        null;


    document
        .getElementById(
            "idPlanta"
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
            "tituloPlanta"
        )
        .textContent =
            "Nueva Planta";


    document
        .getElementById(
            "btnDeshabilitar"
        )
        .classList.add(
            "oculto"
        );


    document
        .getElementById(
            "seccionDestinos"
        )
        .classList.add(
            "oculto"
        );


    cancelarDestino();

    limpiarMarcadoresDestinosPlanta();

    mostrarPlantas();


    document
        .getElementById(
            "nombre"
        )
        .focus();
}


// =======================================================
// SELECCIONAR PLANTA
// =======================================================

async function seleccionarPlanta(
    idPlanta
)
{
    try
    {
        const planta =
            await API.get(
                `/api/Plantas/${idPlanta}`
            );


        plantaSeleccionada =
            planta;


        const btnNuevoDestino =
            document.getElementById(
                "btnNuevoDestino"
            );


        const aviso =
            document.getElementById(
                "avisoPlantaDeshabilitada"
            );


        if (planta.habilitado)
        {
            btnNuevoDestino.disabled =
                false;


            aviso.classList.add(
                "oculto"
            );
        }
        else
        {
            btnNuevoDestino.disabled =
                true;


            aviso.classList.remove(
                "oculto"
            );
        }


        document
            .getElementById(
                "idPlanta"
            )
            .value =
                planta.idPlanta;


        document
            .getElementById(
                "nombre"
            )
            .value =
                planta.nombre || "";


        document
            .getElementById(
                "domicilio"
            )
            .value =
                planta.domicilio || "";


        document
            .getElementById(
                "cuit"
            )
            .value =
                planta.cuit || "";


        document
            .getElementById(
                "iva"
            )
            .value =
                planta.iva || "";


        document
            .getElementById(
                "habilitado"
            )
            .checked =
                planta.habilitado;


        document
            .getElementById(
                "tituloPlanta"
            )
            .textContent =
                `Planta #${planta.idPlanta}`;


        document
            .getElementById(
                "btnDeshabilitar"
            )
            .classList.remove(
                "oculto"
            );


        document
            .getElementById(
                "nombrePlantaDestinos"
            )
            .textContent =
                planta.nombre;


        document
            .getElementById(
                "seccionDestinos"
            )
            .classList.remove(
                "oculto"
            );


        cancelarDestino();


        mostrarPlantas();


        await cargarDestinos();
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
// GUARDAR PLANTA
// =======================================================

async function guardarPlanta()
{
    const id =
        document
            .getElementById(
                "idPlanta"
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
            "Ingrese el nombre de la planta.",
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


    if (!datos.iva)
    {
        mensaje(
            "Seleccione la condición de IVA.",
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
                    `/api/Plantas/${id}`,
                    datos
                );


            mensaje(
                respuesta.mensaje
            );


            await cargarPlantas();


            await seleccionarPlanta(
                Number(id)
            );
        }
        else
        {
            respuesta =
                await API.post(
                    "/api/Plantas",
                    datos
                );


            mensaje(
                respuesta.mensaje
            );


            await cargarPlantas();


            await seleccionarPlanta(
                respuesta.idPlanta
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
// DESHABILITAR PLANTA
// =======================================================

async function deshabilitarPlanta()
{
    if (!plantaSeleccionada)
        return;


    if (
        !confirm(
            `¿Deshabilitar a ${plantaSeleccionada.nombre}?`
        )
    )
    {
        return;
    }


    try
    {
        const respuesta =
            await API.delete(
                `/api/Plantas/${plantaSeleccionada.idPlanta}`
            );


        mensaje(
            respuesta.mensaje
        );


        const id =
            plantaSeleccionada.idPlanta;


        await cargarPlantas();


        await seleccionarPlanta(
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
// DESTINOS
// =======================================================

async function cargarDestinos()
{
    if (!plantaSeleccionada)
        return;


    const lista =
        document.getElementById(
            "listaDestinos"
        );


    lista.innerHTML =
        "Cargando destinos...";


    try
    {
        const destinos =
            await API.get(
                `/api/Destinos/planta/${plantaSeleccionada.idPlanta}`
            );


        lista.innerHTML = "";


        if (
            !destinos ||
            destinos.length === 0
        )
        {
            lista.innerHTML =
                "La planta no tiene destinos.";

            return;
        }


        destinos.forEach(
            destino =>
            {
                const item =
                    document.createElement(
                        "div"
                    );


                item.className =
                    "destino-item";


                let coordenadas =
                    "Sin ubicación";


                if (
                    destino.latitud != null &&
                    destino.longitud != null
                )
                {
                    coordenadas =
                        `${destino.latitud}, ${destino.longitud}`;
                }


                item.innerHTML =
                    `
                    <div class="destino-nombre">

                        ${escapar(destino.descripDestino)}

                    </div>

                    <div class="destino-coordenadas">

                        📍 ${coordenadas}

                    </div>

                    <div class="destino-botones">

                        <button
                            type="button"
                            class="btn btn-secundario btn-chico"
                            onclick="editarDestino(${destino.idDestino})">

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
// NUEVO DESTINO
// =======================================================

async function nuevoDestino()
{
    if (!plantaSeleccionada)
    {
        mensaje(
            "Primero seleccione una planta.",
            true
        );

        return;
    }


    if (!plantaSeleccionada.habilitado)
    {
        mensaje(
            "La planta está deshabilitada. No se pueden agregar destinos.",
            true
        );

        return;
    }


    document
        .getElementById(
            "seccionDestinoEditar"
        )
        .classList.remove(
            "oculto"
        );


    document
        .getElementById(
            "tituloDestino"
        )
        .textContent =
            "Nuevo Destino";


    document
        .getElementById(
            "idDestino"
        )
        .value = "";


    document
        .getElementById(
            "descripDestino"
        )
        .value = "";


    limpiarUbicacion();


    document
        .getElementById(
            "btnEliminarDestino"
        )
        .classList.add(
            "oculto"
        );


    setTimeout(
        async () =>
        {
            mapaDestino.invalidateSize();


            await mostrarDestinosPlantaEnMapa(
                null,
                true
            );
        },
        100
    );


    document
        .getElementById(
            "descripDestino"
        )
        .focus();
}


// =======================================================
// EDITAR DESTINO
// =======================================================

async function editarDestino(
    idDestino
)
{
    try
    {
        const destino =
            await API.get(
                `/api/Destinos/${idDestino}`
            );


        document
            .getElementById(
                "seccionDestinoEditar"
            )
            .classList.remove(
                "oculto"
            );


        document
            .getElementById(
                "tituloDestino"
            )
            .textContent =
                `Destino #${destino.idDestino}`;


        document
            .getElementById(
                "idDestino"
            )
            .value =
                destino.idDestino;


        document
            .getElementById(
                "descripDestino"
            )
            .value =
                destino.descripDestino;


        document
            .getElementById(
                "btnEliminarDestino"
            )
            .classList.remove(
                "oculto"
            );


        setTimeout(
            async () =>
            {
                mapaDestino.invalidateSize();


                // Mostramos los demás destinos de la planta
                // como referencia.
                await mostrarDestinosPlantaEnMapa(
                    destino.idDestino,
                    false
                );


                if (
                    destino.latitud != null &&
                    destino.longitud != null
                )
                {
                    establecerUbicacion(
                        destino.latitud,
                        destino.longitud,
                        true
                    );
                }
                else
                {
                    limpiarUbicacion();

                    encuadrarDestinosPlanta();
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
// GUARDAR DESTINO
// =======================================================

async function guardarDestino()
{
    if (!plantaSeleccionada)
        return;


    if (!plantaSeleccionada.habilitado)
    {
        mensaje(
            "La planta está deshabilitada.",
            true
        );

        return;
    }


    const idDestino =
        document
            .getElementById(
                "idDestino"
            )
            .value;


    const descripcion =
        document
            .getElementById(
                "descripDestino"
            )
            .value
            .trim();


    if (!descripcion)
    {
        mensaje(
            "Ingrese la descripción del destino.",
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
        idPlanta:
            plantaSeleccionada.idPlanta,

        descripDestino:
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


        if (idDestino)
        {
            respuesta =
                await API.put(
                    `/api/Destinos/${idDestino}`,
                    datos
                );
        }
        else
        {
            respuesta =
                await API.post(
                    "/api/Destinos",
                    datos
                );
        }


        mensaje(
            respuesta.mensaje
        );


        cancelarDestino();


        await cargarDestinos();
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
// ELIMINAR DESTINO
// =======================================================

async function eliminarDestino()
{
    const idDestino =
        document
            .getElementById(
                "idDestino"
            )
            .value;


    if (!idDestino)
        return;


    if (
        !confirm(
            "¿Eliminar este destino?"
        )
    )
    {
        return;
    }


    try
    {
        const respuesta =
            await API.delete(
                `/api/Destinos/${idDestino}`
            );


        mensaje(
            respuesta.mensaje
        );


        cancelarDestino();


        await cargarDestinos();
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
// CANCELAR DESTINO
// =======================================================

function cancelarDestino()
{
    document
        .getElementById(
            "seccionDestinoEditar"
        )
        .classList.add(
            "oculto"
        );


    document
        .getElementById(
            "idDestino"
        )
        .value = "";


    document
        .getElementById(
            "descripDestino"
        )
        .value = "";


    limpiarUbicacion();

    limpiarMarcadoresDestinosPlanta();
}


// =======================================================
// MAPA
// =======================================================

function inicializarMapa()
{
    mapaDestino =
        L.map(
            "mapaDestino"
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
        mapaDestino
    );


    mapaDestino.setView(
        [-35.5, -63.0],
        6
    );


    mapaDestino.on(
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
// DESTINOS DE LA PLANTA EN EL MAPA
// =======================================================

async function mostrarDestinosPlantaEnMapa(
    idDestinoExcluir = null,
    encuadrar = true
)
{
    limpiarMarcadoresDestinosPlanta();


    if (
        !plantaSeleccionada ||
        !mapaDestino
    )
    {
        return;
    }


    try
    {
        const destinos =
            await API.get(
                `/api/Destinos/planta/${plantaSeleccionada.idPlanta}`
            );


        if (
            !destinos ||
            destinos.length === 0
        )
        {
            if (encuadrar)
            {
                mapaDestino.setView(
                    [-35.5, -63.0],
                    6
                );
            }

            return;
        }


        destinos.forEach(
            destino =>
            {
                // En edición excluimos el destino actual,
                // porque ese tendrá el marcador editable.

                if (
                    idDestinoExcluir != null &&
                    Number(destino.idDestino) ===
                    Number(idDestinoExcluir)
                )
                {
                    return;
                }


                if (
                    destino.latitud == null ||
                    destino.longitud == null
                )
                {
                    return;
                }


                const lat =
                    Number(destino.latitud);


                const lon =
                    Number(destino.longitud);


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
                        mapaDestino
                    );


                // Nombre siempre visible.
                marcador.bindTooltip(
                    escapar(destino.descripDestino),
                    {
                        permanent: true,
                        direction: "top",
                        offset: [0, -8],
                        opacity: 0.90
                    }
                );


                // Popup útil especialmente en celular.
                marcador.bindPopup(
                    `<strong>${escapar(destino.descripDestino)}</strong>`
                );


                marcadoresDestinosPlanta.push(
                    marcador
                );
            }
        );


        if (encuadrar)
        {
            encuadrarDestinosPlanta();
        }
    }
    catch (error)
    {
        console.error(
            "No se pudieron mostrar los destinos en el mapa:",
            error
        );
    }
}


// =======================================================
// ENCUADRAR DESTINOS DE LA PLANTA
// =======================================================

function encuadrarDestinosPlanta()
{
    if (!mapaDestino)
        return;


    if (
        marcadoresDestinosPlanta.length === 0
    )
    {
        mapaDestino.setView(
            [-35.5, -63.0],
            6
        );

        return;
    }


    if (
        marcadoresDestinosPlanta.length === 1
    )
    {
        const posicion =
            marcadoresDestinosPlanta[0]
                .getLatLng();


        mapaDestino.setView(
            posicion,
            15
        );

        return;
    }


    const grupo =
        L.featureGroup(
            marcadoresDestinosPlanta
        );


    mapaDestino.fitBounds(
        grupo.getBounds(),
        {
            padding: [40, 40],

            maxZoom: 15
        }
    );
}


// =======================================================
// LIMPIAR DESTINOS DE REFERENCIA
// =======================================================

function limpiarMarcadoresDestinosPlanta()
{
    if (!mapaDestino)
    {
        marcadoresDestinosPlanta = [];

        return;
    }


    marcadoresDestinosPlanta.forEach(
        marcador =>
        {
            if (
                mapaDestino.hasLayer(
                    marcador
                )
            )
            {
                mapaDestino.removeLayer(
                    marcador
                );
            }
        }
    );


    marcadoresDestinosPlanta = [];
}


// =======================================================
// ESTABLECER UBICACION DEL DESTINO ACTUAL
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


    document
        .getElementById(
            "coordenadasPegadas"
        )
        .value =
            `${lat.toFixed(8)}, ${lon.toFixed(8)}`;


    if (!marcadorDestino)
    {
        marcadorDestino =
            L.marker(
                [lat, lon],
                {
                    draggable: true
                }
            )
            .addTo(
                mapaDestino
            );


        marcadorDestino.on(
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
        marcadorDestino
            .setLatLng(
                [lat, lon]
            );
    }


    if (centrar)
    {
        mapaDestino.setView(
            [lat, lon],
            15
        );
    }
}


// =======================================================
// LIMPIAR UBICACION DEL DESTINO ACTUAL
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
            "coordenadasPegadas"
        )
        .value = "";


    document
        .getElementById(
            "longitud"
        )
        .value = "";


    if (
        mapaDestino &&
        marcadorDestino
    )
    {
        mapaDestino.removeLayer(
            marcadorDestino
        );


        marcadorDestino =
            null;
    }
}


// =======================================================
// UBICAR COORDENADAS PEGADAS
// =======================================================

function ubicarCoordenadasPegadas()
{
    const texto =
        document
            .getElementById(
                "coordenadasPegadas"
            )
            .value
            .trim();


    const coincidencia =
        texto.match(
            /^\s*([+-]?(?:\d+(?:\.\d+)?|\.\d+))\s*[,;]\s*([+-]?(?:\d+(?:\.\d+)?|\.\d+))\s*$/
        );


    if (!coincidencia)
    {
        mensaje(
            "Pegue las coordenadas con el formato latitud, longitud.",
            true
        );

        return;
    }


    const latitud =
        Number(coincidencia[1]);


    const longitud =
        Number(coincidencia[2]);


    if (
        latitud < -90 ||
        latitud > 90 ||
        longitud < -180 ||
        longitud > 180
    )
    {
        mensaje(
            "La latitud debe estar entre -90 y 90 y la longitud entre -180 y 180.",
            true
        );

        return;
    }


    establecerUbicacion(
        latitud,
        longitud,
        true
    );


    mensaje(
        "Ubicación seleccionada."
    );
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
