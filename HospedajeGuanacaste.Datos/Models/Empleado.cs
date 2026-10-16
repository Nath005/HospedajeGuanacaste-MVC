/* *UNED II CUATRIMESTRE
* PROYECTO 3: Tercera etapa del Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Romelia Enríquez Cordero
* Fecha:05/08/2026*/

using System;
using System.Collections.Generic;
using System.Text;

namespace HospedajeGuanacaste.Datos.Models
{
    public class Empleado
    {
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
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
