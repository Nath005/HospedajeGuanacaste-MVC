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
using System.Linq;

namespace HospedajeGuanacaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        //categorías 
        private static readonly string[] CategoriasPermitidas =
        {
            "mesero", "salonero", "lavaplatos", "recepcionista", "administrador",
            "mantenimiento", "cocinero", "chef", "limpieza", "cocina", "seguridad",
            "atención al cliente", "guía turístico", "encargado de reservaciones"
        };

        private readonly EmpleadoDatos empleadoDatos;

        public EmpleadosController()
        {
            empleadoDatos = new EmpleadoDatos();
        }

        //se obtiene la lista con todos los empleados
        [HttpGet]
        public ActionResult<List<Empleado>> ObtenerTodos()
        {
            return Ok(empleadoDatos.ObtenerTodos());
        }

        //se busca un empleado mediante su identificacion
        [HttpGet("buscar/{identificacion}")]
        public ActionResult<Empleado> Buscar(string identificacion)
        {
            Empleado empleado =
                empleadoDatos.ObtenerPorIdentificacion(identificacion);

            if (empleado == null)
            {
                return NotFound("No se encontró un empleado con esa identificación.");
            }

            return Ok(empleado);
        }

        //se inserta un nuevo empleado en el sistema
        [HttpPost]
        public ActionResult Insertar(Empleado empleado)
        {
            Normalizar(empleado);

            string mensajeValidacion = ValidarEmpleado(empleado);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            if (empleadoDatos.ObtenerPorIdentificacion(empleado.Identificacion) != null)
            {
                return BadRequest("Ya existe un empleado con esa identificación.");
            }

            try
            {
                bool insertado = empleadoDatos.Insertar(empleado);

                if (!insertado)
                {
                    return BadRequest("No fue posible guardar el empleado.");
                }

                return CreatedAtAction(
                    nameof(Buscar),
                    new { identificacion = empleado.Identificacion },
                    empleado
                );
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible guardar el empleado. Verifique los datos ingresados.");
            }
        }

        //se edita la informacion de un empleado
        [HttpPut("{identificacion}")]
        public ActionResult Actualizar(string identificacion, Empleado empleado)
        {
            if (identificacion != empleado.Identificacion)
            {
                return BadRequest("La identificación de la ruta no coincide con la del empleado.");
            }

            Normalizar(empleado);

            string mensajeValidacion = ValidarEmpleado(empleado);

            if (mensajeValidacion != "")
            {
                return BadRequest(mensajeValidacion);
            }

            if (empleadoDatos.ObtenerPorIdentificacion(identificacion) == null)
            {
                return NotFound("No se encontró el empleado que desea actualizar.");
            }

            try
            {
                bool actualizado = empleadoDatos.Actualizar(empleado);

                if (!actualizado)
                {
                    return BadRequest("No fue posible actualizar el empleado.");
                }

                return Ok("Empleado actualizado correctamente.");
            }
            catch (SqlException)
            {
                return BadRequest("No fue posible actualizar el empleado. Verifique los datos ingresados.");
            }
        }

        //se borra el empleado registrado
        [HttpDelete("{identificacion}")]
        public ActionResult Eliminar(string identificacion)
        {
            if (empleadoDatos.ObtenerPorIdentificacion(identificacion) == null)
            {
                return NotFound("No se encontró el empleado que desea eliminar.");
            }

            bool eliminado = empleadoDatos.Eliminar(identificacion);

            if (!eliminado)
            {
                return BadRequest("No fue posible eliminar el empleado.");
            }

            return Ok("Empleado eliminado correctamente.");
        }

        //se limpian espacios y se evitan valores nulos
        private void Normalizar(Empleado empleado)
        {
            empleado.Identificacion = (empleado.Identificacion ?? "").Trim();
            empleado.TipoIdentificacion = ValidacionesComunes.NormalizarTipo(empleado.TipoIdentificacion);
            empleado.Nombre = (empleado.Nombre ?? "").Trim();
            empleado.PrimerApellido = (empleado.PrimerApellido ?? "").Trim();
            empleado.SegundoApellido = (empleado.SegundoApellido ?? "").Trim();
            empleado.Categoria = (empleado.Categoria ?? "").Trim();
            empleado.Provincia = (empleado.Provincia ?? "").Trim();
            empleado.Canton = (empleado.Canton ?? "").Trim();
            empleado.Distrito = (empleado.Distrito ?? "").Trim();
            empleado.DireccionExacta = (empleado.DireccionExacta ?? "").Trim();

            //se guarda la categoría con el mismo formato del combo
            string categoria = CategoriasPermitidas.FirstOrDefault(c =>
                string.Equals(c, empleado.Categoria, StringComparison.OrdinalIgnoreCase));

            if (categoria != null)
            {
                empleado.Categoria = categoria;
            }
        }

        //se comprueban identificación, nombre, salario, fechas, categoría y ubicación
        private string ValidarEmpleado(Empleado empleado)
        {
            string mensaje = ValidacionesComunes.ValidarIdentificacion(
                empleado.TipoIdentificacion, empleado.Identificacion, false);

            if (mensaje != "") return mensaje;

            mensaje = ValidacionesComunes.ValidarNombreCompleto(
                empleado.Nombre, empleado.PrimerApellido, empleado.SegundoApellido);

            if (mensaje != "") return mensaje;

            mensaje = ValidacionesComunes.ValidarFechaNacimiento(empleado.FechaNacimiento);

            if (mensaje != "") return mensaje;

            if (empleado.SalarioMensual < 0 || empleado.SalarioMensual > 5000000)
            {
                return "El salario mensual debe estar entre 0 y 5,000,000.";
            }

            if (empleado.FechaIngreso < new DateTime(1900, 1, 1) ||
                empleado.FechaIngreso.Date <= empleado.FechaNacimiento.Date)
            {
                return "Debe indicar una fecha de ingreso válida (posterior a la fecha de nacimiento).";
            }

            if (!CategoriasPermitidas.Contains(empleado.Categoria))
            {
                return "Debe seleccionar una categoría válida.";
            }

            if (empleado.Provincia == "" || empleado.Canton == "" || empleado.Distrito == "")
            {
                return "Debe seleccionar la provincia, el cantón y el distrito.";
            }

            if (empleado.DireccionExacta.Length < 1 || empleado.DireccionExacta.Length > 150)
            {
                return "La dirección exacta debe tener entre 1 y 150 caracteres.";
            }

            return "";
        }
    }
}
