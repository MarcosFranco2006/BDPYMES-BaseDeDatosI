// Panel de gestión BDPYMES: HTML + CSS + JavaScript sin frameworks.
// Todas las secciones salen de la lista ENTIDADES: para agregar una nueva
// solo hay que sumar un objeto ahí (no hace falta tocar el resto del código).

// Si la página se sirve desde la propia API (http://localhost:5089) usamos una
// ruta relativa; si se abre con Live Server (puerto 5500) o como archivo,
// apuntamos directo a la API.
const API_EXTERNA = "http://localhost:5089/api/";
const PUERTOS_FRONT = ["5500", "5501", "5502"];
const API_URL = (location.protocol === "file:" || PUERTOS_FRONT.includes(location.port))
    ? API_EXTERNA
    : "/api/";

const ENTIDADES = [
    {
        clave: "pymes", grupo: "Empresas", etiqueta: "PyMEs", ruta: "Pymes",
        subtitulo: "Pequeñas y medianas empresas registradas.",
        campos: [
            { nombre: "CUIT", etiqueta: "CUIT", max: 13, placeholder: "30-12345678-9" },
            { nombre: "Razon_Social", etiqueta: "Razón social", max: 50 },
            { nombre: "Direccion", etiqueta: "Dirección", max: 50 }
        ],
        columnas: [
            { nombre: "CUIT", etiqueta: "CUIT" },
            { nombre: "Razon_Social", etiqueta: "Razón social" },
            { nombre: "Direccion", etiqueta: "Dirección" }
        ]
    },
    {
        clave: "garantias", grupo: "Empresas", etiqueta: "Garantías", ruta: "Garantias",
        subtitulo: "Garantías asociadas a cada PyME.",
        campos: [
            { nombre: "CuitPyme", etiqueta: "PyME", tipo: "select",
              origen: { ruta: "Pymes", valor: "CUIT", texto: x => `${x.cuit} — ${x.razon_Social}` } },
            { nombre: "Descripcion", etiqueta: "Descripción", max: 100 },
            { nombre: "Monto", etiqueta: "Monto", tipo: "number", min: 0 }
        ],
        columnas: [
            { nombre: "Id", etiqueta: "Id", numerica: true },
            { nombre: "CuitPyme", etiqueta: "CUIT PyME" },
            { nombre: "Descripcion", etiqueta: "Descripción" },
            { nombre: "Monto", etiqueta: "Monto", formato: "moneda", numerica: true }
        ]
    },
    {
        clave: "sgrs", grupo: "Empresas", etiqueta: "SGRs", ruta: "Sgrs",
        subtitulo: "Sociedades de Garantía Recíproca.",
        campos: [{ nombre: "Nombre", etiqueta: "Nombre", max: 50 }],
        columnas: [
            { nombre: "Id", etiqueta: "Id", numerica: true },
            { nombre: "Nombre", etiqueta: "Nombre" }
        ]
    },
    {
        clave: "brokers", grupo: "Mercado", etiqueta: "Brokers", ruta: "Brokers",
        subtitulo: "Brokers que operan con las PyMEs.",
        campos: [{ nombre: "Nombre", etiqueta: "Nombre", max: 50 }],
        columnas: [
            { nombre: "Id", etiqueta: "Id", numerica: true },
            { nombre: "Nombre", etiqueta: "Nombre" }
        ]
    },
    {
        clave: "comisiones", grupo: "Mercado", etiqueta: "Comisiones", ruta: "BrokersComisiones",
        subtitulo: "Porcentaje de comisión vigente de cada broker.",
        campos: [
            { nombre: "IdBroker", etiqueta: "Broker", tipo: "select", numerico: true,
              origen: { ruta: "Brokers", valor: "Id", texto: x => `${x.id} — ${x.nombre}` } },
            { nombre: "PorcentajeComision", etiqueta: "Comisión (%)", tipo: "number", min: 0 }
        ],
        columnas: [
            { nombre: "IdBroker", etiqueta: "Id broker", numerica: true },
            { nombre: "PorcentajeComision", etiqueta: "Comisión", formato: "porcentaje", numerica: true },
            { nombre: "UltimaActualizacion", etiqueta: "Última actualización", formato: "fecha" }
        ]
    },
    {
        clave: "alycs", grupo: "Mercado", etiqueta: "ALYCs", ruta: "Alycs",
        subtitulo: "Agentes de Liquidación y Compensación.",
        campos: [{ nombre: "Nombre", etiqueta: "Nombre", max: 50 }],
        columnas: [
            { nombre: "Id", etiqueta: "Id", numerica: true },
            { nombre: "Nombre", etiqueta: "Nombre" }
        ]
    },
    {
        clave: "instrumentos", grupo: "Mercado", etiqueta: "Instrumentos", ruta: "Instrumentos",
        subtitulo: "Tipos de instrumentos financieros.",
        campos: [{ nombre: "TipoInstrumento", etiqueta: "Tipo de instrumento", max: 50 }],
        columnas: [
            { nombre: "Id", etiqueta: "Id", numerica: true },
            { nombre: "TipoInstrumento", etiqueta: "Tipo de instrumento" }
        ]
    },
    {
        clave: "tasas", grupo: "Mercado", etiqueta: "Tasas de interés", ruta: "TasasInteres",
        subtitulo: "Tasa de interés vigente de cada instrumento.",
        campos: [
            { nombre: "IdInstrumento", etiqueta: "Instrumento", tipo: "select", numerico: true,
              origen: { ruta: "Instrumentos", valor: "Id", texto: x => `${x.id} — ${x.tipoInstrumento}` } },
            { nombre: "TasaDeInteres", etiqueta: "Tasa de interés (%)", tipo: "number", min: 0 }
        ],
        columnas: [
            { nombre: "IdInstrumento", etiqueta: "Id instrumento", numerica: true },
            { nombre: "TasaDeInteres", etiqueta: "Tasa", formato: "porcentaje", numerica: true },
            { nombre: "UltimaActualizacion", etiqueta: "Última actualización", formato: "fecha" }
        ]
    }
];

