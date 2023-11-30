using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using To_Do_List.Data;
using To_Do_List.Models;
using Task = To_Do_List.Models.Task;

namespace To_Do_List.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(ApplicationDbContext context,  UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Selects list of profiles that belong to logged in user
        /// </summary>
        /// <returns>List of Profiles</returns>
        public async Task<IActionResult> Index()
        {
            List<Profile> Profiles = await (from Profile in _context.Profiles
                                      where Profile.UserId == _userManager.GetUserId(User)
                                      select Profile).ToListAsync();

            return View(Profiles);
        }

        /// <summary>
        /// Selects list of Tasks that a profile holds
        /// </summary>
        /// <param name="id">The profile's id</param>
        /// <returns>List of Tasks</returns>
        public async Task<IActionResult> AssignedTasks(int id)
        {
            Profile? profile = await (from Profile in _context.Profiles
                                     where(Profile.ProfileId == id)
                                     select Profile).FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound();
            }

            // Populate ViewModel
            ProfileDisplayTaskViewModel viewModel = new()
            {
                AllTasks = await (from Task in _context.Tasks
                                  join Profile in _context.Profiles on Task.Assignee.ProfileId equals profile.ProfileId
                                  where Task.Assignee.ProfileId == Profile.ProfileId
                                  select Task).OrderBy(t => t.TaskId).ToListAsync()

                , Name = profile.Name
                , ProfileId = profile.ProfileId
            };

            return View(viewModel);
        }

        /// <summary>
        /// Creates a profile then populates 'UserId' field
        /// </summary>
        /// <returns>View and profile</returns>
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var profile = new Profile
                {
                    UserId = userId
                };

                return View(profile);
            }
            return View();
        }

        /// <summary>
        /// Creates new profile
        /// </summary>
        /// <param name="profile">THe view Profile</param>
        /// <returns>View if 'Model.State' is invalid</returns>
        [HttpPost]
        public async Task<IActionResult> Create(Profile profile)
        {
            if (ModelState.IsValid)
            {
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"{profile.Name} was added successfully!";
                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }

        /// <summary>
        /// Checks for Profile to delete
        /// </summary>
        /// <param name="id">The Profile's Id</param>
        /// <returns>Delete confirmation View</returns>
        public async Task<IActionResult> Delete(int id)
        {
            Profile? profileToDelete = await _context.Profiles.FindAsync(id);

            if (profileToDelete == null)
            {
                return NotFound();
            }

            return View(profileToDelete);
        }

        /// <summary>
        /// Delete's given profile and all assigned tasks
        /// </summary>
        /// <param name="id">The Profile's Id</param>
        /// <returns>Redirects to Profile Index View</returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfimed(int id)
        {
            Profile profileToDelete = await _context.Profiles.FindAsync(id);

            if (profileToDelete != null)
            {
                // List of connected tasks
                List<Models.Task> TaskToDelete = await (from task in _context.Tasks
                                          where task.Assignee.ProfileId == profileToDelete.ProfileId
                                          select task).ToListAsync();
                // Remove tasks
                foreach (Task task in TaskToDelete)
                {
                    _context.Remove(task);
                }

                // Remove then deletes Profile and Tasks
                _context.Remove(profileToDelete);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{profileToDelete.Name}\" and all Tasks associated were deleted successfully!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Message"] = "This profile was already deleted";
            return RedirectToAction("Index");
        }
    }
}
