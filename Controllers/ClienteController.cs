using HospedajeGuanacaste.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospedajeGuanacaste.Controllers
{
    //este controlador consume la api y no accede directo a sql server
    public class ClienteController : Controller
    {
        //es el servicio para conectarse con la api web
        private readonly IHttpClientFactory httpClientFactory;

        public ClienteController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        //crea el cliente http para consumir los endpoints de la api
        private HttpClient ObtenerClienteApi()
        {
            return httpClientFactory.CreateClient("HospedajeApi");
        }

        //se obtiene la lista de clientes desde la api
        public async Task<IActionResult> Index()
        {
            HttpClient clienteApi = ObtenerClienteApi();

            HttpResponseMessage respuesta =
                await clienteApi.GetAsync("api/Clientes");

            if (!respuesta.IsSuccessStatusCode)
            {
                TempData["Error"] = "No fue posible obtener los clientes desde la API.";
                return View(new List<Cliente>());
            }

            List<Cliente> clientes =
                await respuesta.Content.ReadFromJsonAsync<List<Cliente>>();

            return View(clientes);
        }

        //muestra el formulario para registrar un cliente
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //envía el nuevo cliente a la api para guardarlo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                HttpClient clienteApi = ObtenerClienteApi();

                HttpResponseMessage respuesta =
                    await clienteApi.PostAsJsonAsync("api/Clientes", cliente);

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                string mensaje = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError("", mensaje);

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cliente);
            }
        }

        //muestra la pantalla para editar los datos de un cliente
        [HttpGet]
        public async Task<IActionResult> Edit(string identificacion)
        {
            HttpClient clienteApi = ObtenerClienteApi();

            HttpResponseMessage respuesta =
                await clienteApi.GetAsync(
                    "api/Clientes/buscar/" +
                    Uri.EscapeDataString(identificacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Cliente cliente =
                await respuesta.Content.ReadFromJsonAsync<Cliente>();

            return View(cliente);
        }

        //envía los datos modificados del cliente a la api
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                HttpClient clienteApi = ObtenerClienteApi();

                HttpResponseMessage respuesta =
                    await clienteApi.PutAsJsonAsync(
                        "api/Clientes/" + Uri.EscapeDataString(cliente.Identificacion),
                        cliente
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                string mensaje = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError("", mensaje);

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cliente);
            }
        }

        //muestra la confirmación para borrar un cliente
        [HttpGet]
        public async Task<IActionResult> Delete(string identificacion)
        {
            HttpClient clienteApi = ObtenerClienteApi();

            HttpResponseMessage respuesta =
                await clienteApi.GetAsync(
                    "api/Clientes/buscar/" +
                    Uri.EscapeDataString(identificacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Cliente cliente =
                await respuesta.Content.ReadFromJsonAsync<Cliente>();

            return View(cliente);
        }

        //confirma y ejecuta el borrado del cliente en la api
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string identificacion)
        {
            try
            {
                HttpClient clienteApi = ObtenerClienteApi();

                HttpResponseMessage respuesta =
                    await clienteApi.DeleteAsync(
                        "api/Clientes/" +
                        Uri.EscapeDataString(identificacion)
                    );

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                string mensajeError = await respuesta.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(mensajeError) || mensajeError.Contains("<html") || mensajeError.Length > 150)
                {
                    TempData["Error"] = "No se puede eliminar el cliente porque tiene registros asociados.";
                }
                else
                {
                    TempData["Error"] = mensajeError;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        //se busca un cliente por su identificación para mostrarlo en pantalla
        [HttpGet]
        public async Task<IActionResult> Buscar(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return View();
            }

            HttpClient clienteApi = ObtenerClienteApi();

            HttpResponseMessage respuesta =
                await clienteApi.GetAsync(
                    "api/Clientes/buscar/" +
                    Uri.EscapeDataString(identificacion)
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                ViewBag.Mensaje =
                    "No se encontró un cliente con esa identificación.";

                return View();
            }

            Cliente cliente =
                await respuesta.Content.ReadFromJsonAsync<Cliente>();

            return View(cliente);
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