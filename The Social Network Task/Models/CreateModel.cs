using System.ComponentModel.DataAnnotations;

namespace The_Social_Network_Task.Models
{
    public class CreateModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}