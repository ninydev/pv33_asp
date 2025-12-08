using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLikeCountColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "likesCount",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "likesCount",
                table: "Posts");
        }
    }
}
