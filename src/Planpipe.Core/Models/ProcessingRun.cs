using Planpipe.Core.Enums;

namespace Planpipe.Core.Models;

public class ProcessingRun
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public ProcessingStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    public ICollection<PlanFeature> Features { get; set; } = new List<PlanFeature>();
    public ICollection<QuantityItem> QuantityItems { get; set; } = new List<QuantityItem>();
    public ICollection<ComparisonResult> ComparisonResults { get; set; } = new List<ComparisonResult>();
}
