using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using To_Do_List.Data;
using To_Do_List.Models;

namespace To_Do_List.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager
                              )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public  IActionResult Index()
        {
            int numProfiles = (from Profile in _context.Profiles
                                     where Profile.UserId == _userManager.GetUserId(User)
                                     select Profile).Count();

            if (numProfiles > 0)
            {
                HttpContext.Session.SetInt32("DoesUserHaveProfiles", numProfiles);
                return RedirectToAction("Index", "Profile");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}