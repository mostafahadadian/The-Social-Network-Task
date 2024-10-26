using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using The_Social_Network_Task.Models;
using The_Social_Network_Task.Entities;



public class AccountController : Controller
{

    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public ActionResult Login(string returnUrl)
    {
        ViewBag.returnUrl = returnUrl ?? "/Account/Login";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel login, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (returnUrl == "/Account/Login")
                {
                    returnUrl = null;
                }
                User user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/User/Index");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.Email), "Invalid Email Or Password");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again later.");
            }
            
        }
        ViewBag.returnUrl = returnUrl;
        return View(login);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}

