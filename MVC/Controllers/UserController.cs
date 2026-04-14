using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver;
using MVC.Models.Account;
using MVC.ViewModels.UserViewModel;
using System.Security.Cryptography.X509Certificates;

namespace MVC.Controllers
{
    public class UserController : Controller
    {

        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private RoleManager<ApplicationRole> _roleManager;


        public UserController(IUserRepository userRepository, IRoleRepository roleRepository, RoleManager<ApplicationRole> rm)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleManager = rm;
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


            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Phonenumber = user.PhoneNumber,
                Password = "",
                roles = stringRoles,
                selectedRole = selectedRoleName
            };
            
          
            return View(registerViewModel);  
            





        }

    }
}
