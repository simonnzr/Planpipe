using Planpipe.Application.Services;
using Planpipe.Core.Models;
using Xunit;

namespace Planpipe.UnitTests;

public class ConfidenceScorerServiceTests
{
    private readonly ConfidenceScorerService _service;

    public ConfidenceScorerServiceTests()
    {
        _service = new ConfidenceScorerService();
    }

    [Fact]
    public void Score_WithNoRelatedFeatures_ReturnsBaseScore()
    {
        var item = new QuantityItem
        {
            Id = Guid.NewGuid(),
            ItemCode = "TEST123",
            Unit = "m2",
            Quantity = 100
        };
        var features = new List<PlanFeature>();

        var score = _service.Score(item, features);

        Assert.Equal(0.5, score);
    }

    [Fact]
    public void Score_WithMatchingItemCode_IncreasesScore()
    {
        var item = new QuantityItem
        {
            Id = Guid.NewGuid(),
            ItemCode = "TEST123",
            Unit = "m2",
            Quantity = 100
        };
        var features = new List<PlanFeature>
        {
            new() { Category = "ItemCode", Value = "TEST123" }
        };

        var score = _service.Score(item, features);

        Assert.Equal(0.7, score);
    }

    [Fact]
    public void Score_WithRelatedQuantityFeatures_IncreasesScore()
    {
        var item = new QuantityItem
        {
            Id = Guid.NewGuid(),
            ItemCode = "TEST123",
            Unit = "m2",
            Quantity = 100
        };
        var features = new List<PlanFeature>
        {
            new() { Category = "Quantity", Unit = "m2", Value = "50" },
            new() { Category = "Quantity", Unit = "m2", Value = "50" }
        };

        var score = _service.Score(item, features);

        Assert.Equal(0.6, score);
    }

    [Fact]
    public void Score_NeverExceedsMaximum()
    {
        var item = new QuantityItem
        {
            Id = Guid.NewGuid(),
            ItemCode = "TEST123",
            Unit = "m2",
            Quantity = 100
        };
        var features = Enumerable.Range(0, 20)
            .Select(_ => new PlanFeature { Category = "Quantity", Unit = "m2", Value = "10" })
            .ToList();
        features.Add(new PlanFeature { Category = "ItemCode", Value = "TEST123" });

        var score = _service.Score(item, features);

        Assert.Equal(1.0, score);
    }
}
