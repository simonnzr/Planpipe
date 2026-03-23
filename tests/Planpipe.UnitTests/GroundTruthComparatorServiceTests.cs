using Planpipe.Application.Services;
using Planpipe.Core.Models;
using Xunit;

namespace Planpipe.UnitTests;

public class GroundTruthComparatorServiceTests
{
    private readonly GroundTruthComparatorService _service;

    public GroundTruthComparatorServiceTests()
    {
        _service = new GroundTruthComparatorService();
    }

    [Fact]
    public void Compare_WithMatchingQuantities_ReturnsMatch()
    {
        var runId = Guid.NewGuid();
        var extracted = new List<QuantityItem>
        {
            new() { ItemCode = "ABC123", Quantity = 100m }
        };
        var groundTruth = new List<GroundTruthItem>
        {
            new() { ItemCode = "ABC123", Quantity = 100m }
        };

        var results = _service.Compare(runId, extracted, groundTruth);

        Assert.Single(results);
        Assert.True(results[0].IsMatch);
        Assert.Equal(0.0, results[0].Variance);
    }

    [Fact]
    public void Compare_WithSmallVariance_ReturnsMatch()
    {
        var runId = Guid.NewGuid();
        var extracted = new List<QuantityItem>
        {
            new() { ItemCode = "ABC123", Quantity = 102m }
        };
        var groundTruth = new List<GroundTruthItem>
        {
            new() { ItemCode = "ABC123", Quantity = 100m }
        };

        var results = _service.Compare(runId, extracted, groundTruth);

        Assert.Single(results);
        Assert.True(results[0].IsMatch);
        Assert.True(results[0].Variance < 0.05);
    }

    [Fact]
    public void Compare_WithLargeVariance_ReturnsNoMatch()
    {
        var runId = Guid.NewGuid();
        var extracted = new List<QuantityItem>
        {
            new() { ItemCode = "ABC123", Quantity = 120m }
        };
        var groundTruth = new List<GroundTruthItem>
        {
            new() { ItemCode = "ABC123", Quantity = 100m }
        };

        var results = _service.Compare(runId, extracted, groundTruth);

        Assert.Single(results);
        Assert.False(results[0].IsMatch);
        Assert.Equal(0.2, results[0].Variance);
    }

    [Fact]
    public void Compare_WithMissingExtracted_HandlesCorrectly()
    {
        var runId = Guid.NewGuid();
        var extracted = new List<QuantityItem>();
        var groundTruth = new List<GroundTruthItem>
        {
            new() { ItemCode = "ABC123", Quantity = 100m }
        };

        var results = _service.Compare(runId, extracted, groundTruth);

        Assert.Single(results);
        Assert.Null(results[0].ExtractedQuantity);
        Assert.Equal(100m, results[0].GroundTruthQuantity);
        Assert.False(results[0].IsMatch);
    }

    [Fact]
    public void Compare_WithMissingGroundTruth_HandlesCorrectly()
    {
        var runId = Guid.NewGuid();
        var extracted = new List<QuantityItem>
        {
            new() { ItemCode = "ABC123", Quantity = 100m }
        };
        var groundTruth = new List<GroundTruthItem>();

        var results = _service.Compare(runId, extracted, groundTruth);

        Assert.Single(results);
        Assert.Equal(100m, results[0].ExtractedQuantity);
        Assert.Null(results[0].GroundTruthQuantity);
        Assert.False(results[0].IsMatch);
    }
}
