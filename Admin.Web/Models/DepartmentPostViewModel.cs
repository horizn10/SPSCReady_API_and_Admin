using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPSCReady.Admin.Models
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }

    public class PostViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Post name is required.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a department.")]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
    }

    public class DepartmentPostPageModel
    {
        public DepartmentViewModel NewDepartment { get; set; } = new();
        public PostViewModel NewPost { get; set; } = new();

        public List<DepartmentViewModel> Departments { get; set; } = new();
        public List<PostViewModel> Posts { get; set; } = new();
    }
}