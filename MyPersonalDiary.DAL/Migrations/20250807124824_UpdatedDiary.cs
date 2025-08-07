using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPersonalDiary.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiaryImages_DiaryRecords_RecordId",
                table: "DiaryImages");

            migrationBuilder.DropIndex(
                name: "IX_DiaryImages_RecordId",
                table: "DiaryImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DiaryRecords");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "DiaryImages");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "DiaryImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DiaryImages_UserId",
                table: "DiaryImages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaryImages_AspNetUsers_UserId",
                table: "DiaryImages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiaryImages_AspNetUsers_UserId",
                table: "DiaryImages");

            migrationBuilder.DropIndex(
                name: "IX_DiaryImages_UserId",
                table: "DiaryImages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DiaryImages");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DiaryRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RecordId",
                table: "DiaryImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DiaryImages_RecordId",
                table: "DiaryImages",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaryImages_DiaryRecords_RecordId",
                table: "DiaryImages",
                column: "RecordId",
                principalTable: "DiaryRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
