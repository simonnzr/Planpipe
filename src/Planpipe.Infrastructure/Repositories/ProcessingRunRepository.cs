using Microsoft.EntityFrameworkCore;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;
using Planpipe.Infrastructure.Data;

namespace Planpipe.Infrastructure.Repositories;

public class ProcessingRunRepository : IProcessingRunRepository
{
    private readonly PlanpipeDbContext _context;

    public ProcessingRunRepository(PlanpipeDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessingRun?> GetByIdAsync(Guid id)
    {
        return await _context.ProcessingRuns
            .Include(r => r.Features)
            .Include(r => r.QuantityItems)
            .Include(r => r.ComparisonResults)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<ProcessingRun>> GetAllAsync()
    {
        return await _context.ProcessingRuns
            .Include(r => r.Features)
            .Include(r => r.QuantityItems)
            .Include(r => r.ComparisonResults)
            .ToListAsync();
    }

    public async Task<ProcessingRun> CreateAsync(ProcessingRun run)
    {
        _context.ProcessingRuns.Add(run);
        await _context.SaveChangesAsync();
        return run;
    }

    public async Task UpdateAsync(ProcessingRun run)
    {
        _context.ProcessingRuns.Update(run);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var run = await _context.ProcessingRuns.FindAsync(id);
        if (run != null)
        {
            _context.ProcessingRuns.Remove(run);
            await _context.SaveChangesAsync();
        }
    }
}
