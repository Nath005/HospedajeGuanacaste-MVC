/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/

using HospedajeGuanacaste.Datos.AccesoDatos;
using HospedajeGuanacaste.Datos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospedajeGuanacaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionesController : ControllerBase
    {
        //tipos 
        private static readonly string[] TiposPermitidos =
        {
            "Start Junior", "Start Vista al Mar", "Master Start"
        };

        //para comunicarse con la capa de datos
        private readonly HabitacionDatos habitacionDatos;
        private readonly ReservacionDatos reservacionDatos;

        public HabitacionesController()
        {
            habitacionDatos = new HabitacionDatos();
            reservacionDatos = new ReservacionDatos();
        }

        //se obtiene la lista de todas las habitaciones
        [HttpGet]
        public ActionResult<List<Habitacion>> ObtenerTodos()
        {
            return Ok(habitacionDatos.ObtenerTodos());
        }

        //se busca una habitacion por su numero
        [HttpGet("buscar/{numHabitacion}")]
        public ActionResult<Habitacion> Buscar(int numHabitacion)
        {
            Habitacion habitacion = habitacionDatos.ObtenerPorNumero(numHabitacion);

            if (habitacion == null)
            {
                return NotFound("No se encontró una habitación con ese número.");
            }

            return Ok(habitacion);
        }

        //se inserta la habitacion validando campos obligatorios
        [HttpPost]
        public ActionResult Insertar(Habitacion habitacion)
        {
            Normalizar(habitacion);

            string mensajeValidacion = ValidarHabitacion(habitacion);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            if (habitacionDatos.ObtenerPorNumero(habitacion.NumHabitacion) != null)
            {
                return BadRequest("Ya existe una habitación con ese número.");
            }

            try
            {
                bool insertado = habitacionDatos.Insertar(habitacion);

                if (!insertado)
                {
                    return BadRequest("No fue posible guardar la habitación.");
                }

                return CreatedAtAction(
                    nameof(Buscar),
                    new { numHabitacion = habitacion.NumHabitacion },
                    habitacion
                );
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible guardar la habitación. Verifique los datos ingresados.");
            }
        }

        //se actualizan los datos de la habitación
        [HttpPut("{numHabitacion}")]
        public ActionResult Actualizar(int numHabitacion, Habitacion habitacion)
        {
            if (numHabitacion != habitacion.NumHabitacion)
            {
                return BadRequest("El número de habitación de la ruta no coincide con el enviado.");
            }

            Normalizar(habitacion);

            string mensajeValidacion = ValidarHabitacion(habitacion);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            if (habitacionDatos.ObtenerPorNumero(numHabitacion) == null)
            {
                return NotFound("No se encontró la habitación que desea actualizar.");
            }

            try
            {
                bool actualizado = habitacionDatos.Actualizar(habitacion);

                if (!actualizado)
                {
                    return BadRequest("No fue posible actualizar la habitación.");
                }

                return Ok("Habitación actualizada correctamente.");
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible actualizar la habitación. Verifique los datos ingresados.");
            }
        }

        //se borra la habitación solo si no esta en una reservación
        [HttpDelete("{numHabitacion}")]
        public ActionResult Eliminar(int numHabitacion)
        {
            if (habitacionDatos.ObtenerPorNumero(numHabitacion) == null)
            {
                return NotFound("No se encontró la habitación que desea eliminar.");
            }

            //regla de integridad: no se borra una habitación asociada a una reservación
            if (reservacionDatos.ExisteReservacionDeHabitacion(numHabitacion))
            {
                return Conflict("No se puede eliminar la habitación porque tiene reservaciones registradas.");
            }

            try
            {
                bool eliminado = habitacionDatos.Eliminar(numHabitacion);

                if (!eliminado)
                {
                    return BadRequest("No fue posible eliminar la habitación.");
                }

                return Ok("Habitación eliminada correctamente.");
            }
            catch (SqlException)
            {
                //la llave foránea de la base de datos también protege la relación
                return Conflict("No se puede eliminar esta habitación porque tiene reservaciones relacionadas.");
            }
        }

        //se limpian espacios y se evitan valores nulos
        private void Normalizar(Habitacion habitacion)
        {
            habitacion.TipoHabitacion = (habitacion.TipoHabitacion ?? "").Trim();
            habitacion.PendienteMantenimiento = (habitacion.PendienteMantenimiento ?? "").Trim();

            //se guarda el tipo con el mismo formato del combo
            string tipo = TiposPermitidos.FirstOrDefault(t =>
                string.Equals(t, habitacion.TipoHabitacion, StringComparison.OrdinalIgnoreCase));

            if (tipo != null)
            {
                habitacion.TipoHabitacion = tipo;
            }
        }

        
        private string ValidarHabitacion(Habitacion habitacion)
        {
            if (habitacion.NumHabitacion < 1 || habitacion.NumHabitacion > 500)
            {
                return "El número de habitación debe estar entre 1 y 500.";
            }

            if (!TiposPermitidos.Contains(habitacion.TipoHabitacion))
            {
                return "Debe seleccionar un tipo de habitación válido (Start Junior, Start Vista al Mar o Master Start).";
            }

            if (habitacion.TarifaNoche < 50 || habitacion.TarifaNoche > 800)
            {
                return "La tarifa por noche debe estar entre 50 y 800 dólares.";
            }

            if (habitacion.PendienteMantenimiento.Length > 500)
            {
                return "Los pendientes de mantenimiento no pueden superar los 500 caracteres.";
            }

            return "";
        }
    }
}
