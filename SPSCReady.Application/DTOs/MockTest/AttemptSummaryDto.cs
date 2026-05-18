namespace SPSCReady.Application.DTOs.MockTest;

public record AttemptSummaryDto(
    int AttemptId,
    int MockTestId,
    string MockTestTitle,
    DateTime StartedAt,
    DateTime? SubmittedAt,
    decimal? TotalScore,
    int TotalMarks,
    decimal? Percentage,
    string Status
);
