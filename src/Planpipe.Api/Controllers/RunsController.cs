using Microsoft.AspNetCore.Mvc;
using Planpipe.Application.Services;
using Planpipe.Core.Interfaces;
using Planpipe.Core.Models;

namespace Planpipe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RunsController : ControllerBase
{
    private readonly PlanIngestionPipeline _pipeline;
    private readonly IProcessingRunRepository _runRepository;
    private readonly IQuantityRepository _quantityRepository;
    private readonly IComparisonRepository _comparisonRepository;

    public RunsController(
        PlanIngestionPipeline pipeline,
        IProcessingRunRepository runRepository,
        IQuantityRepository quantityRepository,
        IComparisonRepository comparisonRepository)
    {
        _pipeline = pipeline;
        _runRepository = runRepository;
        _quantityRepository = quantityRepository;
        _comparisonRepository = comparisonRepository;
    }

    [HttpPost]
    public async Task<ActionResult<ProcessingRun>> CreateRun(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
            !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only PDF files are supported");
        }

        using var stream = file.OpenReadStream();
        var groundTruth = new List<GroundTruthItem>();
        
        var run = await _pipeline.IngestAsync(stream, file.FileName, groundTruth);
        return CreatedAtAction(nameof(GetRun), new { id = run.Id }, run);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProcessingRun>>> GetRuns()
    {
        var runs = await _runRepository.GetAllAsync();
        return Ok(runs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProcessingRun>> GetRun(Guid id)
    {
        var run = await _runRepository.GetByIdAsync(id);
        if (run == null)
        {
            return NotFound();
        }
        return Ok(run);
    }

    [HttpGet("{id}/quantities")]
    public async Task<ActionResult<IReadOnlyList<QuantityItem>>> GetQuantities(Guid id)
    {
        var quantities = await _quantityRepository.GetQuantitiesByRunIdAsync(id);
        return Ok(quantities);
    }

    [HttpGet("{id}/comparison")]
    public async Task<ActionResult<IReadOnlyList<ComparisonResult>>> GetComparison(Guid id)
    {
        var comparison = await _comparisonRepository.GetByRunIdAsync(id);
        return Ok(comparison);
    }
}
