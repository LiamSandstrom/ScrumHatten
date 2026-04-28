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
        //[Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UserList()
        {
            List<ApplicationRole> allRoles = await _roleRepository.GetAllRolesAsync();
            List<string> allRolesString = new List<string>();
            foreach (var item in allRoles)
            {
                allRolesString.Add(item.ToString());
            }

            List<User> allUsers = await _userRepository.GetAllUsersAsync();
            UserListViewModel vm = new UserListViewModel();
            List<ApplicationRole> roles = await _roleRepository.GetAllRolesAsync();

            vm.Users = allUsers;
            vm.UserRoles = await _roleRepository.GetAllRolesAsync();
            vm.allRoles = allRolesString;

            return View(vm);
        }

        [HttpGet("FilteredUserList")]
        public async Task<IActionResult> FilteredUserList(string selectedRole)
        {

            if (selectedRole == null)
            {
                return RedirectToAction(nameof(UserList));
            }


            List<ApplicationRole> roles = await _roleRepository.GetAllRolesAsync();
            Guid roleId = roles.Where(r => r.Name == selectedRole).FirstOrDefault().Id;

            List<User> allUsers = await _userRepository.GetAllUsersAsync();
            List<User> filteredUsers = allUsers.Where(u => u.Roles.FirstOrDefault() == roleId).ToList();

            List<string> allRolesString = new List<string>();
            foreach (var item in roles)
            {
                allRolesString.Add(item.ToString());
            }

            UserListViewModel vm = new UserListViewModel
            {
                Users = filteredUsers,
                UserRoles = await _roleRepository.GetAllRolesAsync(),
                allRoles = allRolesString

            };

            return View(nameof(UserList), vm);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            List<User> result = await _userRepository.GetUserByStringMatch(searchTerm);
            List<UserWithRoleName> userListExpanded = new List<UserWithRoleName>();

            for (int i = 0; i < result.Count; i++)
            {
                UserWithRoleName userWithRoleName = new UserWithRoleName
                {
                    User = result[i],
                };
                userListExpanded.Add(userWithRoleName);
                ApplicationRole gottenRole = await _roleRepository.GetRoleByIdAsync(result[i].Roles.FirstOrDefault());
                string? gottenRoleName = gottenRole.Name;
                if (gottenRoleName != null)
                {
                    userListExpanded[i].RoleName = gottenRoleName;
                }
                else
                {
                    userListExpanded[i].RoleName = "No role found";

                }
            }

            return Json(userListExpanded);
        }


        //[Authorize(Roles = "Admin")]
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
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel editUserViewModel)
        {
            User user = await _userRepository.GetUser(editUserViewModel.Id);

            if (string.IsNullOrWhiteSpace(editUserViewModel.Password))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (!ModelState.IsValid)
            {
                return View(editUserViewModel);
            }

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
