using SPSCReady.Domain.Entities;

namespace SPSCReady.Application.Interfaces;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(int questionId);
    Task<List<Question>> GetBySectionIdAsync(int sectionId);
    Task CreateBulkAsync(List<Question> questions);
}
