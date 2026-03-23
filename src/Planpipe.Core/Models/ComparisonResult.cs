namespace Planpipe.Core.Models;

public class ComparisonResult
{
    public Guid Id { get; set; }
    public Guid ProcessingRunId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal? ExtractedQuantity { get; set; }
    public decimal? GroundTruthQuantity { get; set; }
    public double Variance { get; set; }
    public bool IsMatch { get; set; }
    
    public ProcessingRun ProcessingRun { get; set; } = null!;
}
