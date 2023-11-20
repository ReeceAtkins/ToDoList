using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models;
using To_Do_List.Data;
using Task = To_Do_List.Models.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Profile = To_Do_List.Models.Profile;

namespace To_Do_List.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            // Selects all profiles that the user has created
            TaskCreateViewModel viewModel = new()
            {
                AllProfiles = (from Profile in _context.Profiles
                             where Profile.UserId == _userManager.GetUserId(User)
                             select Profile).OrderBy(p => p.UserId).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateViewModel task)
        {

            if (ModelState.IsValid)
            {
                Task newTask = new()
                {
                    Description = task.Description,
                    Title = task.Title,
                    Assignee = new Profile()
                    {
                        ProfileId = task.ChosenProfile
                    }
                };

                _context.Entry(newTask.Assignee).State = EntityState.Unchanged;

                _context.Add(newTask);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{newTask.Title}\" was created!";

                return RedirectToAction("Index");
            }

            task.AllProfiles = _context.Profiles.OrderBy(p => p.Name).ToList();
            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Task? task = await _context.Tasks
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

			TaskEditAndDeleteViewModel taskToEdit = new()
            {
                TaskId = task.TaskId,
                ProfileId = task.Assignee.ProfileId,
                Title = task.Title,
                Description = task.Description
            };

            return View(taskToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskEditAndDeleteViewModel TaskModel)
        {
            if (ModelState.IsValid)
            {
                Task taskToEdit = new()
                {
                    TaskId = TaskModel.TaskId,
                    Title = TaskModel.Title,
                    Description = TaskModel.Description,
                    Assignee = await _context.Profiles.FindAsync(TaskModel.ProfileId)
                };


                _context.Tasks.Update(taskToEdit);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{taskToEdit.Title}\" was updated successfully!";

                return RedirectToAction("AssignedTasks", "Profile", new { Id = taskToEdit.Assignee.ProfileId });
            }

            return View(TaskModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
			Task? task = await _context.Tasks
				.Include(t => t.Assignee)
				.FirstOrDefaultAsync(t => t.TaskId == id);

			if (task == null)
            {
                return NotFound();
            }

			TaskEditAndDeleteViewModel taskToDelete = new()
			{
				TaskId = task.TaskId,
				ProfileId = task.Assignee.ProfileId,
				Title = task.Title,
				Description = task.Description
			};

			return View(taskToDelete);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfimed(TaskEditAndDeleteViewModel TaskModel)
        {
            //Task taskToDelete = await _context.Tasks.FindAsync(id);

            Task taskToDelete = new()
            {
                TaskId = TaskModel.TaskId,
                Title = TaskModel.Title,
                Description = TaskModel.Description,
                Assignee = await _context.Profiles.FindAsync(TaskModel.ProfileId)
            };

            if (taskToDelete != null)
            {
                _context.Remove(taskToDelete);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{taskToDelete.Title}\" was deleted successfully!";

                return RedirectToAction("AssignedTasks", "Profile", new { Id = taskToDelete.Assignee.ProfileId });
            }

            TempData["Message"] = "This task was already deleted";
            return RedirectToAction("Index");
        }
    }
}
