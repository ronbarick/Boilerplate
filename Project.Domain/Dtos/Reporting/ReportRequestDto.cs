namespace Project.Domain.Dtos.Reporting;

public class ReportRequestDto
{
    public string ReportType { get; set; } = "csv"; // csv, excel, pdf
    public Dictionary<string, object> Filters { get; set; } = new();
    public bool ForceBackgroundJob { get; set; }
    
    /// <summary>
    /// Optional custom suffix for the filename (max 50 chars, alphanumeric + underscore/dash only)
    /// Example: "Q4_2024" will generate "StudentReport_Q4_2024_20241127120000.pdf"
    /// </summary>
    public string? CustomSuffix { get; set; }
}
