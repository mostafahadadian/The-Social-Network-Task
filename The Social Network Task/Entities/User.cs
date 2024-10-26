using Microsoft.AspNetCore.Identity;

namespace The_Social_Network_Task.Entities
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public ICollection<Friendship> Friendships { get; set; }//= new List<Friendship>();
    }
}
