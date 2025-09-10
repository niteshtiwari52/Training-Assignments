using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieCatalogAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSETUP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BirthYear = table.Column<int>(type: "INTEGER", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DirectorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleaseYear = table.Column<int>(type: "INTEGER", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Directors_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Directors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Directors",
                columns: new[] { "Id", "BirthYear", "Name", "Nationality" },
                values: new object[,]
                {
                    { 1, 1970, "Christopher Nolan", "British-American" },
                    { 2, 1963, "Quentin Tarantino", "American" },
                    { 3, 1942, "Martin Scorsese", "American" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "DirectorId", "Genre", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Sci-Fi", 2010, "Inception" },
                    { 2, 1, "Action", 2008, "The Dark Knight" },
                    { 3, 2, "Crime", 1994, "Pulp Fiction" },
                    { 4, 3, "Crime", 2006, "The Departed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DirectorId",
                table: "Movies",
                column: "DirectorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Directors");
        }
    }
}
