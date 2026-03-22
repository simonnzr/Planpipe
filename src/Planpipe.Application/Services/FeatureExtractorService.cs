using System.Text.RegularExpressions;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;

namespace Planpipe.Application.Services;

public class FeatureExtractorService : IFeatureExtractor
{
    private static readonly Regex QuantityPattern = new(
        @"(\d+(?:\.\d+)?)\s*(m2|m3|lm|kg|ea|nr|each|sqm|cbm)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex DimensionPattern = new(
        @"(\d+(?:\.\d+)?)\s*[xX×]\s*(\d+(?:\.\d+)?)\s*(?:mm|cm|m)?",
        RegexOptions.Compiled);

    private static readonly Regex ItemCodePattern = new(
        @"\b[A-Z]{2,4}\d{3,6}\b",
        RegexOptions.Compiled);

    public IReadOnlyList<PlanFeature> Extract(Guid runId, string text)
    {
        var features = new List<PlanFeature>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        int pageNumber = 1;

        foreach (var line in lines)
        {
            ExtractQuantities(runId, line, pageNumber, features);
            ExtractDimensions(runId, line, pageNumber, features);
            ExtractItemCodes(runId, line, pageNumber, features);
        }

        return features;
    }

    private void ExtractQuantities(Guid runId, string line, int page, List<PlanFeature> features)
    {
        var matches = QuantityPattern.Matches(line);
        foreach (Match match in matches)
        {
            features.Add(new PlanFeature
            {
                Id = Guid.NewGuid(),
                ProcessingRunId = runId,
                Category = "Quantity",
                Name = "QuantityValue",
                Value = match.Groups[1].Value,
                Unit = match.Groups[2].Value,
                Page = page
            });
        }
    }

    private void ExtractDimensions(Guid runId, string line, int page, List<PlanFeature> features)
    {
        var matches = DimensionPattern.Matches(line);
        foreach (Match match in matches)
        {
            features.Add(new PlanFeature
            {
                Id = Guid.NewGuid(),
                ProcessingRunId = runId,
                Category = "Dimension",
                Name = "DimensionValue",
                Value = $"{match.Groups[1].Value}x{match.Groups[2].Value}",
                Unit = null,
                Page = page
            });
        }
    }

    private void ExtractItemCodes(Guid runId, string line, int page, List<PlanFeature> features)
    {
        var matches = ItemCodePattern.Matches(line);
        foreach (Match match in matches)
        {
            features.Add(new PlanFeature
            {
                Id = Guid.NewGuid(),
                ProcessingRunId = runId,
                Category = "ItemCode",
                Name = "ItemCodeValue",
                Value = match.Value,
                Unit = null,
                Page = page
            });
        }
    }
}
