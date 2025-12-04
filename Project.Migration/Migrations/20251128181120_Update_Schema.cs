using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migration.Migrations
{
    /// <inheritdoc />
    public partial class Update_Schema : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaasPlanFeatures_SaaSPlans_PlanId",
                table: "SaasPlanFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_SaasTenantSubscriptions_SaaSPlans_PlanId",
                table: "SaasTenantSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaaSPlans",
                table: "SaaSPlans");

            migrationBuilder.RenameTable(
                name: "SaaSPlans",
                newName: "SaasPlans");

            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "SaaSWebhookLogs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "SaaSWebhookLogs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ProrationAmount",
                table: "SaasTenantSubscriptionPayments",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "SaasTenantSubscriptionPayments",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "SaaSSubscriptionAudits",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundAmount",
                table: "SaaSRefunds",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SaasPlans",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SaasPlans",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SaasPlans",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "SaasPlans",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "SaaSInvoices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "SaaSInvoices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "SaaSInvoices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SaaSFeatures",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "SaaSFeatures",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SaaSFeatures",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "SaasCoupons",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SaasCoupons",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SaaSAddons",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SaaSAddons",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaasPlans",
                table: "SaasPlans",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SaaSWebhookLogs_EventId",
                table: "SaaSWebhookLogs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantSubscriptions_TenantId",
                table: "SaasTenantSubscriptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantPaymentMethods_TenantId_IsDefault",
                table: "SaasTenantPaymentMethods",
                columns: new[] { "TenantId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantCoupons_TenantId_CouponId",
                table: "SaasTenantCoupons",
                columns: new[] { "TenantId", "CouponId" });

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantAddons_TenantId",
                table: "SaasTenantAddons",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SaaSSubscriptionAudits_TenantId",
                table: "SaaSSubscriptionAudits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SaaSInvoices_InvoiceNumber",
                table: "SaaSInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaaSFeatureUsages_TenantId_FeatureName",
                table: "SaaSFeatureUsages",
                columns: new[] { "TenantId", "FeatureName" });

            migrationBuilder.CreateIndex(
                name: "IX_SaasCoupons_Code",
                table: "SaasCoupons",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SaasPlanFeatures_SaasPlans_PlanId",
                table: "SaasPlanFeatures",
                column: "PlanId",
                principalTable: "SaasPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaasTenantSubscriptions_SaasPlans_PlanId",
                table: "SaasTenantSubscriptions",
                column: "PlanId",
                principalTable: "SaasPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaasPlanFeatures_SaasPlans_PlanId",
                table: "SaasPlanFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_SaasTenantSubscriptions_SaasPlans_PlanId",
                table: "SaasTenantSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SaaSWebhookLogs_EventId",
                table: "SaaSWebhookLogs");

            migrationBuilder.DropIndex(
                name: "IX_SaasTenantSubscriptions_TenantId",
                table: "SaasTenantSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SaasTenantPaymentMethods_TenantId_IsDefault",
                table: "SaasTenantPaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_SaasTenantCoupons_TenantId_CouponId",
                table: "SaasTenantCoupons");

            migrationBuilder.DropIndex(
                name: "IX_SaasTenantAddons_TenantId",
                table: "SaasTenantAddons");

            migrationBuilder.DropIndex(
                name: "IX_SaaSSubscriptionAudits_TenantId",
                table: "SaaSSubscriptionAudits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaasPlans",
                table: "SaasPlans");

            migrationBuilder.DropIndex(
                name: "IX_SaaSInvoices_InvoiceNumber",
                table: "SaaSInvoices");

            migrationBuilder.DropIndex(
                name: "IX_SaaSFeatureUsages_TenantId_FeatureName",
                table: "SaaSFeatureUsages");

            migrationBuilder.DropIndex(
                name: "IX_SaasCoupons_Code",
                table: "SaasCoupons");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "SaaSWebhookLogs");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "SaaSWebhookLogs");

            migrationBuilder.RenameTable(
                name: "SaasPlans",
                newName: "SaaSPlans");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProrationAmount",
                table: "SaasTenantSubscriptionPayments",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "SaasTenantSubscriptionPayments",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "SaaSSubscriptionAudits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundAmount",
                table: "SaaSRefunds",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SaaSPlans",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SaaSPlans",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SaaSPlans",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "SaaSPlans",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "SaaSInvoices",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "SaaSInvoices",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "SaaSInvoices",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SaaSFeatures",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "SaaSFeatures",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SaaSFeatures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "SaasCoupons",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SaasCoupons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SaaSAddons",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SaaSAddons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaaSPlans",
                table: "SaaSPlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaasPlanFeatures_SaaSPlans_PlanId",
                table: "SaasPlanFeatures",
                column: "PlanId",
                principalTable: "SaaSPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaasTenantSubscriptions_SaaSPlans_PlanId",
                table: "SaasTenantSubscriptions",
                column: "PlanId",
                principalTable: "SaaSPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
