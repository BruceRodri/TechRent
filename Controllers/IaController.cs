using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize]
    public class IaController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IAuditService _audit;

        public IaController(IAIService aiService, IAuditService audit)
        {
            _aiService = aiService;
            _audit = audit;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Generar(string mensaje, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                return Json(new { respuesta = "Por favor escribe una pregunta." });

            var result = await _aiService.GenerateAsync(mensaje, cancellationToken);

            if (result.Exitoso)
            {
                await _audit.RegistrarAsync(
                    "Ejecucion de IA", "IA", null,
                    $"Pregunta: {mensaje}",
                    $"Respuesta: {result.Respuesta?.Substring(0, Math.Min(result.Respuesta.Length, 200))}",
                    $"Tiempo: {result.TiempoRespuestaMs}ms | Modelo: {result.ModeloNombre}",
                    User, HttpContext);
                return Json(new { respuesta = result.Respuesta, tiempoMs = result.TiempoRespuestaMs, modelo = result.ModeloNombre });
            }
            else
            {
                await _audit.RegistrarAsync(
                    "Ejecucion de IA", "IA", null,
                    $"Pregunta: {mensaje}",
                    $"Error: {result.Error}",
                    $"Tiempo: {result.TiempoRespuestaMs}ms | Modelo: {result.ModeloNombre}",
                    User, HttpContext);
                return Json(new { respuesta = "Error al conectar con Ollama. Verifica que este corriendo." });
            }
        }
    }
}
