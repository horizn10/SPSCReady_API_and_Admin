using System.ComponentModel.DataAnnotations;

namespace SPSCReady.Application.DTOs
{
    public class UploadPaperDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int StageId { get; set; }

        [Required]
        public int SubjectId { get; set; }
    }
}
