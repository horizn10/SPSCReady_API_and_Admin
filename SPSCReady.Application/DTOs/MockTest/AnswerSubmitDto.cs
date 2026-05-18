namespace SPSCReady.Application.DTOs.MockTest;

public record AnswerSubmitDto(
    int QuestionId,
    char? SelectedOption,       // null = skipped
    bool IsMarkedForReview
);
