using Project.Core.Entities.SaaS;

namespace Project.Core.Interfaces;

public interface IProrationCalculator
{
    decimal CalculateProratedAmount(decimal currentPlanPrice, decimal newPlanPrice, DateTime cycleStartDate, DateTime cycleEndDate, DateTime changeDate);
    decimal CalculateUnusedAmount(decimal planPrice, DateTime cycleStartDate, DateTime cycleEndDate, DateTime cancellationDate);
    decimal CalculateUpgradeCost(decimal currentPlanPrice, decimal newPlanPrice, DateTime cycleStartDate, DateTime cycleEndDate, DateTime upgradeDate);
}
