using Project.Application.Common.Dtos;

namespace Project.Application.Users.Dtos;

/// <summary>
/// Input DTO for getting paginated list of users.
/// </summary>
public class GetUsersInput : PagedAndSortedRequestDto
{
    public GetUsersInput()
    {
        Sorting = "Name ASC";
    }
}
