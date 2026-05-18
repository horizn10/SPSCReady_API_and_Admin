namespace SPSCReady.Application.DTOs.MockTest;

public record ExamDto(
    int ExamId,
    string ExamName,
    string ExamCode,
    int ExamYear,
    string? Description
);
