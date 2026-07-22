using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;
using TechRent.Services.Payments;

namespace TechRent.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PayPhoneApiLinkService _payPhoneService;
        private readonly PayPalService _payPalService;
        private readonly IEmailSender _emailSender;

        public PagoController(
            AppDbContext context,
            PayPhoneApiLinkService payPhoneService,
            PayPalService payPalService,
            IEmailSender emailSender)
        {
            _context = context;
            _payPhoneService = payPhoneService;
            _payPalService = payPalService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> CreateLink(int ordenId)
        {
            var orden = await _context.OrdenesAlquiler
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            if (orden == null) return NotFound();

            if (orden.Total < 1.00m)
            {
                TempData["Error"] = "El total es menor a $1.00.";
                return RedirectToAction("Index", "Carrito");
            }

            string clientTransactionId = DateTime.Now.ToString("yyMMddHHmmssfff")[..15];
            string reference = $"TechRent Orden #{orden.Id}";

            string link = await _payPhoneService.CreatePaymentLinkAsync(
                orden.Total, clientTransactionId, reference);

            var transaccion = new TransaccionPago
            {
                OrdenAlquilerId = orden.Id,
                ClientTransactionId = clientTransactionId,
                Proveedor = "PayPhone",
                PayphonePaymentUrl = link,
                MontoEnCentavos = ToCents(orden.Total),
                Estado = "Pendiente"
            };

            _context.TransaccionesPago.Add(transaccion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = transaccion.Id });
        }

        public async Task<IActionResult> CreatePayPalOrder(int ordenId)
        {
            var orden = await _context.OrdenesAlquiler
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            if (orden == null) return NotFound();

            if (orden.Total < 1.00m)
            {
                TempData["Error"] = "El total es menor a $1.00.";
                return RedirectToAction("Index", "Carrito");
            }

            string reference = $"TechRent Orden #{orden.Id}";

            var result = await _payPalService.CreateOrderAsync(orden.Total, reference);

            var transaccion = new TransaccionPago
            {
                OrdenAlquilerId = orden.Id,
                ClientTransactionId = result.OrderId,
                Proveedor = "PayPal",
                PayPalOrderId = result.OrderId,
                PayPalApprovalUrl = result.ApprovalUrl,
                MontoEnCentavos = ToCents(orden.Total),
                Estado = "Pendiente",
                RespuestaGateway = result.RawResponse
            };

            _context.TransaccionesPago.Add(transaccion);
            await _context.SaveChangesAsync();

            return Redirect(result.ApprovalUrl);
        }

        public async Task<IActionResult> Success(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("PayPal no devolvió token.");

            var transaccion = await _context.TransaccionesPago
                .Include(t => t.OrdenAlquiler)
                .ThenInclude(o => o.Detalles)
                .FirstOrDefaultAsync(t => t.Proveedor == "PayPal" && t.PayPalOrderId == token);

            if (transaccion == null) return NotFound();

            if (transaccion.Estado == "Pagado")
                return RedirectToAction(nameof(Details), new { id = transaccion.Id });

            var capture = await _payPalService.CaptureOrderAsync(token);

            transaccion.PayPalCaptureId = capture.CaptureId;
            transaccion.RespuestaGateway = capture.RawResponse;
            transaccion.FechaConfirmacion = DateTime.UtcNow;

            if (capture.Status == "COMPLETED")
            {
                transaccion.Estado = "Pagado";
                transaccion.OrdenAlquiler.Estado = "Pagado";
                transaccion.OrdenAlquiler.FechaPago = DateTime.UtcNow;
                await DescontarStockAsync(transaccion.OrdenAlquiler);
                await EnviarEmailConfirmacionAsync(transaccion.OrdenAlquiler);
            }
            else
            {
                transaccion.Estado = capture.Status;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = transaccion.Id });
        }

        public async Task<IActionResult> Cancel(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var transaccion = await _context.TransaccionesPago
                    .FirstOrDefaultAsync(t => t.Proveedor == "PayPal" && t.PayPalOrderId == token);

                if (transaccion != null && transaccion.Estado == "Pendiente")
                {
                    transaccion.Estado = "Cancelado";
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Error"] = "El pago con PayPal fue cancelado.";
            return RedirectToAction("Index", "Carrito");
        }

        public async Task<IActionResult> Details(int id)
        {
            var transaccion = await _context.TransaccionesPago
                .Include(t => t.OrdenAlquiler)
                .ThenInclude(o => o.Detalles)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaccion == null) return NotFound();
            return View(transaccion);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> MarcarPagado(int id)
        {
            var transaccion = await _context.TransaccionesPago
                .Include(t => t.OrdenAlquiler)
                .ThenInclude(o => o.Detalles)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaccion == null) return NotFound();

            if (transaccion.Estado != "Pagado")
            {
                transaccion.Estado = "Pagado";
                transaccion.FechaConfirmacion = DateTime.UtcNow;
                transaccion.OrdenAlquiler.Estado = "Pagado";
                transaccion.OrdenAlquiler.FechaPago = DateTime.UtcNow;
                await DescontarStockAsync(transaccion.OrdenAlquiler);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> PayPalButton(int ordenId)
        {
            var orden = await _context.OrdenesAlquiler
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            if (orden == null) return NotFound();
            return View(orden);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayPalButtonOrderJson(int ordenId)
        {
            var orden = await _context.OrdenesAlquiler
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            if (orden == null)
                return Json(new { success = false, message = "Orden no encontrada." });

            string reference = $"TechRent Orden #{orden.Id}";
            var result = await _payPalService.CreateOrderAsync(orden.Total, reference);

            var transaccion = new TransaccionPago
            {
                OrdenAlquilerId = orden.Id,
                ClientTransactionId = result.OrderId,
                Proveedor = "PayPalButton",
                PayPalOrderId = result.OrderId,
                PayPalApprovalUrl = result.ApprovalUrl,
                MontoEnCentavos = ToCents(orden.Total),
                Estado = "Pendiente",
                RespuestaGateway = result.RawResponse
            };

            _context.TransaccionesPago.Add(transaccion);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                paypalOrderId = result.OrderId,
                transaccionPagoId = transaccion.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> CapturePayPalButtonOrderJson(
            [FromBody] PayPalButtonCaptureRequest request)
        {
            var transaccion = await _context.TransaccionesPago
                .Include(t => t.OrdenAlquiler)
                .ThenInclude(o => o.Detalles)
                .FirstOrDefaultAsync(t =>
                    t.Id == request.TransaccionPagoId &&
                    t.PayPalOrderId == request.PayPalOrderId);

            if (transaccion == null)
                return Json(new { success = false, message = "Transacción no encontrada." });

            var capture = await _payPalService.CaptureOrderAsync(request.PayPalOrderId);

            transaccion.PayPalCaptureId = capture.CaptureId;
            transaccion.RespuestaGateway = capture.RawResponse;
            transaccion.FechaConfirmacion = DateTime.UtcNow;

            if (capture.Status == "COMPLETED")
            {
                transaccion.Estado = "Pagado";
                transaccion.OrdenAlquiler.Estado = "Pagado";
                transaccion.OrdenAlquiler.FechaPago = DateTime.UtcNow;
                await DescontarStockAsync(transaccion.OrdenAlquiler);
                await EnviarEmailConfirmacionAsync(transaccion.OrdenAlquiler);
            }
            else
            {
                transaccion.Estado = capture.Status;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Details", "Pago", new { id = transaccion.Id })
            });
        }

        private async Task DescontarStockAsync(OrdenAlquiler orden)
        {
            foreach (var detalle in orden.Detalles)
            {
                var equipo = await _context.Equipos.FindAsync(detalle.EquipoId);
                if (equipo != null)
                    equipo.Stock = Math.Max(0, equipo.Stock - detalle.Cantidad);
            }
        }

        private static int ToCents(decimal value)
        {
            return (int)Math.Round(value * 100, MidpointRounding.AwayFromZero);
        }

        private async Task EnviarEmailConfirmacionAsync(OrdenAlquiler orden)
        {
            try
            {
                var detallesHtml = "";
                foreach (var d in orden.Detalles)
                {
                    detallesHtml += $@"
                    <tr>
                        <td style='padding:8px;border:1px solid #ddd;'>{d.NombreEquipo}</td>
                        <td style='padding:8px;border:1px solid #ddd;text-align:center;'>{d.Cantidad}</td>
                        <td style='padding:8px;border:1px solid #ddd;text-align:center;'>{d.Dias}</td>
                        <td style='padding:8px;border:1px solid #ddd;text-align:right;'>${d.PrecioPorDia:F2}</td>
                        <td style='padding:8px;border:1px solid #ddd;text-align:right;'>${d.Subtotal:F2}</td>
                    </tr>";
                }

                var html = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#4361ee;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Confirmación de Reserva</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#4361ee;'>¡Reserva confirmada!</h2>
                        <p>Tu orden <strong>#{orden.Id}</strong> ha sido pagada exitosamente.</p>
                        <p><strong>Fecha:</strong> {orden.FechaCreacion:dd/MM/yyyy HH:mm}</p>
                        <table style='width:100%;border-collapse:collapse;margin:15px 0;'>
                            <thead>
                                <tr style='background:#f5f5f5;'>
                                    <th style='padding:8px;border:1px solid #ddd;'>Equipo</th>
                                    <th style='padding:8px;border:1px solid #ddd;'>Cant.</th>
                                    <th style='padding:8px;border:1px solid #ddd;'>Días</th>
                                    <th style='padding:8px;border:1px solid #ddd;'>Precio/Día</th>
                                    <th style='padding:8px;border:1px solid #ddd;'>Subtotal</th>
                                </tr>
                            </thead>
                            <tbody>{detallesHtml}</tbody>
                        </table>
                        <div style='text-align:right;font-size:1.2em;margin-top:15px;'>
                            <strong>Total: <span style='color:#06d6a0;'>${orden.Total:F2}</span></strong>
                        </div>
                        <hr />
                        <p style='color:#666;'>Gracias por confiar en TechRent.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnológicos
                    </div>
                </div>";

                await _emailSender.SendEmailAsync(
                    orden.UsuarioEmail,
                    $"TechRent - Confirmación de Orden #{orden.Id}",
                    html);
            }
            catch
            {
                // No fallar el pago por error de email
            }
        }
    }

    public class PayPalButtonCaptureRequest
    {
        public string PayPalOrderId { get; set; } = string.Empty;
        public int TransaccionPagoId { get; set; }
    }
}
