using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechRent.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogFieldsAndCreadoPor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Usuarios",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Usuarios",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Reservas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Reservas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Pagos",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Pagos",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Marcas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Marcas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "EstadosReserva",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "EstadosReserva",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Equipos",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Equipos",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "DetalleReservas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "DetalleReservas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Clientes",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Clientes",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualizadoPor",
                table: "Categorias",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Categorias",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Entidad",
                table: "AuditLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificadorEntidad",
                table: "AuditLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValorAnterior",
                table: "AuditLogs",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValorNuevo",
                table: "AuditLogs",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Marcas");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Marcas");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "EstadosReserva");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "EstadosReserva");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "DetalleReservas");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "DetalleReservas");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ActualizadoPor",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Entidad",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IdentificadorEntidad",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ValorAnterior",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ValorNuevo",
                table: "AuditLogs");
        }
    }
}
