using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Data;
using BDPYMES.LogicaServicio.Entidades;
using Microsoft.Data.SqlClient;

namespace BDPYMES.LogicaServicio.Servicios;

public class PymesService
{
    private readonly string connectionString = "Server=.;Database=BDD-PYME;Integrated Security=true;TrustServerCertificate=true;";

    // VISTA
    public List<OperacionPyme> ConsultarOperaciones()
    {
        List<OperacionPyme> operaciones = new List<OperacionPyme>();

        const string query =
            "SELECT [ID-OPERACION], [CUIT-PYME], PyME_RazonSocial, MontoOperacion, Estado " +
            "FROM VW_Operaciones_Pymes";

        try
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                operaciones.Add(new OperacionPyme
                {
                    IdOperacion = reader.GetInt32(0),
                    CuitPyme = reader.GetInt32(1),
                    RazonSocial = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    MontoOperacion = reader.IsDBNull(3) ? 0.0m : reader.GetDecimal(3),
                    Estado = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al consultar la vista: {ex.Message}", ex);
        }

        return operaciones;
    }

    // PROCEDIMIENTO ALMACENADO
    public void RegistrarTasa(int idInstrumento, decimal tasaInteres)
    {
        if (idInstrumento <= 0)
            throw new ArgumentException("El ID del instrumento debe ser mayor a cero.");

        if (tasaInteres < 0 || tasaInteres > 999.999m)
            throw new ArgumentException("La tasa de interés debe estar entre 0 y 999,99.");

        try
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand("SP_RegistrarTasaConValidacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@IdInstrumento", SqlDbType.Int).Value = idInstrumento;

            SqlParameter parametroTasa = command.Parameters.Add("@TasaInteres", SqlDbType.Decimal);
            parametroTasa.Precision = 6;
            parametroTasa.Scale = 3;
            parametroTasa.Value = tasaInteres;

            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            throw new ArgumentException(ex.Message);
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al registrar la tasa: {ex.Message}", ex);
        }
    }

    // MÉTODOS GET Y POST PARA PROBAR CON SWAGGER

    public Pyme InsertarDatoPyme(Pyme pyme)
    {
        if (!string.IsNullOrEmpty(pyme.Direccion) && pyme.Direccion.Length > 32)
            throw new ArgumentException("La dirección no puede superar los 50 caracteres.");

        if (!string.IsNullOrEmpty(pyme.Razon_Social) && pyme.Razon_Social.Length > 32)
            throw new ArgumentException("La razón social no puede superar los 32 caracteres.");

        string query = "INSERT INTO PYMES ([CUIT-PYME], Direccion, RazonSocial) VALUES (@CUIT, @Direccion, @Razon_Social)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@CUIT", SqlDbType.Int).Value = (object)pyme.CUIT;
                command.Parameters.Add("@Direccion", SqlDbType.VarChar, 32).Value = (object)pyme.Direccion ?? DBNull.Value;
                command.Parameters.Add("@Razon_Social", SqlDbType.VarChar, 32).Value = (object)pyme.Razon_Social ?? DBNull.Value;

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
        string query = "SELECT [CUIT-PYME], Direccion, RazonSocial FROM PYMES";

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
                            int cuit = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
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

        string query = "INSERT INTO ALYCS ([ID-ALYCS], NombreALYC) VALUES (@ID, @Nombre)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)alyc.Id;
                command.Parameters.Add("@Nombre", SqlDbType.VarChar, 32).Value = (object)alyc.Nombre ?? DBNull.Value;
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
        string query = "SELECT [ID-ALYCS], NombreALYC FROM ALYCS";

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

        if (!string.IsNullOrEmpty(broker.Nombre) && broker.Nombre.Length > 32)
            throw new ArgumentException("El nombre no puede superar los 32 caracteres.");

