using Project.Application.Common.Dtos;

namespace Project.Application.Tenants.Dtos;

/// <summary>
/// Input DTO for getting paginated list of tenants.
/// </summary>
public class GetTenantsInput : PagedAndSortedRequestDto
{
    public GetTenantsInput()
    {
        Sorting = "Name ASC";
    }
}
