using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainLab.Modules.Profile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddZoneSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "zone_sets",
                schema: "profile",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sport = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    lthr = table.Column<int>(type: "integer", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_zone_sets", x => x.id);
                    table.ForeignKey(
                        name: "fk_zone_sets_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "profile",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_zone_sets_user_id_sport_effective_from",
                schema: "profile",
                table: "zone_sets",
                columns: new[] { "user_id", "sport", "effective_from" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "zone_sets",
                schema: "profile");
        }
    }
}