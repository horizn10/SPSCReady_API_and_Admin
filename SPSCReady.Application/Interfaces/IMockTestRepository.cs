using SPSCReady.Domain.Entities;

namespace SPSCReady.Application.Interfaces;

public interface IMockTestRepository
{
    Task<List<MockTest>> GetByExamIdAsync(int examId);
    Task<MockTest?> GetByIdWithDetailsAsync(int mockTestId);
    Task<MockTest?> GetByIdAsync(int mockTestId);
    Task<MockTest> CreateAsync(MockTest mockTest);
}
