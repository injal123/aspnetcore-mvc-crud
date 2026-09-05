using System.ComponentModel.DataAnnotations;



namespace MyMvcApp.Models
{
    public class User
    {
        [Key]         // Primary key
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;
    }
}

