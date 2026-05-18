using SPSCReady.Domain.Entities;

namespace SPSCReady.Application.Interfaces;

public interface IExamRepository
{
    Task<List<Exam>> GetActiveExamsAsync();
    Task<Exam?> GetByIdAsync(int examId);
    Task<Exam> CreateAsync(Exam exam);
}
