const API_EXTERNA = "http://localhost:5089/api/";
const PUERTOS_FRONT = ["5500", "5501", "5502"];
const API_URL = (location.protocol === "file:" || PUERTOS_FRONT.includes(location.port))
    ? API_EXTERNA
    : "/api/";

// ---------- Elementos de la página ----------
const formulario = document.getElementById("formulario");
const campoInstrumento = document.getElementById("idInstrumento");
const campoTasa = document.getElementById("tasaInteres");
const botonGuardar = document.getElementById("guardar");
const botonRecargar = document.getElementById("recargar");
const buscador = document.getElementById("buscador");
const cuerpoTabla = document.getElementById("cuerpo-tabla");
const vacio = document.getElementById("vacio");
const conexion = document.getElementById("conexion");
const aviso = document.getElementById("aviso");
const totalOperaciones = document.getElementById("total-operaciones");
const totalPymes = document.getElementById("total-pymes");

let operaciones = [];
let temporizadorAviso;

// ---------- Utilidades ----------
function valorDe(objeto, nombre) {
    const clave = Object.keys(objeto).find(k => k.toLowerCase() === nombre.toLowerCase());
    return clave === undefined ? undefined : objeto[clave];
}

function formatearMonto(valor) {
    const numero = Number(valor);
    if (valor === "" || valor === undefined || valor === null || !Number.isFinite(numero) || String(valor).startsWith("0x")) {
        return valor || "—";
    }
    return numero.toLocaleString("es-AR", { style: "currency", currency: "ARS" });
}

function claseEstado(estado) {
    const texto = String(estado).toLowerCase();
    if (/(aprob|activ|complet|acredit|ok)/.test(texto)) return "ok";
    if (/(pend|proceso|espera|revis)/.test(texto)) return "alerta";
    if (/(rechaz|cancel|anul|venc)/.test(texto)) return "error";
    return "";
}

function mostrarAviso(mensaje, tipo = "") {
    aviso.textContent = mensaje;
    aviso.className = `aviso visible ${tipo}`;
    clearTimeout(temporizadorAviso);
    temporizadorAviso = setTimeout(() => aviso.classList.remove("visible"), 5000);
}

function marcarConexion(conectada) {
    conexion.className = `chip ${conectada ? "ok" : "error"}`;
    conexion.textContent = conectada ? "API conectada" : "API sin conexión";
}

async function pedir(ruta, opciones) {
    let respuesta;
    try {
        respuesta = await fetch(API_URL + ruta, opciones);
    } catch {
        marcarConexion(false);
        throw new Error("No se pudo conectar con la API. Revisá que esté corriendo (dotnet run).");
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

// ---------- Listado (GET a la vista) ----------
async function cargar() {
    botonRecargar.disabled = true;
    try {
        operaciones = await pedir("OperacionesVista");
    } catch (error) {
        operaciones = [];
        mostrarAviso(error.message, "error");
    } finally {
        botonRecargar.disabled = false;
        dibujar();
    }
}

function celda(texto, clase = "") {
    const td = document.createElement("td");
    td.textContent = texto;
    if (clase) td.className = clase;
    return td;
}

function dibujar() {
    const filtro = buscador.value.trim().toLowerCase();

    const visibles = operaciones.filter(op =>
        !filtro || ["idOperacion", "cuitPyme", "razonSocial", "montoOperacion", "estado"]
            .some(campo => String(valorDe(op, campo) ?? "").toLowerCase().includes(filtro))
    );

    cuerpoTabla.innerHTML = "";

    for (const op of visibles) {
        const fila = document.createElement("tr");
        const estado = valorDe(op, "estado") ?? "";

        const celdaEstado = document.createElement("td");
        const etiqueta = document.createElement("span");
        etiqueta.className = `estado ${claseEstado(estado)}`;
        etiqueta.textContent = estado || "—";
        celdaEstado.appendChild(etiqueta);

        fila.append(
            celda(valorDe(op, "idOperacion"), "num"),
            celda(valorDe(op, "cuitPyme"), "num"),
            celda(valorDe(op, "razonSocial") || "—"),
            celda(formatearMonto(valorDe(op, "montoOperacion")), "num"),
            celdaEstado
        );
        cuerpoTabla.appendChild(fila);
    }

    vacio.hidden = visibles.length > 0;
    totalOperaciones.textContent = operaciones.length;
    totalPymes.textContent = new Set(operaciones.map(op => valorDe(op, "cuitPyme"))).size;
}

// ---------- Formulario (POST que llama al stored procedure) ----------
formulario.addEventListener("submit", async (evento) => {
    evento.preventDefault();

    const idInstrumento = Number(campoInstrumento.value);
    const tasaInteres = Number(campoTasa.value);

    const instrumentoValido = campoInstrumento.value.trim() !== "" && Number.isInteger(idInstrumento) && idInstrumento > 0;
    const tasaValida = campoTasa.value.trim() !== "" && Number.isFinite(tasaInteres) && tasaInteres >= 0 && tasaInteres <= 999.99;

    campoInstrumento.classList.toggle("invalido", !instrumentoValido);
    campoTasa.classList.toggle("invalido", !tasaValida);

    if (!instrumentoValido || !tasaValida) {
        mostrarAviso("Revisá los datos: el instrumento debe ser un número entero y la tasa estar entre 0 y 999,99.", "error");
        return;
    }

    botonGuardar.disabled = true;

    try {
        const respuesta = await pedir("Tasas", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ idInstrumento, tasaInteres })
        });

        formulario.reset();
        mostrarAviso(respuesta.mensaje || "Tasa registrada correctamente.", "ok");
    } catch (error) {
        mostrarAviso(error.message, "error");
    } finally {
        botonGuardar.disabled = false;
    }
});

