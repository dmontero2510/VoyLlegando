// =======================================================
// VOYLLEGANDO
// AUTENTICACION
// =======================================================


// =======================================================
// LOGIN
// =======================================================

async function login()
{
    const celular =
        document
            .getElementById(
                "celular"
            )
            .value
            .trim();


    const clave =
        document
            .getElementById(
                "clave"
            )
            .value;


    if (!celular || !clave)
    {
        mostrarMensaje(
            "Ingrese celular y clave.",
            true
        );

        return;
    }


    try
    {
        bloquearLogin(true);


        mostrarMensaje(
            "Ingresando..."
        );


        const response =
            await fetch(
                "/api/auth/login",
                {
                    method:
                        "POST",

                    headers:
                    {
                        "Content-Type":
                            "application/json"
                    },

                    body:
                        JSON.stringify(
                            {
                                celular:
                                    celular,

                                clave:
                                    clave
                            }
                        )
                }
            );


        const datos =
            await response.json();


        if (
            !response.ok ||
            !datos.success
        )
        {
            throw new Error(
                datos.mensaje ||
                "No se pudo iniciar sesión."
            );
        }


        API.guardarSesion(
            datos.token,
            datos.usuario
        );


        if (datos.usuario.debeCambiarClave)
        {
            window.location.href =
                "/cambiar-clave.html";

            return;
        }


        redirigirSegunRol(
            datos.usuario.rol
        );
    }
    catch (error)
    {
        mostrarMensaje(
            error.message,
            true
        );
    }
    finally
    {
        bloquearLogin(false);
    }
}


// =======================================================
// REDIRECCION SEGUN ROL
// =======================================================

function redirigirSegunRol(
    rol
)
{
    if (rol === "L")
    {
        window.location.href =
            "/logistica.html";

        return;
    }


    if (rol === "E")
    {
        window.location.href =
            "/empresa.html";

        return;
    }


    if (rol === "S")
    {
        window.location.href =
            "/admin.html";

        return;
    }


    API.cerrarSesion();


    mostrarMensaje(
        "Este usuario no tiene acceso al sistema.",
        true
    );
}


// =======================================================
// VALIDAR SESION
// =======================================================

async function validarSesion(
    rolPermitido = null
)
{
    const token =
        API.obtenerToken();


    if (!token)
    {
        irLogin();

        return null;
    }


    try
    {
        const perfil =
            await API.get(
                "/api/auth/perfil"
            );


        // Actualizamos localStorage
        // con el perfil real actual.

        API.guardarSesion(
            token,
            perfil
        );


        if (
            perfil.debeCambiarClave &&
            window.location.pathname !==
                "/cambiar-clave.html"
        )
        {
            window.location.href =
                "/cambiar-clave.html";

            return null;
        }


        if (
            rolPermitido &&
            perfil.rol !== rolPermitido
        )
        {
            redirigirSegunRol(
                perfil.rol
            );

            return null;
        }


        return perfil;
    }
    catch
    {
        API.cerrarSesion();

        irLogin();

        return null;
    }
}


// =======================================================
// LOGOUT
// =======================================================

function logout()
{
    API.cerrarSesion();

    irLogin();
}


// =======================================================
// IR AL LOGIN
// =======================================================

function irLogin()
{
    window.location.href =
        "/index.html";
}


// =======================================================
// SI YA ESTA LOGUEADO
// =======================================================

function redirigirSiYaEstaLogueado()
{
    const usuario =
        API.obtenerUsuario();


    if (!usuario)
        return;


    if (usuario.debeCambiarClave)
    {
        window.location.href =
            "/cambiar-clave.html";

        return;
    }


    redirigirSegunRol(
        usuario.rol
    );
}


// =======================================================
// ENTER EN CLAVE
// =======================================================

function configurarEnterLogin()
{
    const clave =
        document.getElementById(
            "clave"
        );


    if (!clave)
        return;


    clave.addEventListener(
        "keydown",
        event =>
        {
            if (
                event.key ===
                "Enter"
            )
            {
                login();
            }
        }
    );
}


// =======================================================
// MENSAJE
// =======================================================

function mostrarMensaje(
    texto,
    error = false
)
{
    const mensaje =
        document.getElementById(
            "mensaje"
        );


    if (!mensaje)
        return;


    mensaje.textContent =
        texto;


    mensaje.className =
        error
            ? "mensaje error"
            : "mensaje";
}


// =======================================================
// BLOQUEAR LOGIN
// =======================================================

function bloquearLogin(
    bloqueado
)
{
    const boton =
        document.getElementById(
            "btnIngresar"
        );


    if (!boton)
        return;


    boton.disabled =
        bloqueado;


    boton.textContent =
        bloqueado
            ? "Ingresando..."
            : "Ingresar";
}
