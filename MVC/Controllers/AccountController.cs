using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver.Linq;
using MVC.Models.Account;
using MVC.ViewModels.Account;


namespace MVC.Controllers
{
    public class AccountController : BaseController
    {

        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private RoleManager<ApplicationRole> roleManager;


        public AccountController(UserManager<User> um, SignInManager<User> sm, RoleManager<ApplicationRole> rm)
        {
            userManager = um;
            signInManager = sm;
            roleManager = rm;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            //if (User.Identity!.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            var model = new RegisterViewModel
            {
                roles = await roleManager.Roles.Select(r => r.Name).ToListAsync()

            };
            return View(model);

        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelStateErrorResponse("Felaktig registrering"));
            }

            try
            {
                User user = new()
                {
                    PhoneNumber = registerViewModel.Phonenumber,
                    Email = registerViewModel.Email,
                    Name = registerViewModel.Name,
                    UserName = registerViewModel.Email

                };

                var result = await userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, registerViewModel.selectedRole);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return Json(CreateResponse(
                        success: true,
                        message: "Registration successful!",
                        redirectUrl: Url.Action("Index", "Home") ?? "/",
                        notify: true
                    ));
                }

                return Json(CreateResponse(
                      success: false,
                      message: "Registration failed",
                      notify: true
                  ));

            }

            catch (Exception ex)
            {
                return Json(CreateResponse(
                  success: false,
                  message: "An error occurred during registration. Please try again.",
                  notify: true
              ));
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {

                return Json(ModelStateErrorResponse(ModelState.ErrorCount.ToString()));
            }

            try
            {
                var result = await signInManager.PasswordSignInAsync(
                     loginViewModel.Email,
                     loginViewModel.Password,
                     isPersistent: loginViewModel.RememberMe,
                     lockoutOnFailure: true
                 );

                if (result.Succeeded)
                {
                    return Json(CreateResponse(
                        success: true,
                        message: "Login lyckades!",
                        redirectUrl: Url.Action("Index", "Home") ?? "/"
                    ));
                }

                return Json(CreateResponse(
                    success: false,
                    errors: new Dictionary<string, string>
                    {
                        ["Password"] = "Ogiltigt username or password"
                    },
                    notify: true
                ));
            }
            catch (Exception ex)
            {
                return Json(CreateResponse(
                success: false,
                message: "Ett error hände vid login. Försök igen.",
                notify: true
                ));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Protected()
        {
            return Json(ModelStateErrorResponse("Logged in only"));
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Json(ModelStateErrorResponse("Admin only"));
        }
    }
}
