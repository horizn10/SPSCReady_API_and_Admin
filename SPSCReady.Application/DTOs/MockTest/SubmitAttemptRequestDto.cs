namespace SPSCReady.Application.DTOs.MockTest;

public record SubmitAttemptRequestDto(
    int AttemptId,
    List<AnswerSubmitDto> Answers
);
