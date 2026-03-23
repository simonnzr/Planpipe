using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;

namespace Planpipe.Application.Services;

public class ConfidenceScorerService : IConfidenceScorer
{
    public double Score(QuantityItem item, IReadOnlyList<PlanFeature> features)
    {
        double score = 0.5;

        var relatedFeatures = features
            .Where(f => f.Category == "Quantity" && 
                       (f.Unit?.Equals(item.Unit, StringComparison.OrdinalIgnoreCase) ?? false))
            .Count();

        score += relatedFeatures * 0.05;

        var hasItemCode = features.Any(f => 
            f.Category == "ItemCode" && 
            f.Value.Equals(item.ItemCode, StringComparison.OrdinalIgnoreCase));

        if (hasItemCode)
        {
            score += 0.2;
        }

        return Math.Min(score, 1.0);
    }
}
