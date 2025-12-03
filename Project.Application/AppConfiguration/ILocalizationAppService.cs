using System.Threading.Tasks;
using Project.Application.AppConfiguration.Dtos;
using Project.Domain.Interfaces;

namespace Project.Application.AppConfiguration;

public interface ILocalizationAppService : IApplicationService
{
    Task<LocalizationTextsDto> GetTextsAsync(string culture, string? resourceName = null);
}
