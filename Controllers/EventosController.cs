using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TechRent.Controllers;

[Authorize]
[Route("eventos")]
public class EventosController(IConfiguration configuration, ILogger<EventosController> logger) : Controller
{
    [HttpGet("reservas")]
    public async Task Reservas(CancellationToken cancel)
    {
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache, no-transform";
        Response.Headers.Append("X-Accel-Buffering", "no");
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync(cancel);
        await using (var command = new NpgsqlCommand("LISTEN techrent_reservas", connection))
            await command.ExecuteNonQueryAsync(cancel);

        connection.Notification += (_, notification) =>
        {
            if (int.TryParse(notification.Payload, out var id))
                Response.WriteAsync($"event: reserva\ndata: {id}\n\n", cancel).GetAwaiter().GetResult();
        };
        await Response.WriteAsync(": conectado\n\n", cancel);
        await Response.Body.FlushAsync(cancel);
        try
        {
            while (!cancel.IsCancellationRequested)
            {
                await connection.WaitAsync(TimeSpan.FromSeconds(15), cancel);
                await Response.WriteAsync(": latido\n\n", cancel);
                await Response.Body.FlushAsync(cancel);
            }
        }
        catch (OperationCanceledException) when (cancel.IsCancellationRequested)
        {
            logger.LogDebug("Cliente de eventos desconectado");
        }
    }
}
