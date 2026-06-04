using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPSCReady.Admin.Models
{
    /// <summary>
    /// Form model for uploading a question paper.
    /// Maps to: ExamPapers + ExamPaperDepartments + ExamPaperPosts + ExamPaperStages + ExamPaperSubjects
    /// </summary>
    public class ExamPaperCreateModel
    {
        // ── Header fields ────────────────────────────────────────────────────

        [Required(ErrorMessage = "Please select a department.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a post.")]
        [Display(Name = "Exam Post")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Please enter the exam year.")]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100.")]
        [Display(Name = "Exam Year")]
        public int ExamYear { get; set; }

        [Required(ErrorMessage = "Please enter a title for this paper set.")]
        [Display(Name = "Paper Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select the number of papers.")]
        [Range(1, 10, ErrorMessage = "Must be between 1 and 10.")]
        [Display(Name = "Number of Papers")]
        public int NumSubjects { get; set; }

        // ── Per-paper rows (bound as SubjectPapers[0].X, SubjectPapers[1].X …) ──
        public List<SubjectPaperInputDto> SubjectPapers { get; set; } = new();
    }

    /// <summary>
    /// One row in the dynamic table → maps to ExamPaperSubjects.
    /// SubjectId is looked up/created by the controller from SubjectName + StageId.
    /// The uploaded PDF is saved to disk and its path stored in ExamPaperSubjects.Url.
    /// </summary>
    public class SubjectPaperInputDto
    {
        [Required(ErrorMessage = "Paper name is required.")]
        [StringLength(200)]
        public string SubjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a phase.")]
        public int StageId { get; set; }

        [Required(ErrorMessage = "Please upload a PDF file.")]
        public IFormFile? PdfFile { get; set; }
    }

    // ── Lookup DTOs (populated from DB for dropdowns) ─────────────────────────

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PostDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }

    public class ExamStageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}