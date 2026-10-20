using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class FixDuplicateCountrySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                column: "CountryName",
                value: "Greece");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                column: "CountryName",
                value: "China");
        }
    }
}
