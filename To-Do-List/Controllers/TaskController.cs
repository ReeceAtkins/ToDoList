using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models;
using To_Do_List.Data;
using Task = To_Do_List.Models.Task;

namespace To_Do_List.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

                ViewData["Message"] = $"{task.Title} was created!";

                return View();
            }

            return View(task);
        }
    }
}
