using System.Threading.Tasks;
using Project.Application.AppConfiguration.Dtos;
using Project.Domain.Interfaces;

namespace Project.Application.AppConfiguration;

public interface IProfileAppService : IApplicationService
{
    Task<MyProfileDto> GetMyProfileAsync();
}
