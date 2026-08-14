// =======================================================
// VOYLLEGANDO
// VIAJES - LOGISTICA
// =======================================================

let viajes = [];
let productores = [];
let plantas = [];
let cereales = [];
let tiposIva = [];
let empresasDisponibles = [];

let viajeSeleccionado = null;
let filtroActual = "TODOS";

// Mapas de altas rápidas dentro de Nuevo Viaje
let mapaAltaCampo = null;
let marcadorAltaCampo = null;
let marcadoresCamposReferencia = [];

let mapaAltaDestino = null;
let marcadorAltaDestino = null;
let marcadoresDestinosReferencia = [];


// =======================================================
// INICIO
// =======================================================

async function iniciar()
{
    const perfil =
        await validarSesion("L");

    if (!perfil)
        return;


    const nombreUsuario =
        document.getElementById(
            "nombreUsuario"
        );

    const nombreLogistica =
        document.getElementById(
            "nombreLogistica"
        );


    if (nombreUsuario)
    {
        nombreUsuario.textContent =
            perfil.nombre || "";
    }


    if (nombreLogistica)
    {
        nombreLogistica.textContent =
            perfil.nombreLogistica ||
            "Logística";
    }


    await Promise.all(
        [
            cargarProductores(),
            cargarTiposIva(),
            cargarPlantas(),
            cargarCereales(),
            cargarEmpresasDisponibles()
        ]
    );


    await cargarViajes();

    limpiarFormulario();
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
// MENSAJE GENERAL DE VIAJES
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
// MENSAJES DE ALTAS RAPIDAS
//
// CREA EL MENSAJE DENTRO DEL MODAL.
// NO NECESITA CAMBIOS EN viajes.html
// =======================================================

function mensajeAlta(
    idModal,
    texto,
    esError = false
)
{
    const modal =
        document.getElementById(
            idModal
        );


    if (!modal)
    {
        mensaje(
            texto,
            esError
        );

        return;
    }


    let elemento =
        modal.querySelector(
            ".mensaje-alta-rapida"
        );


    if (!elemento)
    {
        elemento =
            document.createElement(
                "div"
            );


        elemento.className =
            "mensaje mensaje-alta-rapida";


        const contenido =
            modal.querySelector(
                ".modal-contenido"
            )
            ||
            modal.firstElementChild
            ||
            modal;


        const botones =
            contenido.querySelector(
                ".modal-botones, .acciones, .botones"
            );


        if (botones)
        {
            contenido.insertBefore(
                elemento,
                botones
            );
        }
        else
        {
            contenido.appendChild(
                elemento
            );
        }
    }


    elemento.textContent =
        texto || "";


    elemento.className =
        esError
            ? "mensaje error mensaje-alta-rapida"
            : "mensaje mensaje-alta-rapida";


    elemento.style.display =
        texto
            ? ""
            : "none";
}


// =======================================================
// PRODUCTORES
// =======================================================

async function cargarProductores(
    idSeleccionar = null
)
{
    try
    {
        productores =
            await API.get(
                "/api/Productores"
            );


        const combo =
            document.getElementById(
                "idProduc"
            );


        if (!combo)
            return;


        combo.innerHTML =
            `
            <option value="">
                Seleccione...
            </option>
            `;


        productores
            .filter(
                productor =>
                    productor.habilitado
            )
            .forEach(
                productor =>
                {
                    const option =
                        document.createElement(
                            "option"
                        );


                    option.value =
                        productor.idProductor;


                    option.textContent =
                        productor.nombre;


                    combo.appendChild(
                        option
                    );
                }
            );


        if (idSeleccionar !== null)
        {
            combo.value =
                String(
                    idSeleccionar
                );
        }
    }
    catch (error)
    {
        mensaje(
            "No se pudieron cargar los productores: " +
            error.message,
            true
        );
    }
}


// =======================================================
// TIPOS DE IVA
// =======================================================

async function cargarTiposIva()
{
    try
    {
        tiposIva =
            await API.get(
                "/api/TiposIva"
            );


        llenarComboIva(
            "nuevoProductorIva"
        );


        llenarComboIva(
            "nuevaPlantaIva"
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
// LLENAR COMBO IVA
//
// LA API DEVUELVE:
//
// idIva
// descripcion
//
// EJEMPLO:
// { idIva: 1, descripcion: "IVA Resp. Inscripto" }
// =======================================================

function llenarComboIva(
    idCombo
)
{
    const combo =
        document.getElementById(
            idCombo
        );


    if (!combo)
        return;


    combo.innerHTML =
        `
        <option value="">
            Seleccione...
        </option>
        `;


    tiposIva.forEach(
        tipo =>
        {
            const option =
                document.createElement(
                    "option"
                );


            option.value =
                String(
                    tipo.idIva
                );


            option.textContent =
                tipo.descripcion;


            combo.appendChild(
                option
            );
        }
    );
}


// =======================================================
// ALTA RAPIDA PRODUCTOR
// =======================================================

function abrirAltaProductor()
{
    mensajeAlta(
        "modalProductor",
        ""
    );


    document
        .getElementById(
            "nuevoProductorNombre"
        )
        .value = "";


    document
        .getElementById(
            "nuevoProductorDomicilio"
        )
        .value = "";


    document
        .getElementById(
            "nuevoProductorCuit"
        )
        .value = "";


    document
        .getElementById(
            "nuevoProductorIva"
        )
        .value = "";


    document
        .getElementById(
            "modalProductor"
        )
        .classList.remove(
            "oculto"
        );


    setTimeout(
        () =>
        {
            document
                .getElementById(
                    "nuevoProductorNombre"
                )
                .focus();
        },
        50
    );
}


// =======================================================
// CERRAR PRODUCTOR
// =======================================================

function cerrarAltaProductor()
{
    mensajeAlta(
        "modalProductor",
        ""
    );


    document
        .getElementById(
            "modalProductor"
        )
        .classList.add(
            "oculto"
        );
}


// =======================================================
// GUARDAR PRODUCTOR
// =======================================================

async function guardarNuevoProductor()
{
    const nombre =
        document
            .getElementById(
                "nuevoProductorNombre"
            )
            .value
            .trim();


    const domicilio =
        document
            .getElementById(
                "nuevoProductorDomicilio"
            )
            .value
            .trim();


    const iva =
        document
            .getElementById(
                "nuevoProductorIva"
            )
            .value
            .trim();


    const cuit =
        document
            .getElementById(
                "nuevoProductorCuit"
            )
            .value
            .replace(
                /\D/g,
                ""
            );


    if (!nombre)
    {
        mensajeAlta(
            "modalProductor",
            "Ingrese el nombre del productor.",
            true
        );

        return;
    }


    if (nombre.length > 80)
    {
        mensajeAlta(
            "modalProductor",
            "El nombre no puede superar los 80 caracteres.",
            true
        );

        return;
    }


    if (domicilio.length > 100)
    {
        mensajeAlta(
            "modalProductor",
            "El domicilio no puede superar los 100 caracteres.",
            true
        );

        return;
    }


    if (
        cuit.length !== 11 ||
        !/^\d{11}$/.test(
            cuit
        )
    )
    {
        mensajeAlta(
            "modalProductor",
            "El CUIT debe contener 11 dígitos.",
            true
        );

        return;
    }


    if (!iva)
    {
        mensajeAlta(
            "modalProductor",
            "Seleccione la condición de IVA.",
            true
        );

        return;
    }


    try
    {
        mensajeAlta(
            "modalProductor",
            "Guardando productor..."
        );


        const respuesta =
            await API.post(
                "/api/Productores",
                {
                    nombre:
                        nombre,

                    domicilio:
                        domicilio,

                    iva:
                        iva,

                    cuit:
                        cuit,

                    habilitado:
                        true
                }
            );


        cerrarAltaProductor();


        await cargarProductores(
            respuesta.idProductor
        );


        await cambioProductor();


        mensaje(
            respuesta.mensaje ||
            "Productor creado correctamente."
        );
    }
    catch (error)
    {
        mensajeAlta(
            "modalProductor",
            error.message,
            true
        );
    }
}


// =======================================================
// CAMBIO PRODUCTOR
// CARGAR CAMPOS
// =======================================================

async function cambioProductor(
    idCampoSeleccionar = null
)
{
    const idProductor =
        valorEntero(
            "idProduc"
        );


    const combo =
        document.getElementById(
            "idOrigen"
        );


    const botonMas =
        document.getElementById(
            "btnMasCampo"
        );


    combo.disabled =
        true;


    botonMas.disabled =
        !idProductor;


    if (!idProductor)
    {
        combo.innerHTML =
            `
            <option value="">
                Seleccione productor...
            </option>
            `;

        return;
    }


    combo.innerHTML =
        `
        <option value="">
            Cargando...
        </option>
        `;


    try
    {
        const campos =
            await API.get(
                `/api/Campos/productor/${idProductor}`
            );


        combo.innerHTML =
            `
            <option value="">
                Seleccione...
            </option>
            `;


        campos.forEach(
            campo =>
            {
                const option =
                    document.createElement(
                        "option"
                    );


                option.value =
                    campo.idCampo;


                option.textContent =
                    campo.descripCampo;


                option.dataset.nombre =
                    campo.descripCampo ||
                    "";


                option.dataset.latitud =
                    campo.latitud ??
                    "";


                option.dataset.longitud =
                    campo.longitud ??
                    "";


                combo.appendChild(
                    option
                );
            }
        );


        combo.disabled =
            false;


        if (
            idCampoSeleccionar !==
            null
        )
        {
            combo.value =
                String(
                    idCampoSeleccionar
                );
        }
    }
    catch (error)
    {
        combo.innerHTML =
            `
            <option value="">
                No se pudieron cargar
            </option>
            `;


        mensaje(
            "No se pudieron cargar los campos: " +
            error.message,
            true
        );
    }
}


// =======================================================
// ALTA RAPIDA CAMPO
// =======================================================

async function abrirAltaCampo()
{
    const comboProductor =
        document.getElementById(
            "idProduc"
        );


    const idProductor =
        Number(
            comboProductor.value
        );


    if (!idProductor)
    {
        mensaje(
            "Primero seleccione un productor.",
            true
        );

        return;
    }


    mensajeAlta(
        "modalCampo",
        ""
    );


    const opcion =
        comboProductor.options[
            comboProductor.selectedIndex
        ];


    document
        .getElementById(
            "nombreProductorModalCampo"
        )
        .textContent =
            opcion.textContent.trim();


    document
        .getElementById(
            "nuevoCampoDescripcion"
        )
        .value = "";


    document
        .getElementById(
            "nuevoCampoLatitud"
        )
        .value = "";


    document
        .getElementById(
            "nuevoCampoLongitud"
        )
        .value = "";


    document
        .getElementById(
            "modalCampo"
        )
        .classList.remove(
            "oculto"
        );


    setTimeout(
        async () =>
        {
            inicializarMapaAltaCampo();

            if (mapaAltaCampo)
                mapaAltaCampo.invalidateSize();

            await mostrarCamposReferenciaAlta(
                idProductor
            );

            document
                .getElementById(
                    "nuevoCampoDescripcion"
                )
                .focus();
        },
        80
    );
}


function inicializarMapaAltaCampo()
{
    if (mapaAltaCampo)
        return;


    const contenedor =
        document.getElementById(
            "mapaAltaCampo"
        );


    if (!contenedor || typeof L === "undefined")
        return;


    mapaAltaCampo =
        L.map(
            contenedor
        );


    L.tileLayer(
        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        {
            maxZoom: 19,
            attribution: "&copy; OpenStreetMap"
        }
    )
    .addTo(
        mapaAltaCampo
    );


    mapaAltaCampo.on(
        "click",
        evento =>
        {
            establecerUbicacionCampo(
                evento.latlng.lat,
                evento.latlng.lng,
                true
            );
        }
    );


    mapaAltaCampo.setView(
        [-35.5, -63],
        6
    );
}


async function mostrarCamposReferenciaAlta(
    idProductor
)
{
    limpiarMarcadoresCamposReferencia();


    if (!mapaAltaCampo || !idProductor)
        return;


    try
    {
        const campos =
            await API.get(
                `/api/Campos/productor/${idProductor}`
            );


        campos.forEach(
            campo =>
            {
                if (
                    campo.latitud === null ||
                    campo.latitud === undefined ||
                    campo.latitud === "" ||
                    campo.longitud === null ||
                    campo.longitud === undefined ||
                    campo.longitud === ""
                )
                    return;

                const latitud = Number(campo.latitud);
                const longitud = Number(campo.longitud);


                if (
                    !Number.isFinite(latitud) ||
                    !Number.isFinite(longitud)
                )
                    return;


                const marcador =
                    L.marker(
                        [latitud, longitud],
                        {
                            draggable: false
                        }
                    )
                    .addTo(
                        mapaAltaCampo
                    );


                const nombre =
                    campo.descripCampo ||
                    `Campo ${campo.idCampo}`;


                marcador.bindTooltip(
                    nombre,
                    {
                        permanent: true,
                        direction: "top",
                        offset: [0, -8]
                    }
                );


                marcador.bindPopup(
                    nombre
                );


                marcadoresCamposReferencia.push(
                    marcador
                );
            }
        );


        encuadrarCamposReferenciaAlta();
    }
    catch (error)
    {
        mensajeAlta(
            "modalCampo",
            "No se pudieron mostrar los campos existentes en el mapa: " +
            error.message,
            true
        );

        mapaAltaCampo.setView(
            [-35.5, -63],
            6
        );
    }
}


function encuadrarCamposReferenciaAlta()
{
    if (!mapaAltaCampo)
        return;


    if (marcadoresCamposReferencia.length === 0)
    {
        mapaAltaCampo.setView(
            [-35.5, -63],
            6
        );

        return;
    }


    if (marcadoresCamposReferencia.length === 1)
    {
        mapaAltaCampo.setView(
            marcadoresCamposReferencia[0]
                .getLatLng(),
            15
        );

        return;
    }


    const grupo =
        L.featureGroup(
            marcadoresCamposReferencia
        );


    mapaAltaCampo.fitBounds(
        grupo.getBounds(),
        {
            padding: [30, 30],
            maxZoom: 15
        }
    );
}


function limpiarMarcadoresCamposReferencia()
{
    marcadoresCamposReferencia.forEach(
        marcador =>
        {
            if (mapaAltaCampo)
                mapaAltaCampo.removeLayer(
                    marcador
                );
        }
    );


    marcadoresCamposReferencia = [];
}


function establecerUbicacionCampo(
    latitud,
    longitud,
    centrar = false
)
{
    if (
        !Number.isFinite(Number(latitud)) ||
        !Number.isFinite(Number(longitud))
    )
        return;


    latitud = Number(latitud);
    longitud = Number(longitud);


    document
        .getElementById(
            "nuevoCampoLatitud"
        )
        .value =
            latitud.toFixed(8);


    document
        .getElementById(
            "nuevoCampoLongitud"
        )
        .value =
            longitud.toFixed(8);


    if (!mapaAltaCampo)
        return;


    if (!marcadorAltaCampo)
    {
        marcadorAltaCampo =
            L.marker(
                [latitud, longitud],
                {
                    draggable: true
                }
            )
            .addTo(
                mapaAltaCampo
            );


        marcadorAltaCampo.bindTooltip(
            "Nuevo campo",
            {
                permanent: true,
                direction: "top",
                offset: [0, -8]
            }
        );


        marcadorAltaCampo.on(
            "dragend",
            () =>
            {
                const posicion =
                    marcadorAltaCampo.getLatLng();

                establecerUbicacionCampo(
                    posicion.lat,
                    posicion.lng,
                    false
                );
            }
        );
    }
    else
    {
        marcadorAltaCampo.setLatLng(
            [latitud, longitud]
        );
    }


    if (centrar)
    {
        mapaAltaCampo.setView(
            [latitud, longitud],
            Math.max(
                mapaAltaCampo.getZoom(),
                15
            )
        );
    }
}


function usarMiUbicacionCampo()
{
    if (!navigator.geolocation)
    {
        mensajeAlta(
            "modalCampo",
            "El navegador no permite obtener la ubicación.",
            true
        );

        return;
    }


    mensajeAlta(
        "modalCampo",
        "Obteniendo ubicación..."
    );


    navigator.geolocation.getCurrentPosition(
        posicion =>
        {
            mensajeAlta(
                "modalCampo",
                ""
            );

            establecerUbicacionCampo(
                posicion.coords.latitude,
                posicion.coords.longitude,
                true
            );
        },
        error =>
        {
            mensajeAlta(
                "modalCampo",
                "No se pudo obtener la ubicación: " +
                error.message,
                true
            );
        },
        {
            enableHighAccuracy: true,
            timeout: 12000,
            maximumAge: 30000
        }
    );
}


function limpiarUbicacionCampo()
{
    document
        .getElementById(
            "nuevoCampoLatitud"
        )
        .value = "";


    document
        .getElementById(
            "nuevoCampoLongitud"
        )
        .value = "";


    if (
        mapaAltaCampo &&
        marcadorAltaCampo
    )
    {
        mapaAltaCampo.removeLayer(
            marcadorAltaCampo
        );

        marcadorAltaCampo = null;
    }


    encuadrarCamposReferenciaAlta();
}


// =======================================================
// CERRAR CAMPO
// =======================================================

function cerrarAltaCampo()
{
    mensajeAlta(
        "modalCampo",
        ""
    );


    if (mapaAltaCampo)
    {
        mapaAltaCampo.remove();
        mapaAltaCampo = null;
    }


    marcadorAltaCampo = null;
    marcadoresCamposReferencia = [];


    document
        .getElementById(
            "modalCampo"
        )
        .classList.add(
            "oculto"
        );
}


// =======================================================
// GUARDAR CAMPO
// =======================================================

async function guardarNuevoCampo()
{
    const idProductor =
        valorEntero(
            "idProduc"
        );


    const descripcion =
        document
            .getElementById(
                "nuevoCampoDescripcion"
            )
            .value
            .trim();


    const latitud =
        valorDecimalONull(
            "nuevoCampoLatitud"
        );


    const longitud =
        valorDecimalONull(
            "nuevoCampoLongitud"
        );


    if (!idProductor)
    {
        mensajeAlta(
            "modalCampo",
            "No hay un productor seleccionado.",
            true
        );

        return;
    }


    if (!descripcion)
    {
        mensajeAlta(
            "modalCampo",
            "Ingrese la descripción del campo.",
            true
        );

        return;
    }


    if (
        descripcion.length >
        30
    )
    {
        mensajeAlta(
            "modalCampo",
            "La descripción del campo no puede superar los 30 caracteres.",
            true
        );

        return;
    }


    try
    {
        mensajeAlta(
            "modalCampo",
            "Guardando campo..."
        );


        const respuesta =
            await API.post(
                "/api/Campos",
                {
                    idProductor:
                        idProductor,

                    descripCampo:
                        descripcion,

                    latitud:
                        latitud,

                    longitud:
                        longitud
                }
            );


        cerrarAltaCampo();


        const idCampo =
            respuesta.idCampo ??
            respuesta.id;


        await cambioProductor(
            idCampo
        );


        mensaje(
            respuesta.mensaje ||
            "Campo creado correctamente."
        );
    }
    catch (error)
    {
        mensajeAlta(
            "modalCampo",
            error.message,
            true
        );
    }
}


// =======================================================
// PLANTAS
// =======================================================

async function cargarPlantas(
    idSeleccionar = null
)
{
    try
    {
        plantas =
            await API.get(
                "/api/Plantas"
            );


        const combo =
            document.getElementById(
                "idPlanta"
            );


        if (!combo)
            return;


        combo.innerHTML =
            `
            <option value="">
                Seleccione...
            </option>
            `;


        plantas
            .filter(
                planta =>
                    planta.habilitado
            )
            .forEach(
                planta =>
                {
                    const option =
                        document.createElement(
                            "option"
                        );


                    option.value =
                        planta.idPlanta;


                    option.textContent =
                        planta.nombre;


                    combo.appendChild(
                        option
                    );
                }
            );


        if (
            idSeleccionar !==
            null
        )
        {
            combo.value =
                String(
                    idSeleccionar
                );
        }
    }
    catch (error)
    {
        mensaje(
            "No se pudieron cargar las plantas: " +
            error.message,
            true
        );
    }
}


// =======================================================
// ALTA RAPIDA PLANTA
// =======================================================

function abrirAltaPlanta()
{
    mensajeAlta(
        "modalPlanta",
        ""
    );


    document
        .getElementById(
            "nuevaPlantaNombre"
        )
        .value = "";


    document
        .getElementById(
            "nuevaPlantaDomicilio"
        )
        .value = "";


    document
        .getElementById(
            "nuevaPlantaCuit"
        )
        .value = "";


    document
        .getElementById(
            "nuevaPlantaIva"
        )
        .value = "";


    document
        .getElementById(
            "modalPlanta"
        )
        .classList.remove(
            "oculto"
        );


    setTimeout(
        () =>
        {
            document
                .getElementById(
                    "nuevaPlantaNombre"
                )
                .focus();
        },
        50
    );
}


// =======================================================
// CERRAR PLANTA
// =======================================================

function cerrarAltaPlanta()
{
    mensajeAlta(
        "modalPlanta",
        ""
    );


    document
        .getElementById(
            "modalPlanta"
        )
        .classList.add(
            "oculto"
        );
}


// =======================================================
// GUARDAR PLANTA
// =======================================================

async function guardarNuevaPlanta()
{
    const nombre =
        document
            .getElementById(
                "nuevaPlantaNombre"
            )
            .value
            .trim();


    const domicilio =
        document
            .getElementById(
                "nuevaPlantaDomicilio"
            )
            .value
            .trim();


    const iva =
        document
            .getElementById(
                "nuevaPlantaIva"
            )
            .value
            .trim();


    const cuit =
        document
            .getElementById(
                "nuevaPlantaCuit"
            )
            .value
            .replace(
                /\D/g,
                ""
            );


    if (!nombre)
    {
        mensajeAlta(
            "modalPlanta",
            "Ingrese el nombre de la planta.",
            true
        );

        return;
    }


    if (
        nombre.length >
        80
    )
    {
        mensajeAlta(
            "modalPlanta",
            "El nombre no puede superar los 80 caracteres.",
            true
        );

        return;
    }


    if (
        domicilio.length >
        100
    )
    {
        mensajeAlta(
            "modalPlanta",
            "El domicilio no puede superar los 100 caracteres.",
            true
        );

        return;
    }


    if (
        cuit.length !== 11 ||
        !/^\d{11}$/.test(
            cuit
        )
    )
    {
        mensajeAlta(
            "modalPlanta",
            "El CUIT debe contener 11 dígitos.",
            true
        );

        return;
    }


    if (!iva)
    {
        mensajeAlta(
            "modalPlanta",
            "Seleccione la condición de IVA.",
            true
        );

        return;
    }


    try
    {
        mensajeAlta(
            "modalPlanta",
            "Guardando planta..."
        );


        const respuesta =
            await API.post(
                "/api/Plantas",
                {
                    nombre:
                        nombre,

                    domicilio:
                        domicilio,

                    iva:
                        iva,

                    cuit:
                        cuit,

                    habilitado:
                        true
                }
            );


        cerrarAltaPlanta();


        const idPlanta =
            respuesta.idPlanta ??
            respuesta.id;


        await cargarPlantas(
            idPlanta
        );


        await cambioPlanta();


        mensaje(
            respuesta.mensaje ||
            "Planta creada correctamente."
        );
    }
    catch (error)
    {
        mensajeAlta(
            "modalPlanta",
            error.message,
            true
        );
    }
}


// =======================================================
// CAMBIO PLANTA
// CARGAR DESTINOS
// =======================================================

async function cambioPlanta(
    idDestinoSeleccionar = null
)
{
    const idPlanta =
        valorEntero(
            "idPlanta"
        );


    const combo =
        document.getElementById(
            "idDestino"
        );


    const botonMas =
        document.getElementById(
            "btnMasDestino"
        );


    combo.disabled =
        true;


    botonMas.disabled =
        !idPlanta;


    if (!idPlanta)
    {
        combo.innerHTML =
            `
            <option value="">
                Seleccione planta...
            </option>
            `;

        return;
    }


    combo.innerHTML =
        `
        <option value="">
            Cargando...
        </option>
        `;


    try
    {
        const destinos =
            await API.get(
                `/api/Destinos/planta/${idPlanta}`
            );


        combo.innerHTML =
            `
            <option value="">
                Seleccione...
            </option>
            `;


        destinos.forEach(
            destino =>
            {
                const option =
                    document.createElement(
                        "option"
                    );


                option.value =
                    destino.idDestino;


                option.textContent =
                    destino.descripDestino;


                option.dataset.nombre =
                    destino.descripDestino ||
                    "";


                option.dataset.latitud =
                    destino.latitud ??
                    "";


                option.dataset.longitud =
                    destino.longitud ??
                    "";


                combo.appendChild(
                    option
                );
            }
        );


        combo.disabled =
            false;


        if (
            idDestinoSeleccionar !==
            null
        )
        {
            combo.value =
                String(
                    idDestinoSeleccionar
                );
        }
    }
    catch (error)
    {
        combo.innerHTML =
            `
            <option value="">
                No se pudieron cargar
            </option>
            `;


        mensaje(
            "No se pudieron cargar los destinos: " +
            error.message,
            true
        );
    }
}


// =======================================================
// ALTA RAPIDA DESTINO
// =======================================================

async function abrirAltaDestino()
{
    const comboPlanta =
        document.getElementById(
            "idPlanta"
        );


    const idPlanta =
        Number(
            comboPlanta.value
        );


    if (!idPlanta)
    {
        mensaje(
            "Primero seleccione una planta.",
            true
        );

        return;
    }


    mensajeAlta(
        "modalDestino",
        ""
    );


    const opcion =
        comboPlanta.options[
            comboPlanta.selectedIndex
        ];


    document
        .getElementById(
            "nombrePlantaModalDestino"
        )
        .textContent =
            opcion.textContent.trim();


    document
        .getElementById(
            "nuevoDestinoDescripcion"
        )
        .value = "";


    document
        .getElementById(
            "nuevoDestinoLatitud"
        )
        .value = "";


    document
        .getElementById(
            "nuevoDestinoLongitud"
        )
        .value = "";


    document
        .getElementById(
            "modalDestino"
        )
        .classList.remove(
            "oculto"
        );


    setTimeout(
        async () =>
        {
            inicializarMapaAltaDestino();

            if (mapaAltaDestino)
                mapaAltaDestino.invalidateSize();

            await mostrarDestinosReferenciaAlta(
                idPlanta
            );

            document
                .getElementById(
                    "nuevoDestinoDescripcion"
                )
                .focus();
        },
        80
    );
}


function inicializarMapaAltaDestino()
{
    if (mapaAltaDestino)
        return;


    const contenedor =
        document.getElementById(
            "mapaAltaDestino"
        );


    if (!contenedor || typeof L === "undefined")
        return;


    mapaAltaDestino =
        L.map(
            contenedor
        );


    L.tileLayer(
        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        {
            maxZoom: 19,
            attribution: "&copy; OpenStreetMap"
        }
    )
    .addTo(
        mapaAltaDestino
    );


    mapaAltaDestino.on(
        "click",
        evento =>
        {
            establecerUbicacionDestino(
                evento.latlng.lat,
                evento.latlng.lng,
                true
            );
        }
    );


    mapaAltaDestino.setView(
        [-35.5, -63],
        6
    );
}


async function mostrarDestinosReferenciaAlta(
    idPlanta
)
{
    limpiarMarcadoresDestinosReferencia();


    if (!mapaAltaDestino || !idPlanta)
        return;


    try
    {
        const destinos =
            await API.get(
                `/api/Destinos/planta/${idPlanta}`
            );


        destinos.forEach(
            destino =>
            {
                if (
                    destino.latitud === null ||
                    destino.latitud === undefined ||
                    destino.latitud === "" ||
                    destino.longitud === null ||
                    destino.longitud === undefined ||
                    destino.longitud === ""
                )
                    return;

                const latitud = Number(destino.latitud);
                const longitud = Number(destino.longitud);


                if (
                    !Number.isFinite(latitud) ||
                    !Number.isFinite(longitud)
                )
                    return;


                const marcador =
                    L.marker(
                        [latitud, longitud],
                        {
                            draggable: false
                        }
                    )
                    .addTo(
                        mapaAltaDestino
                    );


                const nombre =
                    destino.descripDestino ||
                    `Destino ${destino.idDestino}`;


                marcador.bindTooltip(
                    nombre,
                    {
                        permanent: true,
                        direction: "top",
                        offset: [0, -8]
                    }
                );


                marcador.bindPopup(
                    nombre
                );


                marcadoresDestinosReferencia.push(
                    marcador
                );
            }
        );


        encuadrarDestinosReferenciaAlta();
    }
    catch (error)
    {
        mensajeAlta(
            "modalDestino",
            "No se pudieron mostrar los destinos existentes en el mapa: " +
            error.message,
            true
        );

        mapaAltaDestino.setView(
            [-35.5, -63],
            6
        );
    }
}


function encuadrarDestinosReferenciaAlta()
{
    if (!mapaAltaDestino)
        return;


    if (marcadoresDestinosReferencia.length === 0)
    {
        mapaAltaDestino.setView(
            [-35.5, -63],
            6
        );

        return;
    }


    if (marcadoresDestinosReferencia.length === 1)
    {
        mapaAltaDestino.setView(
            marcadoresDestinosReferencia[0]
                .getLatLng(),
            15
        );

        return;
    }


    const grupo =
        L.featureGroup(
            marcadoresDestinosReferencia
        );


    mapaAltaDestino.fitBounds(
        grupo.getBounds(),
        {
            padding: [30, 30],
            maxZoom: 15
        }
    );
}


function limpiarMarcadoresDestinosReferencia()
{
    marcadoresDestinosReferencia.forEach(
        marcador =>
        {
            if (mapaAltaDestino)
                mapaAltaDestino.removeLayer(
                    marcador
                );
        }
    );


    marcadoresDestinosReferencia = [];
}


function establecerUbicacionDestino(
    latitud,
    longitud,
    centrar = false
)
{
    if (
        !Number.isFinite(Number(latitud)) ||
        !Number.isFinite(Number(longitud))
    )
        return;


    latitud = Number(latitud);
    longitud = Number(longitud);


    document
        .getElementById(
            "nuevoDestinoLatitud"
        )
        .value =
            latitud.toFixed(8);


    document
        .getElementById(
            "nuevoDestinoLongitud"
        )
        .value =
            longitud.toFixed(8);


    if (!mapaAltaDestino)
        return;


    if (!marcadorAltaDestino)
    {
        marcadorAltaDestino =
            L.marker(
                [latitud, longitud],
                {
                    draggable: true
                }
            )
            .addTo(
                mapaAltaDestino
            );


        marcadorAltaDestino.bindTooltip(
            "Nuevo destino",
            {
                permanent: true,
                direction: "top",
                offset: [0, -8]
            }
        );


        marcadorAltaDestino.on(
            "dragend",
            () =>
            {
                const posicion =
                    marcadorAltaDestino.getLatLng();

                establecerUbicacionDestino(
                    posicion.lat,
                    posicion.lng,
                    false
                );
            }
        );
    }
    else
    {
        marcadorAltaDestino.setLatLng(
            [latitud, longitud]
        );
    }


    if (centrar)
    {
        mapaAltaDestino.setView(
            [latitud, longitud],
            Math.max(
                mapaAltaDestino.getZoom(),
                15
            )
        );
    }
}


function usarMiUbicacionDestino()
{
    if (!navigator.geolocation)
    {
        mensajeAlta(
            "modalDestino",
            "El navegador no permite obtener la ubicación.",
            true
        );

        return;
    }


    mensajeAlta(
        "modalDestino",
        "Obteniendo ubicación..."
    );


    navigator.geolocation.getCurrentPosition(
        posicion =>
        {
            mensajeAlta(
                "modalDestino",
                ""
            );

            establecerUbicacionDestino(
                posicion.coords.latitude,
                posicion.coords.longitude,
                true
            );
        },
        error =>
        {
            mensajeAlta(
                "modalDestino",
                "No se pudo obtener la ubicación: " +
                error.message,
                true
            );
        },
        {
            enableHighAccuracy: true,
            timeout: 12000,
            maximumAge: 30000
        }
    );
}


function limpiarUbicacionDestino()
{
    document
        .getElementById(
            "nuevoDestinoLatitud"
        )
        .value = "";


    document
        .getElementById(
            "nuevoDestinoLongitud"
        )
        .value = "";


    if (
        mapaAltaDestino &&
        marcadorAltaDestino
    )
    {
        mapaAltaDestino.removeLayer(
            marcadorAltaDestino
        );

        marcadorAltaDestino = null;
    }


    encuadrarDestinosReferenciaAlta();
}


// =======================================================
// CERRAR DESTINO
// =======================================================

function cerrarAltaDestino()
{
    mensajeAlta(
        "modalDestino",
        ""
    );


    if (mapaAltaDestino)
    {
        mapaAltaDestino.remove();
        mapaAltaDestino = null;
    }


    marcadorAltaDestino = null;
    marcadoresDestinosReferencia = [];


    document
        .getElementById(
            "modalDestino"
        )
        .classList.add(
            "oculto"
        );
}


// =======================================================
// GUARDAR DESTINO
// =======================================================

async function guardarNuevoDestino()
{
    const idPlanta =
        valorEntero(
            "idPlanta"
        );


    const descripcion =
        document
            .getElementById(
                "nuevoDestinoDescripcion"
            )
            .value
            .trim();


    const latitud =
        valorDecimalONull(
            "nuevoDestinoLatitud"
        );


    const longitud =
        valorDecimalONull(
            "nuevoDestinoLongitud"
        );


    if (!idPlanta)
    {
        mensajeAlta(
            "modalDestino",
            "No hay una planta seleccionada.",
            true
        );

        return;
    }


    if (!descripcion)
    {
        mensajeAlta(
            "modalDestino",
            "Ingrese la descripción del destino.",
            true
        );

        return;
    }


    if (
        descripcion.length >
        40
    )
    {
        mensajeAlta(
            "modalDestino",
            "La descripción del destino no puede superar los 40 caracteres.",
            true
        );

        return;
    }


    try
    {
        mensajeAlta(
            "modalDestino",
            "Guardando destino..."
        );


        const respuesta =
            await API.post(
                "/api/Destinos",
                {
                    idPlanta:
                        idPlanta,

                    descripDestino:
                        descripcion,

                    latitud:
                        latitud,

                    longitud:
                        longitud
                }
            );


        cerrarAltaDestino();


        const idDestino =
            respuesta.idDestino ??
            respuesta.id;


        await cambioPlanta(
            idDestino
        );


        mensaje(
            respuesta.mensaje ||
            "Destino creado correctamente."
        );
    }
    catch (error)
    {
        mensajeAlta(
            "modalDestino",
            error.message,
            true
        );
    }
}


// =======================================================
// CEREALES
// =======================================================

async function cargarCereales(
    idSeleccionar = null
)
{
    try
    {
        cereales =
            await API.get(
                "/api/Cereales/habilitados"
            );


        const combo =
            document.getElementById(
                "idCereal"
            );


        if (!combo)
            return;


        combo.innerHTML =
            `
            <option value="">
                Seleccione...
            </option>
            `;


        cereales
            .forEach(
                cereal =>
                {
                    const option =
                        document.createElement(
                            "option"
                        );


                    option.value =
                        cereal.idCereal;


                    option.textContent =
                        cereal.nombre;


                    combo.appendChild(
                        option
                    );
                }
            );


        if (
            idSeleccionar !==
            null
        )
        {
            combo.value =
                String(
                    idSeleccionar
                );
        }
    }
    catch (error)
    {
        mensaje(
            "No se pudieron cargar los cereales: " +
            error.message,
            true
        );
    }
}


// =======================================================
// EMPRESAS DISPONIBLES
// =======================================================

async function cargarEmpresasDisponibles()
{
    try
    {
        empresasDisponibles =
            await API.get(
                "/api/Viajes/empresas-disponibles"
            );


        llenarComboEmpresas(
            "idEmpresaCrear",
            true
        );


        llenarComboEmpresas(
            "idEmpresaAsignar",
            false
        );
    }
    catch (error)
    {
        empresasDisponibles =
            [];


        llenarComboEmpresas(
            "idEmpresaCrear",
            true
        );


        llenarComboEmpresas(
            "idEmpresaAsignar",
            false
        );


        mensaje(
            "No se pudieron cargar las Empresas de Transporte: " +
            error.message,
            true
        );
    }
}


// =======================================================
// LLENAR COMBO EMPRESAS
// =======================================================

function llenarComboEmpresas(
    idCombo,
    permitirVacio
)
{
    const combo =
        document.getElementById(
            idCombo
        );


    if (!combo)
        return;


    combo.innerHTML =
        permitirVacio
            ?
            `
            <option value="">
                Sin asignar por ahora
            </option>
            `
            :
            `
            <option value="">
                Seleccione...
            </option>
            `;


    empresasDisponibles.forEach(
        empresa =>
        {
            const option =
                document.createElement(
                    "option"
                );


            option.value =
                empresa.idUsuario;


            option.textContent =
                empresa.nombre;


            combo.appendChild(
                option
            );
        }
    );
}


// =======================================================
// CARGAR VIAJES
// =======================================================

async function cargarViajes()
{
    const lista =
        document.getElementById(
            "listaViajes"
        );


    if (!lista)
        return;


    lista.innerHTML =
        "Cargando...";


    try
    {
        viajes =
            await API.get(
                "/api/Viajes"
            );


        mostrarViajes();
    }
    catch (error)
    {
        lista.innerHTML =
            "";


        mensaje(
            "No se pudieron cargar los viajes: " +
            error.message,
            true
        );
    }
}


// =======================================================
// MOSTRAR VIAJES
// =======================================================

function mostrarViajes()
{
    const lista =
        document.getElementById(
            "listaViajes"
        );


    if (!lista)
        return;


    lista.innerHTML =
        "";


    const filtrados =
        viajes.filter(
            viaje =>
            {
                if (
                    filtroActual ===
                    "TODOS"
                )
                {
                    return true;
                }


                if (
                    filtroActual ===
                    "CURSO"
                )
                {
                    return [
                        "V",
                        "O",
                        "R",
                        "D"
                    ].includes(
                        viaje.estado
                    );
                }


                return (
                    viaje.estado ===
                    filtroActual
                );
            }
        );


    if (
        filtrados.length === 0
    )
    {
        lista.innerHTML =
            `
            <div class="sin-registros">
                No hay viajes para este filtro.
            </div>
            `;

        return;
    }


    filtrados.forEach(
        viaje =>
        {
            const item =
                document.createElement(
                    "div"
                );


            item.className =
                "viaje-item";


            if (
                viajeSeleccionado &&
                viajeSeleccionado.idViaje ===
                viaje.idViaje
            )
            {
                item.classList.add(
                    "seleccionado"
                );
            }


            item.innerHTML =
                `
                <div class="viaje-item-cabecera">

                    <div class="viaje-id">
                        Viaje #${viaje.idViaje}
                    </div>

                    <span
                        class="
                            estado
                            estado-${escapar(viaje.estado)}
                        ">

                        ${escapar(
                            descripcionEstado(
                                viaje.estado
                            )
                        )}

                    </span>

                </div>

                <div class="viaje-ruta">

                    ${escapar(
                        viaje.origen
                    )}

                    <br>

                    ↓

                    <br>

                    ${escapar(
                        viaje.destino
                    )}

                </div>

                <div class="viaje-meta">

                    ${formatearFecha(
                        viaje.fechaPedido
                    )}

                </div>
                `;


            item.onclick =
                () =>
                    seleccionarViaje(
                        viaje.idViaje
                    );


            lista.appendChild(
                item
            );
        }
    );
}


// =======================================================
// FILTROS
// =======================================================

function cambiarFiltro(
    filtro
)
{
    filtroActual =
        filtro;


    document
        .querySelectorAll(
            ".filtro-viaje"
        )
        .forEach(
            boton =>
                boton
                    .classList
                    .remove(
                        "activo"
                    )
        );


    let idBoton =
        "filtroTodos";


    if (filtro === "P")
    {
        idBoton =
            "filtroP";
    }
    else if (filtro === "A")
    {
        idBoton =
            "filtroA";
    }
    else if (
        filtro ===
        "CURSO"
    )
    {
        idBoton =
            "filtroCurso";
    }
    else if (filtro === "T")
    {
        idBoton =
            "filtroT";
    }


    const boton =
        document.getElementById(
            idBoton
        );


    if (boton)
    {
        boton.classList.add(
            "activo"
        );
    }


    mostrarViajes();
}


// =======================================================
// NUEVO VIAJE
// =======================================================

function nuevoViaje()
{
    viajeSeleccionado =
        null;


    document
        .getElementById(
            "seccionDetalle"
        )
        .classList.add(
            "oculto"
        );


    document
        .getElementById(
            "seccionNuevo"
        )
        .classList.remove(
            "oculto"
        );


    mostrarViajes();


    document
        .getElementById(
            "idProduc"
        )
        .focus();
}


// =======================================================
// LIMPIAR FORMULARIO
// =======================================================

function limpiarFormulario()
{
    document
        .getElementById(
            "idProduc"
        )
        .value = "";


    const comboOrigen =
        document.getElementById(
            "idOrigen"
        );


    comboOrigen.innerHTML =
        `
        <option value="">
            Seleccione productor...
        </option>
        `;


    comboOrigen.disabled =
        true;


    document
        .getElementById(
            "btnMasCampo"
        )
        .disabled =
            true;


    document
        .getElementById(
            "idPlanta"
        )
        .value = "";


    const comboDestino =
        document.getElementById(
            "idDestino"
        );


    comboDestino.innerHTML =
        `
        <option value="">
            Seleccione planta...
        </option>
        `;


    comboDestino.disabled =
        true;


    document
        .getElementById(
            "btnMasDestino"
        )
        .disabled =
            true;


    document
        .getElementById(
            "idCereal"
        )
        .value = "";


    const tipo =
        document.getElementById(
            "tipo"
        );


    if (tipo)
    {
        tipo.value =
            "A";
    }


    document
        .getElementById(
            "ctg"
        )
        .value = "";


    document
        .getElementById(
            "kms"
        )
        .value = "";


    document
        .getElementById(
            "tarifa"
        )
        .value = "";


    document
        .getElementById(
            "observaciones"
        )
        .value = "";


    document
        .getElementById(
            "batea"
        )
        .checked =
            false;


    document
        .getElementById(
            "corta"
        )
        .checked =
            false;


    document
        .getElementById(
            "larga"
        )
        .checked =
            false;


    document
        .getElementById(
            "idEmpresaCrear"
        )
        .value = "";


    mensaje("");
}


// =======================================================
// GUARDAR VIAJE
// =======================================================

async function guardarViaje()
{
    const comboOrigen =
        document.getElementById(
            "idOrigen"
        );


    const comboDestino =
        document.getElementById(
            "idDestino"
        );


    const opcionOrigen =
        comboOrigen.options[
            comboOrigen.selectedIndex
        ];


    const opcionDestino =
        comboDestino.options[
            comboDestino.selectedIndex
        ];


    const datos =
    {
        idCamionero:
            valorEnteroONull(
                "idEmpresaCrear"
            ),

        tipo:
            "A",

        idCereal:
            valorEntero(
                "idCereal"
            ),

        idProduc:
            valorEntero(
                "idProduc"
            ),

        idOrigen:
            valorEntero(
                "idOrigen"
            ),

        idPlanta:
            valorEntero(
                "idPlanta"
            ),

        idDestino:
            valorEntero(
                "idDestino"
            ),

        origen:
            opcionOrigen
                ?.dataset
                .nombre
            ||
            opcionOrigen
                ?.textContent
                ?.trim()
            ||
            "",

        destino:
            opcionDestino
                ?.dataset
                .nombre
            ||
            opcionDestino
                ?.textContent
                ?.trim()
            ||
            "",

        ctg:
            document
                .getElementById(
                    "ctg"
                )
                .value
                .trim(),

        kms:
            valorDecimal(
                "kms"
            ),

        tarifa:
            valorDecimal(
                "tarifa"
            ),

        observaciones:
            document
                .getElementById(
                    "observaciones"
                )
                .value
                .trim(),

        batea:
            document
                .getElementById(
                    "batea"
                )
                .checked,

        corta:
            document
                .getElementById(
                    "corta"
                )
                .checked,

        larga:
            document
                .getElementById(
                    "larga"
                )
                .checked
    };


    const error =
        validarViaje(
            datos
        );


    if (error)
    {
        mensaje(
            error,
            true
        );

        return;
    }


    try
    {
        mensaje(
            "Guardando viaje..."
        );


        const respuesta =
            await API.post(
                "/api/Viajes",
                datos
            );


        const idViaje =
            respuesta.id;


        limpiarFormulario();


        await cargarEmpresasDisponibles();

        await cargarViajes();


        mensaje(
            `Viaje #${idViaje} creado correctamente.`
        );


        await seleccionarViaje(
            idViaje
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
// VALIDAR VIAJE
// =======================================================

function validarViaje(
    datos
)
{
    if (!datos.idProduc)
    {
        return "Seleccione un productor.";
    }


    if (!datos.idOrigen)
    {
        return "Seleccione un campo de origen.";
    }


    if (!datos.idPlanta)
    {
        return "Seleccione una planta.";
    }


    if (!datos.idDestino)
    {
        return "Seleccione un destino.";
    }


    if (!datos.idCereal)
    {
        return "Seleccione un cereal.";
    }


    if (!datos.origen)
    {
        return "El origen no es válido.";
    }


    if (!datos.destino)
    {
        return "El destino no es válido.";
    }


    if (
        datos.origen.length >
        30
    )
    {
        return "El nombre del origen supera los 30 caracteres.";
    }


    if (
        datos.destino.length >
        30
    )
    {
        return "El nombre del destino supera los 30 caracteres.";
    }


    if (
        !Number.isFinite(
            datos.kms
        ) ||
        datos.kms < 0
    )
    {
        return "Ingrese kilómetros válidos.";
    }


    if (
        !Number.isFinite(
            datos.tarifa
        ) ||
        datos.tarifa < 0
    )
    {
        return "Ingrese una tarifa válida.";
    }


    if (
        datos.observaciones.length >
        50
    )
    {
        return "Las observaciones no pueden superar los 50 caracteres.";
    }


    return null;
}


// =======================================================
// SELECCIONAR VIAJE
// =======================================================

async function seleccionarViaje(
    idViaje
)
{
    try
    {
        const viaje =
            await API.get(
                `/api/Viajes/${idViaje}`
            );


        viajeSeleccionado =
            viaje;


        document
            .getElementById(
                "seccionNuevo"
            )
            .classList.add(
                "oculto"
            );


        document
            .getElementById(
                "seccionDetalle"
            )
            .classList.remove(
                "oculto"
            );


        document
            .getElementById(
                "tituloDetalle"
            )
            .textContent =
                `Viaje #${viaje.idViaje}`;


        mostrarDetalle(
            viaje
        );


        const seccionAsignar =
            document.getElementById(
                "seccionAsignar"
            );


        if (
            viaje.estado ===
            "P"
        )
        {
            await cargarEmpresasDisponibles();


            seccionAsignar
                .classList
                .remove(
                    "oculto"
                );
        }
        else
        {
            seccionAsignar
                .classList
                .add(
                    "oculto"
                );
        }


        mostrarViajes();
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
// MOSTRAR DETALLE
// =======================================================

function mostrarDetalle(
    viaje
)
{
    const detalle =
        document.getElementById(
            "detalleViaje"
        );


    detalle.innerHTML =
        `
        ${datoDetalle(
            "Estado",
            descripcionEstado(
                viaje.estado
            )
        )}

        ${datoDetalle(
            "Fecha pedido",
            formatearFecha(
                viaje.fechaPedido
            )
        )}

        ${datoDetalle(
            "Origen",
            viaje.origen
        )}

        ${datoDetalle(
            "Destino",
            viaje.destino
        )}

        ${datoDetalle(
            "CTG",
            viaje.ctg
        )}

        ${datoDetalle(
            "Kilómetros",
            formatearNumero(
                viaje.kms
            )
        )}

        ${datoDetalle(
            "Tarifa",
            formatearNumero(
                viaje.tarifa
            )
        )}

${datoDetalle(
    "Empresa asignada",
    viaje.nombreEmpresa ||
    "Sin asignar"
)}

        ${datoDetalle(
            "Equipo",
            descripcionEquipo(
                viaje
            )
        )}

        ${datoDetalle(
            "Observaciones",
            viaje.observaciones ||
            "-"
        )}
        `;
}


// =======================================================
// DATO DETALLE
// =======================================================

function datoDetalle(
    titulo,
    valor
)
{
    return `
        <div class="detalle-dato">

            <strong>
                ${escapar(titulo)}
            </strong>

            ${escapar(
                valor ?? "-"
            )}

        </div>
    `;
}


// =======================================================
// ASIGNAR VIAJE
// =======================================================

async function asignarViaje()
{
    if (!viajeSeleccionado)
        return;


    if (
        viajeSeleccionado.estado !==
        "P"
    )
    {
        mensaje(
            "El viaje ya no está pendiente.",
            true
        );

        return;
    }


    const idEmpresa =
        valorEntero(
            "idEmpresaAsignar"
        );


    if (!idEmpresa)
    {
        mensaje(
            "Seleccione una Empresa de Transporte.",
            true
        );

        return;
    }


    try
    {
        mensaje(
            "Asignando viaje..."
        );


        const respuesta =
            await API.post(
                `/api/Viajes/${viajeSeleccionado.idViaje}/asignar?idEmpresa=${idEmpresa}`,
                {}
            );


        await cargarEmpresasDisponibles();

        await cargarViajes();


        mensaje(
            respuesta.mensaje ||
            "Viaje asignado correctamente."
        );


        await seleccionarViaje(
            viajeSeleccionado.idViaje
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
// DESCRIPCION ESTADO
// =======================================================

function descripcionEstado(
    estado
)
{
    switch (estado)
    {
        case "P":
            return "Pendiente";

        case "A":
            return "Asignado";

        case "V":
            return "Iniciado";

        case "O":
            return "En origen";

        case "R":
            return "En ruta";

        case "D":
            return "En destino";

        case "T":
            return "Terminado";

        case "I":
            return "Informado";

        default:
            return estado || "-";
    }
}


// =======================================================
// EQUIPO
// =======================================================

function descripcionEquipo(
    viaje
)
{
    const equipo =
        [];


    if (viaje.batea)
    {
        equipo.push(
            "Batea"
        );
    }


    if (viaje.corta)
    {
        equipo.push(
            "Corta"
        );
    }


    if (viaje.larga)
    {
        equipo.push(
            "Larga"
        );
    }


    return equipo.length
        ? equipo.join(", ")
        : "Sin requisito";
}


// =======================================================
// NOMBRE EMPRESA
// =======================================================

function nombreEmpresa(
    idUsuario
)
{
    if (!idUsuario)
    {
        return "Sin asignar";
    }


    const empresa =
        empresasDisponibles.find(
            item =>
                Number(
                    item.idUsuario
                )
                ===
                Number(
                    idUsuario
                )
        );


    if (empresa)
    {
        return empresa.nombre;
    }


    return `Empresa #${idUsuario}`;
}


// =======================================================
// ENTERO
// =======================================================

function valorEntero(
    id
)
{
    const elemento =
        document.getElementById(
            id
        );


    if (!elemento)
    {
        return 0;
    }


    const valor =
        Number(
            elemento.value
        );


    return Number.isInteger(
        valor
    )
        ? valor
        : 0;
}


// =======================================================
// ENTERO O NULL
// =======================================================

function valorEnteroONull(
    id
)
{
    const elemento =
        document.getElementById(
            id
        );


    if (!elemento)
    {
        return null;
    }


    const texto =
        elemento.value;


    if (!texto)
    {
        return null;
    }


    const valor =
        Number(
            texto
        );


    return Number.isInteger(
        valor
    )
        ? valor
        : null;
}


// =======================================================
// DECIMAL
// =======================================================

function valorDecimal(
    id
)
{
    const elemento =
        document.getElementById(
            id
        );


    if (!elemento)
    {
        return NaN;
    }


    const texto =
        elemento.value
            .trim()
            .replace(
                ",",
                "."
            );


    if (texto === "")
    {
        return NaN;
    }


    return Number(
        texto
    );
}


// =======================================================
// DECIMAL O NULL
// =======================================================

function valorDecimalONull(
    id
)
{
    const elemento =
        document.getElementById(
            id
        );


    if (!elemento)
    {
        return null;
    }


    const texto =
        elemento.value
            .trim()
            .replace(
                ",",
                "."
            );


    if (texto === "")
    {
        return null;
    }


    const valor =
        Number(
            texto
        );


    return Number.isFinite(
        valor
    )
        ? valor
        : null;
}


// =======================================================
// FECHA
// =======================================================

function formatearFecha(
    valor
)
{
    if (!valor)
    {
        return "-";
    }


    const fecha =
        new Date(
            valor
        );


    if (
        Number.isNaN(
            fecha.getTime()
        )
    )
    {
        return valor;
    }


    return fecha.toLocaleString(
        "es-AR"
    );
}


// =======================================================
// NUMERO
// =======================================================

function formatearNumero(
    valor
)
{
    if (
        valor === null ||
        valor === undefined ||
        valor === ""
    )
    {
        return "-";
    }


    const numero =
        Number(
            valor
        );


    if (
        !Number.isFinite(
            numero
        )
    )
    {
        return String(
            valor
        );
    }


    return numero.toLocaleString(
        "es-AR",
        {
            minimumFractionDigits:
                2,

            maximumFractionDigits:
                2
        }
    );
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