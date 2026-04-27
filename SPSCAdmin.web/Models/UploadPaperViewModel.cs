using System.ComponentModel.DataAnnotations;

namespace SPSCAdmin.web.Models
{
    public class UploadPaperViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Department ID required")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Post ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Post ID required")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Stage ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Stage ID required")]
        public int StageId { get; set; }

        [Required(ErrorMessage = "Subject ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Subject ID required")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Please upload a PDF file")]
        public IFormFile PdfFile { get; set; } = null!;
    }
}