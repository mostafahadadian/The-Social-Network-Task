using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



    public class RoleController : Controller
    {
        private RoleManager<IdentityRole<int>> _roleManager;
        public RoleController(RoleManager<IdentityRole<int>> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        
        public IActionResult Create() { return View(); }
        [HttpPost]
        public IActionResult Create(string rolename)
        {
            if (!string.IsNullOrEmpty(rolename))
            {
                if (!_roleManager.RoleExistsAsync(rolename).Result)
                {
                   var result =  _roleManager.CreateAsync(new IdentityRole<int>(rolename)).Result;
                }
            }
            return RedirectToAction("Index");   
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var role = _roleManager.FindByIdAsync(id).Result;
                if (role != null)
                {
                   await _roleManager.DeleteAsync(role);
                }
            }
            return RedirectToAction("Index");
        }
    }

