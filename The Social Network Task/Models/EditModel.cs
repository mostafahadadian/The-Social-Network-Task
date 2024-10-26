using System.ComponentModel.DataAnnotations;

namespace The_Social_Network_Task.Models
{
    public class EditModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
