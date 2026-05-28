namespace SPSCReady.Application.DTOs.MockTest;

public record MockTestDetailDto(
    int MockTestId,
    string Title,
    string PaperType,
    int DurationMinutes,
    decimal TotalMarks,
    decimal TotalQuestions,
    List<SectionWithQuestionsDto> Sections
);
