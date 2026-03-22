using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IComparisonRepository
{
    Task<ComparisonResult?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ComparisonResult>> GetByRunIdAsync(Guid runId);
    Task<ComparisonResult> CreateAsync(ComparisonResult result);
    Task UpdateAsync(ComparisonResult result);
    Task DeleteAsync(Guid id);
}