// ---------- Elementos de la página ----------
const menu = document.getElementById("menu");
const titulo = document.getElementById("titulo");
const subtitulo = document.getElementById("subtitulo");
const conexion = document.getElementById("conexion");
const formulario = document.getElementById("formulario");
const cabecera = document.getElementById("cabecera");
const cuerpoTabla = document.getElementById("cuerpo-tabla");
const contador = document.getElementById("contador");
const vacio = document.getElementById("vacio");
const buscador = document.getElementById("buscador");
const botonRecargar = document.getElementById("recargar");
const aviso = document.getElementById("aviso");

let actual = ENTIDADES[0];
let registros = [];
let temporizadorAviso;

// ---------- Utilidades ----------
function el(etiqueta, atributos = {}, ...hijos) {
    const nodo = document.createElement(etiqueta);
    for (const [k, v] of Object.entries(atributos)) {
        if (k === "class") nodo.className = v;
        else if (k === "text") nodo.textContent = v;
        else nodo.setAttribute(k, v);
    }
    nodo.append(...hijos);
    return nodo;
}

// ASP.NET devuelve las propiedades en camelCase ("razon_Social"), así que
// buscamos la propiedad sin distinguir mayúsculas.
function valorDe(objeto, nombre) {
    const clave = Object.keys(objeto).find(k => k.toLowerCase() === nombre.toLowerCase());
    return clave === undefined ? undefined : objeto[clave];
}

function formatear(valor, formato) {
    if (valor === undefined || valor === null || valor === "") return "—";
    switch (formato) {
        case "moneda":
            return Number(valor).toLocaleString("es-AR", { style: "currency", currency: "ARS" });
        case "porcentaje":
            return `${Number(valor).toLocaleString("es-AR", { maximumFractionDigits: 4 })} %`;
        case "fecha": {
            const fecha = new Date(valor);
            return isNaN(fecha) ? String(valor)
                : fecha.toLocaleString("es-AR", { dateStyle: "medium", timeStyle: "short" });
        }
        default:
            return String(valor);
    }
}

function mostrarAviso(mensaje, tipo = "") {
    aviso.textContent = mensaje;
    aviso.className = `aviso visible ${tipo}`;
    clearTimeout(temporizadorAviso);
    temporizadorAviso = setTimeout(() => aviso.classList.remove("visible"), 4500);
}

function marcarConexion(estado) {
    conexion.className = `chip ${estado ? "ok" : "error"}`;
    conexion.textContent = estado ? "API conectada" : "API sin conexión";
}

// Un único lugar para hablar con la API. Si el servidor no responde, fetch
// lanza "Failed to fetch": lo convertimos en un mensaje que se entienda.
async function pedir(ruta, opciones) {
    let respuesta;
    try {
        respuesta = await fetch(API_URL + ruta, opciones);
    } catch {
        marcarConexion(false);
        throw new Error("No se pudo conectar con la API. Revisá que esté corriendo (dotnet run) y que la URL sea la correcta.");
    }

    marcarConexion(true);

    if (!respuesta.ok) {
        let detalle = "";
        try {
            const cuerpo = await respuesta.json();
            detalle = cuerpo.detalle || cuerpo.mensaje || cuerpo.title || "";
        } catch { /* la respuesta no era JSON */ }
        throw new Error(detalle || `La API respondió ${respuesta.status}`);
    }

    return respuesta.json();
}

// ---------- Menú ----------
function dibujarMenu() {
    menu.innerHTML = "";
    let grupoActual = "";

    for (const entidad of ENTIDADES) {
        if (entidad.grupo !== grupoActual) {
            grupoActual = entidad.grupo;
            menu.appendChild(el("div", { class: "menu-grupo", text: grupoActual }));
        }
        const boton = el("button", { class: "menu-item", type: "button", "data-clave": entidad.clave, text: entidad.etiqueta });
        boton.addEventListener("click", () => seleccionar(entidad.clave));
        menu.appendChild(boton);
    }
}

