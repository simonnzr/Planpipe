using Planpipe.Core.Enums;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;

namespace Planpipe.Application.Services;

public class PlanIngestionPipeline
{
    private readonly IPdfTextExtractor _pdfExtractor;
    private readonly IFeatureExtractor _featureExtractor;
    private readonly IQuantityDeriver _quantityDeriver;
    private readonly IConfidenceScorer _confidenceScorer;
    private readonly IGroundTruthComparator _comparator;
    private readonly IProcessingRunRepository _runRepository;

    public PlanIngestionPipeline(
        IPdfTextExtractor pdfExtractor,
        IFeatureExtractor featureExtractor,
        IQuantityDeriver quantityDeriver,
        IConfidenceScorer confidenceScorer,
        IGroundTruthComparator comparator,
        IProcessingRunRepository runRepository)
    {
        _pdfExtractor = pdfExtractor;
        _featureExtractor = featureExtractor;
        _quantityDeriver = quantityDeriver;
        _confidenceScorer = confidenceScorer;
        _comparator = comparator;
        _runRepository = runRepository;
    }

    public async Task<ProcessingRun> IngestAsync(
        Stream pdfStream,
        string fileName,
        IReadOnlyList<GroundTruthItem> groundTruth)
    {
        var run = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            Status = ProcessingStatus.Pending,
            StartedAt = DateTime.UtcNow
        };

        try
        {
            run.Status = ProcessingStatus.Running;
            await _runRepository.CreateAsync(run);

            var text = await _pdfExtractor.ExtractTextAsync(pdfStream);

            var features = _featureExtractor.Extract(run.Id, text);
            run.Features = features.ToList();

            var quantities = _quantityDeriver.Derive(run.Id, features);
            foreach (var quantity in quantities)
            {
                quantity.ConfidenceScore = _confidenceScorer.Score(quantity, features);
            }
            run.QuantityItems = quantities.ToList();

            var comparisons = _comparator.Compare(run.Id, quantities, groundTruth);
            run.ComparisonResults = comparisons.ToList();

            run.Status = ProcessingStatus.Completed;
            run.CompletedAt = DateTime.UtcNow;
            await _runRepository.UpdateAsync(run);
        }
        catch (Exception ex)
        {
            run.Status = ProcessingStatus.Failed;
            run.ErrorMessage = ex.Message;
            run.CompletedAt = DateTime.UtcNow;
            await _runRepository.UpdateAsync(run);
        }

        return run;
    }
}
