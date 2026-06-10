using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// REGISTRAR EL DBCONTEXT CON POSTGRESQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();
    // ==================== SEEDER ====================

    // 1. Insertar Categorías (5 registros)
    if (!context.Categorias.Any())
    {
        var categorias = new[]
        {
            new Categoria { Nombre = "Laptops", Descripcion = "Equipos portátiles de cómputo", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Categoria { Nombre = "Tablets", Descripcion = "Dispositivos táctiles", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Categoria { Nombre = "Proyectores", Descripcion = "Equipos de proyección de imagen", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Categoria { Nombre = "Cámaras", Descripcion = "Equipos de fotografía y video", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Categoria { Nombre = "Audio", Descripcion = "Equipos de sonido y amplificación", Activo = true, FechaCreacion = DateTime.UtcNow }
        };
        context.Categorias.AddRange(categorias);
        context.SaveChanges();
    }

    // 2. Insertar Marcas (5 registros)
    if (!context.Marcas.Any())
    {
        var marcas = new[]
    {
        new Marca { Nombre = "HP", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
        new Marca { Nombre = "Dell", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
        new Marca { Nombre = "Apple", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
        new Marca { Nombre = "Lenovo", PaisOrigen = "China", Activo = true, FechaCreacion = DateTime.UtcNow },
        new Marca { Nombre = "Samsung", PaisOrigen = "Corea del Sur", Activo = true, FechaCreacion = DateTime.UtcNow },

        new Marca { Nombre = "Epson", PaisOrigen = "Japón", Activo = true, FechaCreacion = DateTime.UtcNow },
        new Marca { Nombre = "Canon", PaisOrigen = "Japón", Activo = true, FechaCreacion = DateTime.UtcNow },
        new Marca { Nombre = "JBL", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow }
    };
        context.Marcas.AddRange(marcas);
        context.SaveChanges();
    }

    // 3. Insertar EstadosReserva (4 registros)
    if (!context.EstadosReserva.Any())
    {
        var estados = new[]
        {
            new EstadoReserva { Nombre = "Pendiente", Descripcion = "Reserva creada, esperando pago", Activo = true, FechaCreacion = DateTime.UtcNow },
            new EstadoReserva { Nombre = "Activa", Descripcion = "Pago confirmado, equipo entregado", Activo = true, FechaCreacion = DateTime.UtcNow },
            new EstadoReserva { Nombre = "Completada", Descripcion = "Equipo devuelto, reserva finalizada", Activo = true, FechaCreacion = DateTime.UtcNow },
            new EstadoReserva { Nombre = "Cancelada", Descripcion = "Reserva cancelada", Activo = true, FechaCreacion = DateTime.UtcNow }
        };
        context.EstadosReserva.AddRange(estados);
        context.SaveChanges();
    }

    // 4. Insertar Clientes (5 registros)
    if (!context.Clientes.Any())
    {
        var clientes = new[]
        {
            new Cliente { NombreCompleto = "Ana López García", Email = "ana.lopez@email.com", Telefono = "987654321", Direccion = "Av. Principal 123", DocumentoIdentidad = "12345678", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Cliente { NombreCompleto = "Luis Pérez Mendoza", Email = "luis.perez@email.com", Telefono = "987654322", Direccion = "Calle Los Olivos 456", DocumentoIdentidad = "23456789", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Cliente { NombreCompleto = "Carlos Ruiz Díaz", Email = "carlos.ruiz@email.com", Telefono = "987654323", Direccion = "Av. Universitaria 789", DocumentoIdentidad = "34567890", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Cliente { NombreCompleto = "Marta Díaz Flores", Email = "marta.diaz@email.com", Telefono = "987654324", Direccion = "Calle Las Flores 101", DocumentoIdentidad = "45678901", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Cliente { NombreCompleto = "Jorge Mora Sánchez", Email = "jorge.mora@email.com", Telefono = "987654325", Direccion = "Av. Los Pinos 202", DocumentoIdentidad = "56789012", Activo = true, FechaCreacion = DateTime.UtcNow }
        };
        context.Clientes.AddRange(clientes);
        context.SaveChanges();
    }

    // 5. Insertar Equipos (5 registros)
    if (!context.Equipos.Any())
    {
        var categoriaLaptops = context.Categorias.First(c => c.Nombre == "Laptops");
        var categoriaTablets = context.Categorias.First(c => c.Nombre == "Tablets");
        var categoriaProyectores = context.Categorias.First(c => c.Nombre == "Proyectores");
        var categoriaCamaras = context.Categorias.First(c => c.Nombre == "Cámaras");
        var categoriaAudio = context.Categorias.First(c => c.Nombre == "Audio");

        var marcaHP = context.Marcas.First(m => m.Nombre == "HP");
        var marcaApple = context.Marcas.First(m => m.Nombre == "Apple");
        var marcaSamsung = context.Marcas.First(m => m.Nombre == "Samsung");

        var equipos = new[]
        {
            new Equipo { Nombre = "Laptop HP Pavilion 15", Descripcion = "Intel i5, 8GB RAM, 512GB SSD", PrecioPorDia = 25.00m, Stock = 5, Especificaciones = "Pantalla 15.6\", Windows 11", CategoriaId = categoriaLaptops.Id, MarcaId = marcaHP.Id, Activo = true, FechaCreacion = DateTime.UtcNow },
            new Equipo { Nombre = "iPad Pro 12.9\"", Descripcion = "M2, 256GB, WiFi + Cellular", PrecioPorDia = 15.00m, Stock = 3, Especificaciones = "Pantalla Liquid Retina XDR", CategoriaId = categoriaTablets.Id, MarcaId = marcaApple.Id, Activo = true, FechaCreacion = DateTime.UtcNow },
            new Equipo { Nombre = "Proyector Epson EB-FH06", Descripcion = "Full HD, 3500 lúmenes", PrecioPorDia = 30.00m, Stock = 2, Especificaciones = "HDMI, USB, 3LCD", CategoriaId = categoriaProyectores.Id, MarcaId = context.Marcas.First(m => m.Nombre == "Epson").Id, Activo = true, FechaCreacion = DateTime.UtcNow },
            new Equipo { Nombre = "Cámara Canon EOS R", Descripcion = "30MP, 4K Video", PrecioPorDia = 40.00m, Stock = 2, Especificaciones = "Full Frame, RF Mount", CategoriaId = categoriaCamaras.Id, MarcaId = context.Marcas.First(m => m.Nombre == "Canon").Id, Activo = true, FechaCreacion = DateTime.UtcNow },
            new Equipo { Nombre = "Parlante JBL PartyBox 110", Descripcion = "Potencia 160W, Bluetooth", PrecioPorDia = 20.00m, Stock = 4, Especificaciones = "Batería integrada, luces LED", CategoriaId = categoriaAudio.Id, MarcaId = context.Marcas.First(m => m.Nombre == "JBL").Id, Activo = true, FechaCreacion = DateTime.UtcNow }
        };
        context.Equipos.AddRange(equipos);
        context.SaveChanges();
    }

    // 6. Insertar Reservas (5 registros)
    if (!context.Reservas.Any())
    {
        var cliente1 = context.Clientes.First(c => c.NombreCompleto.Contains("Ana"));
        var cliente2 = context.Clientes.First(c => c.NombreCompleto.Contains("Luis"));
        var cliente3 = context.Clientes.First(c => c.NombreCompleto.Contains("Carlos"));
        var cliente4 = context.Clientes.First(c => c.NombreCompleto.Contains("Marta"));
        var cliente5 = context.Clientes.First(c => c.NombreCompleto.Contains("Jorge"));

        var estadoPendiente = context.EstadosReserva.First(e => e.Nombre == "Pendiente");
        var estadoActiva = context.EstadosReserva.First(e => e.Nombre == "Activa");
        var estadoCompletada = context.EstadosReserva.First(e => e.Nombre == "Completada");

        // Fechas en UTC para PostgreSQL
        var fechaBase = DateTime.SpecifyKind(new DateTime(2025, 6, 1), DateTimeKind.Utc);

        var reservas = new[]
        {
            new Reserva {
                FechaInicio = DateTime.SpecifyKind(new DateTime(2025, 6, 1), DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(new DateTime(2025, 6, 5), DateTimeKind.Utc),
                MontoTotal = 100.00m,
                Observaciones = "Necesita cargador",
                ClienteId = cliente1.Id,
                EstadoReservaId = estadoActiva.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Reserva {
                FechaInicio = DateTime.SpecifyKind(new DateTime(2025, 6, 2), DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(new DateTime(2025, 6, 4), DateTimeKind.Utc),
                MontoTotal = 60.00m,
                Observaciones = "Para presentación",
                ClienteId = cliente2.Id,
                EstadoReservaId = estadoActiva.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Reserva {
                FechaInicio = DateTime.SpecifyKind(new DateTime(2025, 6, 1), DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(new DateTime(2025, 6, 3), DateTimeKind.Utc),
                MontoTotal = 50.00m,
                Observaciones = null,
                ClienteId = cliente3.Id,
                EstadoReservaId = estadoCompletada.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Reserva {
                FechaInicio = DateTime.SpecifyKind(new DateTime(2025, 6, 5), DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(new DateTime(2025, 6, 7), DateTimeKind.Utc),
                MontoTotal = 80.00m,
                Observaciones = "Requiere factura",
                ClienteId = cliente4.Id,
                EstadoReservaId = estadoPendiente.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Reserva {
                FechaInicio = DateTime.SpecifyKind(new DateTime(2025, 6, 3), DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(new DateTime(2025, 6, 6), DateTimeKind.Utc),
                MontoTotal = 120.00m,
                Observaciones = "Entrega después de 6pm",
                ClienteId = cliente5.Id,
                EstadoReservaId = estadoActiva.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };
        context.Reservas.AddRange(reservas);
        context.SaveChanges();
    }

    // 7. Insertar DetalleReservas (5 registros)
    if (!context.DetalleReservas.Any())
    {
        var reserva1 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("cargador"));
        var reserva2 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("presentación"));
        var reserva3 = context.Reservas.First(r => r.Observaciones == null);
        var reserva4 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("factura"));
        var reserva5 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("6pm"));

        var equipoHp = context.Equipos.First(e => e.Nombre.Contains("HP"));
        var equipoIpad = context.Equipos.First(e => e.Nombre.Contains("iPad"));
        var equipoEpson = context.Equipos.First(e => e.Nombre.Contains("Epson"));
        var equipoCanon = context.Equipos.First(e => e.Nombre.Contains("Canon"));
        var equipoJbl = context.Equipos.First(e => e.Nombre.Contains("JBL"));

        var detalleReservas = new[]
        {
            new DetalleReserva { Cantidad = 1, PrecioUnitarioPorDia = 25.00m, CantidadDias = 4, Subtotal = 100.00m, ReservaId = reserva1.Id, EquipoId = equipoHp.Id, FechaCreacion = DateTime.UtcNow },
            new DetalleReserva { Cantidad = 1, PrecioUnitarioPorDia = 15.00m, CantidadDias = 4, Subtotal = 60.00m, ReservaId = reserva2.Id, EquipoId = equipoIpad.Id, FechaCreacion = DateTime.UtcNow },
            new DetalleReserva { Cantidad = 1, PrecioUnitarioPorDia = 30.00m, CantidadDias = 2, Subtotal = 60.00m, ReservaId = reserva3.Id, EquipoId = equipoEpson.Id, FechaCreacion = DateTime.UtcNow },
            new DetalleReserva { Cantidad = 1, PrecioUnitarioPorDia = 40.00m, CantidadDias = 2, Subtotal = 80.00m, ReservaId = reserva4.Id, EquipoId = equipoCanon.Id, FechaCreacion = DateTime.UtcNow },
            new DetalleReserva { Cantidad = 2, PrecioUnitarioPorDia = 20.00m, CantidadDias = 3, Subtotal = 120.00m, ReservaId = reserva5.Id, EquipoId = equipoJbl.Id, FechaCreacion = DateTime.UtcNow }
        };
        context.DetalleReservas.AddRange(detalleReservas);
        context.SaveChanges();
    }

    // 8. Insertar Pagos (5 registros)
    if (!context.Pagos.Any())
    {
        var reserva1 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("cargador"));
        var reserva2 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("presentación"));
        var reserva3 = context.Reservas.First(r => r.Observaciones == null);
        var reserva5 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("6pm"));
        var reserva4 = context.Reservas.First(r => r.Observaciones != null && r.Observaciones.Contains("factura"));

        var pagos = new[]
        {
            new Pago {
                Monto = 100.00m,
                FechaPago = DateTime.SpecifyKind(new DateTime(2025, 6, 1), DateTimeKind.Utc),
                MetodoPago = "Tarjeta",
                ReferenciaPago = "TXN-001",
                ReservaId = reserva1.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Pago {
                Monto = 60.00m,
                FechaPago = DateTime.SpecifyKind(new DateTime(2025, 6, 2), DateTimeKind.Utc),
                MetodoPago = "Efectivo",
                ReferenciaPago = "TXN-002",
                ReservaId = reserva2.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Pago {
                Monto = 50.00m,
                FechaPago = DateTime.SpecifyKind(new DateTime(2025, 6, 1), DateTimeKind.Utc),
                MetodoPago = "Transferencia",
                ReferenciaPago = "TXN-003",
                ReservaId = reserva3.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Pago {
                Monto = 120.00m,
                FechaPago = DateTime.SpecifyKind(new DateTime(2025, 6, 3), DateTimeKind.Utc),
                MetodoPago = "Tarjeta",
                ReferenciaPago = "TXN-005",
                ReservaId = reserva5.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Pago {
                Monto = 80.00m,
                FechaPago = DateTime.SpecifyKind(new DateTime(2025, 6, 5), DateTimeKind.Utc),
                MetodoPago = "Efectivo",
                ReferenciaPago = "TXN-004",
                ReservaId = reserva4.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };
        context.Pagos.AddRange(pagos);
        context.SaveChanges();
    }
}
app.Run();