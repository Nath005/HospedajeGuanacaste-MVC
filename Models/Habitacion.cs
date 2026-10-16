/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/

namespace HospedajeGuanacaste.Models
{
    //clase que representa la estructura de datos para una habitación en el sistema
    public class Habitacion
    {
        public int NumHabitacion { get; set; }
        public string TipoHabitacion { get; set; }
        public decimal TarifaNoche { get; set; }
        public bool TvSatelital { get; set; }
        public string PendienteMantenimiento { get; set; }
    }
}
