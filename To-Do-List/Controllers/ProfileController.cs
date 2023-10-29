using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Data;
using To_Do_List.Models;

namespace To_Do_List.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext context,  UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(int id)
        {
            // Selects profiles that UserId matches with the current logged in user
            List<Profile> Profiles = await (from Profile in _context.Profiles
                                      where Profile.UserId == _userManager.GetUserId(User)
                                      select Profile).ToListAsync();

            return View(Profiles);
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
                return RedirectToAction("Index");
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
                _context.Remove(profileToDelete);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"\"{profileToDelete.Name}\" was deleted successfully!";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "This task was already deleted";
            return RedirectToAction("Index");
        }
    }
}
