using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs.MockTest;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.API.Controllers;

[ApiController]
[Route("api/v1/mocktests")]
[Authorize]
public class MockTestsController : ControllerBase
{
    private readonly IMockTestService _mockTestService;

    public MockTestsController(IMockTestService mockTestService)
    {
        _mockTestService = mockTestService;
    }

    /// <summary>
    /// Get full paper with sections + questions (no correct answers) - REQUIRES AUTH
    /// </summary>
    [HttpGet("{mockTestId}")]
    public async Task<ActionResult<MockTestDetailDto>> GetMockTestDetail(int mockTestId)
    {
        var detail = await _mockTestService.GetMockTestDetailAsync(mockTestId);
        if (detail == null)
            return NotFound(new { message = "MockTest not found" });

        return Ok(detail);
    }

    /// <summary>
    /// Create a mock test under an exam (admin) - REQUIRES AUTH
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateMockTest([FromBody] CreateMockTestRequest request)
    {
        // Placeholder for admin endpoint
        return Ok(new { message = "MockTest creation - admin endpoint" });
    }

    /// <summary>
    /// Bulk add questions to a section (admin) - REQUIRES AUTH
    /// </summary>
    [HttpPost("{mockTestId}/questions")]
    public async Task<ActionResult> AddQuestions(int mockTestId, [FromBody] AddQuestionsRequest request)
    {
        // Placeholder for admin endpoint
        return Ok(new { message = "Questions bulk add - admin endpoint" });
    }

    /// <summary>
    /// Update mock test metadata (admin) - REQUIRES AUTH
    /// </summary>
    [HttpPut("{mockTestId}")]
    public async Task<ActionResult> UpdateMockTest(int mockTestId, [FromBody] UpdateMockTestRequest request)
    {
        // Placeholder for admin endpoint
        return Ok(new { message = "MockTest update - admin endpoint" });
    }
}

public record CreateMockTestRequest(int ExamId, string Title, string PaperType, int PaperNumber, int DurationMinutes, int TotalMarks, int TotalQuestions, decimal? PassingMarks);
public record AddQuestionsRequest(List<QuestionDataDto> Questions);
public record QuestionDataDto(int SectionId, string QuestionText, string OptionA, string OptionB, string OptionC, string OptionD, char CorrectOption, string? Explanation, string? DifficultyLevel);
public record UpdateMockTestRequest(string Title, int DurationMinutes, int TotalMarks, decimal? PassingMarks);
