using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechRent.Migrations
{
    /// <inheritdoc />
    public partial class AddEliminadoPorField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Usuarios",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Reservas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Pagos",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Marcas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "EstadosReserva",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Equipos",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "DetalleReservas",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Clientes",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Categorias",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Marcas");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "EstadosReserva");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "DetalleReservas");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Categorias");
        }
    }
}
