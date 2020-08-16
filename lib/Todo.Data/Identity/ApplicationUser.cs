using AspNetCore.Identity.Mongo.Model;

namespace Todo.Data.Identity
{
    public class ApplicationUser : MongoUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
