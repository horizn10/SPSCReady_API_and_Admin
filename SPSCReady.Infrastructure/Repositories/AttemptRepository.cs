using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Domain.Enums;
using SPSCReady.Infrastructure.Data;

namespace SPSCReady.Infrastructure.Repositories;

public class AttemptRepository : IAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public AttemptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserAttempt> CreateAsync(UserAttempt attempt)
    {
        _context.UserAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt;
    }

    public async Task<UserAttempt?> GetByIdAsync(int attemptId)
    {
        return await _context.UserAttempts
            .Include(a => a.MockTest)
            .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
    }

    public async Task<UserAttempt?> GetWithAnswersAsync(int attemptId)
    {
        return await _context.UserAttempts
            .Include(a => a.MockTest)
            .Include(a => a.UserAnswers)
            .ThenInclude(ua => ua.Question)
            .ThenInclude(q => q.Section)
            .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
    }

    public async Task<List<UserAttempt>> GetByUserIdAsync(string userId)
    {
        return await _context.UserAttempts
            .Where(a => a.UserId == userId)
            .Include(a => a.MockTest)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync();
    }

    // Fetch by unique key (UserId, MockTestId) regardless of status.
    public async Task<UserAttempt?> GetExistingAttemptAsync(string userId, int mockTestId)
    {
        return await _context.UserAttempts
            .FirstOrDefaultAsync(a => a.UserId == userId && a.MockTestId == mockTestId);
    }


    public async Task AddAnswersAsync(List<UserAnswer> answers)
    {
        _context.UserAnswers.AddRange(answers);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAnswersByAttemptIdAsync(int attemptId)
    {
        // Delete existing answers so retry/resume doesn't violate unique (AttemptId, QuestionId)
        var existing = await _context.UserAnswers
            .Where(a => a.AttemptId == attemptId)
            .ToListAsync();

        if (existing.Count == 0) return;

        _context.UserAnswers.RemoveRange(existing);
        await _context.SaveChangesAsync();
    }



    public async Task UpdateAsync(UserAttempt attempt)
    {
        _context.UserAttempts.Update(attempt);
        await _context.SaveChangesAsync();
    }
}
