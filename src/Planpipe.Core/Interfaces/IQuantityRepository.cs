using Planpipe.Core.Models;

namespace Planpipe.Core.Interfaces;

public interface IQuantityRepository
{
    Task<QuantityItem?> GetQuantityByIdAsync(Guid id);
    Task<IReadOnlyList<QuantityItem>> GetQuantitiesByRunIdAsync(Guid runId);
    Task<QuantityItem> CreateQuantityAsync(QuantityItem item);
    Task UpdateQuantityAsync(QuantityItem item);
    Task DeleteQuantityAsync(Guid id);
    
    Task<GroundTruthItem?> GetGroundTruthByIdAsync(Guid id);
    Task<IReadOnlyList<GroundTruthItem>> GetAllGroundTruthAsync();
    Task<GroundTruthItem> CreateGroundTruthAsync(GroundTruthItem item);
    Task UpdateGroundTruthAsync(GroundTruthItem item);
    Task DeleteGroundTruthAsync(Guid id);
}
