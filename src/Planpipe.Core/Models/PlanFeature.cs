namespace Planpipe.Core.Models;

public class PlanFeature
{
    public Guid Id { get; set; }
    public Guid ProcessingRunId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public int Page { get; set; }
    
    public ProcessingRun ProcessingRun { get; set; } = null!;
}
