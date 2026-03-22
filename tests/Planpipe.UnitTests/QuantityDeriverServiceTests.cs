using Planpipe.Application.Services;
using Planpipe.Core.Models;
using Xunit;

namespace Planpipe.UnitTests;

public class QuantityDeriverServiceTests
{
    private readonly QuantityDeriverService _service;

    public QuantityDeriverServiceTests()
    {
        _service = new QuantityDeriverService();
    }

    [Fact]
    public void Derive_WithMultipleQuantitiesSameUnit_SumsCorrectly()
    {
        var runId = Guid.NewGuid();
        var features = new List<PlanFeature>
        {
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Quantity", Value = "10", Unit = "m2" },
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Quantity", Value = "20", Unit = "m2" },
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Quantity", Value = "15", Unit = "m2" }
        };

        var items = _service.Derive(runId, features);

        Assert.Single(items);
        Assert.Equal(45m, items[0].Quantity);
        Assert.Equal("m2", items[0].Unit);
    }

    [Fact]
    public void Derive_WithDifferentUnits_CreatesSeparateItems()
    {
        var runId = Guid.NewGuid();
        var features = new List<PlanFeature>
        {
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Quantity", Value = "10", Unit = "m2" },
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Quantity", Value = "5", Unit = "m3" }
        };

        var items = _service.Derive(runId, features);

        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.Unit == "m2" && i.Quantity == 10m);
        Assert.Contains(items, i => i.Unit == "m3" && i.Quantity == 5m);
    }

    [Fact]
    public void Derive_WithNoQuantityFeatures_ReturnsEmptyList()
    {
        var runId = Guid.NewGuid();
        var features = new List<PlanFeature>
        {
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Dimension", Value = "100x200" }
        };

        var items = _service.Derive(runId, features);

        Assert.Empty(items);
    }

    [Fact]
    public void Derive_WithItemCode_AssignsItemCode()
    {
        var runId = Guid.NewGuid();
        var features = new List<PlanFeature>
        {
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "Quantity", Value = "10", Unit = "m2" },
            new() { Id = Guid.NewGuid(), ProcessingRunId = runId, Category = "ItemCode", Value = "ABC123" }
        };

        var items = _service.Derive(runId, features);

        Assert.Single(items);
        Assert.Equal("ABC123", items[0].ItemCode);
    }
}
