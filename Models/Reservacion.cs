/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/

namespace HospedajeGuanacaste.Models
{
    //clase que representa la estructura de datos para una reservación en el sistema
    public class Reservacion
    {
        public string CodReservacion { get; set; }
        public string IdentificacionCliente { get; set; }
        public int NumHabitacion { get; set; }
        public DateTime FechaReservacion { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal TarifaReservacion { get; set; }
        public string SolicitudesEspeciales { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; }
    }
}