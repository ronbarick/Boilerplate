namespace Project.Domain.Interfaces;

public interface ITaxCalculator
{
    decimal CalculateTax(decimal amount, string country, string state, string? taxId);
    decimal GetTaxPercentage(string country, string state);
    string GetTaxName(string country);
}
