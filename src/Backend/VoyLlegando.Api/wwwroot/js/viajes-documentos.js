// =======================================================
// VOYLLEGANDO
// DOCUMENTACION DE VIAJES - LOGISTICA
// =======================================================

let documentoCP = null;


// =======================================================
// GUARDAMOS LA FUNCION ORIGINAL seleccionarViaje
// Y LE AGREGAMOS LA CARGA DE DOCUMENTACION
// =======================================================

const seleccionarViajeOriginal =
    seleccionarViaje;


seleccionarViaje =
    async function(idViaje)
    {
        await seleccionarViajeOriginal(
            idViaje
        );


        if (
            viajeSeleccionado &&
            Number(viajeSeleccionado.idViaje) ===
            Number(idViaje)
        )
        {
            await cargarDocumentosViaje(
                idViaje
            );
        }
    };


// =======================================================
// CARGAR DOCUMENTOS DEL VIAJE
// =======================================================

async function cargarDocumentosViaje(
    idViaje
)
{
    documentoCP = null;


    const nombreCP =
        document.getElementById(
            "nombreCP"
        );


    const btnAdjuntarCP =
        document.getElementById(
            "btnAdjuntarCP"
        );


    const btnVerCP =
        document.getElementById(
            "btnVerCP"
        );


    const btnQuitarCP =
        document.getElementById(
            "btnQuitarCP"
        );


    if (nombreCP)
    {
        nombreCP.textContent =
            "Cargando documentación...";
    }


    if (btnAdjuntarCP)
    {
        btnAdjuntarCP.textContent =
            "ADJUNTAR CP";
    }


    if (btnVerCP)
    {
        btnVerCP.classList.add(
            "oculto"
        );
    }


    if (btnQuitarCP)
    {
        btnQuitarCP.classList.add(
            "oculto"
        );
    }


    try
    {
        const documentos =
            await API.get(
                `/api/ViajeDocumentos/viaje/${idViaje}`
            );


        documentoCP =
            documentos.find(
                documento =>
                    String(
                        documento.tipo || ""
                    )
                    .toUpperCase() ===
                    "CP"
            ) || null;


        // ---------------------------------------------------
        // NO HAY CP
        // ---------------------------------------------------

        if (!documentoCP)
        {
            if (nombreCP)
            {
                nombreCP.textContent =
                    "Sin CP adjunta.";
            }


            if (btnAdjuntarCP)
            {
                btnAdjuntarCP.textContent =
                    "ADJUNTAR CP";
            }


            return;
        }


        // ---------------------------------------------------
        // HAY CP
        // ---------------------------------------------------

        if (nombreCP)
        {
            nombreCP.textContent =
                documentoCP.nombreArchivo ||
                "CP adjunta";
        }


        if (btnAdjuntarCP)
        {
            btnAdjuntarCP.textContent =
                "REEMPLAZAR CP";
        }


        if (btnVerCP)
        {
            btnVerCP.classList.remove(
                "oculto"
            );
        }


        if (btnQuitarCP)
        {
            btnQuitarCP.classList.remove(
                "oculto"
            );
        }
    }
    catch (error)
    {
        if (nombreCP)
        {
            nombreCP.textContent =
                "No se pudo consultar la CP.";
        }


        mensaje(
            "No se pudo consultar la documentación: " +
            error.message,
            true
        );
    }
}


// =======================================================
// SELECCIONAR ARCHIVO CP
// =======================================================

function seleccionarCP()
{
    if (!viajeSeleccionado)
    {
        mensaje(
            "Primero seleccione un viaje.",
            true
        );

        return;
    }


    const input =
        document.getElementById(
            "archivoCP"
        );


    if (!input)
        return;


    // Permite seleccionar nuevamente el mismo archivo.
    input.value = "";

    input.click();
}


// =======================================================
// SUBIR / REEMPLAZAR CP
// =======================================================

