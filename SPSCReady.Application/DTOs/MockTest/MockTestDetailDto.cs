namespace SPSCReady.Application.DTOs.MockTest;

public record MockTestDetailDto(
    int MockTestId,
    string Title,
    string PaperType,
    int DurationMinutes,
    int TotalMarks,
    int TotalQuestions,
    List<SectionWithQuestionsDto> Sections
);
