using System.ComponentModel.DataAnnotations;

namespace SPSCReady.Application.DTOs
{
    public class UploadPaperDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public Guid ExamCycleId { get; set; }

        [Required]
        public Guid ExamStageId { get; set; }

        [Required]
        public Guid SubjectId { get; set; }
    }
}
