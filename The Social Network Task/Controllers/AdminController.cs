using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Social_Network_Task.Models;
using The_Social_Network_Task.DAL;
using The_Social_Network_Task.Entities;
using Microsoft.AspNetCore.Authorization;


[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private UserManager<User> _userManager;
    private IUserValidator<User> _userValidator;
    private IPasswordValidator<User> _passwordValidator;
    private IPasswordHasher<User> _passwordHasher;
    private RoleManager<IdentityRole<int>> _roleManager;
    public AdminController(
        UserManager<User> userManager,
        IUserValidator<User> userValidator,
        IPasswordValidator<User> passwordValidator,
        IPasswordHasher<User> passwordHasher,
        RoleManager<IdentityRole<int>> roleManager
        )

    {
        _userManager = userManager;
        _userValidator = userValidator;
        _passwordValidator = passwordValidator;
        _passwordHasher = passwordHasher;
        _roleManager = roleManager;
    }


    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new List<UserWithRolesViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserWithRolesViewModel
            {
                User = user,
                Roles = string.Join(", ", roles)
            });
        }
        return View(userRoles);
    }
    public async Task<IActionResult> SearchUser(string search)
    {
        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u => u.UserName.Contains(search));
        }

        return View(await users.ToListAsync());
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult Create() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(CreateModel createModel)
    {
        if (ModelState.IsValid)
        {
            User user = new User
            {
                UserName = createModel.Username,
                Email = createModel.Email,
                FullName = createModel.FullName
            };
            IdentityResult result = await _userManager.CreateAsync(user, createModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (IdentityError er in result.Errors)
                {
                    ModelState.AddModelError("", er.Description);
                }
            }
        }

        return View(createModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string Id)
    {
        if (!string.IsNullOrEmpty(Id))
        {

            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                  return  RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError er in result.Errors)
                    {
                        ModelState.AddModelError("", er.Description);
                    }
                }
            }

        }

        return View(Id);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(string Id)
    {
        if (!string.IsNullOrEmpty(Id))
        {
            EditModel editModel = new EditModel();
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    editModel.FullName = user.FullName;
                    editModel.Email = user.Email;
                    return View(editModel);
                }
                else
                {
                    foreach (IdentityError er in result.Errors)
                    {
                        ModelState.AddModelError("", er.Description);
                    }
                }
            }

        }

        return View("Index", Id);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(EditModel editModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(editModel.Id);
            if (user != null)
            {
                user.FullName = editModel.FullName;
                user.Email = editModel.Email;
                IdentityResult validemail = await _userValidator.ValidateAsync(_userManager, user);
                if (!validemail.Succeeded)
                {
                    foreach (IdentityError er in validemail.Errors)
                    {
                        ModelState.AddModelError("", er.Description);
                    }

                }
                IdentityResult passValid = null;
                if (!string.IsNullOrEmpty(editModel.Password))
                {
                    passValid = _passwordValidator.ValidateAsync(_userManager, user, editModel.Password).Result;
                    if (passValid.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, editModel.Password);
                    }
                    else
                    {
                        foreach (IdentityError er in validemail.Errors)
                        {
                            ModelState.AddModelError("", er.Description);
                        }
                    }
                }

                if (validemail.Succeeded && passValid.Succeeded)
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }

            }

        }
        return View(editModel);
    }

    [HttpGet]
    public async Task<IActionResult> AssignRole()
    {
        var users = await _userManager.Users.ToListAsync();
        var roles = await _roleManager.Roles.ToListAsync();
        var model = new AssignRoleViewModel
        {
            Users = users,
            Roles = roles
        };
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _roleManager.RoleExistsAsync(role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction("AssignRole");
    }

    public async Task<IActionResult> GetUserRole(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            var user =await _userManager.FindByNameAsync(username);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                return Json(roles);
            }

        }
        return View();
    }
}