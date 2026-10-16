/*UNED II CUATRIMESTRE
* PROYECTO 3: Tercera etapa del Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Romelia Enríquez Cordero
* Fecha:05/08/2026*/

using HospedajeGuanacaste.Datos.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
#nullable disable

namespace HospedajeGuanacaste.Datos.AccesoDatos
{
    //se realizan las operaciones de acceso a datos para los empleados
    public class EmpleadoDatos
    {
        //se obtienen todos los empleados registrados en la base de datos
        public List<Empleado> ObtenerTodos()
        {
            List<Empleado> empleados = new List<Empleado>();

            string consulta = @"
                SELECT Identificacion, TipoIdentificacion, Nombre,
                       PrimerApellido, SegundoApellido, FechaNacimiento,
                       SalarioMensual, FechaIngreso, Categoria, Provincia,
                       Canton, Distrito, DireccionExacta
                FROM Empleados
                ORDER BY PrimerApellido, Nombre";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Empleado empleado = new Empleado
                        {
                            Identificacion = lector["Identificacion"].ToString(),
                            TipoIdentificacion = lector["TipoIdentificacion"].ToString(),
                            Nombre = lector["Nombre"].ToString(),
                            PrimerApellido = lector["PrimerApellido"].ToString(),
                            SegundoApellido = lector["SegundoApellido"] == DBNull.Value ? ""
                                : lector["SegundoApellido"].ToString(),
                            FechaNacimiento = Convert.ToDateTime(lector["FechaNacimiento"]),
                            SalarioMensual = Convert.ToDecimal(lector["SalarioMensual"]),
                            FechaIngreso = Convert.ToDateTime(lector["FechaIngreso"]),
                            Categoria = lector["Categoria"].ToString(),
                            Provincia = lector["Provincia"].ToString(),
                            Canton = lector["Canton"].ToString(),
                            Distrito = lector["Distrito"].ToString(),
                            DireccionExacta = lector["DireccionExacta"].ToString()
                        };

                        empleados.Add(empleado);
                    }
                }
            }

            return empleados;
        }

        //se busca un empleado por su número de identificación
        public Empleado ObtenerPorIdentificacion(string identificacion)
        {
            Empleado empleado = null;

            string consulta = @"
                SELECT Identificacion, TipoIdentificacion, Nombre,
                       PrimerApellido, SegundoApellido, FechaNacimiento,
                       SalarioMensual, FechaIngreso, Categoria, Provincia,
                       Canton, Distrito, DireccionExacta
                FROM Empleados
                WHERE Identificacion = @Identificacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Identificacion", identificacion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        empleado = new Empleado
                        {
                            Identificacion = lector["Identificacion"].ToString(),
                            TipoIdentificacion = lector["TipoIdentificacion"].ToString(),
                            Nombre = lector["Nombre"].ToString(),
                            PrimerApellido = lector["PrimerApellido"].ToString(),
                            SegundoApellido = lector["SegundoApellido"] == DBNull.Value? ""
                                : lector["SegundoApellido"].ToString(),
                            FechaNacimiento = Convert.ToDateTime(lector["FechaNacimiento"]),
                            SalarioMensual = Convert.ToDecimal(lector["SalarioMensual"]),
                            FechaIngreso = Convert.ToDateTime(lector["FechaIngreso"]),
                            Categoria = lector["Categoria"].ToString(),
                            Provincia = lector["Provincia"].ToString(),
                            Canton = lector["Canton"].ToString(),
                            Distrito = lector["Distrito"].ToString(),
                            DireccionExacta = lector["DireccionExacta"].ToString()
                        };
                    }
                }
            }

            return empleado;
        }

        //se inserta un nuevo empleado en la base de datos
        public bool Insertar(Empleado empleado)
        {
            string consulta = @"
                INSERT INTO Empleados
                (
                    Identificacion, TipoIdentificacion, Nombre,
                    PrimerApellido, SegundoApellido, FechaNacimiento,
                    SalarioMensual, FechaIngreso, Categoria, Provincia,
                    Canton, Distrito, DireccionExacta
                )
                VALUES
                (
                    @Identificacion, @TipoIdentificacion, @Nombre,
                    @PrimerApellido, @SegundoApellido, @FechaNacimiento,
                    @SalarioMensual, @FechaIngreso, @Categoria, @Provincia,
                    @Canton, @Distrito, @DireccionExacta
                )";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                AgregarParametros(comando, empleado);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se actualizan los datos de un empleado existente
        public bool Actualizar(Empleado empleado)
        {
            string consulta = @"
                UPDATE Empleados
                SET TipoIdentificacion = @TipoIdentificacion,
                    Nombre = @Nombre,
                    PrimerApellido = @PrimerApellido,
                    SegundoApellido = @SegundoApellido,
                    FechaNacimiento = @FechaNacimiento,
                    SalarioMensual = @SalarioMensual,
                    FechaIngreso = @FechaIngreso,
                    Categoria = @Categoria,
                    Provincia = @Provincia,
                    Canton = @Canton,
                    Distrito = @Distrito,
                    DireccionExacta = @DireccionExacta
                WHERE Identificacion = @Identificacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                AgregarParametros(comando, empleado);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se elimina un empleado de la base de datos por su identificacion
        public bool Eliminar(string identificacion)
        {
            string consulta = @"
                DELETE FROM Empleados
                WHERE Identificacion = @Identificacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Identificacion", identificacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se agregan los parametros requeridos para las consultas sql
        private void AgregarParametros(SqlCommand comando, Empleado empleado)
        {
            comando.Parameters.AddWithValue("@Identificacion", empleado.Identificacion);
            comando.Parameters.AddWithValue("@TipoIdentificacion", empleado.TipoIdentificacion);
            comando.Parameters.AddWithValue("@Nombre", empleado.Nombre);
            comando.Parameters.AddWithValue("@PrimerApellido", empleado.PrimerApellido);
            comando.Parameters.AddWithValue("@SegundoApellido",
                string.IsNullOrWhiteSpace(empleado.SegundoApellido)? DBNull.Value
                    : empleado.SegundoApellido );
            comando.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
            comando.Parameters.AddWithValue("@SalarioMensual", empleado.SalarioMensual);
            comando.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);
            comando.Parameters.AddWithValue("@Categoria", empleado.Categoria);
            comando.Parameters.AddWithValue("@Provincia", empleado.Provincia);
            comando.Parameters.AddWithValue("@Canton", empleado.Canton);
            comando.Parameters.AddWithValue("@Distrito", empleado.Distrito);
            comando.Parameters.AddWithValue("@DireccionExacta", empleado.DireccionExacta);
        }
    }
}