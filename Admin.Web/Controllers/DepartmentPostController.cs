using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Admin.Models;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace SPSCReady.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentPostController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DepartmentPostController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildPageModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDepartment(DepartmentPostPageModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewDepartment.Name))
            {
                ModelState.AddModelError("NewDepartment.Name", "Department name is required.");
                var page = await BuildPageModelAsync();
                page.NewDepartment = model.NewDepartment;
                return View("Index", page);
            }

            bool exists = await _db.Departments
                .AnyAsync(d => d.Name.ToLower() == model.NewDepartment.Name.Trim().ToLower());

            if (exists)
            {
                ModelState.AddModelError("NewDepartment.Name", "This department already exists.");
                var page = await BuildPageModelAsync();
                page.NewDepartment = model.NewDepartment;
                return View("Index", page);
            }

            _db.Departments.Add(new Department
            {
                Name = model.NewDepartment.Name.Trim()
            });
            await _db.SaveChangesAsync();

            TempData["SuccessDept"] = $"Department '{model.NewDepartment.Name.Trim()}' added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost(DepartmentPostPageModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPost.Name))
            {
                ModelState.AddModelError("NewPost.Name", "Post name is required.");
                var page = await BuildPageModelAsync();
                page.NewPost = model.NewPost;
                return View("Index", page);
            }

            if (model.NewPost.DepartmentId == 0)
            {
                ModelState.AddModelError("NewPost.DepartmentId", "Please select a department.");
                var page = await BuildPageModelAsync();
                page.NewPost = model.NewPost;
                return View("Index", page);
            }

            bool exists = await _db.Posts
                .AnyAsync(p =>
                    p.DepartmentId == model.NewPost.DepartmentId &&
                    p.Name.ToLower() == model.NewPost.Name.Trim().ToLower());

            if (exists)
            {
                ModelState.AddModelError("NewPost.Name", "This post already exists in the selected department.");
                var page = await BuildPageModelAsync();
                page.NewPost = model.NewPost;
                return View("Index", page);
            }

            _db.Posts.Add(new Post
            {
                Name = model.NewPost.Name.Trim(),
                DepartmentId = model.NewPost.DepartmentId
            });
            await _db.SaveChangesAsync();

            TempData["SuccessPost"] = $"Post '{model.NewPost.Name.Trim()}' added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept != null)
            {
                _db.Departments.Remove(dept);
                await _db.SaveChangesAsync();
                TempData["SuccessDept"] = $"Department '{dept.Name}' deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post != null)
            {
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                TempData["SuccessPost"] = $"Post '{post.Name}' deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<DepartmentPostPageModel> BuildPageModelAsync()
        {
            var departments = await _db.Departments
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentViewModel { Id = d.Id, Name = d.Name })
                .ToListAsync();

            var posts = await _db.Posts
                .Include(p => p.Department)
                .OrderBy(p => p.Department.Name)
                .ThenBy(p => p.Name)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DepartmentId = p.DepartmentId,
                    DepartmentName = p.Department.Name
                })
                .ToListAsync();

            return new DepartmentPostPageModel
            {
                Departments = departments,
                Posts = posts
            };
        }
    }
}