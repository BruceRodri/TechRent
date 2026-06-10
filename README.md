# TechRent - Sistema de Alquiler de Equipos Tecnológicos

![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

## Descripción

TechRent es una aplicación web desarrollada con ASP.NET Core MVC, Entity Framework Core y PostgreSQL que permite gestionar el alquiler de equipos tecnológicos como laptops, tablets, proyectores, cámaras y equipos de audio.

Este proyecto fue desarrollado para la asignatura Des Web Para Integ De Tecnol y tiene como objetivo implementar una aplicación web con arquitectura MVC, persistencia de datos mediante Entity Framework Core y una base de datos PostgreSQL.

---

## Características

* Gestión de categorías de equipos.
* Gestión de marcas.
* Gestión de equipos tecnológicos.
* Gestión de clientes.
* Gestión de reservas.
* Gestión de pagos.
* Base de datos PostgreSQL.
* Entity Framework Core con migraciones.
* Arquitectura MVC.
* Eliminación lógica mediante campo Activo.
* Campos de auditoría para seguimiento de registros.

---

## Tecnologías Utilizadas

| Tecnología            | Versión        |
| --------------------- | -------------- |
| ASP.NET Core MVC      | 10.0           |
| Entity Framework Core | 10.0           |
| PostgreSQL            | 18             |
| Bootstrap             | 5              |
| Bootstrap Icons       | 1.11           |
| Visual Studio Code    | Última versión |

---

## Modelo de Base de Datos

La base de datos está compuesta por las siguientes tablas:

| Tabla           | Descripción                            |
| --------------- | -------------------------------------- |
| Categorias      | Clasificación de equipos               |
| Marcas          | Fabricantes de equipos                 |
| Equipos         | Equipos disponibles para alquiler      |
| Clientes        | Personas que realizan alquileres       |
| EstadosReserva  | Estados de las reservas                |
| Reservas        | Información principal de cada alquiler |
| DetalleReservas | Equipos incluidos en cada reserva      |
| Pagos           | Registro de pagos realizados           |

---

## Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/BruceRodri/TechRent.git
cd TechRent
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Configurar PostgreSQL

Crear una base de datos llamada:

```sql
CREATE DATABASE "TechRentDB";
```

### 4. Configurar la cadena de conexión

Editar el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TechRentDB;Username=postgres;Password=TU_PASSWORD"
  }
}
```

### 5. Aplicar migraciones

```bash
dotnet ef database update
```

### 6. Ejecutar el proyecto

```bash
dotnet run
```

### 7. Abrir en el navegador

La aplicación estará disponible en una dirección similar a:

```text
https://localhost:5001
```

o

```text
http://localhost:5120
```

---

## Estructura del Proyecto

```text
TechRent
│
├── Controllers
├── Data
├── Models
├── Views
├── wwwroot
├── Migrations
├── Properties
├── Program.cs
├── appsettings.json
└── TechRent.csproj
```

---

## Funcionalidades Implementadas

### CRUD de Equipos

* Crear equipos.
* Consultar equipos.
* Editar equipos.
* Visualizar detalles.
* Eliminación lógica.

### CRUD de Clientes

* Crear clientes.
* Consultar clientes.
* Editar clientes.
* Visualizar detalles.
* Eliminación lógica.

---

## Escalabilidad del Proyecto

La estructura fue diseñada para soportar futuras mejoras como:

* Carga masiva de más de 500.000 registros.
* Implementación de paginación.
* Filtros avanzados de búsqueda.
* Reportes estadísticos.
* Gestión de usuarios y roles.
* Control de sesiones.
* Dashboard administrativo.

---

## Autor

Bruce Rodriguez

Proyecto académico desarrollado para la asignatura Des Web Para Integ De Tecnol
