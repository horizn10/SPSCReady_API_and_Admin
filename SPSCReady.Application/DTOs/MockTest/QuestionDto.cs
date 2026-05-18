namespace SPSCReady.Application.DTOs.MockTest;

public record QuestionDto(
    int QuestionId,
    string QuestionText,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    int OrderIndex
);
