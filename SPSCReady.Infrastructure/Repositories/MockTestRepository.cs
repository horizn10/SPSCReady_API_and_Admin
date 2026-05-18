using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;

namespace SPSCReady.Infrastructure.Repositories;

public class MockTestRepository : IMockTestRepository
{
    private readonly ApplicationDbContext _context;

    public MockTestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MockTest>> GetByExamIdAsync(int examId)
    {
        return await _context.MockTests
            .Where(m => m.ExamId == examId && m.IsActive)
            .OrderBy(m => m.PaperType)
            .ThenBy(m => m.PaperNumber)
            .ToListAsync();
    }

    public async Task<MockTest?> GetByIdWithDetailsAsync(int mockTestId)
    {
        return await _context.MockTests
            .Include(m => m.Sections)
            .ThenInclude(s => s.Questions)
            .FirstOrDefaultAsync(m => m.MockTestId == mockTestId && m.IsActive);
    }

    public async Task<MockTest?> GetByIdAsync(int mockTestId)
    {
        return await _context.MockTests
            .FirstOrDefaultAsync(m => m.MockTestId == mockTestId);
    }

    public async Task<MockTest> CreateAsync(MockTest mockTest)
    {
        _context.MockTests.Add(mockTest);
        await _context.SaveChangesAsync();
        return mockTest;
    }
}
