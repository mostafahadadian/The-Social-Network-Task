using Microsoft.AspNetCore.Identity;
using The_Social_Network_Task.Entities;

namespace The_Social_Network_Task.Models
{
    public class AssignRoleViewModel
    {
        public List<User> Users { get; set; }
        public List<IdentityRole<int>> Roles { get; set; }
    }
}
