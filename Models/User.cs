using AspNetCore.Identity.MongoDbCore.Models;

namespace Models
{
    public class User : MongoIdentityUser<Guid>
    {
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
    }
}

