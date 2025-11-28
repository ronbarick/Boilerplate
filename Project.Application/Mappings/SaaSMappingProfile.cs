using AutoMapper;
using Project.Application.SaaS.Dtos;
using Project.Core.Entities.SaaS;

namespace Project.Application.Mappings;

public class SaaSMappingProfile : Profile
{
    public SaaSMappingProfile()
    {
        // Plans
        CreateMap<SaasPlan, SaasPlanDto>();
        CreateMap<CreateSaasPlanDto, SaasPlan>();
        CreateMap<UpdateSaasPlanDto, SaasPlan>();
        
        CreateMap<SaasPlanFeature, SaasPlanFeatureDto>();
        CreateMap<CreateSaasPlanFeatureDto, SaasPlanFeature>();

        // Subscriptions
        CreateMap<SaasTenantSubscription, SaasTenantSubscriptionDto>()
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name));
        
        CreateMap<SaasTenantSubscription, SubscriptionHistoryDto>()
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name));

        // Payments
        CreateMap<SaasTenantSubscriptionPayment, SaasTenantSubscriptionPaymentDto>();
        CreateMap<SaasTenantPaymentMethod, PaymentMethodDto>();

        // Invoices
        CreateMap<SaaSInvoice, SaaSInvoiceDto>()
            .ForMember(dest => dest.PdfUrl, opt => opt.MapFrom(src => src.PdfPath));

        // Features
        CreateMap<SaaSFeature, SaaSFeatureDto>();
        CreateMap<UpdateFeatureDto, SaaSFeature>();

        // Coupons
        CreateMap<SaasCoupon, SaasCouponDto>()
            .ForMember(dest => dest.ApplicablePlanIds, opt => opt.Ignore()); // Handle manually or via custom resolver if needed
        CreateMap<CreateSaasCouponDto, SaasCoupon>();

        // Addons
        CreateMap<SaaSAddon, SaaSAddonDto>();
        CreateMap<CreateSaaSAddonDto, SaaSAddon>();

        // Audit & Logs
        CreateMap<SaaSSubscriptionAudit, SaaSSubscriptionAuditDto>();
        CreateMap<SaaSWebhookLog, SaaSWebhookLogDto>();
        CreateMap<SaaSRefund, SaaSRefundDto>();
    }
}
