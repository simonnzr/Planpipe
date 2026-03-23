using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IQuantityDeriver
{
    IReadOnlyList<QuantityItem> Derive(Guid runId, IReadOnlyList<PlanFeature> features);
}
