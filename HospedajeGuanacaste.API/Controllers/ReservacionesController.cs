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
    public class ReservacionesController : ControllerBase
    {
        //estados
        private static readonly string[] EstadosPermitidos =
        {
            "Reservada", "Cancelada", "En proceso", "Finalizada"
        };

        private const decimal PorcentajeIva = 0.13m;

        private readonly ReservacionDatos reservacionDatos;
        private readonly ClienteDatos clienteDatos;
        private readonly HabitacionDatos habitacionDatos;

        public ReservacionesController()
        {
            reservacionDatos = new ReservacionDatos();
            clienteDatos = new ClienteDatos();
            habitacionDatos = new HabitacionDatos();
        }

        //se obtiene la lista de todas las reservaciones
        [HttpGet]
        public ActionResult<List<Reservacion>> ObtenerTodos()
        {
            return Ok(reservacionDatos.ObtenerTodos());
        }

        //se busca reservación segun su código
        [HttpGet("buscar/{codReservacion}")]
        public ActionResult<Reservacion> Buscar(string codReservacion)
        {
            Reservacion reservacion =
                reservacionDatos.ObtenerPorCodigo(codReservacion);

            if (reservacion == null)
            {
                return NotFound("No se encontró una reservación con ese código.");
            }

            return Ok(reservacion);
        }

        //se buscan las reservaciones de un cliente segun su identificación
        [HttpGet("cliente/{identificacion}")]
        public ActionResult<List<Reservacion>> BuscarPorCliente(string identificacion)
        {
            List<Reservacion> reservaciones =
                reservacionDatos.ObtenerPorCliente(identificacion);

            if (reservaciones.Count == 0)
            {
                return NotFound("No se encontraron reservaciones para ese cliente.");
            }

            return Ok(reservaciones);
        }

        //se genera reporte con las reservaciones de la proxima semana 
        [HttpGet("reporte-semana-entrante")]
        public ActionResult<List<ReporteReservacion>> ObtenerReporteSemanaEntrante()
        {
            return Ok(reservacionDatos.ObtenerReporteSemanaEntrante());
        }

        //se crea una nueva reservación calculando sus montos
        [HttpPost]
        public ActionResult Insertar(Reservacion reservacion)
        {
            //se asignan valores por defecto y código automático
            reservacion.IdentificacionCliente = (reservacion.IdentificacionCliente ?? "").Trim();
            reservacion.SolicitudesEspeciales = (reservacion.SolicitudesEspeciales ?? "").Trim();
            reservacion.Estado = NormalizarEstado(reservacion.Estado, "Reservada");
            reservacion.CodReservacion = GenerarCodigoReservacion();
            reservacion.FechaReservacion = DateTime.Today;

            string mensajeValidacion = ValidarReservacion(reservacion);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            //se calcula tarifa, descuento, iva y total una vez que los datos son válidos
            CalcularMontos(reservacion);

            try
            {
                bool insertado = reservacionDatos.Insertar(reservacion);

                if (!insertado)
                {
                    return BadRequest("No fue posible guardar la reservación.");
                }

                return CreatedAtAction(
                    nameof(Buscar),
                    new { codReservacion = reservacion.CodReservacion },
                    reservacion
                );
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible guardar la reservación. Verifique los datos ingresados.");
            }
        }

        //se modifica una reservacion recalculando montos
        [HttpPut("{codReservacion}")]
        public ActionResult Actualizar(string codReservacion, Reservacion reservacion)
        {
            if (codReservacion != reservacion.CodReservacion)
            {
                return BadRequest("El código de la ruta no coincide con el de la reservación.");
            }

            Reservacion reservacionExistente =
                reservacionDatos.ObtenerPorCodigo(codReservacion);

            if (reservacionExistente == null)
            {
                return NotFound("No se encontró la reservación que desea actualizar.");
            }

            //la fecha en que se hizo la reservación nunca cambia al editar
            reservacion.FechaReservacion = reservacionExistente.FechaReservacion;

            reservacion.IdentificacionCliente = (reservacion.IdentificacionCliente ?? "").Trim();
            reservacion.SolicitudesEspeciales = (reservacion.SolicitudesEspeciales ?? "").Trim();
            reservacion.Estado = NormalizarEstado(reservacion.Estado, "");

            string mensajeValidacion = ValidarReservacion(reservacion);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            //se recalcula subtotal descuento e impuesto
            CalcularMontos(reservacion);

            try
            {
                bool actualizado = reservacionDatos.Actualizar(reservacion);

                if (!actualizado)
                {
                    return BadRequest("No fue posible actualizar la reservación.");
                }

                return Ok("Reservación actualizada correctamente.");
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible actualizar la reservación. Verifique los datos ingresados.");
            }
        }

        //se elimina la reservacion segun su código
        [HttpDelete("{codReservacion}")]
        public ActionResult Eliminar(string codReservacion)
        {
            if (reservacionDatos.ObtenerPorCodigo(codReservacion) == null)
            {
                return NotFound("No se encontró la reservación que desea eliminar.");
            }

            bool eliminado = reservacionDatos.Eliminar(codReservacion);

            if (!eliminado)
            {
                return BadRequest("No fue posible eliminar la reservación.");
            }

            return Ok("Reservación eliminada correctamente.");
        }

        //devuelve el estado con el formato oficial, o el valor por defecto si viene vacío
        private string NormalizarEstado(string estado, string valorPorDefecto)
        {
            string texto = (estado ?? "").Trim();

            if (texto == "")
            {
                return valorPorDefecto;
            }

            string oficial = EstadosPermitidos.FirstOrDefault(e =>
                string.Equals(e, texto, StringComparison.OrdinalIgnoreCase));

            return oficial ?? texto;
        }

        //se revisan reglas de negocio
        private string ValidarReservacion(Reservacion reservacion)
        {
            if (string.IsNullOrWhiteSpace(reservacion.IdentificacionCliente))
            {
                return "Debe indicar la identificación del cliente.";
            }

            if (clienteDatos.ObtenerPorIdentificacion(reservacion.IdentificacionCliente) == null)
            {
                return "El cliente indicado no está registrado.";
            }

            if (reservacion.NumHabitacion <= 0)
            {
                return "Debe indicar un número de habitación válido.";
            }

            if (habitacionDatos.ObtenerPorNumero(reservacion.NumHabitacion) == null)
            {
                return "La habitación indicada no está registrada.";
            }

            if (reservacion.FechaIngreso < new DateTime(1753, 1, 1) ||
                reservacion.FechaSalida < new DateTime(1753, 1, 1))
            {
                return "Debe indicar fechas válidas de inicio y de salida.";
            }

            if (reservacion.FechaSalida.Date <= reservacion.FechaIngreso.Date)
            {
                return "La fecha de salida debe ser posterior a la fecha de inicio.";
            }

            if (reservacion.SolicitudesEspeciales.Length > 300)
            {
                return "Las solicitudes especiales no pueden superar los 300 caracteres.";
            }

            if (reservacion.PorcentajeDescuento < 0 ||
                reservacion.PorcentajeDescuento > 100)
            {
                return "El porcentaje de descuento debe estar entre 0 y 100.";
            }

            if (!EstadosPermitidos.Contains(reservacion.Estado))
            {
                return "El estado debe ser: Reservada, Cancelada, En proceso o Finalizada.";
            }

            //no se permiten reservaciones en la misma habitación
            if (reservacion.Estado != "Cancelada" &&
                reservacionDatos.ExisteTraslape(
                    reservacion.NumHabitacion,
                    reservacion.FechaIngreso,
                    reservacion.FechaSalida,
                    reservacion.CodReservacion))
            {
                return "La habitación " + reservacion.NumHabitacion +
                       " no está disponible en el rango de fechas seleccionado.";
            }

            return "";
        }

       
        private void CalcularMontos(Reservacion reservacion)
        {
            Habitacion habitacion =
                habitacionDatos.ObtenerPorNumero(reservacion.NumHabitacion);

            int cantidadNoches =
                (reservacion.FechaSalida.Date - reservacion.FechaIngreso.Date).Days;

            reservacion.TarifaReservacion =
                Math.Round(habitacion.TarifaNoche * cantidadNoches, 2, MidpointRounding.AwayFromZero);

            decimal montoDescuento =
                reservacion.TarifaReservacion * (reservacion.PorcentajeDescuento / 100m);

            decimal tarifaConDescuento =
                reservacion.TarifaReservacion - montoDescuento;

            decimal iva = tarifaConDescuento * PorcentajeIva;

            reservacion.MontoTotal =
                Math.Round(tarifaConDescuento + iva, 2, MidpointRounding.AwayFromZero);
        }

        //se genera un codigo de manera secuencial y que no se repita
        private string GenerarCodigoReservacion()
        {
            List<Reservacion> todas = reservacionDatos.ObtenerTodos();
            int numero = todas.Count + 1;
            string codigo = "CodR-" + numero;

            while (todas.Exists(r => r.CodReservacion == codigo))
            {
                numero++;
                codigo = "CodR-" + numero;
            }

            return codigo;
        }
    }
}
