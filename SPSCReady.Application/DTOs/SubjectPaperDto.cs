using System.ComponentModel.DataAnnotations;

namespace SPSCReady.Application.DTOs
{
    public class SubjectPaperDto
    {
        [Required]
        public string Title { get; set; } = string.Empty; // subject name (paper name itself)

        [Required]
        public int StageId { get; set; }
    }
}


