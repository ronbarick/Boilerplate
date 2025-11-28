using Project.Application.Common.Dtos;

namespace Project.Application.Students.Dtos;

/// <summary>
/// Input DTO for getting paginated list of students.
/// </summary>
public class GetStudentsInput : PagedAndSortedRequestDto
{
    public GetStudentsInput()
    {
        Sorting = "FirstName ASC";
    }
}
