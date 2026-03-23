using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IGroundTruthComparator
{
    IReadOnlyList<ComparisonResult> Compare(
        Guid runId, 
        IReadOnlyList<QuantityItem> extracted, 
        IReadOnlyList<GroundTruthItem> groundTruth);
}
