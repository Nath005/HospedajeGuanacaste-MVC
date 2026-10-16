/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/ 

using HospedajeGuanacaste.API.Validaciones;
using HospedajeGuanacaste.Datos.AccesoDatos;
using HospedajeGuanacaste.Datos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace HospedajeGuanacaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        //se usa la capa de acceso a datos, el controlador nunca se conecta directo a sql server
        private readonly ClienteDatos clienteDatos;
        private readonly ReservacionDatos reservacionDatos;

        public ClientesController()
        {
            clienteDatos = new ClienteDatos();
            reservacionDatos = new ReservacionDatos();
        }

        //se obtiene la lista de todos los clientes
        [HttpGet]
        public ActionResult<List<Cliente>> ObtenerTodos()
        {
            return Ok(clienteDatos.ObtenerTodos());
        }

        //se busca un cliente especifico por su identificacion
        [HttpGet("buscar/{identificacion}")]
        public ActionResult<Cliente> Buscar(string identificacion)
        {
            Cliente cliente =
                clienteDatos.ObtenerPorIdentificacion(identificacion);

            if (cliente == null)
            {
                return NotFound("No se encontró un cliente con esa identificación.");
            }

            return Ok(cliente);
        }

        //se guarda un nuevo cliente en la base de datos
        [HttpPost]
        public ActionResult Insertar(Cliente cliente)
        {
            Normalizar(cliente);

            string mensajeValidacion = ValidarCliente(cliente);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            if (clienteDatos.ObtenerPorIdentificacion(cliente.Identificacion) != null)
            {
                return BadRequest("Ya existe un cliente con esa identificación.");
            }

            try
            {
                bool insertado = clienteDatos.Insertar(cliente);

                if (!insertado)
                {
                    return BadRequest("No fue posible guardar el cliente en la base de datos.");
                }

                return CreatedAtAction(
                    nameof(Buscar),
                    new { identificacion = cliente.Identificacion },
                    cliente
                );
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible guardar el cliente. Verifique los datos ingresados.");
            }
        }

        //se actualizan los datos del cliente existente
        [HttpPut("{identificacion}")]
        public ActionResult Actualizar(string identificacion, Cliente cliente)
        {
            if (identificacion != cliente.Identificacion)
            {
                return BadRequest("La identificación de la ruta no coincide con la del cliente.");
            }

            Normalizar(cliente);

            string mensajeValidacion = ValidarCliente(cliente);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            if (clienteDatos.ObtenerPorIdentificacion(identificacion) == null)
            {
                return NotFound("No se encontró el cliente que desea actualizar.");
            }

            try
            {
                bool actualizado = clienteDatos.Actualizar(cliente);

                if (!actualizado)
                {
                    return BadRequest("No fue posible actualizar el cliente.");
                }

                return Ok("Cliente actualizado correctamente.");
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible actualizar el cliente. Verifique los datos ingresados.");
            }
        }

        //se elimina un cliente solo si no tiene reservaciones
        [HttpDelete("{identificacion}")]
        public ActionResult Eliminar(string identificacion)
        {
            if (clienteDatos.ObtenerPorIdentificacion(identificacion) == null)
            {
                return NotFound("No se encontró el cliente que desea eliminar.");
            }

            //regla de integridad: no se borra un cliente asociado a una reservación
            if (reservacionDatos.ExisteReservacionDeCliente(identificacion))
            {
                return Conflict("No se puede eliminar el cliente porque tiene reservaciones registradas.");
            }

            try
            {
                bool eliminado = clienteDatos.Eliminar(identificacion);

                if (!eliminado)
                {
                    return BadRequest("No fue posible eliminar el cliente.");
                }

                return Ok("Cliente eliminado correctamente.");
            }
            catch (SqlException)
            {
                //la llave foránea de la base de datos también protege la relación
                return Conflict("No se puede eliminar este cliente porque tiene reservaciones relacionadas.");
            }
        }

        //se limpian espacios y se evitan valores nulos
        private void Normalizar(Cliente cliente)
        {
            cliente.Identificacion = (cliente.Identificacion ?? "").Trim();
            cliente.TipoIdentificacion = ValidacionesComunes.NormalizarTipo(cliente.TipoIdentificacion);
            cliente.Nombre = (cliente.Nombre ?? "").Trim();
            cliente.PrimerApellido = (cliente.PrimerApellido ?? "").Trim();
            cliente.SegundoApellido = (cliente.SegundoApellido ?? "").Trim();
        }

        //se comprueban formatos de identificación, largos de texto y fecha de nacimiento
        private string ValidarCliente(Cliente cliente)
        {
            string mensaje = ValidacionesComunes.ValidarIdentificacion(
                cliente.TipoIdentificacion, cliente.Identificacion, true);

            if (mensaje != "") return mensaje;

            mensaje = ValidacionesComunes.ValidarNombreCompleto(
                cliente.Nombre, cliente.PrimerApellido, cliente.SegundoApellido);

            if (mensaje != "") return mensaje;

            return ValidacionesComunes.ValidarFechaNacimiento(cliente.FechaNacimiento);
        }
    }
}
