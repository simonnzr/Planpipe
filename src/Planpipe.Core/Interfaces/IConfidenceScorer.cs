using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IConfidenceScorer
{
    double Score(QuantityItem item, IReadOnlyList<PlanFeature> features);
}
