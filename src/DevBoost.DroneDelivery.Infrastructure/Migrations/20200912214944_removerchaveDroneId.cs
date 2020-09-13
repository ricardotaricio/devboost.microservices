﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBoost.DroneDelivery.Infrastructure.Migrations
{
    public partial class removerchaveDroneId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DroneId",
                table: "Pedido",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DroneId",
                table: "Pedido",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
