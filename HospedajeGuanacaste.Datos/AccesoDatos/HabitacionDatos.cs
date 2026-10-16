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
    //se realizan las operaciones CRUD sobre la tabla de las habitaciones
    public class HabitacionDatos
    {
        //se obtienen todas las habitaciones registradas en la base de datos
        public List<Habitacion> ObtenerTodos()
        {
            List<Habitacion> habitaciones = new List<Habitacion>();

            string consulta = @"
                SELECT NumHabitacion, TipoHabitacion, TarifaNoche,
                       TvSatelital, PendienteMantenimiento
                FROM Habitaciones
                ORDER BY NumHabitacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Habitacion habitacion = new Habitacion
                        {
                            NumHabitacion = Convert.ToInt32(lector["NumHabitacion"]),
                            TipoHabitacion = lector["TipoHabitacion"].ToString(),
                            TarifaNoche = Convert.ToDecimal(lector["TarifaNoche"]),
                            TvSatelital = Convert.ToBoolean(lector["TvSatelital"]),
                            PendienteMantenimiento = lector["PendienteMantenimiento"] == DBNull.Value ? ""
                                : lector["PendienteMantenimiento"].ToString()
                        };
                        habitaciones.Add(habitacion);
                    }
                }
            }

            return habitaciones;
        }

        //se busca una habitacion especifica por su numero
        public Habitacion ObtenerPorNumero(int numHabitacion)
        {
            Habitacion habitacion = null;

            string consulta = @"
                SELECT NumHabitacion, TipoHabitacion, TarifaNoche,
                       TvSatelital, PendienteMantenimiento
                FROM Habitaciones
                WHERE NumHabitacion = @NumHabitacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@NumHabitacion", numHabitacion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        habitacion = new Habitacion
                        {
                            NumHabitacion = Convert.ToInt32(lector["NumHabitacion"]),
                            TipoHabitacion = lector["TipoHabitacion"].ToString(),
                            TarifaNoche = Convert.ToDecimal(lector["TarifaNoche"]),
                            TvSatelital = Convert.ToBoolean(lector["TvSatelital"]),
                            PendienteMantenimiento = lector["PendienteMantenimiento"] == DBNull.Value ? ""
                              : lector["PendienteMantenimiento"].ToString()
                        };
                    }
                }
            }

            return habitacion;
        }

        //se inserta una nueva habitacion en la base de datos
        public bool Insertar(Habitacion habitacion)
        {
            string consulta = @"
                INSERT INTO Habitaciones
                (
                    NumHabitacion,
                    TipoHabitacion,
                    TarifaNoche,
                    TvSatelital,
                    PendienteMantenimiento
                )
                VALUES
                (
                    @NumHabitacion,
                    @TipoHabitacion,
                    @TarifaNoche,
                    @TvSatelital,
                    @PendienteMantenimiento
                )";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                AgregarParametros(comando, habitacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se actualizan los datos de una habitacion existente
        public bool Actualizar(Habitacion habitacion)
        {
            string consulta = @"
                UPDATE Habitaciones
                SET TipoHabitacion = @TipoHabitacion,
                    TarifaNoche = @TarifaNoche,
                    TvSatelital = @TvSatelital,
                    PendienteMantenimiento = @PendienteMantenimiento
                WHERE NumHabitacion = @NumHabitacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                AgregarParametros(comando, habitacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se elimina una habitacion de la base de datos por su número
        public bool Eliminar(int numHabitacion)
        {
            string consulta = @"
                DELETE FROM Habitaciones
                WHERE NumHabitacion = @NumHabitacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@NumHabitacion", numHabitacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se agregan los parametros requeridos para las consultas sql
        private void AgregarParametros(SqlCommand comando, Habitacion habitacion)
        {
            comando.Parameters.AddWithValue("@NumHabitacion", habitacion.NumHabitacion);
            comando.Parameters.AddWithValue("@TipoHabitacion", habitacion.TipoHabitacion);
            comando.Parameters.AddWithValue("@TarifaNoche", habitacion.TarifaNoche);
            comando.Parameters.AddWithValue("@TvSatelital", habitacion.TvSatelital);
            comando.Parameters.AddWithValue("@PendienteMantenimiento",
                string.IsNullOrWhiteSpace(habitacion.PendienteMantenimiento)? DBNull.Value
                : habitacion.PendienteMantenimiento
            );
        }
    }
}