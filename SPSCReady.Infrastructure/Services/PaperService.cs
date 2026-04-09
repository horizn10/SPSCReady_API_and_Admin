using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SPSCReady.Infrastructure.Services
{
    public class PaperService : IPaperService
    {
        private readonly ApplicationDbContext _context;

        public PaperService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<PostDto>> GetPostsAsync(Guid departmentId)
        {
            return await _context.Posts
                .Where(p => p.DepartmentId == departmentId)
                .Select(p => new PostDto { Id = p.Id, Name = p.Name, DepartmentId = p.DepartmentId })
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<ExamCycleDto>> GetExamCyclesAsync(Guid postId)
        {
            return await _context.ExamCycles
                .Where(ec => ec.PostId == postId)
                .Select(ec => new ExamCycleDto { Id = ec.Id, ExamYear = ec.ExamYear, PostId = ec.PostId })
                .OrderByDescending(ec => ec.ExamYear)
                .ToListAsync();
        }

        public async Task<List<ExamStageDto>> GetExamStagesAsync()
        {
            return await _context.ExamStages
                .Select(es => new ExamStageDto { Id = es.Id, Name = es.Name })
                .OrderBy(es => es.Name)
                .ToListAsync();
        }

        public async Task<List<ExamSubjectDto>> GetSubjectsAsync()
        {
            return await _context.Subjects
                .Select(s => new ExamSubjectDto { Id = s.Id, Name = s.Name })
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<bool> UploadPaperAsync(IFormFile pdfFile, UploadPaperDto request, string webRootPath)
        {
            try
            {
                // Validate PDF
                if (pdfFile == null || pdfFile.Length == 0 || !pdfFile.ContentType.Equals("application/pdf"))
                    return false;

                // Create pdfs dir if not exists
                var pdfsDir = Path.Combine(webRootPath, "pdfs");
                Directory.CreateDirectory(pdfsDir);

                // Unique filename
                var fileName = $"{Guid.NewGuid():N}.pdf";
                var filePath = Path.Combine(pdfsDir, fileName);

                // Save real PDF file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                // Create ExamPaper
                var paper = new ExamPaper
                {
                    ExamCycleId = request.ExamCycleId,
                    ExamStageId = request.ExamStageId,
                    SubjectId = request.SubjectId,
                    Title = request.Title,
                    PdfUrl = $"/pdfs/{fileName}",
                    UploadedAt = DateTime.UtcNow
                };

                _context.ExamPapers.Add(paper);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

