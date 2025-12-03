using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;
using Project.Domain.Entities.SaaS;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Localization;

namespace Project.Application.SaaS.Plans;

public class PlanAppService : AppServiceBase, IPlanAppService
{
    private readonly IRepository<SaasPlan, Guid> _planRepository;
    private readonly IMapper _mapper;

    public PlanAppService(
        IRepository<SaasPlan, Guid> planRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _planRepository = planRepository;
        _mapper = mapper;
    }

    public async Task<List<SaasPlanDto>> GetListAsync()
    {
        var plans = await _planRepository.GetQueryable()
            .Include(x => x.Features)
            .ToListAsync();
        return _mapper.Map<List<SaasPlanDto>>(plans);
    }

    public async Task<SaasPlanDto> GetAsync(Guid id)
    {
        var plan = await _planRepository.GetQueryable()
            .Include(x => x.Features)
            .FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.Map<SaasPlanDto>(plan);
    }

    public async Task<SaasPlanDto> CreateAsync(CreateSaasPlanDto input)
    {
        var plan = _mapper.Map<SaasPlan>(input);
        await _planRepository.InsertAsync(plan);
        return _mapper.Map<SaasPlanDto>(plan);
    }

    public async Task<SaasPlanDto> UpdateAsync(Guid id, UpdateSaasPlanDto input)
    {
        var plan = await _planRepository.GetAsync(id);
        if (plan == null) return null;
        
        _mapper.Map(input, plan);
        await _planRepository.UpdateAsync(plan);
        return _mapper.Map<SaasPlanDto>(plan);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _planRepository.DeleteAsync(id);
    }
}
