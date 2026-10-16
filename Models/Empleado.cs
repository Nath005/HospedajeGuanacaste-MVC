/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/
namespace HospedajeGuanacaste.Models
{
    //clase que representa la estructura de datos para un empleado en el sistema
    public class Empleado
    {
        public string TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public decimal SalarioMensual { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Categoria { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string DireccionExacta { get; set; }
    }
}
