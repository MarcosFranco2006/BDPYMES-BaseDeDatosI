# BDPYMES

Backend y frontend hechos para comunicarse con la base de datos BDPYMES, para el proyecto final de Base de Datos I.

Alumnos: Fagiano Bautista, Franco Marcos, Nuñez Marko, Salusso Gabriel

# Clonar el repositorio y entrar a la carpeta raíz
En la terminal de VSCode escribir el siguiente comando 'git clone https://github.com/MarcosFranco2006/BDPYMES-BaseDeDatosI', apretar ENTER y acto seguido escribir 'cd BDPYMES-BaseDeDatosI' y apretar ENTER para acceder a la carpeta raíz del proyecto.

# Crear la base de datos
Requisito: SQL Server con SQL Server Management Studio (SSMS).

1. Crear en el disco C: las carpetas que usa el script para los archivos físicos de la base: 'C:\BDD-PYME\SSD(A)', 'C:\BDD-PYME\SSD(B)', 'C:\BDD-PYME\SSD(G)', 'C:\BDD-PYME\SSD(H)', 'C:\BDD-PYME\RAID5(E)', 'C:\BDD-PYME\RAID5(F)', 'C:\BDD-PYME\RAID5(I)' y 'C:\BDD-PYME\RAID5(J)'. SQL Server no crea las carpetas, si no existen el script falla.
2. Abrir en SSMS el archivo 'BaseDeDatos/levantar-base-de-datos.sql' y ejecutarlo. Crea la base 'BDD-PYME' con sus grupos de archivos, tablas, restricciones, la vista 'VW_Operaciones_Pymes', el procedimiento almacenado 'SP_RegistrarTasaConValidacion' y el trigger 'TR_Auditoria_Operaciones'.
3. Abrir y ejecutar 'BaseDeDatos/datos-de-prueba.sql' para cargar datos de ejemplo.

La API se conecta a 'Server=.;Database=BDD-PYME;Integrated Security=true' (cadena definida en 'BDPYMES.LogicaServicio/Servicios/PymesService.cs'). Si el servidor SQL tiene otro nombre, hay que cambiarla ahí.

# Correr Test unitario (Aún no implementados)
El proyecto de tests es 'BDPYMES.LogicaTest', pero todavía no tiene tests implementados.

-Desde la terminal:

Abrir una terminal desde la carpeta raíz en donde se va a escribir el comando 'cd BDPYMES.LogicaTest', apretar ENTER y luego escribir 'dotnet test' y apretar ENTER.

# Levantar APIs
-Desde la terminal:

En la terminal abierta desde la carpeta raíz poner: 'cd BDPYMES.Api' darle a ENTER, luego poner en la misma terminal 'dotnet run', darle a la combinación 'CTRL + CLICK' al puerto aclarado como 'http://localhost:5089/' y agregar 'swagger' al final de la URL para probar los endpoints.

La URL final quedaría: http://localhost:5089/swagger/index.html

# Levantar Frontend
Una vez levantada la API, se debe iniciar el Live Server en la ventana donde está la carpeta raíz (Abajo a la derecha de VSCode aparece un logo de una antena y un texto que dice 'Go Live', apretar ahí).

Una vez iniciado el Live Server se puede acceder a la pantalla mediante esta URL: http://127.0.0.1:5500/Frontend/

También se puede acceder sin Live Server, con la API levantada, desde: http://localhost:5089/

# Qué muestra el frontend
- Operaciones de las PyMEs: consulta a la vista 'VW_Operaciones_Pymes' (GET /api/Operaciones).
- Registrar tasa de interés: ejecuta el procedimiento almacenado 'SP_RegistrarTasaConValidacion' (POST /api/Tasas).
- Garantías: listado y alta (GET y POST /api/Garantias).
- Disponibilidad de garantías: listado y alta (GET y POST /api/GarantiasDisponibilidad).
- Auditoría de operaciones: listado de la tabla que completa el trigger (GET /api/OperacionesAuditoria).