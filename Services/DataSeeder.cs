using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Services
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAllData()
        {
            // 1. Categorías (10)
            if (!_context.Categorias.Any())
            {
                var categorias = new[]
                {
                    new Categoria { Nombre = "Laptops", Descripcion = "Computadoras portátiles para trabajo, estudio y entretenimiento", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Tablets", Descripcion = "Dispositivos móviles con pantalla táctil ideales para lectura y diseño", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Proyectores", Descripcion = "Equipos de proyección para presentaciones empresariales y educativas", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Cámaras", Descripcion = "Cámaras fotográficas y de video profesionales para eventos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Audio", Descripcion = "Equipos de sonido, parlantes y micrófonos para conferencias", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Monitores", Descripcion = "Pantallas y monitores para estaciones de trabajo y gaming", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Accesorios", Descripcion = "Periféricos y accesorios tecnológicos como teclados y mouses", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Redes", Descripcion = "Equipos de conectividad como routers, switches y access points", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Software", Descripcion = "Licencias y programas de software empresarial y de diseño", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Categoria { Nombre = "Servidores", Descripcion = "Servidores y equipos de infraestructura para centros de datos", Activo = true, FechaCreacion = DateTime.UtcNow }
                };
                await _context.Categorias.AddRangeAsync(categorias);
                await _context.SaveChangesAsync();
            }

            // 2. Marcas (20)
            if (!_context.Marcas.Any())
            {
                var marcas = new[]
                {
                    new Marca { Nombre = "Dell", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "HP", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Lenovo", PaisOrigen = "China", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Apple", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Samsung", PaisOrigen = "Corea del Sur", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Sony", PaisOrigen = "Japón", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "LG", PaisOrigen = "Corea del Sur", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Asus", PaisOrigen = "Taiwán", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Acer", PaisOrigen = "Taiwán", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Microsoft", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Canon", PaisOrigen = "Japón", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Epson", PaisOrigen = "Japón", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Cisco", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Xiaomi", PaisOrigen = "China", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Huawei", PaisOrigen = "China", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Logitech", PaisOrigen = "Suiza", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Seagate", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Kingston", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Nvidia", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new Marca { Nombre = "Intel", PaisOrigen = "Estados Unidos", Activo = true, FechaCreacion = DateTime.UtcNow }
                };
                await _context.Marcas.AddRangeAsync(marcas);
                await _context.SaveChangesAsync();
            }

            // 3. EstadosReserva (4)
            if (!_context.EstadosReserva.Any())
            {
                var estados = new[]
                {
                    new EstadoReserva { Nombre = "Pendiente", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new EstadoReserva { Nombre = "Activa", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new EstadoReserva { Nombre = "Completada", Activo = true, FechaCreacion = DateTime.UtcNow },
                    new EstadoReserva { Nombre = "Cancelada", Activo = true, FechaCreacion = DateTime.UtcNow }
                };
                await _context.EstadosReserva.AddRangeAsync(estados);
                await _context.SaveChangesAsync();
            }

            // 4. Equipos (30,000)
            if (!_context.Equipos.Any())
            {
                var categorias = await _context.Categorias.ToListAsync();
                var marcas = await _context.Marcas.ToListAsync();
                var equipos = new List<Equipo>();
                var random = new Random();
                var tiposEquipo = new[]
                {
                    "Laptop", "Ultrabook", "Notebook", "Workstation", "Tablet",
                    "Monitor", "Proyector", "Cámara DSLR", "Cámara Mirrorless", "Cámara Deportiva",
                    "Audífonos", "Parlante Bluetooth", "Micrófono", "Barra de Sonido", "Subwoofer",
                    "Router", "Switch", "Access Point", "Firewall", "Módem",
                    "Servidor", "NAS", "Disco SSD", "Disco HDD", "Memoria RAM",
                    "Tarjeta Gráfica", "Fuente de Poder", "Teclado", "Mouse", "Webcam",
                    "Hub USB", "Docking Station", "Impresora", "Escáner", "UPS",
                    "Tablet Gráfica", "Lápiz Óptico", "Lector Huella", "Cargador", "Base Enfriadora"
                };
                var descripciones = new[]
                {
                    "Equipo de alto rendimiento ideal para uso profesional",
                    "Perfecto para el hogar y la oficina con gran durabilidad",
                    "Última generación con tecnología avanzada y eficiencia energética",
                    "Versátil y portátil, ideal para viajes y movilidad",
                    "Diseño elegante con materiales premium y acabados de calidad",
                    "Rendimiento óptimo para tareas exigentes y multitarea",
                    "Equipo certificado con garantía internacional de calidad",
                    "Solución innovadora con conectividad de última generación",
                    "Máxima potencia para aplicaciones profesionales y creativas",
                    "Equipo confiable con soporte técnico especializado incluido",
                    "Tecnología de punta para maximizar tu productividad diaria",
                    "Compacto y eficiente, ahorra espacio sin sacrificar rendimiento",
                    "Construido con materiales ecológicos y reciclables",
                    "Sistema robusto con refrigeración avanzada y bajo ruido",
                    "Equipo multifuncional con amplia compatibilidad de software"
                };

                for (int i = 1; i <= 30000; i++)
                {
                    var tipo = tiposEquipo[random.Next(tiposEquipo.Length)];
                    var descripcion = descripciones[random.Next(descripciones.Length)];
                    var categoria = categorias[random.Next(categorias.Count)];
                    var marca = marcas[random.Next(marcas.Count)];
                    var modelo = random.Next(1000, 9999);

                    equipos.Add(new Equipo
                    {
                        Nombre = $"{tipo} {marca.Nombre} {modelo}",
                        Descripcion = $"{descripcion}. Modelo {modelo} de {marca.Nombre}",
                        PrecioPorDia = Math.Round((decimal)(random.NextDouble() * 100 + 10), 2),
                        Stock = random.Next(1, 50),
                        Especificaciones = $"Procesador: Última generación | Almacenamiento: 512GB | Conectividad: WiFi 6",
                        CategoriaId = categoria.Id,
                        MarcaId = marca.Id,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });

                    if (i % 5000 == 0)
                    {
                        await _context.Equipos.AddRangeAsync(equipos);
                        await _context.SaveChangesAsync();
                        equipos.Clear();
                        Console.WriteLine($"Equipos: {i} registros insertados");
                    }
                }

                if (equipos.Any())
                {
                    await _context.Equipos.AddRangeAsync(equipos);
                    await _context.SaveChangesAsync();
                }
            }

            // 5. Clientes (80,000)
            if (!_context.Clientes.Any())
            {
                var clientes = new List<Cliente>();
                var random = new Random();
                var nombres = new[]
                {
                    "Ana", "Luis", "Carlos", "Marta", "Jorge", "Maria", "Pedro", "Juan", "Sofia", "Diego",
                    "Elena", "Pablo", "Laura", "Miguel", "Carmen", "David", "Andrea", "Jose", "Lucia", "Antonio",
                    "Isabel", "Francisco", "Sara", "Manuel", "Teresa", "Ricardo", "Patricia", "Javier", "Rosa", "Fernando",
                    "Monica", "Gabriel", "Claudia", "Alberto", "Silvia", "Andres", "Carolina", "Daniel", "Valentina", "Sergio",
                    "Natalia", "Enrique", "Marcela", "Rafael", "Adriana", "Oscar", "Mariana", "Hugo", "Paola", "Ivan"
                };
                var apellidos = new[]
                {
                    "Lopez", "Perez", "Ruiz", "Diaz", "Mora", "Garcia", "Sanchez", "Ramirez", "Torres", "Vargas",
                    "Castro", "Ortiz", "Rios", "Medina", "Cruz", "Vega", "Mendoza", "Guerrero", "Rojas", "Soto",
                    "Herrera", "Peña", "Campos", "Flores", "Aguilar", "Morales", "Jimenez", "Navarro", "Salazar", "Delgado",
                    "Castillo", "Paredes", "Cordova", "Leon", "Valdez", "Espinoza", "Sandoval", "Pacheco", "Solis", "Melendez"
                };

                for (int i = 1; i <= 80000; i++)
                {
                    var idxNombre = (i - 1) % nombres.Length;
                    var idxApellido1 = ((i - 1) / nombres.Length) % apellidos.Length;
                    var idxApellido2 = (i - 1) / (nombres.Length * apellidos.Length);
                    var nombreCompleto = $"{nombres[idxNombre]} {apellidos[idxApellido1]} {apellidos[idxApellido2]}";

                    clientes.Add(new Cliente
                    {
                        NombreCompleto = nombreCompleto,
                        Email = $"cliente{i}@email.com",
                        Telefono = $"099{i:D7}",
                        Direccion = $"Dirección {i}",
                        DocumentoIdentidad = $"{i:D10}",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });

                    if (i % 10000 == 0)
                    {
                        await _context.Clientes.AddRangeAsync(clientes);
                        await _context.SaveChangesAsync();
                        clientes.Clear();
                        Console.WriteLine($"Clientes: {i} registros insertados");
                    }
                }

                if (clientes.Any())
                {
                    await _context.Clientes.AddRangeAsync(clientes);
                    await _context.SaveChangesAsync();
                }
            }

            // 6. Reservas (150,000)
            if (!_context.Reservas.Any())
            {
                var clientes = await _context.Clientes.ToListAsync();
                var estados = await _context.EstadosReserva.ToListAsync();
                var reservas = new List<Reserva>();
                var random = new Random();
                var fechaInicio = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                for (int i = 1; i <= 150000; i++)
                {
                    var dias = random.Next(1, 30);
                    var inicio = fechaInicio.AddDays(random.Next(0, 800));
                    reservas.Add(new Reserva
                    {
                        FechaInicio = inicio,
                        FechaFin = inicio.AddDays(dias),
                        MontoTotal = 0,
                        Observaciones = i % 10 == 0 ? $"Observación de reserva {i}" : null,
                        ClienteId = clientes[random.Next(clientes.Count)].Id,
                        EstadoReservaId = estados[random.Next(estados.Count)].Id,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });

                    if (i % 10000 == 0)
                    {
                        await _context.Reservas.AddRangeAsync(reservas);
                        await _context.SaveChangesAsync();
                        reservas.Clear();
                        Console.WriteLine($"Reservas: {i} registros insertados");
                    }
                }

                if (reservas.Any())
                {
                    await _context.Reservas.AddRangeAsync(reservas);
                    await _context.SaveChangesAsync();
                }
            }

            // 7. DetalleReservas + actualizar stock de equipos
            if (!_context.DetalleReservas.Any())
            {
                var reservas = await _context.Reservas.ToListAsync();
                var equiposList = await _context.Equipos.ToListAsync();
                var detalles = new List<DetalleReserva>();
                var random = new Random();
                var totalRequerido = 239966;
                var contador = 0;
                var stockDict = equiposList.ToDictionary(e => e.Id, e => e.Stock);
                var actualizarStock = new Dictionary<int, int>();

                for (int i = 1; i <= totalRequerido; i++)
                {
                    var equipoValido = equiposList.Where(e => stockDict[e.Id] > 0).ToList();
                    if (equipoValido.Count == 0) break;

                    var equipo = equipoValido[random.Next(equipoValido.Count)];
                    var cantidad = Math.Min(random.Next(1, 5), stockDict[equipo.Id]);
                    stockDict[equipo.Id] -= cantidad;
                    actualizarStock[equipo.Id] = stockDict[equipo.Id];

                    var dias = random.Next(1, 15);
                    var subtotal = cantidad * equipo.PrecioPorDia * dias;
                    detalles.Add(new DetalleReserva
                    {
                        Cantidad = cantidad,
                        PrecioUnitarioPorDia = equipo.PrecioPorDia,
                        CantidadDias = dias,
                        Subtotal = subtotal,
                        ReservaId = reservas[random.Next(reservas.Count)].Id,
                        EquipoId = equipo.Id,
                        FechaCreacion = DateTime.UtcNow
                    });

                    contador++;
                    if (contador % 20000 == 0)
                    {
                        await _context.DetalleReservas.AddRangeAsync(detalles);
                        await _context.SaveChangesAsync();
                        detalles.Clear();

                        foreach (var kvp in actualizarStock)
                        {
                            var dbEquipo = await _context.Equipos.FindAsync(kvp.Key);
                            if (dbEquipo != null)
                                dbEquipo.Stock = Math.Max(0, kvp.Value);
                        }
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"DetalleReservas: {contador} de {totalRequerido} insertados");
                    }
                }

                if (detalles.Any())
                {
                    await _context.DetalleReservas.AddRangeAsync(detalles);
                    await _context.SaveChangesAsync();

                    foreach (var kvp in actualizarStock)
                    {
                        var dbEquipo = await _context.Equipos.FindAsync(kvp.Key);
                        if (dbEquipo != null)
                            dbEquipo.Stock = Math.Max(0, kvp.Value);
                    }
                    await _context.SaveChangesAsync();
                }

                // Recalcular MontoTotal de cada reserva según sus detalles
                foreach (var reserva in reservas)
                {
                    var sumaDetalles = await _context.DetalleReservas
                        .Where(d => d.ReservaId == reserva.Id)
                        .SumAsync(d => d.Subtotal);
                    if (sumaDetalles > 0 && reserva.MontoTotal != sumaDetalles)
                    {
                        reserva.MontoTotal = sumaDetalles;
                    }
                }
                await _context.SaveChangesAsync();
            }

            // 8. Usuario administrador
            if (!await _context.Usuarios.AnyAsync())
            {
                var admin = new Usuario
                {
                    NombreCompleto = "Administrador",
                    Email = "admin@techrent.com",
                    Password = "123456",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };
                _context.Usuarios.Add(admin);
                await _context.SaveChangesAsync();
                Console.WriteLine("Usuario admin creado");
            }

            // Mostrar total de registros
            var totalCategorias = await _context.Categorias.CountAsync();
            var totalMarcas = await _context.Marcas.CountAsync();
            var totalEquipos = await _context.Equipos.CountAsync();
            var totalClientes = await _context.Clientes.CountAsync();
            var totalEstados = await _context.EstadosReserva.CountAsync();
            var totalReservas = await _context.Reservas.CountAsync();
            var totalDetalles = await _context.DetalleReservas.CountAsync();

            var totalGeneral = totalCategorias + totalMarcas + totalEquipos + totalClientes +
                               totalEstados + totalReservas + totalDetalles;

            Console.WriteLine($"=== TOTAL DE REGISTROS ===");
            Console.WriteLine($"Categorías: {totalCategorias}");
            Console.WriteLine($"Marcas: {totalMarcas}");
            Console.WriteLine($"Equipos: {totalEquipos}");
            Console.WriteLine($"Clientes: {totalClientes}");
            Console.WriteLine($"EstadosReserva: {totalEstados}");
            Console.WriteLine($"Reservas: {totalReservas}");
            Console.WriteLine($"DetalleReservas: {totalDetalles}");
            Console.WriteLine($"TOTAL GENERAL: {totalGeneral}");
        }
    }
}