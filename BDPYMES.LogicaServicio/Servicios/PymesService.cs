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

    public Pyme InsertarDatoPyme(Pyme pyme)
    {
        if (!string.IsNullOrEmpty(pyme.CUIT) && pyme.CUIT.Length > 13)
            throw new ArgumentException("El CUIT no puede superar los 13 caracteres.");

        if (!string.IsNullOrEmpty(pyme.Direccion) && pyme.Direccion.Length > 50)
            throw new ArgumentException("La dirección no puede superar los 50 caracteres.");

        if (!string.IsNullOrEmpty(pyme.Razon_Social) && pyme.Razon_Social.Length > 50)
            throw new ArgumentException("La razón social no puede superar los 50 caracteres.");

        string query = "INSERT INTO PYMES (CUIT, Direccion, Razon_Social) VALUES (@CUIT, @Direccion, @Razon_Social)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@CUIT", SqlDbType.VarChar, 13).Value = (object)pyme.CUIT;
                command.Parameters.Add("@Direccion", SqlDbType.VarChar, 50).Value = (object)pyme.Direccion ?? DBNull.Value;
                command.Parameters.Add("@Razon_Social", SqlDbType.VarChar, 50).Value = (object)pyme.Razon_Social ?? DBNull.Value;

                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return pyme;
    }

    public List<Pyme> ConsultarDatosPymes()
    {
        List<Pyme> pymes = new List<Pyme>();
        string query = "SELECT CUIT, Direccion, Razon_Social FROM PYMES";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return pymes;
    }

    public Alyc InsertarDatoAlyc(Alyc alyc)
    {
        List<Alyc> alycs = ConsultarDatosAlycs();

        if (!string.IsNullOrEmpty(alyc.Nombre) && alyc.Nombre.Length > 50)
            throw new ArgumentException("El nombre no puede superar los 50 caracteres.");

        alyc.Id = alycs.Count > 0 ? alycs.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO ALYCS (ID, Nombre) VALUES (@ID, @Nombre)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)alyc.Id;
                command.Parameters.Add("@Nombre", SqlDbType.VarChar, 50).Value = (object)alyc.Nombre ?? DBNull.Value;
                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return alyc;
    }

    public List<Alyc> ConsultarDatosAlycs()
    {
        List<Alyc> alycs = new List<Alyc>();
        string query = "SELECT ID, Nombre FROM ALYCS";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Es buena práctica validar si la columna viene NULL de la base de datos
                            int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            string nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                            Alyc nuevaAlyc = new Alyc 
                            { 
                                Id = id, 
                                Nombre = nombre, 
                            };
                            
                            alycs.Add(nuevaAlyc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return alycs;
    }

    public Broker InsertarDatoBroker(Broker broker)
    {
        List<Broker> brokers = ConsultarDatosBrokers();

        if (!string.IsNullOrEmpty(broker.Nombre) && broker.Nombre.Length > 50)
            throw new ArgumentException("El nombre no puede superar los 50 caracteres.");

        broker.Id = brokers.Count > 0 ? brokers.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO BROKERS (ID, Nombre) VALUES (@ID, @Nombre)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)broker.Id;
                command.Parameters.Add("@Nombre", SqlDbType.VarChar, 50).Value = (object)broker.Nombre ?? DBNull.Value;
                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return broker;
    }

    public List<Broker> ConsultarDatosBrokers()
    {
        List<Broker> brokers = new List<Broker>();
        string query = "SELECT ID, Nombre FROM BROKERS";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            string nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                            Broker nuevoBroker = new Broker 
                            { 
                                Id = id, 
                                Nombre = nombre, 
                            };
                            
                            brokers.Add(nuevoBroker);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return brokers;
    }

     public Garantia InsertarDatosGarantia(Garantia garantia)
    {
        List<Garantia> garantias = ConsultarDatosGarantias();
        List<Pyme> pymes = ConsultarDatosPymes();

        if (!string.IsNullOrEmpty(garantia.CuitPyme) && garantia.CuitPyme.Length > 13)
            throw new ArgumentException("El CUIT de la Pyme no puede superar los 13 caracteres.");

        bool existePyme = pymes.Any(p => p.CUIT == garantia.CuitPyme);
        if (!existePyme)
            throw new ArgumentException($"No existe una Pyme cargada con el CUIT {garantia.CuitPyme}.");

        if (!string.IsNullOrEmpty(garantia.Descripcion) && garantia.Descripcion.Length > 50)
            throw new ArgumentException("La descripcion no puede superar los 50 caracteres.");

        if (garantia.Monto <= 0)
            throw new ArgumentException("El monto tiene que ser mayor a $0");

        garantia.Id = garantias.Count > 0 ? garantias.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO GARANTIAS (ID, CUIT_PYME, Descripcion, Monto) VALUES (@ID, @CUIT_PYME, @Descripcion, @Monto)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)garantia.Id;
                command.Parameters.Add("@CUIT_PYME", SqlDbType.VarChar, 13).Value = (object)garantia.CuitPyme;
                command.Parameters.Add("@Descripcion", SqlDbType.VarChar, 50).Value = (object)garantia.Descripcion ?? DBNull.Value;
                command.Parameters.Add("@Monto", SqlDbType.Money).Value = (object)garantia.Monto;

                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return garantia;
    }

    public List<Garantia> ConsultarDatosGarantias()
    {
        List<Garantia> garantias = new List<Garantia>();
        string query = "SELECT ID, CUIT_PYME, Descripcion, Monto FROM GARANTIAS";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            string cuitPyme = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            string descripcion = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            decimal monto = reader.IsDBNull(3) ? 0.00m : reader.GetDecimal(3);

                            Garantia nuevaGarantia = new Garantia 
                            { 
                                Id = id, 
                                CuitPyme = cuitPyme,
                                Descripcion = descripcion,
                                Monto = monto 
                            };
                            
                            garantias.Add(nuevaGarantia);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return garantias;
    }

    public Sgr InsertarDatoSgr(Sgr sgr)
    {
        List<Sgr> sgrs = ConsultarDatosSgrs();

        if (!string.IsNullOrEmpty(sgr.Nombre) && sgr.Nombre.Length > 50)
            throw new ArgumentException("El nombre no puede superar los 50 caracteres.");

        sgr.Id = sgrs.Count > 0 ? sgrs.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO SGRs (ID, Nombre) VALUES (@ID, @Nombre)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)sgr.Id;
                command.Parameters.Add("@Nombre", SqlDbType.VarChar, 50).Value = (object)sgr.Nombre ?? DBNull.Value;
                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return sgr;
    }

    public List<Sgr> ConsultarDatosSgrs()
    {
        List<Sgr> sgrs = new List<Sgr>();
        string query = "SELECT ID, Nombre FROM SGRs";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            string nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                            Sgr nuevoSgr = new Sgr 
                            { 
                                Id = id, 
                                Nombre = nombre, 
                            };
                            
                            sgrs.Add(nuevoSgr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return sgrs;
    }

    public Instrumento InsertarDatoInstrumento(Instrumento instrumento)
    {
        List<Instrumento> instrumentos = ConsultarDatosInstrumentos();

        if (!string.IsNullOrEmpty(instrumento.TipoInstrumento) && instrumento.TipoInstrumento.Length > 50)
            throw new ArgumentException("El Tipo de instrumento no puede superar los 50 caracteres.");

        instrumento.Id = instrumentos.Count > 0 ? instrumentos.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO INSTRUMENTOS (ID, Tipo_Instrumento) VALUES (@ID, @Tipo_Instrumento)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)instrumento.Id;
                command.Parameters.Add("@Tipo_Instrumento", SqlDbType.VarChar, 50).Value = (object)instrumento.TipoInstrumento ?? DBNull.Value;
                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return instrumento;
    }

    public List<Instrumento> ConsultarDatosInstrumentos()
    {
        List<Instrumento> instrumentos = new List<Instrumento>();
        string query = "SELECT ID, Tipo_Instrumento FROM INSTRUMENTOS";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            string tipoInstrumento = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                            Instrumento nuevoInstrumento = new Instrumento 
                            { 
                                Id = id, 
                                TipoInstrumento = tipoInstrumento, 
                            };
                            
                            instrumentos.Add(nuevoInstrumento);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return instrumentos;
    }

    public TasaInteres InsertarDatoTasaInteres(TasaInteres tasaInteres)
    {
        List<TasaInteres> tasasInteres = ConsultarDatosTasasInteres();
        List<Instrumento> instrumentos = ConsultarDatosInstrumentos();

        bool existeInstrumento = instrumentos.Any(i => i.Id == tasaInteres.IdInstrumento);
        if (!existeInstrumento)
            throw new ArgumentException($"No hay cargado un instrumento con el ID {tasaInteres.IdInstrumento}.");

        string query = "INSERT INTO [TASAS/INTERES] (ID_INSTRUMENTO, Ultima_Actualizacion, Tasa_Interes) VALUES (@ID_INSTRUMENTO, @Ultima_Actualizacion, @Tasa_Interes)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID_INSTRUMENTO", SqlDbType.Int).Value = (object)tasaInteres.IdInstrumento;
                command.Parameters.Add("@Ultima_Actualizacion", SqlDbType.DateTime).Value = DateTime.Now;
                SqlParameter param = command.Parameters.Add("@Tasa_Interes", SqlDbType.Decimal);
                param.Precision = 6;
                param.Scale = 3;
                param.Value = (object)tasaInteres.TasaDeInteres; 

                try
                {
                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return tasaInteres;
    }

    public List<TasaInteres> ConsultarDatosTasasInteres()
    {
        List<TasaInteres> tasasInteres = new List<TasaInteres>();
        string query = "SELECT ID_Instrumento, Ultima_Actualizacion, Tasa_Interes FROM [TASAS/INTERES]";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idInstrumento = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            DateTime ultimaActualizacion = reader.GetDateTime(1);
                            decimal tasaDeInteres = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);

                            TasaInteres nuevaTasaInteres = new TasaInteres 
                            { 
                                IdInstrumento = idInstrumento, 
                                UltimaActualizacion = ultimaActualizacion,
                                TasaDeInteres =  tasaDeInteres
                            };
                            
                            tasasInteres.Add(nuevaTasaInteres);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return tasasInteres;
    }
}
