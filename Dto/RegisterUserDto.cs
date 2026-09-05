// Dto --- Data Transfer Object
// used when we want to transfer data from one layer to another layer
// for example, from controller to view or from view to controller.





using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Dto
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}



// Now, gotta use this Dto in Register.cshtml and Login.cshtml form to transfer data from view to controller and vice versa.