using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver;
using MVC.Models.Account;
using MVC.ViewModels.UserViewModel;

namespace MVC.Controllers
{
    public class UserController : BaseController
    {

        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private RoleManager<ApplicationRole> _roleManager;
        private UserManager<User> _userManager;


        public UserController(IUserRepository userRepository, IRoleRepository roleRepository, RoleManager<ApplicationRole> rm, UserManager<User> um)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleManager = rm;
            _userManager = um;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")] avkommentera denna i prod
        public async Task<IActionResult> UserList()
        {
            List<User> allUsers = await _userRepository.GetAllUsersAsync();
            UserListViewModel vm = new UserListViewModel();
            List<ApplicationRole> roles = await _roleRepository.GetAllRolesAsync();

            vm.Users = allUsers;
            vm.UserRoles = await _roleRepository.GetAllRolesAsync();

            return View(vm);
        }

        //[Authorize(Roles = "Admin")] avkommentera denna i prod
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            User user = await _userRepository.GetUser(id);

            List<ApplicationRole> rawRoles = await _roleRepository.GetAllRolesAsync();
            List<String?> stringRoles = rawRoles.Select(g => g.Name).ToList();

            string selectedRoleId = user.Roles.First().ToString();
            var selectedRoleTemp = await _roleManager.FindByIdAsync(selectedRoleId);
            string? selectedRoleName = selectedRoleTemp.Name;

            if (selectedRoleName == null)
            {
                selectedRoleName = "-";
            }


            EditUserViewModel editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phonenumber = user.PhoneNumber,
                Password = "",
                roles = stringRoles,
                selectedRole = selectedRoleName
            };
            
          
            return View(editUserViewModel);
            
         }
        [HttpPost]
        //[Authorize(Roles = "Admin")] avkommentera denna i prod
        public async Task<IActionResult> Edit(EditUserViewModel editUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editUserViewModel);
            }

            User user = await _userRepository.GetUser(editUserViewModel.Id);
            user.Name = editUserViewModel.Name;
            user.Email = editUserViewModel.Email;
            user.PhoneNumber = editUserViewModel.Phonenumber;

            if (editUserViewModel.Password != null) 
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, editUserViewModel.Password);
            }
            

            return RedirectToAction("UserList");
        }
    }
}
