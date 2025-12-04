using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Domain.Interfaces;

public interface IPermissionChecker
{
    Task<bool> IsGrantedAsync(ClaimsPrincipal user, string permissionName);
    Task<bool> IsGrantedAsync(string permissionName);
}
