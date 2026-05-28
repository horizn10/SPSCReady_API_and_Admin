namespace SPSCReady.Application.DTOs.MockTest;

public record MockTestDto(
    int MockTestId,
    string Title,
    string PaperType,
    int PaperNumber,
    int DurationMinutes,
    decimal TotalMarks,
    decimal TotalQuestions,
    decimal? PassingMarks
);
