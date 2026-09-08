using System.ComponentModel.DataAnnotations;



namespace MyMvcApp.Dto
{
    public class CreateNoteDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}