/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/

using System.Text.RegularExpressions;

namespace HospedajeGuanacaste.API.Validaciones
{
    //reglas de validación del Proyecto 1 que se reutilizan en los servicios (clientes y empleados)
    public static class ValidacionesComunes
    {
        //cédula con 9 dígitos con guiones por ejemplo 1-1111-1111
        private static readonly Regex PatronCedula = new Regex(@"^\d-\d{4}-\d{4}$");

        //dimex con 12 dígitos numéricos
        private static readonly Regex PatronDimex = new Regex(@"^\d{12}$");

        //pasaporte con alfanumérico de 1 a 50 caracteres
        private static readonly Regex PatronPasaporte = new Regex(@"^[A-Za-z0-9]{1,50}$");

        //letras, números, espacios y algunos signos 
        private static readonly Regex PatronTexto = new Regex(@"^[\p{L}\p{N} .'\-]+$");

        //`para aceptar "Cédula", "DIMEX", "Pasaporte"
        public static string NormalizarTipo(string tipo)
        {
            string t = (tipo ?? "").Trim().ToLowerInvariant();

            if (t == "cedula" || t == "cédula") return "Cedula";
            if (t == "dimex") return "DIMEX";
            if (t == "pasaporte") return "Pasaporte";

            return "";
        }

        //valida el tipo de identificación y el formato del número
        public static string ValidarIdentificacion(string tipo, string identificacion, bool permitePasaporte)
        {
            string tipoNormalizado = NormalizarTipo(tipo);
            string id = (identificacion ?? "").Trim();

            if (tipoNormalizado == "")//devuelve "" si todo está bien
            {
                return "Debe seleccionar un tipo de identificación válido.";
            }

            if (tipoNormalizado == "Pasaporte" && !permitePasaporte)
            {
                return "Los empleados solo pueden identificarse con cédula o DIMEX.";
            }

            if (id == "")
            {
                return "La identificación es obligatoria.";
            }

            if (tipoNormalizado == "Cedula" && !PatronCedula.IsMatch(id))
            {
                return "La cédula debe tener el formato 1-1111-0909.";
            }

            if (tipoNormalizado == "DIMEX" && !PatronDimex.IsMatch(id))
            {
                return "El DIMEX debe tener exactamente 12 dígitos numéricos.";
            }

            if (tipoNormalizado == "Pasaporte" && !PatronPasaporte.IsMatch(id))
            {
                return "El pasaporte debe ser alfanumérico (de 1 a 50 caracteres).";
            }

            return "";
        }

        //valida un texto alfanumérico con longitud mínima y máxima
        public static string ValidarTexto(string valor, string nombreCampo, int minimo, int maximo)
        {
            string texto = (valor ?? "").Trim();

            if (texto.Length < minimo || texto.Length > maximo)
            {
                return "El campo " + nombreCampo + " debe tener entre " + minimo +
                       " y " + maximo + " caracteres.";
            }

            if (!PatronTexto.IsMatch(texto))
            {
                return "El campo " + nombreCampo + " solo admite letras, números y espacios.";
            }

            return "";
        }

        //valida nombre y apellidos 
        public static string ValidarNombreCompleto(string nombre, string primerApellido, string segundoApellido)
        {
            string mensaje = ValidarTexto(nombre, "Nombre", 3, 50);
            if (mensaje != "") return mensaje;

            mensaje = ValidarTexto(primerApellido, "Primer apellido", 3, 75);
            if (mensaje != "") return mensaje;

            return ValidarTexto(segundoApellido, "Segundo apellido", 3, 75);
        }

        //la fecha de nacimiento debe existir y no puede estar en el futuro
        public static string ValidarFechaNacimiento(DateTime fecha)
        {
            if (fecha < new DateTime(1900, 1, 1) || fecha.Date > DateTime.Today)
            {
                return "Debe indicar una fecha de nacimiento válida (no puede ser futura).";
            }

            return "";
        }
    }
}
