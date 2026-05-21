using System.ComponentModel.DataAnnotations;
using SPSCReady.Application.DTOs;

namespace SPSCReady.Application.DTOs
{
    public class MultiUploadPaperDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int PostId { get; set; }

        public int ExamYear { get; set; }

        [Required, MinLength(1)]
        public List<SubjectPaperDto> SubjectPapers { get; set; } = new();
    }
}

