using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class VerifyOtpDto
    {
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Otp { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Identifier))
            {
                throw new ArgumentException("Identifier must be provided.");
            }
            if (string.IsNullOrEmpty(Otp))
            {
                throw new ArgumentException("Otp must be provided.");
            }
            else
            {
                Console.WriteLine(Otp);
            }
        }



    }
}
