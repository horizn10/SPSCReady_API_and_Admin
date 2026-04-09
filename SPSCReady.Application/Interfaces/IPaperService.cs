using Microsoft.AspNetCore.Http;
using SPSCReady.Application.DTOs;
using SPSCReady.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSCReady.Application.Interfaces
{
    public interface IPaperService
    {
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<List<PostDto>> GetPostsAsync(Guid departmentId);
        Task<List<ExamCycleDto>> GetExamCyclesAsync(Guid postId);
        Task<List<ExamStageDto>> GetExamStagesAsync();
        Task<List<ExamSubjectDto>> GetSubjectsAsync();
        Task<bool> UploadPaperAsync(IFormFile pdfFile, UploadPaperDto request, string webRootPath);
    }
}