        broker.Id = brokers.Count > 0 ? brokers.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO BROKERS ([ID-BROKER], NombreBroker) VALUES (@ID, @Nombre)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)broker.Id;
                command.Parameters.Add("@Nombre", SqlDbType.VarChar, 32).Value = (object)broker.Nombre ?? DBNull.Value;
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
        string query = "SELECT [ID-BROKER], NombreBroker FROM BROKERS";

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

        bool existePyme = pymes.Any(p => p.CUIT == garantia.CuitPyme);
        if (!existePyme)
            throw new ArgumentException($"No existe una Pyme cargada con el CUIT {garantia.CuitPyme}.");

        if (!string.IsNullOrEmpty(garantia.Descripcion) && garantia.Descripcion.Length > 50)
            throw new ArgumentException("La descripcion no puede superar los 50 caracteres.");

        if (garantia.Monto <= 0)
            throw new ArgumentException("El monto tiene que ser mayor a $0");

        garantia.Id = garantias.Count > 0 ? garantias.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO GARANTIAS ([ID-GARANTIA], [CUIT-PYME], DescripcionGarantia, MontoGarantia) VALUES (@ID, @CUIT_PYME, @Descripcion, @Monto)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)garantia.Id;
                command.Parameters.Add("@CUIT_PYME", SqlDbType.Int).Value = (object)garantia.CuitPyme;
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
        string query = "SELECT [ID-GARANTIA],  [CUIT-PYME], DescripcionGarantia, MontoGarantia FROM GARANTIAS";

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
                            int cuitPyme = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
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

        string query = "INSERT INTO SGRs ([ID-SGR], [Nombre-SGR]) VALUES (@ID, @Nombre)";

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
        string query = "SELECT [ID-SGR], [Nombre-SGR] FROM SGRs";

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
            throw new ArgumentException("El Tipo de instrumento no puede superar los 32 caracteres.");

        instrumento.Id = instrumentos.Count > 0 ? instrumentos.Max(x => x.Id) + 1 : 1;

        string query = "INSERT INTO INSTRUMENTOS ([ID-INSTRUMENTO], TipoInstrumento) VALUES (@ID, @Tipo_Instrumento)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = (object)instrumento.Id;
                command.Parameters.Add("@Tipo_Instrumento", SqlDbType.VarChar, 32).Value = (object)instrumento.TipoInstrumento ?? DBNull.Value;
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
        string query = "SELECT [ID-INSTRUMENTO], TipoInstrumento FROM INSTRUMENTOS";

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

        string query = "INSERT INTO [TAZA-DE-INTERES] ([ID-INSTRUMENTO], FechaUltimaActualizacion, TazaDeInteres) VALUES (@ID_INSTRUMENTO, @Ultima_Actualizacion, @Tasa_Interes)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID_INSTRUMENTO", SqlDbType.Int).Value = (object)tasaInteres.IdInstrumento;
                command.Parameters.Add("@Ultima_Actualizacion", SqlDbType.DateTime).Value = DateTime.Now;
                SqlParameter param = command.Parameters.Add("@Tasa_Interes", SqlDbType.Decimal);
                param.Precision = 5;
                param.Scale = 2;
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
        string query = "SELECT [ID-INSTRUMENTO], FechaUltimaActualizacion, TazaDeInteres FROM [TAZA-DE-INTERES]";

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

    public BrokerComision InsertarDatoBrokerComision(BrokerComision brokerComision)
    {
        List<BrokerComision> brokerComisiones = ConsultarDatosBrokerComisiones();
        List<Broker> brokers = ConsultarDatosBrokers();

        bool existeBroker = brokers.Any(b => b.Id == brokerComision.IdBroker);
        if (!existeBroker)
            throw new ArgumentException($"No existe un broker cargado con el ID {brokerComision.IdBroker}.");

        string query = "INSERT INTO [BROKER/COMISIONES] ([ID-BROKER], FechaUltimaActualizacion, PorcentajeComision) VALUES (@ID_BROKER, @Ultima_Actualizacion, @Porcentaje_Comision)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ID_BROKER", SqlDbType.Int).Value = (object)brokerComision.IdBroker;
                command.Parameters.Add("@Ultima_Actualizacion", SqlDbType.DateTime).Value = DateTime.Now;
                SqlParameter param = command.Parameters.Add("@Porcentaje_Comision", SqlDbType.Decimal);
                param.Precision = 5;
                param.Scale = 2;
                param.Value = (object)brokerComision.PorcentajeComision;

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
        return brokerComision;
    }

    public List<BrokerComision> ConsultarDatosBrokerComisiones()
    {
        List<BrokerComision> brokerComisiones = new List<BrokerComision>();
        string query = "SELECT [ID-BROKER], FechaUltimaActualizacion, PorcentajeComision FROM [BROKER/COMISIONES]";

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
                            int idBroker = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            DateTime ultimaActualizacion = reader.GetDateTime(1);
                            decimal porcentajeComision = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);

                            BrokerComision nuevoBrokerComision = new BrokerComision 
                            { 
                                IdBroker = idBroker,
                                UltimaActualizacion = ultimaActualizacion, 
                                PorcentajeComision = porcentajeComision, 
                            };
                            
                            brokerComisiones.Add(nuevoBrokerComision);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return brokerComisiones;
    }

    public GarantiaDisponibilidad InsertarDatoGarantiaDisponibilidad(GarantiaDisponibilidad disponibilidad)
    {
        if (disponibilidad.FechaHasta <= disponibilidad.FechaDesde)
            throw new ArgumentException("La fecha hasta tiene que ser posterior a la fecha desde.");
 
        List<Garantia> garantias = ConsultarDatosGarantias();
        int cantidadGarantias = garantias.Count(g => g.Id == disponibilidad.IdGarantia);
        if (cantidadGarantias == 0)
            throw new ArgumentException($"No existe una garantía con el ID {disponibilidad.IdGarantia}.");
 
        List<OperacionPyme> operaciones = ConsultarOperaciones();
        int cantidadOperaciones = operaciones.Count(o => o.IdOperacion == disponibilidad.IdOperacion);
        if (cantidadOperaciones == 0)
            throw new ArgumentException($"No existe una operación con el ID {disponibilidad.IdOperacion}.");
 
        string query = "INSERT INTO GARANTIASDISPONIBLIDAD ([ID-GARANTIA], FechaDesde, FechaHasta, [ID-OPERACION]) " +
                       "VALUES (@IdGarantia, @FechaDesde, @FechaHasta, @IdOperacion)";
 
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@IdGarantia", SqlDbType.Int).Value = disponibilidad.IdGarantia;
                command.Parameters.Add("@FechaDesde", SqlDbType.DateTime).Value = disponibilidad.FechaDesde;
                command.Parameters.Add("@FechaHasta", SqlDbType.DateTime).Value = disponibilidad.FechaHasta;
                command.Parameters.Add("@IdOperacion", SqlDbType.Int).Value = disponibilidad.IdOperacion;
 
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // 2627 es el número de error de SQL Server para clave duplicada
                    if (ex.Number == 2627)
                        throw new ArgumentException("Ya existe esa garantía con esas mismas fechas.");
 
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return disponibilidad;
    }
 
    public List<GarantiaDisponibilidad> ConsultarDatosGarantiasDisponibilidad()
    {
        List<GarantiaDisponibilidad> disponibilidades = new List<GarantiaDisponibilidad>();
        string query = "SELECT [ID-GARANTIA], FechaDesde, FechaHasta, [ID-OPERACION] FROM GARANTIASDISPONIBLIDAD";
 
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
                            GarantiaDisponibilidad nuevaDisponibilidad = new GarantiaDisponibilidad
                            {
                                IdGarantia = reader.GetInt32(0),
                                FechaDesde = reader.GetDateTime(1),
                                FechaHasta = reader.GetDateTime(2),
                                IdOperacion = reader.GetInt32(3)
                            };
 
                            disponibilidades.Add(nuevaDisponibilidad);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return disponibilidades;
    }

    public OperacionAuditoria InsertarDatoOperacionAuditoria(OperacionAuditoria operacionAuditoria)
    {
        if (operacionAuditoria == null)
            throw new ArgumentNullException(nameof(operacionAuditoria));
 
        List<OperacionPyme> operaciones = ConsultarOperaciones();
        int cantidadOperaciones = operaciones.Count(o => o.IdOperacion == operacionAuditoria.IdOperacion);
        if (cantidadOperaciones == 0)
            throw new ArgumentException($"No existe una operación con el ID {operacionAuditoria.IdOperacion}.");
 
        if (operacionAuditoria.MontoOperacion <= 0)
            throw new ArgumentException("El monto de la operación debe ser mayor a 0.");
 
        if (string.IsNullOrWhiteSpace(operacionAuditoria.TipoAccion))
            throw new ArgumentException("El tipo de acción no puede estar vacío.");
 
        if (operacionAuditoria.TipoAccion.Length > 50)
            throw new ArgumentException("El tipo de acción no puede superar los 50 caracteres.");
 
        operacionAuditoria.FechaOperacion = DateTime.Now;
 
        string query = "INSERT INTO OPERACIONES_AUDITORIA ([ID-OPERACION], FechaOperacion, MontoOperacion, TipoAccion) VALUES (@IdOperacion, @FechaOperacion, @MontoOperacion, @TipoAccion)";
 
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@IdOperacion", SqlDbType.Int).Value = operacionAuditoria.IdOperacion;
                command.Parameters.Add("@FechaOperacion", SqlDbType.DateTime).Value = operacionAuditoria.FechaOperacion;
                command.Parameters.Add("@MontoOperacion", SqlDbType.Money).Value = operacionAuditoria.MontoOperacion;
                command.Parameters.Add("@TipoAccion", SqlDbType.VarChar, 50).Value = operacionAuditoria.TipoAccion;
 
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al insertar: {ex.Message}");
                }
            }
        }
        return operacionAuditoria;
    }
 
    public List<OperacionAuditoria> ConsultarDatosOperacionesAuditoria()
    {
        List<OperacionAuditoria> operacionesAuditoria = new List<OperacionAuditoria>();
        string query = "SELECT [ID-OPERACION], FechaOperacion, MontoOperacion, TipoAccion FROM OPERACIONES_AUDITORIA";
 
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
                            OperacionAuditoria nuevaOperacionAuditoria = new OperacionAuditoria();
 
                            // Las columnas de esta tabla aceptan NULL, por eso se verifica cada una
                            if (!reader.IsDBNull(0))
                                nuevaOperacionAuditoria.IdOperacion = reader.GetInt32(0);
 
                            if (!reader.IsDBNull(1))
                                nuevaOperacionAuditoria.FechaOperacion = reader.GetDateTime(1);
 
                            if (!reader.IsDBNull(2))
                                nuevaOperacionAuditoria.MontoOperacion = reader.GetDecimal(2);
 
                            if (!reader.IsDBNull(3))
                                nuevaOperacionAuditoria.TipoAccion = reader.GetString(3);
 
                            operacionesAuditoria.Add(nuevaOperacionAuditoria);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al consultar: {ex.Message}");
                }
            }
        }
        return operacionesAuditoria;
    }

    public GarantiaAlyc InsertarDatoGarantiaAlyc(GarantiaAlyc garantiaAlyc)
    {
    if (garantiaAlyc == null)
        throw new ArgumentNullException(nameof(garantiaAlyc));

    List<Alyc> alycs = ConsultarDatosAlycs();
    if (!alycs.Any(a => a.Id == garantiaAlyc.IdAlyc))
        throw new ArgumentException($"No existe una ALYC con el ID {garantiaAlyc.IdAlyc}.");

    List<Garantia> garantias = ConsultarDatosGarantias();
    if (!garantias.Any(g => g.Id == garantiaAlyc.IdGarantia))
        throw new ArgumentException($"No existe una garantía con el ID {garantiaAlyc.IdGarantia}.");

    string query = "INSERT INTO [GARANTIAS/ALYCs] ([[ID-ALYC], [ID-GARANTIA]) " +
                   "VALUES (@IdAlyc, @IdGarantia)";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add("@IdAlyc", SqlDbType.Int).Value = garantiaAlyc.IdAlyc;
            command.Parameters.Add("@IdGarantia", SqlDbType.Int).Value = garantiaAlyc.IdGarantia;

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    throw new ArgumentException("Esa garantía ya está asociada a esa ALYC.");

                throw new Exception($"Error al insertar: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar: {ex.Message}");
            }
        }
    }
    return garantiaAlyc;
}

public List<GarantiaAlyc> ConsultarDatosGarantiasAlycs()
    {
    List<GarantiaAlyc> garantiasAlycs = new List<GarantiaAlyc>();
    string query = "SELECT [[ID-ALYC], [ID-GARANTIA] FROM [GARANTIAS/ALYCs]";

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
                        garantiasAlycs.Add(new GarantiaAlyc
                        {
                            IdAlyc = reader.GetInt32(0),
                            IdGarantia = reader.GetInt32(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar: {ex.Message}");
            }
        }
    }
    return garantiasAlycs;
    }
}

