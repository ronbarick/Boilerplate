using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;
using Project.Domain.Interfaces;

namespace Project.Application.SaaS.Invoices;

public interface IInvoiceAppService : IApplicationService
{
    Task<List<SaaSInvoiceDto>> GetListByTenantAsync(Guid tenantId);
    Task<SaaSInvoiceDto> GetAsync(Guid id);
    Task<byte[]> DownloadInvoicePdfAsync(Guid invoiceId);
}
