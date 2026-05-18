using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;

namespace SPSCReady.Infrastructure.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly ApplicationDbContext _context;

    public ExamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Exam>> GetActiveExamsAsync()
    {
        return await _context.Exams
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.ExamYear)
            .ToListAsync();
    }

    public async Task<Exam?> GetByIdAsync(int examId)
    {
        return await _context.Exams
            .FirstOrDefaultAsync(e => e.ExamId == examId);
    }

    public async Task<Exam> CreateAsync(Exam exam)
    {
        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();
        return exam;
    }
}
