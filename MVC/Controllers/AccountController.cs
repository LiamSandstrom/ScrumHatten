using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using MVC.Models.Account;


namespace MVC.Controllers
{
    public class AccountController : BaseController
    {

        private UserManager<User> userManager;
        private SignInManager<User> signInManager;


        public AccountController(UserManager<User> um, SignInManager<User> sm)
        {
            userManager = um;
            signInManager = sm;
        }




        [HttpGet]

        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }


            return View();


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
                    Name = registerViewModel.Name

                };

                var result = await userManager.CreateAsync(user, registerViewModel.Password);


                if (result.Succeeded)
                {
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
                      message: "Username already taken",
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





    }

}
