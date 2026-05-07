using System;
using System.Collections.Generic;

namespace SPSCReady.Application.DTOs
{
    public class ExamPaperListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime UploadedAt { get; set; }
        public List<string> DepartmentNames { get; set; } = new();
        public List<string> PostNames { get; set; } = new();
        public List<string> StageNames { get; set; } = new();
        public List<string> SubjectNames { get; set; } = new();

        public string? SubjectName { get; set; }
        public string? StageName { get; set; }

        public string? Url { get; set; }

        public string? ExamName { get; set; }
    }
}

