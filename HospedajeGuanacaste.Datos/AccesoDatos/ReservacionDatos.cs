/*UNED II CUATRIMESTRE
* PROYECTO 3: Tercera etapa del Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Romelia Enríquez Cordero
* Fecha:05/08/2026*/

using HospedajeGuanacaste.Datos.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

#nullable disable//es para que no salgan avisos de nulos

namespace HospedajeGuanacaste.Datos.AccesoDatos
{
    //se realizan las operaciones de acceso a datos y reportes de reservaciones
    public class ReservacionDatos
    {
        //se obtienen todas las reservaciones registradas en la base de datos
        public List<Reservacion> ObtenerTodos()
        {
            List<Reservacion> reservaciones = new List<Reservacion>();

            string consulta = @"
                SELECT CodReservacion, IdentificacionCliente, NumHabitacion,
                       FechaReservacion, FechaIngreso, FechaSalida,
                       TarifaReservacion, SolicitudesEspeciales,
                       PorcentajeDescuento, MontoTotal, Estado
                FROM Reservaciones
                ORDER BY FechaIngreso";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        reservaciones.Add(CrearReservacion(lector));
                    }
                }
            }

            return reservaciones;
        }

        //se busca una reservación especifica por su código
        public Reservacion ObtenerPorCodigo(string codReservacion)
        {
            Reservacion reservacion = null;

            string consulta = @"
                SELECT CodReservacion, IdentificacionCliente, NumHabitacion,
                       FechaReservacion, FechaIngreso, FechaSalida,
                       TarifaReservacion, SolicitudesEspeciales,
                       PorcentajeDescuento, MontoTotal, Estado
                FROM Reservaciones
                WHERE CodReservacion = @CodReservacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@CodReservacion", codReservacion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        reservacion = CrearReservacion(lector);
                    }
                }
            }

            return reservacion;
        }

        //se inserta una nueva reservación en la base de datos
        public bool Insertar(Reservacion reservacion)
        {
            string consulta = @"
                INSERT INTO Reservaciones
                (CodReservacion, IdentificacionCliente, NumHabitacion,
                    FechaReservacion, FechaIngreso, FechaSalida,
                    TarifaReservacion, SolicitudesEspeciales,
                    PorcentajeDescuento, MontoTotal, Estado)
                VALUES
                (@CodReservacion, @IdentificacionCliente, @NumHabitacion,
                    @FechaReservacion, @FechaIngreso, @FechaSalida,
                    @TarifaReservacion, @SolicitudesEspeciales,
                    @PorcentajeDescuento, @MontoTotal, @Estado)";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                AgregarParametros(comando, reservacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se actualizan los datos de una reservación existente
        public bool Actualizar(Reservacion reservacion)
        {
            string consulta = @"
                UPDATE Reservaciones
                SET IdentificacionCliente = @IdentificacionCliente,
                    NumHabitacion = @NumHabitacion,
                    FechaReservacion = @FechaReservacion,
                    FechaIngreso = @FechaIngreso,
                    FechaSalida = @FechaSalida,
                    TarifaReservacion = @TarifaReservacion,
                    SolicitudesEspeciales = @SolicitudesEspeciales,
                    PorcentajeDescuento = @PorcentajeDescuento,
                    MontoTotal = @MontoTotal,
                    Estado = @Estado
                WHERE CodReservacion = @CodReservacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                AgregarParametros(comando, reservacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se elimina una reservación de la base de datos por su código
        public bool Eliminar(string codReservacion)
        {
            string consulta = @"
                DELETE FROM Reservaciones
                WHERE CodReservacion = @CodReservacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@CodReservacion", codReservacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se obtienen las reservaciones de un cliente (busqueda por identificacion del cliente)
        public List<Reservacion> ObtenerPorCliente(string identificacionCliente)
        {
            List<Reservacion> reservaciones = new List<Reservacion>();

            string consulta = @"
                SELECT CodReservacion, IdentificacionCliente, NumHabitacion,
                       FechaReservacion, FechaIngreso, FechaSalida,
                       TarifaReservacion, SolicitudesEspeciales,
                       PorcentajeDescuento, MontoTotal, Estado
                FROM Reservaciones
                WHERE IdentificacionCliente = @IdentificacionCliente
                ORDER BY FechaIngreso";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@IdentificacionCliente", identificacionCliente);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        reservaciones.Add(CrearReservacion(lector));
                    }
                }
            }

            return reservaciones;
        }

        //indica si la habitacion ya esta ocupada en el rango de fechas indicado.
        //dos rangos se traslapan cuando: ingresoExistente < salidaNueva Y salidaExistente > ingresoNuevo
        //(el dia de salida de una reservacion si puede ser el dia de ingreso de otra).
        //las reservaciones canceladas no bloquean la habitacion.
        //codExcluir sirve al editar, para no compararse contra si misma (se envia "" al crear)
        public bool ExisteTraslape(int numHabitacion, DateTime fechaIngreso,
                                   DateTime fechaSalida, string codExcluir)
        {
            string consulta = @"
                SELECT COUNT(1)
                FROM Reservaciones
                WHERE NumHabitacion = @NumHabitacion
                  AND Estado <> 'Cancelada'
                  AND CodReservacion <> @CodExcluir
                  AND FechaIngreso < @FechaSalida
                  AND FechaSalida > @FechaIngreso";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@NumHabitacion", numHabitacion);
                comando.Parameters.AddWithValue("@FechaIngreso", fechaIngreso.Date);
                comando.Parameters.AddWithValue("@FechaSalida", fechaSalida.Date);
                comando.Parameters.AddWithValue("@CodExcluir", codExcluir ?? "");

                conexion.Open();

                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        //indica si el cliente tiene al menos una reservacion registrada
        public bool ExisteReservacionDeCliente(string identificacionCliente)
        {
            string consulta = @"
                SELECT COUNT(1) FROM Reservaciones
                WHERE IdentificacionCliente = @IdentificacionCliente";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@IdentificacionCliente", identificacionCliente);

                conexion.Open();

                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        //indica si la habitacion tiene al menos una reservacion registrada
        public bool ExisteReservacionDeHabitacion(int numHabitacion)
        {
            string consulta = @"
                SELECT COUNT(1) FROM Reservaciones
                WHERE NumHabitacion = @NumHabitacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@NumHabitacion", numHabitacion);

                conexion.Open();

                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        //se obtiene el reporte de reservaciones para la semana entrante
        public List<ReporteReservacion> ObtenerReporteSemanaEntrante()
        {
            List<ReporteReservacion> reporte = new List<ReporteReservacion>();

            //CRITERIO DE "SEMANA ENTRANTE": la semana va de lunes a domingo.
            //la semana entrante es la que empieza el proximo lunes (el lunes siguiente a hoy;
            //si hoy es lunes, es el lunes dentro de 7 dias) y termina el domingo siguiente.
            //se incluyen las reservaciones cuya fecha de ingreso cae dentro de ese rango
            //(inicio incluido, fin excluido), sin importar su estado.
            DateTime hoy = DateTime.Today;
            int diasDesdeLunes = ((int)hoy.DayOfWeek + 6) % 7;//domingo será 6 y lunes será 0

            DateTime inicioSemanaEntrante = hoy.AddDays(7 - diasDesdeLunes);
            DateTime finSemanaEntrante = inicioSemanaEntrante.AddDays(7);

            string consulta = @"
                SELECT r.CodReservacion,
                       r.IdentificacionCliente,
                       c.Nombre + ' ' + c.PrimerApellido + ' ' +
                       ISNULL(c.SegundoApellido, '') AS NombreCliente,
                       r.NumHabitacion,
                       h.TipoHabitacion,
                       r.FechaReservacion,
                       r.FechaIngreso,
                       r.FechaSalida,
                       r.TarifaReservacion,
                       r.PorcentajeDescuento,
                       r.MontoTotal,
                       r.Estado
                FROM Reservaciones r
                INNER JOIN Clientes c
                    ON r.IdentificacionCliente = c.Identificacion
                INNER JOIN Habitaciones h
                    ON r.NumHabitacion = h.NumHabitacion
                WHERE r.FechaIngreso >= @InicioSemanaEntrante
                  AND r.FechaIngreso < @FinSemanaEntrante
                ORDER BY r.MontoTotal DESC";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue( "@InicioSemanaEntrante", inicioSemanaEntrante);

                comando.Parameters.AddWithValue( "@FinSemanaEntrante", finSemanaEntrante);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ReporteReservacion reservacion = new ReporteReservacion
                        {
                            CodReservacion = lector["CodReservacion"].ToString(),
                            IdentificacionCliente = lector["IdentificacionCliente"].ToString(),
                            NombreCliente = lector["NombreCliente"].ToString(),
                            NumHabitacion = Convert.ToInt32(lector["NumHabitacion"]),
                            TipoHabitacion = lector["TipoHabitacion"].ToString(),
                            FechaReservacion = Convert.ToDateTime(lector["FechaReservacion"]),
                            FechaIngreso = Convert.ToDateTime(lector["FechaIngreso"]),
                            FechaSalida = Convert.ToDateTime(lector["FechaSalida"]),
                            TarifaReservacion = Convert.ToDecimal(lector["TarifaReservacion"]),
                            PorcentajeDescuento = Convert.ToDecimal(lector["PorcentajeDescuento"]),
                            MontoTotal = Convert.ToDecimal(lector["MontoTotal"]),
                            Estado = lector["Estado"].ToString()
                        };

                        reporte.Add(reservacion);
                    }
                }
            }

            return reporte;
        }

        //se crea la reservación con los datos leídos
        private Reservacion CrearReservacion(SqlDataReader lector)
        {
            return new Reservacion
            {
                CodReservacion = lector["CodReservacion"].ToString(),
                IdentificacionCliente = lector["IdentificacionCliente"].ToString(),
                NumHabitacion = Convert.ToInt32(lector["NumHabitacion"]),
                FechaReservacion = Convert.ToDateTime(lector["FechaReservacion"]),
                FechaIngreso = Convert.ToDateTime(lector["FechaIngreso"]),
                FechaSalida = Convert.ToDateTime(lector["FechaSalida"]),
                TarifaReservacion = Convert.ToDecimal(lector["TarifaReservacion"]),
                SolicitudesEspeciales = lector["SolicitudesEspeciales"] == DBNull.Value ?
                 "" : lector["SolicitudesEspeciales"].ToString(),
                PorcentajeDescuento = Convert.ToDecimal(lector["PorcentajeDescuento"]),
                MontoTotal = Convert.ToDecimal(lector["MontoTotal"]),
                Estado = lector["Estado"].ToString()
            };
        }

        //se agregan los parámetros para las consultas sql de reservacion
        private void AgregarParametros(SqlCommand comando, Reservacion reservacion)
        {
            //si la fecha de reservación no se registra, se le asigna la fecha de hoy
            DateTime fechaReservacion = (reservacion.FechaReservacion == DateTime.MinValue)
                ? DateTime.Today
                : reservacion.FechaReservacion;

            comando.Parameters.AddWithValue("@CodReservacion", reservacion.CodReservacion);
            comando.Parameters.AddWithValue("@IdentificacionCliente", reservacion.IdentificacionCliente);
            comando.Parameters.AddWithValue("@NumHabitacion", reservacion.NumHabitacion);
            comando.Parameters.AddWithValue("@FechaReservacion", fechaReservacion);
            comando.Parameters.AddWithValue("@FechaIngreso", reservacion.FechaIngreso);
            comando.Parameters.AddWithValue("@FechaSalida", reservacion.FechaSalida);
            comando.Parameters.AddWithValue("@TarifaReservacion", reservacion.TarifaReservacion);
            comando.Parameters.AddWithValue("@SolicitudesEspeciales",
                string.IsNullOrWhiteSpace(reservacion.SolicitudesEspeciales) ? DBNull.Value : reservacion.SolicitudesEspeciales);
            comando.Parameters.AddWithValue("@PorcentajeDescuento", reservacion.PorcentajeDescuento);
            comando.Parameters.AddWithValue("@MontoTotal", reservacion.MontoTotal);
            comando.Parameters.AddWithValue("@Estado", reservacion.Estado);
        }
    }
}