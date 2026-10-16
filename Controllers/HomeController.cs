using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospedajeGuanacaste.Controllers
{
    public class HomeController : Controller
    {
        //carga la pantalla principal 
        public ActionResult Index()
        {
            return View();
        }
    }
}