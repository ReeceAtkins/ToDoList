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


        public async Task<IActionResult> Index(int id)
        {
            // Selects profiles that UserId matches with the current logged in user
            List<Profile> Profiles = await (from Profile in _context.Profiles
                                      where Profile.UserId == _userManager.GetUserId(User)
                                      select Profile).ToListAsync();

            return View(Profiles);
        }

        public async Task<IActionResult> AssignedTasks(int id)
        {
            Profile? profile = await (from Profile in _context.Profiles
                                     where(Profile.ProfileId == id)
                                     select Profile).FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound();
            }

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

        public async Task<IActionResult> Delete(int id)
        {
            Profile? profileToDelete = await _context.Profiles.FindAsync(id);

            if (profileToDelete == null)
            {
                return NotFound();
            }

            return View(profileToDelete);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfimed(int id)
        {
            Profile profileToDelete = await _context.Profiles.FindAsync(id);

            if (profileToDelete != null)
            {
                // set all tasks to be deleted that are assigned to given profile id
                List<Models.Task> TaskToDelete = await (from task in _context.Tasks
                                          where task.Assignee.ProfileId == profileToDelete.ProfileId
                                          select task).ToListAsync();

                foreach (Task task in TaskToDelete)
                {
                    _context.Remove(task);
                }

                // set profile to be deleted
                _context.Remove(profileToDelete);

                // Save changes
                await _context.SaveChangesAsync();

                TempData["Message"] = $"\"{profileToDelete.Name}\" and all Tasks associated were deleted successfully!";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "This profile was already deleted";
            return RedirectToAction("Index");
        }
    }
}
