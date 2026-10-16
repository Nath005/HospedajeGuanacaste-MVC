using HospedajeGuanacaste.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospedajeGuanacaste.Controllers
{
    public class EmpleadoController : Controller
    {
        //es el servicio para conectarse con la api web
        private readonly IHttpClientFactory httpClientFactory;

        public EmpleadoController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        //crea el cliente http para consumir los endpoints de la api
        private HttpClient ObtenerClienteApi()
        {
            return httpClientFactory.CreateClient("HospedajeApi");
        }

        //se obtiene la lista de empleados desde la api
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync("api/Empleados");

            if (!respuesta.IsSuccessStatusCode)
            {
                TempData["Error"] = "No fue posible obtener los empleados desde la API.";
                return View(new List<Empleado>());
            }

            List<Empleado> empleados =
                await respuesta.Content.ReadFromJsonAsync<List<Empleado>>();

            return View(empleados);
        }

        //muestra el formulario para registrar un empleado
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        //envía el nuevo empleado a la api para guardarlo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return View(empleado);
            }

            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().PostAsJsonAsync(
                        "api/Empleados",
                        empleado
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    "",
                    await respuesta.Content.ReadAsStringAsync()
                );

                return View(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View(empleado);
            }
        }

        //envía el nuevo empleado a la api para guardarlo
        [HttpGet]
        public async Task<IActionResult> Edit(string identificacion)
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Empleados/buscar/" +
                    Uri.EscapeDataString(identificacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Empleado empleado =
                await respuesta.Content.ReadFromJsonAsync<Empleado>();

            return View(empleado);
        }

        //envía los datos modificados del empleado a la api
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return View(empleado);
            }

            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().PutAsJsonAsync(
                        "api/Empleados/" +
                        Uri.EscapeDataString(empleado.Identificacion),
                        empleado
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    "",
                    await respuesta.Content.ReadAsStringAsync()
                );

                return View(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View(empleado);
            }
        }

        //muestra la confirmación para borrar un empleado
        [HttpGet]
        public async Task<IActionResult> Delete(string identificacion)
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Empleados/buscar/" +
                    Uri.EscapeDataString(identificacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Empleado empleado =
                await respuesta.Content.ReadFromJsonAsync<Empleado>();

            return View(empleado);
        }

        //confirma y ejecuta el borrado del empleado en la api
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string identificacion)
        {
            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().DeleteAsync(
                        "api/Empleados/" +
                        Uri.EscapeDataString(identificacion)
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = await respuesta.Content.ReadAsStringAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        //se busca un empleado por su identificación para mostrarlo en pantalla
        [HttpGet]
        public async Task<IActionResult> Buscar(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return View();
            }

            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Empleados/buscar/" +
                    Uri.EscapeDataString(identificacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                ViewBag.Mensaje =
                    "No se encontró un empleado con esa identificación.";

                return View();
            }

            Empleado empleado =
                await respuesta.Content.ReadFromJsonAsync<Empleado>();

            return View(empleado);
        }

        //recibe la búsqueda enviada desde el formulario
        [HttpPost]
        [ActionName("Buscar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarPost(string identificacion)
        {
            return await Buscar(identificacion);
        }
    }
}