// =======================================================
// VOYLLEGANDO
// ACCESO GENERAL A LA API
// =======================================================

const API = {

    // ---------------------------------------------------
    // TOKEN
    // ---------------------------------------------------

    obtenerToken()
    {
        return localStorage.getItem(
            "voyllegando_token"
        );
    },


    // ---------------------------------------------------
    // USUARIO
    // ---------------------------------------------------

    obtenerUsuario()
    {
        const json =
            localStorage.getItem(
                "voyllegando_usuario"
            );

        if (!json)
            return null;

        try
        {
            return JSON.parse(json);
        }
        catch
        {
            return null;
        }
    },


    // ---------------------------------------------------
    // GUARDAR SESION
    // ---------------------------------------------------

    guardarSesion(token, usuario)
    {
        localStorage.setItem(
            "voyllegando_token",
            token
        );

        localStorage.setItem(
            "voyllegando_usuario",
            JSON.stringify(usuario)
        );
    },


    // ---------------------------------------------------
    // CERRAR SESION
    // ---------------------------------------------------

    cerrarSesion()
    {
        localStorage.removeItem(
            "voyllegando_token"
        );

        localStorage.removeItem(
            "voyllegando_usuario"
        );
    },


    // ---------------------------------------------------
    // REQUEST GENERAL
    // ---------------------------------------------------

    async request(
        url,
        opciones = {}
    )
    {
        const token =
            this.obtenerToken();

        const headers =
        {
            ...(opciones.headers || {})
        };


        if (token)
        {
            headers.Authorization =
                `Bearer ${token}`;
        }


        if (
            opciones.body &&
            !headers["Content-Type"]
        )
        {
            headers["Content-Type"] =
                "application/json";
        }


        const response =
            await fetch(
                url,
                {
                    ...opciones,
                    headers
                }
            );


        // -----------------------------------------------
        // SESION VENCIDA
        // -----------------------------------------------

        if (response.status === 401)
        {
            this.cerrarSesion();

            throw new Error(
                "La sesión venció. Ingrese nuevamente."
            );
        }


        // -----------------------------------------------
        // SIN PERMISO
        // -----------------------------------------------

        if (response.status === 403)
        {
            throw new Error(
                "No tiene permiso para realizar esta operación."
            );
        }


        const contentType =
            response.headers.get(
                "content-type"
            );


        let datos = null;


        if (
            contentType &&
            contentType.includes(
                "application/json"
            )
        )
        {
            datos =
                await response.json();
        }
        else
        {
            datos =
                await response.text();
        }


        if (!response.ok)
        {
            let mensaje =
                `Error ${response.status}`;


            if (typeof datos === "string")
            {
                if (datos)
                    mensaje = datos;
            }
            else if (datos)
            {
                mensaje =
                    datos.mensaje ||
                    datos.title ||
                    mensaje;
            }


            throw new Error(
                mensaje
            );
        }


        return datos;
    },


    // ---------------------------------------------------
    // GET
    // ---------------------------------------------------

    async get(url)
    {
        return await this.request(
            url,
            {
                method: "GET"
            }
        );
    },


    // ---------------------------------------------------
    // POST
    // ---------------------------------------------------

    async post(
        url,
        datos = null
    )
    {
        const opciones =
        {
            method: "POST"
        };


        if (datos !== null)
        {
            opciones.body =
                JSON.stringify(datos);
        }


        return await this.request(
            url,
            opciones
        );
    },


    // ---------------------------------------------------
    // PUT
    // ---------------------------------------------------

    async put(
        url,
        datos
    )
    {
        return await this.request(
            url,
            {
                method: "PUT",

                body:
                    JSON.stringify(datos)
            }
        );
    },


    // ---------------------------------------------------
    // DELETE
    // ---------------------------------------------------

    async delete(url)
    {
        return await this.request(
            url,
            {
                method: "DELETE"
            }
        );
    }
};
