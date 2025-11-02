using System.Diagnostics;
using Heladeria.Models;
using Microsoft.AspNetCore.Mvc;
using Heladeria.Servicios;
using System.Security.Claims;

namespace Heladeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RolService _rolService;

        public HomeController(ILogger<HomeController> logger, RolService rolService)
        {
            _logger = logger;
            _rolService = rolService;
        }

        public async Task<IActionResult> Index()
        {
            // Pasar información a la vista sobre si está autenticado
            ViewBag.EstaAutenticado = User.Identity?.IsAuthenticated ?? false;
            
            // Verificar si es administrador si está autenticado
            if (ViewBag.EstaAutenticado)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
                ViewBag.EsAdministrador = await _rolService.EsAdministrador(email ?? "");
            }
            else
            {
                ViewBag.EsAdministrador = false;
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
