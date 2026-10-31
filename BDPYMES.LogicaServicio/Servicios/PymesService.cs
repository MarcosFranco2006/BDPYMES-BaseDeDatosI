using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using BDPYMES.LogicaServicio.Entidades;

using Microsoft.Data.SqlClient;

namespace BDPYMES.LogicaServicio.Servicios;

public class PymesService
{
    public string connectionString = "Server=.; Database=BDPYMES; Integrated Security=true; TrustServerCertificate=true;";

    public Pyme InsertarDato(string connString, Pyme pyme)
    {
        // Validación previa opcional en C# para asegurar los límites de la BD
        if (!string.IsNullOrEmpty(pyme.CUIT) && pyme.CUIT.Length > 13)
            throw new ArgumentException("El CUIT no puede superar los 13 caracteres.");

        if (!string.IsNullOrEmpty(pyme.Direccion) && pyme.Direccion.Length > 50)
            throw new ArgumentException("La dirección no puede superar los 50 caracteres.");

        if (!string.IsNullOrEmpty(pyme.Razon_Social) && pyme.Razon_Social.Length > 50)
            throw new ArgumentException("La razón social no puede superar los 50 caracteres.");

        string query = "INSERT INTO PYMES (CUIT, Direccion, Razon_Social) VALUES (@CUIT, @Direccion, @Razon_Social)";

        using (SqlConnection connection = new SqlConnection(connString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Usamos SqlParameter para tipar y acotar exactamente igual que en SQL Server
                command.Parameters.Add("@CUIT", SqlDbType.VarChar, 13).Value = (object)pyme.CUIT ?? DBNull.Value;
                command.Parameters.Add("@Direccion", SqlDbType.VarChar, 50).Value = (object)pyme.Direccion ?? DBNull.Value;
                command.Parameters.Add("@Razon_Social", SqlDbType.VarChar, 50).Value = (object)pyme.Razon_Social ?? DBNull.Value;

                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                    Console.WriteLine($"¡Registro insertado con éxito! Filas afectadas: {filasAfectadas}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al insertar: {ex.Message}");
                    throw; 
                }
            }
        }
        return pyme;
    }

    // Método para leer datos de la base de datos
    public List<Pyme> ConsultarDatos(string connString)
    {
        List<Pyme> pymes = new List<Pyme>();
        string query = "SELECT CUIT, Direccion, Razon_Social FROM PYMES";

        using (SqlConnection connection = new SqlConnection(connString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Lista de Pymes ---"); // Corregido el texto de la consola
                        while (reader.Read())
                        {
                            // Es buena práctica validar si la columna viene NULL de la base de datos
                            string cuit = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            string direccion = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            string razonSocial = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);

                            Pyme nuevaPyme = new Pyme 
                            { 
                                CUIT = cuit, 
                                Direccion = direccion, 
                                Razon_Social = razonSocial 
                            };
                            
                            pymes.Add(nuevaPyme);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al consultar: {ex.Message}");
                }
            }
        }
        return pymes;
    }
}
