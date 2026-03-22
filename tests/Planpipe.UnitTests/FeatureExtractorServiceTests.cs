using Planpipe.Application.Services;
using Planpipe.Core.Models;
using Xunit;

namespace Planpipe.UnitTests;

public class FeatureExtractorServiceTests
{
    private readonly FeatureExtractorService _service;

    public FeatureExtractorServiceTests()
    {
        _service = new FeatureExtractorService();
    }

    [Fact]
    public void Extract_WithQuantities_ExtractsCorrectly()
    {
        var runId = Guid.NewGuid();
        var text = "Total area: 150.5 m2 and volume 25 m3";

        var features = _service.Extract(runId, text);

        var quantityFeatures = features.Where(f => f.Category == "Quantity").ToList();
        Assert.Equal(2, quantityFeatures.Count);
        Assert.Contains(quantityFeatures, f => f.Value == "150.5" && f.Unit == "m2");
        Assert.Contains(quantityFeatures, f => f.Value == "25" && f.Unit == "m3");
    }

    [Fact]
    public void Extract_WithDimensions_ExtractsCorrectly()
    {
        var runId = Guid.NewGuid();
        var text = "Wall dimensions: 2400 x 1200 mm and 3000x1500";

        var features = _service.Extract(runId, text);

        var dimensionFeatures = features.Where(f => f.Category == "Dimension").ToList();
        Assert.Equal(2, dimensionFeatures.Count);
        Assert.Contains(dimensionFeatures, f => f.Value == "2400x1200");
        Assert.Contains(dimensionFeatures, f => f.Value == "3000x1500");
    }

    [Fact]
    public void Extract_WithItemCodes_ExtractsCorrectly()
    {
        var runId = Guid.NewGuid();
        var text = "Items: ABC1234 and XYZ5678 required for construction";

        var features = _service.Extract(runId, text);

        var itemCodeFeatures = features.Where(f => f.Category == "ItemCode").ToList();
        Assert.Equal(2, itemCodeFeatures.Count);
        Assert.Contains(itemCodeFeatures, f => f.Value == "ABC1234");
        Assert.Contains(itemCodeFeatures, f => f.Value == "XYZ5678");
    }

    [Fact]
    public void Extract_WithEmptyText_ReturnsEmptyList()
    {
        var runId = Guid.NewGuid();
        var text = "";

        var features = _service.Extract(runId, text);

        Assert.Empty(features);
    }
}
