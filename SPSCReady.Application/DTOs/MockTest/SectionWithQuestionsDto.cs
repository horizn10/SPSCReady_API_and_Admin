namespace SPSCReady.Application.DTOs.MockTest;

public record SectionWithQuestionsDto(
    int SectionId,
    string SectionName,
    int OrderIndex,
    decimal MarksPerQuestion,
    decimal NegativeMarks,
    List<QuestionDto> Questions
);
