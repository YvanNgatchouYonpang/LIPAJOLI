using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIPAJOLI.Migrations
{
    /// <inheritdoc />
    public partial class initale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Livres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: true),
                    ISBN10 = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ISBN13 = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Auteurs = table.Column<string>(type: "TEXT", nullable: true),
                    Categorie = table.Column<string>(type: "TEXT", nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    Prix = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usagers",
                columns: table => new
                {
                    NoAbonne = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    Defaillance = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usagers", x => x.NoAbonne);
                });

            migrationBuilder.CreateTable(
                name: "Exemplaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Etat = table.Column<string>(type: "TEXT", nullable: false),
                    CodeLivre = table.Column<string>(type: "TEXT", nullable: true),
                    LivreId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exemplaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exemplaires_Livres_LivreId",
                        column: x => x.LivreId,
                        principalTable: "Livres",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Emprunts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateEmprunt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateLimiteRetour = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRetour = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LivreId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsagerNoAbonne = table.Column<string>(type: "TEXT", nullable: false),
                    ExemplaireId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprunts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emprunts_Exemplaires_ExemplaireId",
                        column: x => x.ExemplaireId,
                        principalTable: "Exemplaires",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Emprunts_Livres_LivreId",
                        column: x => x.LivreId,
                        principalTable: "Livres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emprunts_Usagers_UsagerNoAbonne",
                        column: x => x.UsagerNoAbonne,
                        principalTable: "Usagers",
                        principalColumn: "NoAbonne",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_ExemplaireId",
                table: "Emprunts",
                column: "ExemplaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_LivreId",
                table: "Emprunts",
                column: "LivreId");

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_UsagerNoAbonne",
                table: "Emprunts",
                column: "UsagerNoAbonne");

            migrationBuilder.CreateIndex(
                name: "IX_Exemplaires_LivreId",
                table: "Exemplaires",
                column: "LivreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emprunts");

            migrationBuilder.DropTable(
                name: "Exemplaires");

            migrationBuilder.DropTable(
                name: "Usagers");

            migrationBuilder.DropTable(
                name: "Livres");
        }
    }
}
