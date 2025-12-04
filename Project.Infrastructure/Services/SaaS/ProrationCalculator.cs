using System;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Services;

namespace Project.Infrastructure.Services.SaaS;

public class ProrationCalculator : DomainService, IProrationCalculator, ITransientDependency
{
    public decimal CalculateProratedAmount(decimal currentPlanPrice, decimal newPlanPrice, DateTime cycleStartDate, DateTime cycleEndDate, DateTime changeDate)
    {
        var totalDays = (cycleEndDate - cycleStartDate).TotalDays;
        if (totalDays <= 0) return 0;

        var remainingDays = (cycleEndDate - changeDate).TotalDays;
        if (remainingDays <= 0) return 0;

        var dailyRateCurrent = currentPlanPrice / (decimal)totalDays;
        var dailyRateNew = newPlanPrice / (decimal)totalDays;

        var unusedCurrent = dailyRateCurrent * (decimal)remainingDays;
        var costNew = dailyRateNew * (decimal)remainingDays;

        return costNew - unusedCurrent;
    }

    public decimal CalculateUnusedAmount(decimal planPrice, DateTime cycleStartDate, DateTime cycleEndDate, DateTime cancellationDate)
    {
        var totalDays = (cycleEndDate - cycleStartDate).TotalDays;
        if (totalDays <= 0) return 0;

        var remainingDays = (cycleEndDate - cancellationDate).TotalDays;
        if (remainingDays <= 0) return 0;

        var dailyRate = planPrice / (decimal)totalDays;
        return dailyRate * (decimal)remainingDays;
    }

    public decimal CalculateUpgradeCost(decimal currentPlanPrice, decimal newPlanPrice, DateTime cycleStartDate, DateTime cycleEndDate, DateTime upgradeDate)
    {
        return CalculateProratedAmount(currentPlanPrice, newPlanPrice, cycleStartDate, cycleEndDate, upgradeDate);
    }
}
