/* *UNED II CUATRIMESTRE
* PROYECTO 3: Tercera etapa del Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Romelia Enríquez Cordero
* Fecha:05/08/2026*/

using System;
using System.Collections.Generic;
using System.Text;

namespace HospedajeGuanacaste.Datos.Models
{
    public class Habitacion
    {
        public int NumHabitacion { get; set; }
        public string TipoHabitacion { get; set; }
        public decimal TarifaNoche { get; set; }
        public bool TvSatelital { get; set; }
        public string PendienteMantenimiento { get; set; }
    }
}
