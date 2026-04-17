using Microsoft.AspNetCore.Http;
using SPSCReady.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSCReady.Application.Interfaces
{
    public interface IPaperService
    {
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<List<PostDto>> GetPostsAsync(int departmentId);
        Task<List<ExamStageDto>> GetExamStagesAsync();
        Task<List<ExamSubjectDto>> GetSubjectsAsync();
        Task<bool> UploadPaperAsync(IFormFile pdfFile, UploadPaperDto request, string webRootPath);

        Task<List<ExamPaperListDto>> GetPapersAsync(
            string? search = null,
            int? stageId = null,
            string? departmentName = null,
            int? examYear = null,
            string? stageName = null,
            string? postName = null);
    }
}
