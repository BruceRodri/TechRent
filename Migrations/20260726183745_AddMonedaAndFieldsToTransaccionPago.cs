using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechRent.Migrations
{
    /// <inheritdoc />
    public partial class AddMonedaAndFieldsToTransaccionPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntentosVerificacion",
                table: "TransaccionesPago",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MensajeRespuesta",
                table: "TransaccionesPago",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Moneda",
                table: "TransaccionesPago",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntentosVerificacion",
                table: "TransaccionesPago");

            migrationBuilder.DropColumn(
                name: "MensajeRespuesta",
                table: "TransaccionesPago");

            migrationBuilder.DropColumn(
                name: "Moneda",
                table: "TransaccionesPago");
        }
    }
}
