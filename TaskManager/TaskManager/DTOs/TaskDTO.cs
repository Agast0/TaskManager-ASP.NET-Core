using System.ComponentModel.DataAnnotations;

namespace TaskManager.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
    }
}
