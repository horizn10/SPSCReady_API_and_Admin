using SPSCReady.Domain.Entities;

namespace SPSCReady.Application.Interfaces;

public interface IAttemptRepository
{
    Task<UserAttempt> CreateAsync(UserAttempt attempt);
    Task<UserAttempt?> GetByIdAsync(int attemptId);
    Task<UserAttempt?> GetWithAnswersAsync(int attemptId);
    Task<List<UserAttempt>> GetByUserIdAsync(int userId);
    Task<UserAttempt?> GetExistingAttemptAsync(int userId, int mockTestId);
    Task AddAnswersAsync(List<UserAnswer> answers);
    Task UpdateAsync(UserAttempt attempt);
}
