using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;

namespace SPSCReady.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(int questionId)
    {
        return await _context.Questions
            .Include(q => q.Section)
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
    }

    public async Task<List<Question>> GetBySectionIdAsync(int sectionId)
    {
        return await _context.Questions
            .Where(q => q.SectionId == sectionId && q.IsActive)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();
    }

    public async Task CreateBulkAsync(List<Question> questions)
    {
        _context.Questions.AddRange(questions);
        await _context.SaveChangesAsync();
    }
}
