using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IProcessingRunRepository
{
    Task<ProcessingRun?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ProcessingRun>> GetAllAsync();
    Task<ProcessingRun> CreateAsync(ProcessingRun run);
    Task UpdateAsync(ProcessingRun run);
    Task DeleteAsync(Guid id);
}
