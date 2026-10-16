/*
* PROYECTO :Sistema web que permite gestionar la información del emprendimiento Hospedaje Guanacaste
* Estudiante:Nathalie Enríquez 
* UNED 2026
*/

using HospedajeGuanacaste.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HospedajeGuanacaste.Controllers
{
    public class ReservacionController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ReservacionController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        //se crea el cliente para consumir la api
        private HttpClient ObtenerClienteApi()
        {
            return httpClientFactory.CreateClient("HospedajeApi");
        }

        //se obtiene el listado de reservaciones
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync("api/Reservaciones");

            if (!respuesta.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "No fue posible obtener las reservaciones desde la API.";

                return View(new List<Reservacion>());
            }

            List<Reservacion> reservaciones =
                await respuesta.Content.ReadFromJsonAsync<List<Reservacion>>();

            return View(reservaciones);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //se guarda la nueva reservacion enviandola a la api
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservacion reservacion)
        {
            reservacion.CodReservacion ??= "TEMP";
            reservacion.FechaReservacion = DateTime.Now;
            reservacion.SolicitudesEspeciales ??= "";
            reservacion.Estado ??= "Reservada";

            ModelState.Clear();

            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().PostAsJsonAsync(
                        "api/Reservaciones",
                        reservacion
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                string mensajeError = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError("", mensajeError);

                return View(reservacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View(reservacion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string codReservacion)
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Reservaciones/buscar/" +
                    Uri.EscapeDataString(codReservacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Reservacion reservacion =
                await respuesta.Content.ReadFromJsonAsync<Reservacion>();

            return View(reservacion);
        }

        //se actualizan los datos de la reservacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Reservacion reservacion)
        {
            //la fecha en que se hizo la reservación no se modifica: la API conserva la original
            reservacion.SolicitudesEspeciales ??= "";

            ModelState.Clear();

            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().PutAsJsonAsync(
                        "api/Reservaciones/" +
                        Uri.EscapeDataString(reservacion.CodReservacion),
                        reservacion
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                string mensajeError = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError("", mensajeError);

                return View(reservacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View(reservacion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string codReservacion)
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Reservaciones/buscar/" +
                    Uri.EscapeDataString(codReservacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Reservacion reservacion =
                await respuesta.Content.ReadFromJsonAsync<Reservacion>();

            return View(reservacion);
        }

        //se elimina la reservacion de la base de datos
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string codReservacion)
        {
            try
            {
                HttpResponseMessage respuesta =
                    await ObtenerClienteApi().DeleteAsync(
                        "api/Reservaciones/" +
                        Uri.EscapeDataString(codReservacion)
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

        //se busca por código de reservación o por identificación del cliente.
        //el parámetro se llama datoBusqueda porque así se llama el campo del formulario
        [HttpGet]
        public async Task<IActionResult> Buscar(string datoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(datoBusqueda))
            {
                return View(new List<Reservacion>());
            }

            datoBusqueda = datoBusqueda.Trim();

            List<Reservacion> resultados = new List<Reservacion>();

            try
            {
                HttpClient clienteApi = ObtenerClienteApi();

                //primero se busca por código de reservación
                HttpResponseMessage respuestaCodigo =
                    await clienteApi.GetAsync(
                        "api/Reservaciones/buscar/" +
                        Uri.EscapeDataString(datoBusqueda)
                    );

                if (respuestaCodigo.IsSuccessStatusCode)
                {
                    Reservacion porCodigo =
                        await respuestaCodigo.Content.ReadFromJsonAsync<Reservacion>();

                    if (porCodigo != null)
                    {
                        resultados.Add(porCodigo);
                    }
                }

                //luego se buscan las reservaciones del cliente con esa identificación
                HttpResponseMessage respuestaCliente =
                    await clienteApi.GetAsync(
                        "api/Reservaciones/cliente/" +
                        Uri.EscapeDataString(datoBusqueda)
                    );

                if (respuestaCliente.IsSuccessStatusCode)
                {
                    List<Reservacion> porCliente =
                        await respuestaCliente.Content.ReadFromJsonAsync<List<Reservacion>>();

                    if (porCliente != null)
                    {
                        foreach (Reservacion reservacion in porCliente)
                        {
                            if (!resultados.Any(r => r.CodReservacion == reservacion.CodReservacion))
                            {
                                resultados.Add(reservacion);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error al conectar con la API: " + ex.Message;
                return View(new List<Reservacion>());
            }

            if (resultados.Count == 0)
            {
                ViewBag.Mensaje =
                    "No se encontraron reservaciones con ese código ni para un cliente con esa identificación.";
            }

            return View(resultados);
        }

        //recibe la búsqueda enviada desde el formulario
        [HttpPost]
        [ActionName("Buscar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarPost(string datoBusqueda)
        {
            return await Buscar(datoBusqueda);
        }

        //se obtiene el reporte de ingresos para la semana entrante
        public async Task<IActionResult> ReporteSemanaEntrante()
        {
            HttpResponseMessage respuesta =
                await ObtenerClienteApi().GetAsync(
                    "api/Reservaciones/reporte-semana-entrante"
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "No fue posible obtener el reporte semanal desde la API.";

                return View(new List<ReporteReservacion>());
            }

            List<ReporteReservacion> reporte =
                await respuesta.Content.ReadFromJsonAsync<List<ReporteReservacion>>();

            return View(reporte);
        }
    }
}