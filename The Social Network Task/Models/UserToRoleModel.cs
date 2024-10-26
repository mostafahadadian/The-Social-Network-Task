using System.ComponentModel.DataAnnotations;

namespace The_Social_Network_Task.Models
{
    public class UserToRoleModel
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string role { get; set; }
    }
}
