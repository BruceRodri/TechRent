using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TechRent.Migrations
{
    /// <inheritdoc />
    public partial class AddPagosYOrdenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenesAlquiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioEmail = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesAlquiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetallesOrdenAlquiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenAlquilerId = table.Column<int>(type: "integer", nullable: false),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    NombreEquipo = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Dias = table.Column<int>(type: "integer", nullable: false),
                    PrecioPorDia = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrdenAlquiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenAlquiler_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenAlquiler_OrdenesAlquiler_OrdenAlquilerId",
                        column: x => x.OrdenAlquilerId,
                        principalTable: "OrdenesAlquiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransaccionesPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenAlquilerId = table.Column<int>(type: "integer", nullable: false),
                    Proveedor = table.Column<string>(type: "text", nullable: false),
                    ClientTransactionId = table.Column<string>(type: "text", nullable: true),
                    PayphonePaymentUrl = table.Column<string>(type: "text", nullable: true),
                    PayPalOrderId = table.Column<string>(type: "text", nullable: true),
                    PayPalCaptureId = table.Column<string>(type: "text", nullable: true),
                    PayPalApprovalUrl = table.Column<string>(type: "text", nullable: true),
                    MontoEnCentavos = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    RespuestaGateway = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaccionesPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransaccionesPago_OrdenesAlquiler_OrdenAlquilerId",
                        column: x => x.OrdenAlquilerId,
                        principalTable: "OrdenesAlquiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenAlquiler_EquipoId",
                table: "DetallesOrdenAlquiler",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenAlquiler_OrdenAlquilerId",
                table: "DetallesOrdenAlquiler",
                column: "OrdenAlquilerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionesPago_OrdenAlquilerId",
                table: "TransaccionesPago",
                column: "OrdenAlquilerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesOrdenAlquiler");

            migrationBuilder.DropTable(
                name: "TransaccionesPago");

            migrationBuilder.DropTable(
                name: "OrdenesAlquiler");
        }
    }
}
