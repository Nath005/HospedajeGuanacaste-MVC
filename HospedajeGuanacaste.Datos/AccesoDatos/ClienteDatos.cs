/* *UNED II CUATRIMESTRE
* PROYECTO 3: Tercera etapa del Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Romelia Enríquez Cordero
* Fecha:05/08/2026*/

using HospedajeGuanacaste.Datos.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable//es para que no salgan avisos de nulos

namespace HospedajeGuanacaste.Datos.AccesoDatos
{
    //se hacen las operaciones CRUD sobre la tabla de clientes
    public class ClienteDatos
    {
        //se devuelve todos los clientes registrados en la base de datos
        public List<Cliente> ObtenerTodos()
        {
            List<Cliente> clientes = new List<Cliente>();

            string consulta = @"
                SELECT Identificacion, TipoIdentificacion, Nombre,
                       PrimerApellido, SegundoApellido, FechaNacimiento
                FROM Clientes
                ORDER BY PrimerApellido, Nombre";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Cliente cliente = new Cliente
                        {
                            Identificacion = lector["Identificacion"].ToString(),
                            TipoIdentificacion = lector["TipoIdentificacion"].ToString(),
                            Nombre = lector["Nombre"].ToString(),
                            PrimerApellido = lector["PrimerApellido"].ToString(),
                            SegundoApellido = lector["SegundoApellido"] == DBNull.Value? "" 
                            : lector["SegundoApellido"].ToString(),
                            FechaNacimiento = Convert.ToDateTime(lector["FechaNacimiento"])
                        };

                        clientes.Add(cliente);
                    }
                }
            }

            return clientes;
        }

        //se busca un cliente por el número de identificación
        public Cliente ObtenerPorIdentificacion(string identificacion)
        {
            Cliente cliente = null;

            string consulta = @"
                SELECT Identificacion, TipoIdentificacion, Nombre,
                       PrimerApellido, SegundoApellido, FechaNacimiento
                FROM Clientes
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
                        cliente = new Cliente
                        {
                            Identificacion = lector["Identificacion"].ToString(),
                            TipoIdentificacion = lector["TipoIdentificacion"].ToString(),
                            Nombre = lector["Nombre"].ToString(),
                            PrimerApellido = lector["PrimerApellido"].ToString(),
                            SegundoApellido = lector["SegundoApellido"] == DBNull.Value? "" : lector["SegundoApellido"].ToString(),
                            FechaNacimiento = Convert.ToDateTime(lector["FechaNacimiento"])
                        };
                    }
                }
            }

            return cliente;
        }

        //se inserta un nuevo cliente
        public bool Insertar(Cliente cliente)
        {
            string consulta = @"
                INSERT INTO Clientes
                (
                    Identificacion,
                    TipoIdentificacion,
                    Nombre,
                    PrimerApellido,
                    SegundoApellido,
                    FechaNacimiento
                )
                VALUES
                (
                    @Identificacion,
                    @TipoIdentificacion,
                    @Nombre,
                    @PrimerApellido,
                    @SegundoApellido,
                    @FechaNacimiento
                )";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@Identificacion", cliente.Identificacion);
                comando.Parameters.AddWithValue("@TipoIdentificacion", cliente.TipoIdentificacion);
                comando.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                comando.Parameters.AddWithValue("@PrimerApellido", cliente.PrimerApellido);
                comando.Parameters.AddWithValue("@SegundoApellido",
                    string.IsNullOrWhiteSpace(cliente.SegundoApellido) 
                    ? DBNull.Value: cliente.SegundoApellido );
                comando.Parameters.AddWithValue("@FechaNacimiento", cliente.FechaNacimiento);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se va a actualizar los datos de un cliente existente
        public bool Actualizar(Cliente cliente)
        {
            string consulta = @"
                UPDATE Clientes
                SET TipoIdentificacion = @TipoIdentificacion,
                    Nombre = @Nombre,
                    PrimerApellido = @PrimerApellido,
                    SegundoApellido = @SegundoApellido,
                    FechaNacimiento = @FechaNacimiento
                WHERE Identificacion = @Identificacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@Identificacion", cliente.Identificacion);
                comando.Parameters.AddWithValue("@TipoIdentificacion", cliente.TipoIdentificacion);
                comando.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                comando.Parameters.AddWithValue("@PrimerApellido", cliente.PrimerApellido);
                comando.Parameters.AddWithValue("@SegundoApellido",
                    string.IsNullOrWhiteSpace(cliente.SegundoApellido) ? DBNull.Value: cliente.SegundoApellido);
                comando.Parameters.AddWithValue("@FechaNacimiento", cliente.FechaNacimiento);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        //se elimina un cliente por su identificación
        public bool Eliminar(string identificacion)
        {
            string consulta = @"
                DELETE FROM Clientes
                WHERE Identificacion = @Identificacion";

            using (SqlConnection conexion = ConexionBD.CrearConexion())
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Identificacion", identificacion);

                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }
    }
}