async function seleccionar(clave) {
    actual = ENTIDADES.find(e => e.clave === clave) || ENTIDADES[0];
    history.replaceState(null, "", `#${actual.clave}`);

    for (const boton of menu.querySelectorAll(".menu-item")) {
        boton.classList.toggle("activo", boton.dataset.clave === actual.clave);
    }

    titulo.textContent = actual.etiqueta;
    subtitulo.textContent = actual.subtitulo;
    buscador.value = "";

    dibujarCabecera();
    await Promise.all([dibujarFormulario(), cargar()]);
}

// ---------- Formulario (POST) ----------
async function dibujarFormulario() {
    formulario.innerHTML = "";

    for (const campo of actual.campos) {
        const id = `campo-${campo.nombre}`;
        let control;

        if (campo.tipo === "select") {
            control = el("select", { id, name: campo.nombre, required: "" });
            control.appendChild(el("option", { value: "", text: "Cargando…" }));
        } else {
            control = el("input", { id, name: campo.nombre, type: campo.tipo || "text", required: "" });
            if (campo.max) control.setAttribute("maxlength", campo.max);
            if (campo.tipo === "number") {
                control.setAttribute("step", "any");
                if (campo.min !== undefined) control.setAttribute("min", campo.min);
            }
            if (campo.placeholder) control.setAttribute("placeholder", campo.placeholder);
        }

        formulario.appendChild(el("div", { class: "campo" }, el("label", { for: id, text: campo.etiqueta }), control));
    }

    formulario.appendChild(el("div", { class: "campo" }, el("button", { type: "submit", text: "Guardar" })));

    // Los desplegables se llenan con datos de otra tabla (por ejemplo, las PyMEs).
    for (const campo of actual.campos.filter(c => c.tipo === "select")) {
        await llenarSelect(campo);
    }
}

async function llenarSelect(campo) {
    const select = document.getElementById(`campo-${campo.nombre}`);
    try {
        const datos = await pedir(campo.origen.ruta);
        select.innerHTML = "";
        select.appendChild(el("option", { value: "", text: datos.length ? "Elegí una opción…" : "No hay opciones cargadas" }));
        for (const dato of datos) {
            select.appendChild(el("option", { value: valorDe(dato, campo.origen.valor), text: campo.origen.texto(dato) }));
        }
    } catch {
        select.innerHTML = "";
        select.appendChild(el("option", { value: "", text: "No se pudieron cargar las opciones" }));
    }
}

formulario.addEventListener("submit", async (evento) => {
    evento.preventDefault();

    const cuerpo = {};
    let valido = true;

    for (const campo of actual.campos) {
        const control = document.getElementById(`campo-${campo.nombre}`);
        const texto = control.value.trim();
        const faltante = texto === "";

        control.classList.toggle("invalido", faltante);
        if (faltante) valido = false;

        cuerpo[campo.nombre] = (campo.tipo === "number" || campo.numerico) ? Number(texto) : texto;
    }

    if (!valido) {
        mostrarAviso("Completá todos los campos.", "error");
        return;
    }

    const boton = formulario.querySelector("button[type=submit]");
    boton.disabled = true;

    try {
        await pedir(actual.ruta, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(cuerpo)
        });

        formulario.reset();
        mostrarAviso("Registro guardado correctamente.", "ok");
        await cargar();
    } catch (error) {
        mostrarAviso(error.message, "error");
    } finally {
        boton.disabled = false;
    }
});

// ---------- Listado (GET) ----------
function dibujarCabecera() {
    cabecera.innerHTML = "";
    const fila = el("tr");
    for (const columna of actual.columnas) {
        fila.appendChild(el("th", { class: columna.numerica ? "num" : "", text: columna.etiqueta }));
    }
    cabecera.appendChild(fila);
}

async function cargar() {
    botonRecargar.disabled = true;
    try {
        registros = await pedir(actual.ruta);
        dibujarTabla();
    } catch (error) {
        registros = [];
        dibujarTabla();
        mostrarAviso(error.message, "error");
    } finally {
        botonRecargar.disabled = false;
    }
}

function dibujarTabla() {
    const filtro = buscador.value.trim().toLowerCase();

    const visibles = registros.filter(registro =>
        !filtro || actual.columnas.some(c => String(valorDe(registro, c.nombre) ?? "").toLowerCase().includes(filtro))
    );

    cuerpoTabla.innerHTML = "";

    for (const registro of visibles) {
        const fila = el("tr");
        for (const columna of actual.columnas) {
            // textContent (y no innerHTML) evita inyectar HTML desde los datos.
            fila.appendChild(el("td", {
                class: columna.numerica ? "num" : "",
                text: formatear(valorDe(registro, columna.nombre), columna.formato)
            }));
        }
        cuerpoTabla.appendChild(fila);
    }

    contador.textContent = `(${visibles.length}${filtro ? ` de ${registros.length}` : ""})`;
    vacio.hidden = visibles.length > 0;
}

buscador.addEventListener("input", dibujarTabla);
botonRecargar.addEventListener("click", cargar);

// ---------- Inicio ----------
dibujarMenu();
seleccionar(location.hash.slice(1) || ENTIDADES[0].clave);
