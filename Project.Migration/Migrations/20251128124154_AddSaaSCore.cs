using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migration.Migrations
{
    /// <inheritdoc />
    public partial class AddSaaSCore : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EditionName",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaaSAddons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    BillingCycle = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSAddons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaasCoupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DiscountType = table.Column<int>(type: "integer", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxUses = table.Column<int>(type: "integer", nullable: true),
                    UsedCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicablePlans = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasCoupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaaSFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DefaultValue = table.Column<string>(type: "text", nullable: false),
                    FeatureType = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    AlertThresholdPercentage = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaaSFeatureUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureName = table.Column<string>(type: "text", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    UsageMonth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AlertSent = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSFeatureUsages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaaSFeatureValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureName = table.Column<string>(type: "text", nullable: false),
                    FeatureValue = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSFeatureValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaaSPaymentGatewaySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Gateway = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: false),
                    ApiSecret = table.Column<string>(type: "text", nullable: false),
                    WebhookSecret = table.Column<string>(type: "text", nullable: false),
                    TestMode = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSPaymentGatewaySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaaSPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BillingCycle = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    IsFree = table.Column<bool>(type: "boolean", nullable: false),
                    TrialDays = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaasTenantPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gateway = table.Column<int>(type: "integer", nullable: false),
                    GatewayCustomerId = table.Column<string>(type: "text", nullable: false),
                    GatewayPaymentMethodId = table.Column<string>(type: "text", nullable: false),
                    Last4Digits = table.Column<string>(type: "text", nullable: true),
                    CardBrand = table.Column<string>(type: "text", nullable: true),
                    ExpiryMonth = table.Column<int>(type: "integer", nullable: true),
                    ExpiryYear = table.Column<int>(type: "integer", nullable: true),
                    PaymentMethodType = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasTenantPaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaaSWebhookLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Gateway = table.Column<int>(type: "integer", nullable: false),
                    WebhookData = table.Column<string>(type: "text", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSWebhookLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaasPlanFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureName = table.Column<string>(type: "text", nullable: false),
                    FeatureValue = table.Column<string>(type: "text", nullable: false),
                    FeatureType = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasPlanFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasPlanFeatures_SaaSPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SaaSPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaasTenantSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsCurrentSubscription = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrialEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrialExtensionCount = table.Column<int>(type: "integer", nullable: false),
                    TrialExtensionHistory = table.Column<string>(type: "text", nullable: true),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "integer", nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CancellationType = table.Column<int>(type: "integer", nullable: true),
                    PausedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PauseReason = table.Column<string>(type: "text", nullable: true),
                    BillingAddress = table.Column<string>(type: "text", nullable: true),
                    BillingCity = table.Column<string>(type: "text", nullable: true),
                    BillingState = table.Column<string>(type: "text", nullable: true),
                    BillingCountry = table.Column<string>(type: "text", nullable: true),
                    BillingZipCode = table.Column<string>(type: "text", nullable: true),
                    TaxId = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasTenantSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasTenantSubscriptions_SaaSPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SaaSPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaaSSubscriptionAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    PerformedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSSubscriptionAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaaSSubscriptionAudits_SaasTenantSubscriptions_Subscription~",
                        column: x => x.SubscriptionId,
                        principalTable: "SaasTenantSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaasTenantAddons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddonId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasTenantAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasTenantAddons_SaaSAddons_AddonId",
                        column: x => x.AddonId,
                        principalTable: "SaaSAddons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaasTenantAddons_SaasTenantSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "SaasTenantSubscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaasTenantCoupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasTenantCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasTenantCoupons_SaasCoupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "SaasCoupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaasTenantCoupons_SaasTenantSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "SaasTenantSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaasTenantSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gateway = table.Column<int>(type: "integer", nullable: false),
                    GatewayOrderId = table.Column<string>(type: "text", nullable: false),
                    GatewayPaymentId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    WebhookData = table.Column<string>(type: "text", nullable: true),
                    ProrationAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    ProrationReason = table.Column<string>(type: "text", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    NextRetryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RetryHistory = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasTenantSubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasTenantSubscriptionPayments_SaasTenantSubscriptions_Subs~",
                        column: x => x.SubscriptionId,
                        principalTable: "SaasTenantSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaaSInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PdfPath = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaaSInvoices_SaasTenantSubscriptionPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "SaasTenantSubscriptionPayments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaaSInvoices_SaasTenantSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "SaasTenantSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaaSRefunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RefundReason = table.Column<string>(type: "text", nullable: false),
                    RefundStatus = table.Column<int>(type: "integer", nullable: false),
                    GatewayRefundId = table.Column<string>(type: "text", nullable: true),
                    RefundDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefundedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaaSRefunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaaSRefunds_SaasTenantSubscriptionPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "SaasTenantSubscriptionPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaaSInvoices_PaymentId",
                table: "SaaSInvoices",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaaSInvoices_SubscriptionId",
                table: "SaaSInvoices",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPlanFeatures_PlanId",
                table: "SaasPlanFeatures",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SaaSRefunds_PaymentId",
                table: "SaaSRefunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaaSSubscriptionAudits_SubscriptionId",
                table: "SaaSSubscriptionAudits",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantAddons_AddonId",
                table: "SaasTenantAddons",
                column: "AddonId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantAddons_SubscriptionId",
                table: "SaasTenantAddons",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantCoupons_CouponId",
                table: "SaasTenantCoupons",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantCoupons_SubscriptionId",
                table: "SaasTenantCoupons",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantSubscriptionPayments_SubscriptionId",
                table: "SaasTenantSubscriptionPayments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenantSubscriptions_PlanId",
                table: "SaasTenantSubscriptions",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaaSFeatures");

            migrationBuilder.DropTable(
                name: "SaaSFeatureUsages");

            migrationBuilder.DropTable(
                name: "SaaSFeatureValues");

            migrationBuilder.DropTable(
                name: "SaaSInvoices");

            migrationBuilder.DropTable(
                name: "SaaSPaymentGatewaySettings");

            migrationBuilder.DropTable(
                name: "SaasPlanFeatures");

            migrationBuilder.DropTable(
                name: "SaaSRefunds");

            migrationBuilder.DropTable(
                name: "SaaSSubscriptionAudits");

            migrationBuilder.DropTable(
                name: "SaasTenantAddons");

            migrationBuilder.DropTable(
                name: "SaasTenantCoupons");

            migrationBuilder.DropTable(
                name: "SaasTenantPaymentMethods");

            migrationBuilder.DropTable(
                name: "SaaSWebhookLogs");

            migrationBuilder.DropTable(
                name: "SaasTenantSubscriptionPayments");

            migrationBuilder.DropTable(
                name: "SaaSAddons");

            migrationBuilder.DropTable(
                name: "SaasCoupons");

            migrationBuilder.DropTable(
                name: "SaasTenantSubscriptions");

            migrationBuilder.DropTable(
                name: "SaaSPlans");

            migrationBuilder.DropColumn(
                name: "EditionName",
                table: "Tenants");
        }
    }
}
