using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs.MockTest;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.API.Controllers;

[ApiController]
[Route("api/v1/exams")]
public class ExamsController : ControllerBase
{
    private readonly IMockTestService _mockTestService;

    public ExamsController(IMockTestService mockTestService)
    {
        _mockTestService = mockTestService;
    }

    /// <summary>
    /// Get all active exams (for home screen) - PUBLIC
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ExamDto>>> GetExams()
    {
        var exams = await _mockTestService.GetActiveExamsAsync();
        return Ok(exams);
    }

    /// <summary>
    /// Get all mock tests for an exam - PUBLIC
    /// </summary>
    [HttpGet("{examId}/mocktests")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MockTestDto>>> GetMockTestsByExam(int examId)
    {
        var mockTests = await _mockTestService.GetMockTestsByExamAsync(examId);
        return Ok(mockTests);
    }

    /// <summary>
    /// Create a new exam (admin only) - REQUIRES AUTH
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateExam([FromBody] CreateExamRequest request)
    {
        // This is placeholder - implement as needed
        return Ok(new { message = "Exam creation - admin endpoint" });
    }
}

public record CreateExamRequest(string ExamName, string ExamCode, int ExamYear, string? Description);
