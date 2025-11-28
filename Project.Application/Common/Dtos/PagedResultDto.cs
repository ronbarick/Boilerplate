namespace Project.Application.Common.Dtos;

/// <summary>
/// Standard paged result DTO.
/// </summary>
public class PagedResultDto<T>
{
    /// <summary>
    /// List of items in the current page.
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Total count of items (not just current page).
    /// </summary>
    public int TotalCount { get; set; }
}
