using System.ComponentModel.DataAnnotations;

namespace SPSCAdmin.Web.Models
{
    public class UploadPaperViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cycle ID is required")]
        public Guid ExamCycleId { get; set; }

        [Required(ErrorMessage = "Stage ID is required")]
        public Guid ExamStageId { get; set; }

        [Required(ErrorMessage = "Subject ID is required")]
        public Guid SubjectId { get; set; }

        [Required(ErrorMessage = "Please upload a PDF file")]
        public IFormFile PdfFile { get; set; } = null!;
    }
}