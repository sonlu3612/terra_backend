using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class VerifyOtpDto
    {
        [Required] public string UserId { get; set; }
        [Required] public string Otp { get; set; }
    }
}
