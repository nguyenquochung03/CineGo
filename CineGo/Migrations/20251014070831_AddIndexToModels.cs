using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineGo.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedAt",
                table: "Tickets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Status",
                table: "Tickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketCode",
                table: "Tickets",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UsedAt",
                table: "Tickets",
                column: "UsedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Theaters_Name_CinemaId",
                table: "Theaters",
                columns: new[] { "Name", "CinemaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_Date_MovieId",
                table: "Showtimes",
                columns: new[] { "Date", "MovieId" });

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_Date_TheaterId",
                table: "Showtimes",
                columns: new[] { "Date", "TheaterId" });

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_TheaterId_MovieId_Date_StartTime",
                table: "Showtimes",
                columns: new[] { "TheaterId", "MovieId", "Date", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimePrices_SeatType",
                table: "ShowtimePrices",
                column: "SeatType");

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimePrices_ShowtimeId_TicketType_SeatType",
                table: "ShowtimePrices",
                columns: new[] { "ShowtimeId", "TicketType", "SeatType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimePrices_TicketType",
                table: "ShowtimePrices",
                column: "TicketType");

            migrationBuilder.CreateIndex(
                name: "IX_SeatStatuses_ShowtimeId_SeatId",
                table: "SeatStatuses",
                columns: new[] { "ShowtimeId", "SeatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Column",
                table: "Seats",
                column: "Column");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Label",
                table: "Seats",
                column: "Label");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Row",
                table: "Seats",
                column: "Row");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_TheaterId_Row_Column",
                table: "Seats",
                columns: new[] { "TheaterId", "Row", "Column" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CreatedAt",
                table: "Reviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieId_UserId",
                table: "Reviews",
                columns: new[] { "MovieId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code_IsActive",
                table: "PromoCodes",
                columns: new[] { "Code", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_IsActive",
                table: "PromoCodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_ValidFrom",
                table: "PromoCodes",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_ValidTo",
                table: "PromoCodes",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_PricingRules_CreatedAt",
                table: "PricingRules",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PricingRules_IsActive",
                table: "PricingRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PricingRules_Name",
                table: "PricingRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricingRuleDays_PricingRuleId_DayName",
                table: "PricingRuleDays",
                columns: new[] { "PricingRuleId", "DayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricingDetails_PricingRuleId_TicketType_SeatType",
                table: "PricingDetails",
                columns: new[] { "PricingRuleId", "TicketType", "SeatType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricingDetails_SeatType",
                table: "PricingDetails",
                column: "SeatType");

            migrationBuilder.CreateIndex(
                name: "IX_PricingDetails_TicketType",
                table: "PricingDetails",
                column: "TicketType");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Method",
                table: "Payments",
                column: "Method");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProviderTxnId",
                table: "Payments",
                column: "ProviderTxnId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_TicketType",
                table: "OrderItems",
                columns: new[] { "OrderId", "TicketType" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_TicketType",
                table: "OrderItems",
                column: "TicketType");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AgeLimit",
                table: "Movies",
                column: "AgeLimit");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Rating",
                table: "Movies",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ReleaseDate",
                table: "Movies",
                column: "ReleaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Slug",
                table: "Movies",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_MoviePoster_MovieId_Order",
                table: "MoviePoster",
                columns: new[] { "MovieId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_RegionId",
                table: "Cities",
                columns: new[] { "Name", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cinemas_Name_CityId",
                table: "Cinemas",
                columns: new[] { "Name", "CityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Phone",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Role",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CreatedAt",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Status",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketCode",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UsedAt",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Theaters_Name_CinemaId",
                table: "Theaters");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_Date_MovieId",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_Date_TheaterId",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_TheaterId_MovieId_Date_StartTime",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_ShowtimePrices_SeatType",
                table: "ShowtimePrices");

            migrationBuilder.DropIndex(
                name: "IX_ShowtimePrices_ShowtimeId_TicketType_SeatType",
                table: "ShowtimePrices");

            migrationBuilder.DropIndex(
                name: "IX_ShowtimePrices_TicketType",
                table: "ShowtimePrices");

            migrationBuilder.DropIndex(
                name: "IX_SeatStatuses_ShowtimeId_SeatId",
                table: "SeatStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Seats_Column",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_Label",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_Row",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_TheaterId_Row_Column",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_MovieId_UserId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_Rating",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Regions_Name",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code_IsActive",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_IsActive",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_ValidFrom",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_ValidTo",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PricingRules_CreatedAt",
                table: "PricingRules");

            migrationBuilder.DropIndex(
                name: "IX_PricingRules_IsActive",
                table: "PricingRules");

            migrationBuilder.DropIndex(
                name: "IX_PricingRules_Name",
                table: "PricingRules");

            migrationBuilder.DropIndex(
                name: "IX_PricingRuleDays_PricingRuleId_DayName",
                table: "PricingRuleDays");

            migrationBuilder.DropIndex(
                name: "IX_PricingDetails_PricingRuleId_TicketType_SeatType",
                table: "PricingDetails");

            migrationBuilder.DropIndex(
                name: "IX_PricingDetails_SeatType",
                table: "PricingDetails");

            migrationBuilder.DropIndex(
                name: "IX_PricingDetails_TicketType",
                table: "PricingDetails");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Method",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ProviderTxnId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_TicketType",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_TicketType",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Movies_AgeLimit",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Rating",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_ReleaseDate",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Slug",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Title",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_MoviePoster_MovieId_Order",
                table: "MoviePoster");

            migrationBuilder.DropIndex(
                name: "IX_Cities_Name_RegionId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cinemas_Name_CityId",
                table: "Cinemas");
        }
    }
}
