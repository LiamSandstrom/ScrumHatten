using Models;

namespace MVC.ViewModels.UserViewModel
{
    public class UserListViewModel
    {
        public List<User> Users { get; set; } = new();

        public List<ApplicationRole> UserRoles { get; set; } = new();

        public List<String> allRoles { get; set; } = new();

        public string? selectedRole { get; set; }

    }
}
