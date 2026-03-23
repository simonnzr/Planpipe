namespace Planpipe.Core.Models;

public class QuantityItem
{
    public Guid Id { get; set; }
    public Guid ProcessingRunId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    
    public ProcessingRun ProcessingRun { get; set; } = null!;
}
