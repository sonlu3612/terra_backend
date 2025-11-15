using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ResetPasswordDto
    {
        [Required] public string UserId { get; set; } = string.Empty;
        [Required] public string Otp { get; set; } = string.Empty;
        [Required] public string NewPassword { get; set; } = string.Empty;
    }
}