using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
    }
}
