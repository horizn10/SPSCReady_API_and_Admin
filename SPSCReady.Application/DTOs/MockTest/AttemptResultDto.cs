namespace SPSCReady.Application.DTOs.MockTest;

public record AttemptResultDto(
    int AttemptId,
    string MockTestTitle,
    decimal TotalScore,
    int TotalMarks,
    decimal Percentage,
    int CorrectCount,
    int WrongCount,
    int SkippedCount,
    bool IsPassed,
    List<SectionResultDto> SectionBreakdown,
    List<AnswerReviewDto> AnswerReview
);

public record SectionResultDto(
    int SectionId,
    string SectionName,
    int CorrectCount,
    int WrongCount,
    int SkippedCount,
    decimal SectionScore
);

public record AnswerReviewDto(
    int QuestionId,
    string QuestionText,
    char? SelectedOption,
    char CorrectOption,
    bool IsCorrect,
    string? Explanation
);
