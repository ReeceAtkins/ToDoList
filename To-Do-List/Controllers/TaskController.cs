using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models;
using To_Do_List.Data;
using Task = To_Do_List.Models.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace To_Do_List.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            List<Task> tasks = await (from task in _context.Tasks
                                      select task).ToListAsync();

            return View(tasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Create(Task task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{task.Title}\" was created!";

                return RedirectToAction("Index");
            }

            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Task? taskToEdit = await _context.Tasks.FindAsync(id);

            if (taskToEdit == null)
            {
                return NotFound();
            }

            return View(taskToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Task TaskModel)
        {
            if (ModelState.IsValid)
            {
                _context.Tasks.Update(TaskModel);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{TaskModel.Title}\" was updated successfully!";
                return RedirectToAction("Index");
            }

            return View(TaskModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Task? taskToDelete = await _context.Tasks.FindAsync(id);

            if (taskToDelete == null)
            {
                return NotFound();
            }

            return View(taskToDelete);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfimed(int id)
        {
            Task taskToDelete = await _context.Tasks.FindAsync(id);

            if(taskToDelete != null)
            {
                _context.Remove(taskToDelete);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"\"{taskToDelete.Title}\" was deleted successfully!";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "This task was already deleted";
            return RedirectToAction("Index");
        }
    }
}
