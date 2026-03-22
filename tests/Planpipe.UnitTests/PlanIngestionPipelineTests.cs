using Moq;
using Planpipe.Application.Services;
using Planpipe.Core.Enums;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;
using Xunit;

namespace Planpipe.UnitTests;

public class PlanIngestionPipelineTests
{
    private readonly Mock<IPdfTextExtractor> _pdfExtractor;
    private readonly Mock<IFeatureExtractor> _featureExtractor;
    private readonly Mock<IQuantityDeriver> _quantityDeriver;
    private readonly Mock<IConfidenceScorer> _confidenceScorer;
    private readonly Mock<IGroundTruthComparator> _comparator;
    private readonly Mock<IProcessingRunRepository> _runRepository;
    private readonly PlanIngestionPipeline _pipeline;

    public PlanIngestionPipelineTests()
    {
        _pdfExtractor = new Mock<IPdfTextExtractor>();
        _featureExtractor = new Mock<IFeatureExtractor>();
        _quantityDeriver = new Mock<IQuantityDeriver>();
        _confidenceScorer = new Mock<IConfidenceScorer>();
        _comparator = new Mock<IGroundTruthComparator>();
        _runRepository = new Mock<IProcessingRunRepository>();

        _runRepository.Setup(x => x.CreateAsync(It.IsAny<ProcessingRun>()))
            .ReturnsAsync((ProcessingRun r) => r);
        _runRepository.Setup(x => x.UpdateAsync(It.IsAny<ProcessingRun>()))
            .Returns(Task.CompletedTask);

        _pipeline = new PlanIngestionPipeline(
            _pdfExtractor.Object,
            _featureExtractor.Object,
            _quantityDeriver.Object,
            _confidenceScorer.Object,
            _comparator.Object,
            _runRepository.Object);
    }

    [Fact]
    public async Task IngestAsync_HappyPath_CompletesSuccessfully()
    {
        var fileName = "test.pdf";
        var pdfStream = new MemoryStream();
        var groundTruth = new List<GroundTruthItem>();

        _pdfExtractor.Setup(x => x.ExtractTextAsync(It.IsAny<Stream>()))
            .ReturnsAsync("Sample PDF text");

        var features = new List<PlanFeature>
        {
            new() { Id = Guid.NewGuid(), Category = "Quantity", Value = "100", Unit = "m2" }
        };
        _featureExtractor.Setup(x => x.Extract(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(features);

        var quantities = new List<QuantityItem>
        {
            new() { Id = Guid.NewGuid(), ItemCode = "ABC123", Quantity = 100m, Unit = "m2" }
        };
        _quantityDeriver.Setup(x => x.Derive(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<PlanFeature>>()))
            .Returns(quantities);

        _confidenceScorer.Setup(x => x.Score(It.IsAny<QuantityItem>(), It.IsAny<IReadOnlyList<PlanFeature>>()))
            .Returns(0.85);

        var comparisons = new List<ComparisonResult>
        {
            new() { Id = Guid.NewGuid(), ItemCode = "ABC123", IsMatch = true, Variance = 0.0 }
        };
        _comparator.Setup(x => x.Compare(
            It.IsAny<Guid>(),
            It.IsAny<IReadOnlyList<QuantityItem>>(),
            It.IsAny<IReadOnlyList<GroundTruthItem>>()))
            .Returns(comparisons);

        var result = await _pipeline.IngestAsync(pdfStream, fileName, groundTruth);

        Assert.Equal(ProcessingStatus.Completed, result.Status);
        Assert.Equal(fileName, result.FileName);
        Assert.NotNull(result.StartedAt);
        Assert.NotNull(result.CompletedAt);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task IngestAsync_WhenPdfExtractionFails_SetsFailedStatus()
    {
        var fileName = "test.pdf";
        var pdfStream = new MemoryStream();
        var groundTruth = new List<GroundTruthItem>();

        _pdfExtractor.Setup(x => x.ExtractTextAsync(It.IsAny<Stream>()))
            .ThrowsAsync(new Exception("PDF extraction failed"));

        var result = await _pipeline.IngestAsync(pdfStream, fileName, groundTruth);

        Assert.Equal(ProcessingStatus.Failed, result.Status);
        Assert.Contains("PDF extraction failed", result.ErrorMessage);
        Assert.NotNull(result.CompletedAt);
    }

    [Fact]
    public async Task IngestAsync_CallsAllServices_InCorrectOrder()
    {
        var fileName = "test.pdf";
        var pdfStream = new MemoryStream();
        var groundTruth = new List<GroundTruthItem>();
        var callOrder = new List<string>();

        _pdfExtractor.Setup(x => x.ExtractTextAsync(It.IsAny<Stream>()))
            .ReturnsAsync("text")
            .Callback(() => callOrder.Add("PdfExtractor"));

        _featureExtractor.Setup(x => x.Extract(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(new List<PlanFeature>())
            .Callback(() => callOrder.Add("FeatureExtractor"));

        _quantityDeriver.Setup(x => x.Derive(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<PlanFeature>>()))
            .Returns(new List<QuantityItem>())
            .Callback(() => callOrder.Add("QuantityDeriver"));

        _comparator.Setup(x => x.Compare(
            It.IsAny<Guid>(),
            It.IsAny<IReadOnlyList<QuantityItem>>(),
            It.IsAny<IReadOnlyList<GroundTruthItem>>()))
            .Returns(new List<ComparisonResult>())
            .Callback(() => callOrder.Add("Comparator"));

        await _pipeline.IngestAsync(pdfStream, fileName, groundTruth);

        Assert.Equal(new[] { "PdfExtractor", "FeatureExtractor", "QuantityDeriver", "Comparator" }, callOrder);
    }
}
