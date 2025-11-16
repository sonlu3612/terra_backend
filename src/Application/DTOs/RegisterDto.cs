using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }
        public string? Email { get; set; }

        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        public string PasswordHash { get; set; }


        public DateTime? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(PhoneNumber))
            {
                throw new ArgumentException("Either Email or PhoneNumber must be provided.");
            }
        }
    }
}
