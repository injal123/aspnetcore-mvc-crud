using System.ComponentModel.DataAnnotations;




namespace MyMvcApp.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }



        // Relationship with User
        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}