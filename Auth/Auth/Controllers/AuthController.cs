using Auth.DTOs;
using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender; //Dich vu gui email
        private readonly ISmsService _smsService; //Dich vu gui SMS
        private readonly IOtpService _otpService;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IOtpService otpService, ISmsService smsService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsService = smsService;
            _otpService = otpService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            //Validate DTO
            try
            {
                dto.Validate();
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            string identifier = dto.Email ?? dto.PhoneNumber;
            var userByIdentifier = await _userManager.FindByNameAsync(identifier);
            if(userByIdentifier != null)
            {
                return BadRequest("User with this email or phone number already exists.");
            }

            var user = new ApplicationUser {
                UserName = identifier,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            //Tao user trong database
            var result = await _userManager.CreateAsync(user, dto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                dto.Validate();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            string identifier = dto.Email ?? dto.PhoneNumber;
            var user = await _userManager.FindByNameAsync(identifier);
            if(user == null)
            {
                return Unauthorized("Invalid email or phone");
            }

            //check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {

            try
            {
                dto.Validate();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            //Tim user theo email hoac so dien thoai
            ApplicationUser user = null;
            if(!string.IsNullOrEmpty(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
            }
            else if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber || u.UserName == dto.PhoneNumber);
            }

            if(user == null)
            {
                return BadRequest("User not found.");
            }

            //token cho reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //gui mail hoac sms
            if(!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                //Gui SMS o day (bo sung sau)
                var otp = _otpService.GenerateOtp();
                _otpService.StoreOtp(user.Id, otp);
                await _smsService.SendSmsAsync(dto.PhoneNumber, $"Your OTP for password reset is: {otp}");
                return Ok("OTP has been sent to your phone.");

            }
            else
            {
                var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = dto.Email, token = encodedToken }, Request.Scheme);
                await _emailSender.SendEmailAsync(dto.Email, "Password Reset", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return Ok("Password reset link has been sent to your email.");
            }
            //var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = dto.Email, token = encodedToken }, Request.Scheme);
            //var resetLink = Url.Action("ResetPassword", "Auth", new { token = token, email = dto.Email }, Request.Scheme);
            // Gửi email với liên kết đặt lại mật khẩu
            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest("Error resetting password.");
            }
            return Ok("Password has been reset successfully.");
        }
    }

}
