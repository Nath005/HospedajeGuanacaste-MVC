/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/


using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace HospedajeGuanacaste.Datos
{
    //clase para poder manejar la conexión con sql server
    public static class ConexionBD
    {
        //es la cadena para conectar al servidor local sqlexpress y a la base de datos del proyecto
        private const string CadenaConexion =
            "Server=.\\SQLEXPRESS;Database=HospedajeGuanacasteDatos;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connect Timeout=30;";

        //se crea y retorna la conexión cuando se ocupe hacer consultas
        public static SqlConnection CrearConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}