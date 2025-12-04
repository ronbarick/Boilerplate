using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.Application.Mappings;
using Project.Application.Services;
using Project.Application.Validators;
using Project.Emailing;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.Common;
using Project.Domain.Interfaces.Notifications;
using Project.Domain.Interfaces.Reporting;
using Project.BackgroundJobs;
using Project.Domain.Localization;
using Project.Domain.Notifications;
using Project.BackgroundJobs;
using Project.Infrastructure.Data;
using Project.Emailing;
using Project.Infrastructure.Localization;
using Project.Infrastructure.Repositories;
using Project.Infrastructure.Services;
using Project.Infrastructure.Services.Common;
using Project.Infrastructure.Services.FileStorage;
using Project.Infrastructure.Services.Notifications;
using Project.Infrastructure.Services.Reporting;
using Project.Infrastructure.Services.Reporting.Generators;
using Project.Infrastructure.Services.Reporting.Storage;
using Project.Infrastructure.Services.SaaS;
using Project.Infrastructure.Settings;
using Project.WebApi.Infrastructure;
using Project.WebApi.Services;
using Project.WebApi.Validators;

namespace Project.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Automatically registers all IApplicationService implementations from the specified assembly.
    /// This mimics ABP's convention-based dependency injection.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Assembly assembly)
    {
        // Find all types that implement IApplicationService
        var applicationServiceType = typeof(IApplicationService);
        
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && applicationServiceType.IsAssignableFrom(t))
            .ToList();

        foreach (var implementationType in types)
        {
            // Find the interface that matches the naming convention (I{ClassName})
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name}" && i != applicationServiceType);

            if (interfaceType != null)
            {
                // Register with interface
                services.AddScoped(interfaceType, implementationType);
            }
            else
            {
                // Register as self if no matching interface found
                services.AddScoped(implementationType);
            }
        }

        return services;
    }

    /// <summary>
    /// Adds database context with PostgreSQL.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Project.Migration")));
        
        return services;
    }

    /// <summary>
    /// Adds core services (Current User/Tenant, Permissions, Repositories, AutoMapper).
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Current User/Tenant
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ICurrentTenant, CurrentTenant>();

        // Permission System
        services.AddScoped<IPermissionChecker, PermissionChecker>();
        services.AddScoped<IPermissionManager, PermissionManager>();

        // Core Utilities
        services.AddSingleton<IGuidGenerator, GuidGenerator>();
        services.AddSingleton<IClock, Clock>();

        // AutoMapper
        services.AddAutoMapper(typeof(ApplicationMappingProfile).Assembly);

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(EfCoreRepository<>));
        services.AddScoped(typeof(IRepository<,>), typeof(EfCoreRepository<,>));

        // Infrastructure Services
        services.AddScoped<TenantProvisioningService>();
        
        // Domain Services
        services.AddScoped<IUserManager, UserManager>();

        // FluentValidation - Register validators and enable auto-validation
       services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
        services.AddFluentValidationAutoValidation();

        // Audit Context
        services.AddScoped<IAuditContext, AuditContext>();

        // Settings System
        services.AddSingleton<ISettingDefinitionManager, SettingDefinitionManager>();
        services.AddScoped<ISettingStore, SettingStore>();
        services.AddScoped<ISettingProvider, GlobalSettingProvider>();
        services.AddScoped<ISettingProvider, TenantSettingProvider>();
        services.AddScoped<ISettingProvider, UserSettingProvider>();
        services.AddScoped<ISettingManager, SettingManager>();

        // Email System
        services.AddScoped<IEmailSenderConfiguration, EmailSenderConfiguration>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailTemplateProvider, EmailTemplateProvider>();

        // Background Job System (Hangfire-based)
        services.AddScoped<IBackgroundJobManager, HangfireBackgroundJobManager>();
        services.AddScoped<IRecurringJobManager, HangfireRecurringJobManager>();

        // Caching System
        services.AddScoped<Project.Caching.IDistributedCacheService, Project.Caching.DistributedCacheService>();
        services.AddScoped<Project.Caching.ICacheInvalidationService, Project.Caching.CacheInvalidationService>();

        // Localization System
        services.AddLocalization();

        // Notification System
        services.AddScoped<INotificationStore, NotificationStore>();
        services.AddScoped<INotificationPublisher, NotificationPublisher>();
        services.AddSingleton<NotificationDefinitionProvider, Application.Notifications.AppNotificationDefinitionProvider>();
        services.AddSingleton<INotificationDefinitionManager, NotificationDefinitionManager>();
        services.AddScoped<IRealTimeNotifier, RealTimeNotifier>();
        services.AddScoped<IUserNotificationManager, UserNotificationManager>();

        // Reporting Services
        services.AddScoped<IReportGenerator, CsvReportGenerator>();
        services.AddScoped<IReportGenerator, ExcelReportGenerator>();
        services.AddScoped<IReportGenerator, PdfReportGenerator>();
        services.AddScoped<IReportDispatcher, ReportDispatcher>();
        services.AddScoped<IReportStorageProvider, LocalReportStorageProvider>();
        services.AddScoped<IReportService, ReportService>();

        // SaaS Domain Services
        services.AddScoped<ISubscriptionManager, SubscriptionManager>();
        services.AddScoped<IPaymentManager, PaymentManager>();
        services.AddScoped<IFeatureManager, FeatureManager>();
        services.AddScoped<IFeatureChecker, FeatureChecker>();
        services.AddScoped<IInvoiceManager, InvoiceManager>();
        services.AddScoped<IRazorpayService, RazorpayService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<IProrationCalculator, ProrationCalculator>();
        services.AddScoped<ITaxCalculator, TaxCalculator>();
        services.AddScoped<ICouponValidator, CouponValidator>();
        services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();
        services.AddScoped<ITenantSyncService, TenantSyncService>();

        // File Storage Services
        services.AddScoped<IFileHashService, FileHashService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        
        // File Storage Providers
        services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();
        services.AddScoped<IFileStorageProvider, AzureBlobStorageProvider>();
        services.AddScoped<IFileStorageProvider, AwsS3StorageProvider>();
        services.AddScoped<IFileStorageProvider, MinIOStorageProvider>();
        
        // File Storage Provider Selector
        services.AddScoped<IFileStorageProviderSelector, FileStorageProviderSelector>();

        return services;
    }

    /// <summary>
    /// Adds JWT authentication.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        
        // JWT Service
        services.AddSingleton(new JwtTokenService(
            jwtSettings["SecretKey"]!,
            jwtSettings["Issuer"]!,
            jwtSettings["Audience"]!
        ));

        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Adds production security features (CORS, Rate Limiting, HSTS).
    /// </summary>
    public static IServiceCollection AddProductionSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        // CORS Policy
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        // Rate Limiting
        services.AddMemoryCache();
        services.Configure<AspNetCoreRateLimit.IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.ClientIdHeader = "X-ClientId";
            options.GeneralRules = new List<AspNetCoreRateLimit.RateLimitRule>
            {
                new AspNetCoreRateLimit.RateLimitRule
                {
                    Endpoint = "POST:/api/auth/*",
                    Period = "1m",
                    Limit = 5
                },
                // Stricter limits for public password/email endpoints
                new AspNetCoreRateLimit.RateLimitRule
                {
                    Endpoint = "POST:/api/users/forgot-password",
                    Period = "1m",
                    Limit = 3 // Only 3 forgot password attempts per minute
                },
                new AspNetCoreRateLimit.RateLimitRule
                {
                    Endpoint = "POST:/api/users/reset-password",
                    Period = "1m",
                    Limit = 5
                },
                new AspNetCoreRateLimit.RateLimitRule
                {
                    Endpoint = "POST:/api/users/set-password",
                    Period = "1m",
                    Limit = 5
                },
                new AspNetCoreRateLimit.RateLimitRule
                {
                    Endpoint = "POST:/api/users/confirm-email",
                    Period = "1m",
                    Limit = 10
                },
                new AspNetCoreRateLimit.RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1m",
                    Limit = 100
                }
            };
        });

        services.AddSingleton<AspNetCoreRateLimit.IIpPolicyStore, AspNetCoreRateLimit.MemoryCacheIpPolicyStore>();
        services.AddSingleton<AspNetCoreRateLimit.IRateLimitCounterStore, AspNetCoreRateLimit.MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<AspNetCoreRateLimit.IRateLimitConfiguration, AspNetCoreRateLimit.RateLimitConfiguration>();
        services.AddSingleton<AspNetCoreRateLimit.IProcessingStrategy, AspNetCoreRateLimit.AsyncKeyLockProcessingStrategy>();
        services.AddInMemoryRateLimiting();

        // HSTS
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        return services;
    }

    /// <summary>
    /// Adds dynamic API controllers from Application Services.
    /// </summary>
    public static IServiceCollection AddDynamicApiControllers(this IServiceCollection services, Assembly applicationAssembly)
    {
        services.AddControllers(options =>
        {
            options.Conventions.Add(new ApplicationServiceToControllerModelConvention());
            options.Filters.Add<Project.WebApi.Filters.AuditActionFilter>();
            options.Filters.Add<Project.WebApi.Filters.LocalizationValidationFilter>();
        })
        .AddApplicationPart(applicationAssembly)
        .ConfigureApplicationPartManager(manager =>
        {
            manager.FeatureProviders.Add(new DynamicApiControllerFeatureProvider());
        });

        // Suppress default model state validation filter to allow our custom filter to run
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }

    /// <summary>
    /// Adds Swagger/OpenAPI with JWT authentication.
    /// </summary>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Project API",
                Version = "v1",
                Description = "ABP-Style Multi-Tenant API"
            });

            // Fix for dynamic API: Use controller name or group name as tag
            c.TagActionsBy(api =>
            {
                // Use GroupName if available (for minimal APIs with .WithTags())
                if (api.GroupName != null)
                {
                    return new[] { api.GroupName };
                }

                // Use controller name for MVC controllers
                if (api.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controllerActionDescriptor)
                {
                    return new[] { controllerActionDescriptor.ControllerName };
                }

                // Fallback to relative path for other endpoints
                return new[] { api.RelativePath ?? "API" };
            });

            // Resolve conflicts by creating unique operation IDs
            c.CustomOperationIds(apiDesc =>
            {
                var controllerActionDescriptor = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                if (controllerActionDescriptor != null)
                {
                    return $"{controllerActionDescriptor.ControllerName}_{controllerActionDescriptor.ActionName}";
                }
                
                // For minimal APIs, use the HTTP method + relative path
                return $"{apiDesc.HttpMethod}_{apiDesc.RelativePath?.Replace("/", "_").Replace("{", "").Replace("}", "")}";
            });

            // Resolve conflicting actions by taking the first one
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());


            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adds localization services with JSON-based provider.
    /// </summary>
    public static IServiceCollection AddLocalization(this IServiceCollection services)
    {
        // Distributed Cache (required for localization caching)
        services.AddDistributedMemoryCache();

        // Register localization services
        services.AddSingleton<ILocalizationProvider>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<JsonLocalizationProvider>>();
            var provider = new JsonLocalizationProvider(logger);
            
            // Register the Project resource
            provider.AddResource(ProjectLocalizationResource.Resource);
            
            return provider;
        });

        services.AddScoped<ICultureProvider, CultureProvider>();
        services.AddScoped<ILocalizationManager, LocalizationManager>();

        return services;
    }
}
