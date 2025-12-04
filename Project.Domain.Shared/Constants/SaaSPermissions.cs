namespace Project.Domain.Shared.Constants;

public static class SaaSPermissions
{
    public const string GroupName = "SaaS";

    // Plans
    public const string Plans = GroupName + ".Plans";
    public const string Plans_Create = Plans + ".Create";
    public const string Plans_Edit = Plans + ".Edit";
    public const string Plans_Delete = Plans + ".Delete";
    public const string Plans_View = Plans + ".View";

    // Subscriptions
    public const string Subscriptions = GroupName + ".Subscriptions";
    public const string Subscriptions_Create = Subscriptions + ".Create";
    public const string Subscriptions_Edit = Subscriptions + ".Edit";
    public const string Subscriptions_Cancel = Subscriptions + ".Cancel";
    public const string Subscriptions_View = Subscriptions + ".View";
    public const string Subscriptions_Manage = Subscriptions + ".Manage"; // For admin overrides

    // Coupons
    public const string Coupons = GroupName + ".Coupons";
    public const string Coupons_Create = Coupons + ".Create";
    public const string Coupons_Edit = Coupons + ".Edit";
    public const string Coupons_Delete = Coupons + ".Delete";
    public const string Coupons_View = Coupons + ".View";

    // Addons
    public const string Addons = GroupName + ".Addons";
    public const string Addons_Create = Addons + ".Create";
    public const string Addons_Edit = Addons + ".Edit";
    public const string Addons_Delete = Addons + ".Delete";
    public const string Addons_View = Addons + ".View";

    // Reports & Analytics
    public const string Reports = GroupName + ".Reports";
    public const string Reports_Revenue = Reports + ".Revenue";
    public const string Reports_Churn = Reports + ".Churn";
    public const string Reports_Usage = Reports + ".Usage";
}
