using HospedajeGuanacaste.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospedajeGuanacaste.Controllers
{
    public class HabitacionController : Controller
    {
        //es el servicio para conectarse con la api web
        private readonly IHttpClientFactory httpClientFactory;

        public HabitacionController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        //crea el cliente http para consumir los end points de la api
        private HttpClient ObtenerClienteApi()
        {
            return httpClientFactory.CreateClient("HospedajeApi");
        }

        //se obtiene la lista de habitaciones desde la api
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync("api/Habitaciones");

            if (!respuesta.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "No fue posible obtener las habitaciones desde la API.";

                return View(new List<Habitacion>());
            }

            List<Habitacion> habitaciones =
                await respuesta.Content.ReadFromJsonAsync<List<Habitacion>>();

            return View(habitaciones);
        }

        //muestra el formulario para registrar una habitación
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        //envía la nueva habitación a la api para guardarla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Habitacion habitacion)
        {
            if (!ModelState.IsValid)
            {
                return View(habitacion);
            }

            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().PostAsJsonAsync(
                        "api/Habitaciones",
                        habitacion
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    "",
                    await respuesta.Content.ReadAsStringAsync()
                );

                return View(habitacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View(habitacion);
            }
        }

        //muestra la pantalla para editar los datos de una habitación
        [HttpGet]
        public async Task<IActionResult> Edit(int numHabitacion)
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Habitaciones/buscar/" + numHabitacion
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Habitacion habitacion =
                await respuesta.Content.ReadFromJsonAsync<Habitacion>();

            return View(habitacion);
        }

        //se envía los datos modificados de la habitación a la api
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Habitacion habitacion)
        {
            if (!ModelState.IsValid)
            {
                return View(habitacion);
            }

            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().PutAsJsonAsync(
                        "api/Habitaciones/" + habitacion.NumHabitacion,
                        habitacion
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    "",
                    await respuesta.Content.ReadAsStringAsync()
                );

                return View(habitacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View(habitacion);
            }
        }

        //se muestra la confirmación para borrar una habitación
        [HttpGet]
        public async Task<IActionResult> Delete(int numHabitacion)
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Habitaciones/buscar/" + numHabitacion
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Habitacion habitacion =
                await respuesta.Content.ReadFromJsonAsync<Habitacion>();

            return View(habitacion);
        }

        //se confirma y ejecuta el borrado de la habitación en la api
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int numHabitacion)
        {
            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().DeleteAsync(
                        "api/Habitaciones/" + numHabitacion
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

        //se busca una habitación por su número para poder mostrarla en la pantalla
        [HttpGet]
        public async Task<IActionResult> Buscar(int? numHabitacion)
        {
            if (numHabitacion == null)
            {
                return View();
            }

            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Habitaciones/buscar/" + numHabitacion
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                ViewBag.Mensaje =
                    "No se encontró una habitación con ese número.";

                return View();
            }

            Habitacion habitacion =
                await respuesta.Content.ReadFromJsonAsync<Habitacion>();

            return View(habitacion);
        }

        //recibe la búsqueda enviada desde el formulario
        [HttpPost]
        [ActionName("Buscar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarPost(int? numHabitacion)
        {
            return await Buscar(numHabitacion);
        }
    }
}