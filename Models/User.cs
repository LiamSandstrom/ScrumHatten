using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Models
{
    public class User : MongoIdentityUser<Guid>
    {
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
    }
}

