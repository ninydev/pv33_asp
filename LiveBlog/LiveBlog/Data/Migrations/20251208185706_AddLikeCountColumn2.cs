using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLikeCountColumn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "likesCount",
                table: "Posts",
                newName: "LikesCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LikesCount",
                table: "Posts",
                newName: "likesCount");
        }
    }
}