buscador.addEventListener("input", dibujar);
botonRecargar.addEventListener("click", cargar);

// Primera carga al abrir la página.
cargar();


// ── Garantías ──
async function cargarGarantias() {
    const estado = document.getElementById("estado-garantias");

    try {
        const response = await fetch(API_URL + "Garantias");
        if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);

        const garantias = await response.json();

        document.getElementById("cuerpo-garantias").innerHTML = garantias.map(g => `
            <tr>
                <td class="num">${g.id}</td>
                <td class="num">${g.cuitPyme}</td>
                <td>${g.descripcion}</td>
                <td class="num">$${g.monto.toLocaleString("es-AR")}</td>
            </tr>
        `).join("");

        estado.textContent = garantias.length === 0 ? "No hay garantías cargadas." : "";
    } catch (error) {
        estado.textContent = "No se pudieron cargar las garantías. Verificá que la API esté corriendo.";
        console.error(error);
    }
}

async function crearGarantia(nueva) {
    const response = await fetch(API_URL + "Garantias", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(nueva),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.detalle || `Error HTTP: ${response.status}`);
    }
    return response.json();
}

// ── Disponibilidad de garantías ──
async function cargarDisponibilidad() {
    const estado = document.getElementById("estado-disponibilidad");

    try {
        const response = await fetch(API_URL + "GarantiasDisponibilidad");
        if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);

        const disponibilidades = await response.json();

        document.getElementById("cuerpo-disponibilidad").innerHTML = disponibilidades.map(d => `
            <tr>
                <td class="num">${d.idGarantia}</td>
                <td>${d.fechaDesde}</td>
                <td>${d.fechaHasta}</td>
                <td class="num">${d.idOperacion}</td>
            </tr>
        `).join("");

        estado.textContent = disponibilidades.length === 0 ? "No hay disponibilidades cargadas." : "";
    } catch (error) {
        estado.textContent = "No se pudo cargar la disponibilidad. Verificá que la API esté corriendo.";
        console.error(error);
    }
}

async function crearDisponibilidad(nueva) {
    const response = await fetch(API_URL + "GarantiasDisponibilidad", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(nueva),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.detalle || `Error HTTP: ${response.status}`);
    }
    return response.json();
}

// ── Auditoría de operaciones (solo lectura: la completa el trigger) ──
async function cargarAuditoria() {
    const estado = document.getElementById("estado-auditoria");

    try {
        const response = await fetch(API_URL + "OperacionesAuditoria");
        if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);

        const registros = await response.json();

        document.getElementById("cuerpo-auditoria").innerHTML = registros.map(r => `
            <tr>
                <td class="num">${r.idOperacion}</td>
                <td>${r.fechaOperacion}</td>
                <td class="num">$${r.montoOperacion.toLocaleString("es-AR")}</td>
                <td>${r.tipoAccion}</td>
            </tr>
        `).join("");

        estado.textContent = registros.length === 0 ? "No hay registros de auditoría." : "";
    } catch (error) {
        estado.textContent = "No se pudo cargar la auditoría. Verificá que la API esté corriendo.";
        console.error(error);
    }
}

// ── Carga inicial y eventos ──
document.addEventListener("DOMContentLoaded", () => {
    cargarGarantias();
    cargarDisponibilidad();
    cargarAuditoria();

    document.getElementById("recargar-auditoria").addEventListener("click", cargarAuditoria);

    document.getElementById("form-garantia").addEventListener("submit", async (event) => {
        event.preventDefault(); // el navegador ya validó required/min/minlength

        const nueva = {
            cuitPyme: parseInt(document.getElementById("g-cuit").value),
            descripcion: document.getElementById("g-desc").value.trim(),
            monto: parseFloat(document.getElementById("g-monto").value),
        };

        try {
            await crearGarantia(nueva);
            event.target.reset();
            mostrarAviso("Garantía registrada correctamente.", "ok");
            await cargarGarantias();
        } catch (error) {
            mostrarAviso(error.message, "error");
        }
    });

    document.getElementById("form-disponibilidad").addEventListener("submit", async (event) => {
        event.preventDefault();

        const nueva = {
            idGarantia: parseInt(document.getElementById("d-garantia").value),
            fechaDesde: document.getElementById("d-desde").value,
            fechaHasta: document.getElementById("d-hasta").value,
            idOperacion: parseInt(document.getElementById("d-operacion").value),
        };

        try {
            await crearDisponibilidad(nueva);
            event.target.reset();
            mostrarAviso("Disponibilidad registrada correctamente.", "ok");
            await cargarDisponibilidad();
        } catch (error) {
            mostrarAviso(error.message, "error");
        }
    });
});