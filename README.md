# BDPYMES

Backend y frontend hechos para comunicarse con la base de datos BDPYMES, para el proyecto final de Base de Datos I.

Alumnos: Fagiano Bautista, Franco Marcos, Nuñez Marko, Salusso Gabriel

# Clonar el repositorio y entrar a la carpeta raíz
En la terminal de VSCode escribir el siguiente comando 'git clone https://github.com/MarcosFranco2006/BDPYMES-BaseDeDatosI', apretar ENTER y acto seguido escribir 'cd BDPYMES-BaseDeDatosI' y apretar ENTER para acceder a la carpeta raíz del proyecto.

# Correr Test unitario (Aún no implementados)
-Desde la terminal:

Abrir una terminal desde la carpeta raíz en donde se va a escribir el comando 'cd BDPYMES.Tests', apretar ENTER y luego escribir 'dotnet test' y apretar ENTER.

-Desde los archivos en VSCode:

Se debe ir a la siguiente ubicación 'BDPYMES.Tests/PymesServiceTests.cs' apretar CLICK DERECHO en 'PymesServiceTests.cs' y seleccionar la opción de run test.

# Levantar APIs
-Desde la terminal:

En la terminal abierta desde la carpeta raíz poner: 'cd BDPYMES.Api' darle a ENTER, luego poner en la misma terminal 'dotnet run', darle a la combinación 'CTRL + CLICK' al puerto aclarado como 'http://localhost:5089/' y agregar 'swagger' al final de la URL para probar los endpoints.

La URL final quedaría: http://localhost:5089/swagger/index.html

# Levantar Frontend
Una vez levantada la API, se debe iniciar el Live Server en la ventana donde está la carpeta raíz (Abajo a la derecha de VSCode aparece un logo de una antena y un texto que dice 'Go Live', apretar ahí).

Una vez iniciado el Live Server se puede acceder a la pantalla mediante esta URL: http://127.0.0.1:5500/Frontend/
