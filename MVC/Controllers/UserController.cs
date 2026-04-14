using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver;
using MVC.ViewModels.UserViewModel;
using System.Security.Cryptography.X509Certificates;

namespace MVC.Controllers
{
    public class UserController : Controller
    {

        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            List<User> allUsers = await _userRepository.GetAllUsersAsync();
            UserListViewModel vm = new UserListViewModel();
            List<ApplicationRole> roles = await _roleRepository.GetAllRolesAsync();

            vm.Users = allUsers;
            vm.UserRoles = await _roleRepository.GetAllRolesAsync();

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            User user = await _userRepository.GetUser(id);
            
          
            return View();  
            





        }

    }
}
