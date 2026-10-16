/* *UNED II CUATRIMESTRE
* PROYECTO 3: Tercera etapa del Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Romelia Enríquez Cordero
* Fecha:05/08/2026*/

using System;
using System.Collections.Generic;
using System.Text;

namespace HospedajeGuanacaste.Datos.Models
{
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
