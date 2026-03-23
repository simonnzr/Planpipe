using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IFeatureExtractor
{
    IReadOnlyList<PlanFeature> Extract(Guid runId, string text);
}
