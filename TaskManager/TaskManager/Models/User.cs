using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class User
    {
        [Key]
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
