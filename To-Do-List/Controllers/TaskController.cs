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

        /// <summary>
        /// Populates Create ViewModel
        /// </summary>
        /// <returns>ViewModel</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Selects all profiles that the user has created
            TaskCreateViewModel viewModel = new()
            {
                AllProfiles = await (from Profile in _context.Profiles
                             where Profile.UserId == _userManager.GetUserId(User)
                             select Profile).OrderBy(p => p.UserId).ToListAsync()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Populates Create ViewModel with given ProfileId
        /// </summary>
        /// <param name="id">Profile's Id</param>
        /// <returns>Create View and ViewModel</returns>
        [HttpGet("CreateWithId/{id}")]
        public async Task<IActionResult> CreateWithId(int id)
        {
            TaskCreateViewModel viewModel = new()
            {
                // Selects a single profile
                AllProfiles = await(from Profile in _context.Profiles
                                    where Profile.ProfileId == id
                                    select Profile).ToListAsync()
            };

            return View("Create", viewModel);
        }

        /// <summary>
        /// Creates a new Task
        /// </summary>
        /// <param name="task">The create view's ViewModel</param>
        /// <returns>Redirect to AssignedTasks in Profile controller</returns>
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

                // Redirects to the profile that matches the tasks's 'Assignee.ProfileId'
                return RedirectToAction("AssignedTasks", "Profile", new { Id = newTask.Assignee.ProfileId });
            }

            task.AllProfiles = _context.Profiles.OrderBy(p => p.Name).ToList();
            return View(task);
        }

        /// <summary>
        /// Selects task by given Id then populates ViewModel
        /// </summary>
        /// <param name="id">The Task's Id</param>
        /// <returns>Populated ViewModel to Edit View</returns>
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

        /// <summary>
        /// Edits a single task
        /// </summary>
        /// <param name="TaskModel">The Edit View's ViewModel</param>
        /// <returns>Redirects to Assigned Tasks in Profile controller</returns>
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

        /// <summary>
        /// Selects task then populates viewModel with Task to be deleted
        /// </summary>
        /// <param name="id">The Task's id</param>
        /// <returns>Delete confirmation View</returns>
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

        /// <summary>
        /// Deletes given Task
        /// </summary>
        /// <param name="TaskModel">The Delete View's ViewModel</param>
        /// <returns>Redirects to AssignedTasks in Profile controller</returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfimed(TaskEditAndDeleteViewModel TaskModel)
        {
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
            return RedirectToAction("AssignedTasks", "Profile", new { Id = taskToDelete.Assignee.ProfileId });
        }
    }
}
