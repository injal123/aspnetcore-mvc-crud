using System.ComponentModel.DataAnnotations;



namespace MyMvcApp.Models
{
    public class User
    {
        [Key]         // Primary key
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}

