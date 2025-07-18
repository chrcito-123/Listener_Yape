using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Listener_Yape.Migrations
{
    /// <inheritdoc />
    public partial class AddVistaToNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mensaje",
                table: "Notificaciones",
                newName: "MensajeCompleto");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Notificaciones",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<int>(
                name: "AplicacionId",
                table: "Notificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Aplicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PackageName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aplicaciones", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Parametros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Clave = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Valor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TokenRevocados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaRevocado = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenRevocados", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UsuarioAplicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    AplicacionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioAplicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioAplicaciones_Aplicaciones_AplicacionId",
                        column: x => x.AplicacionId,
                        principalTable: "Aplicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioAplicaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_AplicacionId",
                table: "Notificaciones",
                column: "AplicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAplicaciones_AplicacionId",
                table: "UsuarioAplicaciones",
                column: "AplicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAplicaciones_UsuarioId",
                table: "UsuarioAplicaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Aplicaciones_AplicacionId",
                table: "Notificaciones",
                column: "AplicacionId",
                principalTable: "Aplicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Aplicaciones_AplicacionId",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioId",
                table: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Parametros");

            migrationBuilder.DropTable(
                name: "TokenRevocados");

            migrationBuilder.DropTable(
                name: "UsuarioAplicaciones");

            migrationBuilder.DropTable(
                name: "Aplicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_AplicacionId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "AplicacionId",
                table: "Notificaciones");

            migrationBuilder.RenameColumn(
                name: "MensajeCompleto",
                table: "Notificaciones",
                newName: "Mensaje");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Notificaciones",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);
        }
    }
}
