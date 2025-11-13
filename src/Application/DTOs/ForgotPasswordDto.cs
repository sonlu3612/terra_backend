namespace Application.DTOs
{
    public class ForgotPasswordDto
    {
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(PhoneNumber))
            {
                throw new ArgumentException("Either Email or PhoneNumber must be provided.");
            }
        }
    }
}