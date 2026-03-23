using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;

namespace Planpipe.Application.Services;

public class GroundTruthComparatorService : IGroundTruthComparator
{
    public IReadOnlyList<ComparisonResult> Compare(
        Guid runId,
        IReadOnlyList<QuantityItem> extracted,
        IReadOnlyList<GroundTruthItem> groundTruth)
    {
        var results = new List<ComparisonResult>();

        var extractedDict = extracted.ToDictionary(e => e.ItemCode, e => e);
        var groundTruthDict = groundTruth.ToDictionary(g => g.ItemCode, g => g);

        var allItemCodes = extractedDict.Keys.Union(groundTruthDict.Keys).ToList();

        foreach (var itemCode in allItemCodes)
        {
            var hasExtracted = extractedDict.TryGetValue(itemCode, out var extractedItem);
            var hasGroundTruth = groundTruthDict.TryGetValue(itemCode, out var groundTruthItem);

            decimal? extractedQty = hasExtracted ? extractedItem!.Quantity : null;
            decimal? groundTruthQty = hasGroundTruth ? groundTruthItem!.Quantity : null;

            double variance = 0.0;
            bool isMatch = false;

            if (hasExtracted && hasGroundTruth && groundTruthQty > 0)
            {
                variance = (double)Math.Abs(extractedQty!.Value - groundTruthQty.Value) / 
                          (double)groundTruthQty.Value;
                isMatch = variance < 0.05;
            }

            results.Add(new ComparisonResult
            {
                Id = Guid.NewGuid(),
                ProcessingRunId = runId,
                ItemCode = itemCode,
                ExtractedQuantity = extractedQty,
                GroundTruthQuantity = groundTruthQty,
                Variance = variance,
                IsMatch = isMatch
            });
        }

        return results;
    }
}