async function subirCP(
    input
)
{
    if (!viajeSeleccionado)
    {
        mensaje(
            "No hay un viaje seleccionado.",
            true
        );

        input.value = "";

        return;
    }


    const archivo =
        input.files?.[0];


    if (!archivo)
        return;


    const nombre =
        archivo.name || "";


    // ---------------------------------------------------
    // VALIDAR EXTENSION
    // ---------------------------------------------------

    if (
        !nombre
            .toLowerCase()
            .endsWith(".pdf")
    )
    {
        mensaje(
            "La CP debe ser un archivo PDF.",
            true
        );

        input.value = "";

        return;
    }


    // ---------------------------------------------------
    // MAXIMO 2 MB
    // ---------------------------------------------------

    const maximo =
        2 * 1024 * 1024;


    if (archivo.size > maximo)
    {
        mensaje(
            "El PDF no puede superar los 2 MB.",
            true
        );

        input.value = "";

        return;
    }


    // ---------------------------------------------------
    // SI YA EXISTE UNA CP, CONFIRMAMOS REEMPLAZO
    // ---------------------------------------------------

    if (documentoCP)
    {
        const continuar =
            confirm(
                "Este viaje ya tiene una CP adjunta.\n\n" +
                "La nueva CP reemplazará la actual.\n\n" +
                "¿Desea continuar?"
            );


        if (!continuar)
        {
            input.value = "";

            return;
        }
    }


    // ---------------------------------------------------
    // FORM DATA
    // ---------------------------------------------------

    const formData =
        new FormData();


    formData.append(
        "archivo",
        archivo
    );


    formData.append(
        "tipo",
        "CP"
    );


    try
    {
        mensaje(
            documentoCP
                ? "Reemplazando CP..."
                : "Adjuntando CP..."
        );


        const token =
            API.obtenerToken();


        const respuesta =
            await fetch(
                `/api/ViajeDocumentos/${viajeSeleccionado.idViaje}`,
                {
                    method:
                        "POST",

                    headers:
                    {
                        Authorization:
                            `Bearer ${token}`
                    },

                    body:
                        formData
                }
            );


        // ------------------------------------------------
        // ERROR DEL BACKEND
        // ------------------------------------------------

        if (!respuesta.ok)
        {
            let texto =
                "No se pudo guardar la CP.";


            try
            {
                const datosError =
                    await respuesta.json();


                texto =
                    datosError.message ||
                    datosError.mensaje ||
                    texto;
            }
            catch
            {
                try
                {
                    const detalle =
                        await respuesta.text();


                    if (detalle)
                    {
                        texto =
                            detalle;
                    }
                }
                catch
                {
                    // Conservamos mensaje original.
                }
            }


            throw new Error(
                texto
            );
        }


        // ------------------------------------------------
        // RECARGAR DOCUMENTACION
        // ------------------------------------------------

        await cargarDocumentosViaje(
            viajeSeleccionado.idViaje
        );


        mensaje(
            "CP guardada correctamente."
        );
    }
    catch (error)
    {
        mensaje(
            error.message,
            true
        );
    }
    finally
    {
        input.value = "";
    }
}


// =======================================================
// VER CP
// =======================================================

async function verCP()
{
    if (!documentoCP)
    {
        mensaje(
            "El viaje no tiene una CP adjunta.",
            true
        );

        return;
    }


    // ---------------------------------------------------
    // ABRIMOS LA VENTANA ANTES DEL FETCH.
    //
    // Esto evita el bloqueo de popup en algunos
    // navegadores, especialmente en celulares.
    // ---------------------------------------------------

    const ventana =
        window.open(
            "",
            "_blank"
        );


    if (!ventana)
    {
        mensaje(
            "El navegador bloqueó la apertura del PDF.",
            true
        );

        return;
    }


    try
    {
        ventana.document.write(
            "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "<meta charset='utf-8'>" +
            "<title>Cargando CP</title>" +
            "</head>" +
            "<body style='font-family:Arial,sans-serif;padding:20px'>" +
            "Cargando CP..." +
            "</body>" +
            "</html>"
        );


        const token =
            API.obtenerToken();


        const respuesta =
            await fetch(
                `/api/ViajeDocumentos/${documentoCP.idDocumento}`,
                {
                    method:
                        "GET",

                    headers:
                    {
                        Authorization:
                            `Bearer ${token}`
                    }
                }
            );


        if (!respuesta.ok)
        {
            throw new Error(
                "No se pudo obtener la CP."
            );
        }


        const blob =
            await respuesta.blob();


        const url =
            URL.createObjectURL(
                blob
            );


        ventana.location.href =
            url;


        setTimeout(
            () =>
            {
                URL.revokeObjectURL(
                    url
                );
            },
            60000
        );
    }
    catch (error)
    {
        ventana.close();


        mensaje(
            error.message,
            true
        );
    }
}


// =======================================================
// QUITAR CP
// =======================================================

async function quitarCP()
{
    if (!viajeSeleccionado)
    {
        mensaje(
            "No hay un viaje seleccionado.",
            true
        );

        return;
    }


    if (!documentoCP)
    {
        mensaje(
            "El viaje no tiene una CP adjunta.",
            true
        );

        return;
    }


    const nombre =
        documentoCP.nombreArchivo ||
        "CP adjunta";


    const confirmar =
        confirm(
            "¿Desea quitar la CP de este viaje?\n\n" +
            nombre +
            "\n\n" +
            "Esta acción eliminará el documento."
        );


    if (!confirmar)
        return;


    try
    {
        mensaje(
            "Quitando CP..."
        );


        const token =
            API.obtenerToken();


        const respuesta =
            await fetch(
                `/api/ViajeDocumentos/${documentoCP.idDocumento}`,
                {
                    method:
                        "DELETE",

                    headers:
                    {
                        Authorization:
                            `Bearer ${token}`
                    }
                }
            );


        if (!respuesta.ok)
        {
            let texto =
                "No se pudo quitar la CP.";


            try
            {
                const datosError =
                    await respuesta.json();


                texto =
                    datosError.message ||
                    datosError.mensaje ||
                    texto;
            }
            catch
            {
                try
                {
                    const detalle =
                        await respuesta.text();


                    if (detalle)
                    {
                        texto =
                            detalle;
                    }
                }
                catch
                {
                    // Conservamos mensaje original.
                }
            }


            throw new Error(
                texto
            );
        }


        // Ya no existe una CP vigente.
        documentoCP = null;


        // Recargamos para que todos los botones
        // y textos queden sincronizados.
        await cargarDocumentosViaje(
            viajeSeleccionado.idViaje
        );


        mensaje(
            "CP quitada correctamente."
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