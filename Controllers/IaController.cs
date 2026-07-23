using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize]
    public class IaController : Controller
    {
        private readonly OllamaService _ollamaService;

        public IaController(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Generar(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                return Json(new { respuesta = "Por favor escribe una pregunta." });

            try
            {
                var respuesta = await _ollamaService.GenerarRespuestaAsync(mensaje);
                return Json(new { respuesta });
            }
            catch
            {
                return Json(new { respuesta = "Error al conectar con Ollama. Verifica que este corriendo." });
            }
        }
    }
}