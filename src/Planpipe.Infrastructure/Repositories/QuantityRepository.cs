using Microsoft.EntityFrameworkCore;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;
using Planpipe.Infrastructure.Data;

namespace Planpipe.Infrastructure.Repositories;

public class QuantityRepository : IQuantityRepository
{
    private readonly PlanpipeDbContext _context;

    public QuantityRepository(PlanpipeDbContext context)
    {
        _context = context;
    }

    public async Task<QuantityItem?> GetQuantityByIdAsync(Guid id)
    {
        return await _context.QuantityItems.FindAsync(id);
    }

    public async Task<IReadOnlyList<QuantityItem>> GetQuantitiesByRunIdAsync(Guid runId)
    {
        return await _context.QuantityItems
            .Where(q => q.ProcessingRunId == runId)
            .ToListAsync();
    }

    public async Task<QuantityItem> CreateQuantityAsync(QuantityItem item)
    {
        _context.QuantityItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateQuantityAsync(QuantityItem item)
    {
        _context.QuantityItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteQuantityAsync(Guid id)
    {
        var item = await _context.QuantityItems.FindAsync(id);
        if (item != null)
        {
            _context.QuantityItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<GroundTruthItem?> GetGroundTruthByIdAsync(Guid id)
    {
        return await _context.GroundTruthItems.FindAsync(id);
    }

    public async Task<IReadOnlyList<GroundTruthItem>> GetAllGroundTruthAsync()
    {
        return await _context.GroundTruthItems.ToListAsync();
    }

    public async Task<GroundTruthItem> CreateGroundTruthAsync(GroundTruthItem item)
    {
        _context.GroundTruthItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateGroundTruthAsync(GroundTruthItem item)
    {
        _context.GroundTruthItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGroundTruthAsync(Guid id)
    {
        var item = await _context.GroundTruthItems.FindAsync(id);
        if (item != null)
        {
            _context.GroundTruthItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
