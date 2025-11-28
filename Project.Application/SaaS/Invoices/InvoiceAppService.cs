using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;
using Project.Core.Entities.SaaS;
using Project.Core.Interfaces;
using Project.Core.Localization;

namespace Project.Application.SaaS.Invoices;

public class InvoiceAppService : AppServiceBase, IInvoiceAppService
{
    private readonly IRepository<SaaSInvoice, Guid> _invoiceRepository;
    private readonly IInvoicePdfGenerator _pdfGenerator;
    private readonly IMapper _mapper;

    public InvoiceAppService(
        IRepository<SaaSInvoice, Guid> invoiceRepository,
        IInvoicePdfGenerator pdfGenerator,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _invoiceRepository = invoiceRepository;
        _pdfGenerator = pdfGenerator;
        _mapper = mapper;
    }

    public async Task<List<SaaSInvoiceDto>> GetListByTenantAsync(Guid tenantId)
    {
        var invoices = await _invoiceRepository.GetQueryable()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.IssuedDate)
            .ToListAsync();
        return _mapper.Map<List<SaaSInvoiceDto>>(invoices);
    }

    public async Task<SaaSInvoiceDto> GetAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetAsync(id);
        return _mapper.Map<SaaSInvoiceDto>(invoice);
    }

    public async Task<byte[]> DownloadInvoicePdfAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);
        return await _pdfGenerator.GenerateInvoicePdfAsync(invoice, string.Empty);
    }
}
