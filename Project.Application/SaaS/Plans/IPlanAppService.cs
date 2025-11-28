using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;
using Project.Core.Interfaces;

namespace Project.Application.SaaS.Plans;

public interface IPlanAppService : IApplicationService
{
    Task<List<SaasPlanDto>> GetListAsync();
    Task<SaasPlanDto> GetAsync(Guid id);
    Task<SaasPlanDto> CreateAsync(CreateSaasPlanDto input);
    Task<SaasPlanDto> UpdateAsync(Guid id, UpdateSaasPlanDto input);
    Task DeleteAsync(Guid id);
}
