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


            EditUserViewModel editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phonenumber = user.PhoneNumber,
                Password = "",
                roles = stringRoles,
                selectedRole = selectedRoleName,
                selectedRoleID = user.Roles.First().ToString(),
                availableRoles = await _roleRepository.GetAllRolesAsync()
            };


            return View(editUserViewModel);

        }
        /// <summary>
        /// Denna metod postar ändringar av användarinformation
        /// </summary>
        /// <param name="editUserViewModel"></param>
        /// <param name="roleManager"></param>
        /// <returns></returns>
        /// 
        //[Authorize(Roles = "Admin")] avkommentera denna i prod !!!
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel editUserViewModel)
        {
            User user = await _userRepository.GetUser(editUserViewModel.Id);

            // Bortser från validering av lösenord om input är tomt
            if (string.IsNullOrWhiteSpace(editUserViewModel.Password))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            // Kör validering
            if (!ModelState.IsValid)
            {
                return View(editUserViewModel);
            }

            // Ändrar infon i databasen
            user.Name = editUserViewModel.Name;
            user.Email = editUserViewModel.Email;
            user.PhoneNumber = editUserViewModel.Phonenumber;

            if (editUserViewModel.selectedRole != null)
            {
                user.Roles.Clear();
                List<ApplicationRole> allRoles = await _roleRepository.GetAllRolesAsync();
                Guid selectedRole = allRoles.FirstOrDefault(r => r.Name == editUserViewModel.selectedRole).Id;
                user.Roles.Add(selectedRole);

            }


            if (editUserViewModel.Password != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, editUserViewModel.Password);
            }

            // Kör repo-frågan med ändrade uppgifterna
            await _userRepository.UpdateUserAsync(user);

            return RedirectToAction("UserList");
        }




        public async Task<IActionResult> Delete(Guid id)
        {
            await _userRepository.DeleteUserAsync(id);

            return RedirectToAction("UserList");
        }
    }
}
