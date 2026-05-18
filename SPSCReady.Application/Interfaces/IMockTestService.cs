using SPSCReady.Application.DTOs.MockTest;

namespace SPSCReady.Application.Interfaces;

public interface IMockTestService
{
    Task<List<ExamDto>> GetActiveExamsAsync();
    Task<List<MockTestDto>> GetMockTestsByExamAsync(int examId);
    Task<MockTestDetailDto?> GetMockTestDetailAsync(int mockTestId);
}
