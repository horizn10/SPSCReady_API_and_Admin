using SPSCReady.Application.DTOs.MockTest;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.Application.Services;

public class MockTestService : IMockTestService
{
    private readonly IExamRepository _examRepository;
    private readonly IMockTestRepository _mockTestRepository;

    public MockTestService(IExamRepository examRepository, IMockTestRepository mockTestRepository)
    {
        _examRepository = examRepository;
        _mockTestRepository = mockTestRepository;
    }

    public async Task<List<ExamDto>> GetActiveExamsAsync()
    {
        var exams = await _examRepository.GetActiveExamsAsync();
        return exams.Select(e => new ExamDto(
            e.ExamId,
            e.ExamName,
            e.ExamCode,
            e.ExamYear,
            e.Description
        )).ToList();
    }

    public async Task<List<MockTestDto>> GetMockTestsByExamAsync(int examId)
    {
        var mockTests = await _mockTestRepository.GetByExamIdAsync(examId);
        return mockTests.Select(m => new MockTestDto(
            m.MockTestId,
            m.Title,
            m.PaperType.ToString(),
            m.PaperNumber,
            m.DurationMinutes,
            m.TotalMarks,
            m.TotalQuestions,
            m.PassingMarks
        )).ToList();
    }

    public async Task<MockTestDetailDto?> GetMockTestDetailAsync(int mockTestId)
    {
        var mockTest = await _mockTestRepository.GetByIdWithDetailsAsync(mockTestId);
        if (mockTest == null)
            return null;

        var sections = mockTest.Sections
            .OrderBy(s => s.OrderIndex)
            .Select(s => new SectionWithQuestionsDto(
                s.SectionId,
                s.SectionName,
                s.OrderIndex,
                s.MarksPerQuestion,
                s.NegativeMarks,
                s.Questions
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto(
                        q.QuestionId,
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.OrderIndex
                    )).ToList()
            )).ToList();

        return new MockTestDetailDto(
            mockTest.MockTestId,
            mockTest.Title,
            mockTest.PaperType.ToString(),
            mockTest.DurationMinutes,
            mockTest.TotalMarks,
            mockTest.TotalQuestions,
            sections
        );
    }
}
