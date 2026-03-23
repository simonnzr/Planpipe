using Microsoft.EntityFrameworkCore;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;
using Planpipe.Infrastructure.Data;

namespace Planpipe.Infrastructure.Repositories;

public class ComparisonRepository : IComparisonRepository
{
    private readonly PlanpipeDbContext _context;

    public ComparisonRepository(PlanpipeDbContext context)
    {
        _context = context;
    }

    public async Task<ComparisonResult?> GetByIdAsync(Guid id)
    {
        return await _context.ComparisonResults.FindAsync(id);
    }

    public async Task<IReadOnlyList<ComparisonResult>> GetByRunIdAsync(Guid runId)
    {
        return await _context.ComparisonResults
            .Where(c => c.ProcessingRunId == runId)
            .ToListAsync();
    }

    public async Task<ComparisonResult> CreateAsync(ComparisonResult result)
    {
        _context.ComparisonResults.Add(result);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task UpdateAsync(ComparisonResult result)
    {
        _context.ComparisonResults.Update(result);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var result = await _context.ComparisonResults.FindAsync(id);
        if (result != null)
        {
            _context.ComparisonResults.Remove(result);
            await _context.SaveChangesAsync();
        }
    }
}
