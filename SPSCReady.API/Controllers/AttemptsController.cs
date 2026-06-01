using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs.MockTest;
using SPSCReady.Application.Interfaces;
using System.Security.Claims;

namespace SPSCReady.API.Controllers;

[ApiController]
[Route("api/v1/attempts")]
[Authorize]
public class AttemptsController : ControllerBase
{
    private readonly IAttemptService _attemptService;

    public AttemptsController(IAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    /// <summary>
    /// Start attempt - creates UserAttempt row, returns AttemptId + ExpiresAt - REQUIRES AUTH
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<StartAttemptResponseDto>> StartAttempt([FromBody] StartAttemptRequestDto request)
    {
        var userId = GetUserId();
        try
        {
            var result = await _attemptService.StartAttemptAsync(userId, request.MockTestId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Submit all answers - scores the attempt, stores UserAnswers, returns result - REQUIRES AUTH
    /// </summary>
    [HttpPost("{attemptId}/submit")]
    public async Task<ActionResult<AttemptResultDto>> SubmitAttempt(int attemptId, [FromBody] SubmitAttemptRequestDto request)
    {
        var userId = GetUserId();
        try
        {
            var result = await _attemptService.SubmitAttemptAsync(request, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get result for a submitted attempt (result screen) - REQUIRES AUTH
    /// </summary>
    [HttpGet("{attemptId}/result")]
    public async Task<ActionResult<AttemptResultDto>> GetAttemptResult(int attemptId)
    {
        var userId = GetUserId();
        var result = await _attemptService.GetAttemptResultAsync(attemptId, userId);
        if (result == null)
            return NotFound(new { message = "Attempt not found or not submitted" });

        return Ok(result);
    }

    /// <summary>
    /// Get all attempts for current user (history screen) - REQUIRES AUTH
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<List<AttemptSummaryDto>>> GetMyAttempts()
    {
        var userId = GetUserId();
        var attempts = await _attemptService.GetUserAttemptsAsync(userId);
        return Ok(attempts);
    }

    /// <summary>
    /// Mark attempt as Expired when timer runs out - REQUIRES AUTH
    /// </summary>
    [HttpPut("{attemptId}/expire")]
    public async Task<ActionResult> ExpireAttempt(int attemptId)
    {
        // Placeholder - would update attempt status to Expired
        return Ok(new { message = "Attempt marked as expired" });
    }

    private string GetUserId()
    {
        // Check standard XML claim, then shorter 'sub' or 'nameid' claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst("nameid")?.Value;

        if (userIdClaim != null)
            return userIdClaim;

        throw new UnauthorizedAccessException($"User ID not found in claims. Available claims: {string.Join(", ", User.Claims.Select(c => c.Type + "=" + c.Value))}");
    }
}
