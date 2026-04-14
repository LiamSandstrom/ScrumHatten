using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Models
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class User : MongoIdentityUser<Guid>
    {
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public UserRole Role { get; set; }
        public decimal Pay { get; set; }
    }

}

