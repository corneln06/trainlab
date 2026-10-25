using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainLab.Modules.Profile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "athlete_profiles",
                schema: "profile",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sex = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    weight_kg = table.Column<double>(type: "double precision", nullable: true),
                    resting_hr = table.Column<int>(type: "integer", nullable: true),
                    max_hr = table.Column<int>(type: "integer", nullable: true),
                    time_zone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_athlete_profiles", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_athlete_profiles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "profile",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "athlete_profiles",
                schema: "profile");
        }
    }
}