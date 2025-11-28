using System;
using Project.Core.Interfaces;
using Project.Core.Interfaces.DependencyInjection;
using Project.Core.Services;

namespace Project.Infrastructure.Services.SaaS;

public class TaxCalculator : DomainService, ITaxCalculator, ITransientDependency
{
    public decimal CalculateTax(decimal amount, string country, string state, string? taxId)
    {
        var percentage = GetTaxPercentage(country, state);
        return amount * (percentage / 100m);
    }

    public decimal GetTaxPercentage(string country, string state)
    {
        // Mock logic - in real app, fetch from DB or external service
        if (country == "IN") return 18m; // GST
        if (country == "US") return 0m; // Sales tax varies by state, simplified
        return 0m;
    }

    public string GetTaxName(string country)
    {
        if (country == "IN") return "GST";
        if (country == "US") return "Sales Tax";
        return "Tax";
    }
}
