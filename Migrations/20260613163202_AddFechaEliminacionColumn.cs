using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechRent.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaEliminacionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Categorias"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""Marcas"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""Equipos"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""Clientes"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""EstadosReserva"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""Reservas"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""DetalleReservas"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
                ALTER TABLE ""Pagos"" ADD COLUMN IF NOT EXISTS ""FechaEliminacion"" timestamp with time zone;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Categorias"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""Marcas"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""Equipos"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""Clientes"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""EstadosReserva"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""Reservas"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""DetalleReservas"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
                ALTER TABLE ""Pagos"" DROP COLUMN IF EXISTS ""FechaEliminacion"";
            ");
        }
    }
}
