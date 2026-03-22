using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;

namespace Planpipe.Application.Services;

public class QuantityDeriverService : IQuantityDeriver
{
    public IReadOnlyList<QuantityItem> Derive(Guid runId, IReadOnlyList<PlanFeature> features)
    {
        var quantityFeatures = features.Where(f => f.Category == "Quantity").ToList();
        var itemCodeFeatures = features.Where(f => f.Category == "ItemCode").ToList();
        
        var grouped = quantityFeatures
            .GroupBy(f => f.Unit?.ToLowerInvariant() ?? "unknown")
            .ToList();

        var items = new List<QuantityItem>();
        
        foreach (var group in grouped)
        {
            var unit = group.Key;
            var totalQuantity = group
                .Select(f => decimal.TryParse(f.Value, out var val) ? val : 0m)
                .Sum();

            var itemCode = itemCodeFeatures.FirstOrDefault()?.Value ?? "UNKNOWN";

            items.Add(new QuantityItem
            {
                Id = Guid.NewGuid(),
                ProcessingRunId = runId,
                ItemCode = itemCode,
                Description = $"Aggregated {unit} items",
                Quantity = totalQuantity,
                Unit = unit,
                ConfidenceScore = 0.0
            });
        }

        return items;
    }
}
