using System.Text;
using System.Text.Json;
using TechRent.Data;

namespace TechRent.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public OllamaService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<string> GenerarRespuestaAsync(string prompt)
        {
            var equipos = _context.Equipos
                .Where(e => e.Stock > 0)
                .Select(e => new { e.Nombre, e.PrecioPorDia, e.Stock })
                .ToList();

            var listaEquipos = string.Join("\n", equipos.Select(e =>
                $"- {e.Nombre}: ${e.PrecioPorDia}/dia, Stock disponible: {e.Stock}"));

            var systemPrompt = $@"Eres un asistente de alquiler de equipos de tecnologia llamado TechRent AI.

REGLAS ESTRICTAS:
1. SOLO puedes recomendar equipos que aparezcan en la lista de abajo.
2. NO inventes equipos que no esten en la lista.
3. Si ninguno de los equipos disponibles cumple con lo que el usuario pide, di que no hay equipos disponibles para esa necesidad.
4. Siempre menciona el precio por dia y el stock disponible.
5. Responde en espanol, se breve y util.

Equipos disponibles en el catalogo:
{listaEquipos}";

            var request = new
            {
                model = "qwen2.5:0.5b",
                prompt = $"{systemPrompt}\n\nUsuario: {prompt}",
                stream = false
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            return result.GetProperty("response").GetString();
        }
    }
}