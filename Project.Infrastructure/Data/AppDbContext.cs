using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Project.Core.Entities;
using Project.Core.Entities.Base;
using Project.Core.Entities.Notifications;
using Project.Core.Entities.SaaS;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditContext _auditContext;

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<PermissionDefinition> PermissionDefinitions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
    public DbSet<FileStorageItem> FileStorageItems { get; set; }
    public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
    public DbSet<TwoFactorBackupCode> TwoFactorBackupCodes { get; set; }
    public DbSet<TrustedDevice> TrustedDevices { get; set; }
    
    // SaaS Entities
    public DbSet<SaasPlan> SaaSPlans { get; set; }
    public DbSet<SaasPlanFeature> SaasPlanFeatures { get; set; }
    public DbSet<SaasTenantSubscription> SaasTenantSubscriptions { get; set; }
    public DbSet<SaasTenantSubscriptionPayment> SaasTenantSubscriptionPayments { get; set; }
    public DbSet<SaaSInvoice> SaaSInvoices { get; set; }
    public DbSet<SaaSPaymentGatewaySetting> SaaSPaymentGatewaySettings { get; set; }
    public DbSet<SaaSFeature> SaaSFeatures { get; set; }
    public DbSet<SaaSFeatureValue> SaaSFeatureValues { get; set; }
    public DbSet<SaaSFeatureUsage> SaaSFeatureUsages { get; set; }
    public DbSet<SaasTenantPaymentMethod> SaasTenantPaymentMethods { get; set; }
    public DbSet<SaaSRefund> SaaSRefunds { get; set; }
    public DbSet<SaasCoupon> SaasCoupons { get; set; }
    public DbSet<SaasTenantCoupon> SaasTenantCoupons { get; set; }
    public DbSet<SaaSAddon> SaaSAddons { get; set; }
    public DbSet<SaasTenantAddon> SaasTenantAddons { get; set; }
    public DbSet<SaaSSubscriptionAudit> SaaSSubscriptionAudits { get; set; }
    public DbSet<SaaSWebhookLog> SaaSWebhookLogs { get; set; }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IAuditContext auditContext) : base(options)
    {
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _auditContext = auditContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Setting
        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasIndex(s => new { s.Name, s.ProviderName, s.ProviderKey }).IsUnique();
            entity.Property(s => s.Name).HasMaxLength(256).IsRequired();
            entity.Property(s => s.ProviderName).HasMaxLength(64).IsRequired();
            entity.Property(s => s.ProviderKey).HasMaxLength(64);
        });

        // Configure FileStorageItem
        modelBuilder.Entity<FileStorageItem>(entity =>
        {
            entity.HasIndex(f => new { f.TenantId, f.EntityId, f.EntityType });
            entity.HasIndex(f => new { f.StorageProvider, f.StoragePath });
            entity.HasIndex(f => f.Hash);
            entity.Property(f => f.FileName).HasMaxLength(260).IsRequired();
            entity.Property(f => f.Extension).HasMaxLength(16).IsRequired();
            entity.Property(f => f.MimeType).HasMaxLength(128).IsRequired();
            entity.Property(f => f.Hash).HasMaxLength(256).IsRequired();
            entity.Property(f => f.StorageProvider).HasMaxLength(64).IsRequired();
            entity.Property(f => f.StoragePath).HasMaxLength(512).IsRequired();
            entity.Property(f => f.EntityType).HasMaxLength(256);
            entity.Property(f => f.Folder).HasMaxLength(128);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ... (Change detection logic remains unchanged)
        
        // 1. Detect changes
        var auditEntries = new List<AuditEntry>();
        var modifiedEntries = ChangeTracker.Entries<FullAuditedEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in modifiedEntries)
        {
            // ... (AuditEntry creation logic remains unchanged)
            var auditEntry = new AuditEntry
            {
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                Id = entry.Entity.Id
            };
            auditEntries.Add(auditEntry);

            // ... (Property handling logic remains unchanged)
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUser.Id;
                    
                    if (entry.Entity is IMustHaveTenant mustHaveTenant)
                    {
                        if (mustHaveTenant.TenantId == null && _currentTenant.Id.HasValue)
                            mustHaveTenant.TenantId = _currentTenant.Id.Value;
                    }
                    else if (entry.Entity.TenantId == null && _currentTenant.Id.HasValue)
                    {
                        entry.Entity.TenantId = _currentTenant.Id;
                    }
                    entry.Entity.RowVersion = Guid.NewGuid().ToByteArray();
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUser.Id;
                    entry.Entity.RowVersion = Guid.NewGuid().ToByteArray();
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUser.Id;
                    entry.Entity.RowVersion = Guid.NewGuid().ToByteArray();
                    auditEntry.Action = "Deleted";
                    break;
            }

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue;
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey()) continue;

                var originalValue = property.OriginalValue;
                var currentValue = property.CurrentValue;

                if (entry.State == EntityState.Added)
                {
                    if (currentValue != null)
                        auditEntry.Changes[propertyName] = new { Original = (object?)null, New = currentValue };
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (!Equals(originalValue, currentValue))
                        auditEntry.Changes[propertyName] = new { Original = originalValue, New = currentValue };
                }
            }
        }

        // 2. Save changes to business entities
        var result = await base.SaveChangesAsync(cancellationToken);

        // 3. Save audit logs if any changes occurred
        if (auditEntries.Any())
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = _currentUser.Id,
                TenantId = _currentTenant.Id,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = _auditContext.ServiceName ?? "AppDbContext",
                MethodName = _auditContext.MethodName ?? "SaveChangesAsync",
                ClientIpAddress = _auditContext.ClientIpAddress ?? "::1",
                BrowserInfo = _auditContext.BrowserInfo,
                CustomData = System.Text.Json.JsonSerializer.Serialize(auditEntries)
            };

            AuditLogs.Add(auditLog);
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    // Helper class for serialization
    private class AuditEntry
    {
        public string EntityName { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, object> Changes { get; set; } = new();
    }
}
