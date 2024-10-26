using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using The_Social_Network_Task.DAL;

namespace The_Social_Network_Task.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

      

        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var currentUserId =int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var users = string.IsNullOrEmpty(search)
                ? await _context.Users
                    .Where(u => u.Id != currentUserId) 
                    .ToListAsync()
                : await _context.Users
                    .Where(u => u.Id != currentUserId && u.UserName.Contains(search)) 
                    .ToListAsync();

            return View(users);
        }
    }
}
