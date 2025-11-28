using Project.Core.Dtos.Reporting;
using Project.Core.Interfaces.Reporting;

namespace Project.Infrastructure.Services.Reporting;

public class ReportDispatcher : IReportDispatcher
{
    private readonly IEnumerable<IReportGenerator> _generators;

    public ReportDispatcher(IEnumerable<IReportGenerator> generators)
    {
        _generators = generators;
    }

    public async Task<ReportFileDto> DispatchAsync<T>(string format, IEnumerable<T> data, string reportName, string? customSuffix = null)
    {
        var generator = _generators.FirstOrDefault(g => g.Format.Equals(format, StringComparison.OrdinalIgnoreCase));

        if (generator == null)
        {
            throw new NotSupportedException($"Report format '{format}' is not supported.");
        }

        return await generator.GenerateAsync(data, reportName, customSuffix);
    }
}
