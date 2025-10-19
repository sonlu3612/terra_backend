using System.ComponentModel.DataAnnotations;

namespace Auth.DTOs
{
    public class RegisterDto
    {
        public string? Email { get; set; }

        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }

        public void Validate()
        {
            if(string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(PhoneNumber))
            {
                throw new ArgumentException("Either Email or PhoneNumber must be provided.");
            }
        }
    }
}
