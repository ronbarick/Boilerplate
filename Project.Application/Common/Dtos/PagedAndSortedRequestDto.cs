namespace Project.Application.Common.Dtos;

/// <summary>
/// Base class for paged and sorted request DTOs.
/// </summary>
public class PagedAndSortedRequestDto
{
    /// <summary>
    /// Number of records to skip (for pagination).
    /// </summary>
    public int SkipCount { get; set; } = 0;

    /// <summary>
    /// Maximum number of records to return.
    /// Default: 10, Max: 1000
    /// </summary>
    public int MaxResultCount { get; set; } = 10;

    /// <summary>
    /// Sorting string (e.g., "Name ASC", "CreatedOn DESC").
    /// </summary>
    public string? Sorting { get; set; }

    /// <summary>
    /// Filter/search term.
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Validates and normalizes the request.
    /// </summary>
    public virtual void Normalize()
    {
        if (MaxResultCount > 1000)
            MaxResultCount = 1000;

        if (MaxResultCount < 1)
            MaxResultCount = 10;

        if (SkipCount < 0)
            SkipCount = 0;
    }
}
