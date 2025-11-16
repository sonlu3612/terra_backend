using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender; //Dich vu gui email
        private readonly ISmsService _smsService; // Dich vu gui SMS
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IOtpService otpService, ISmsService smsService, IMemoryCache cache)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsService = smsService;
            _otpService = otpService;
            _cache = cache;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            //Validate DTO
            try
            {
                dto.Validate();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            var identifier = dto.Email ?? dto.PhoneNumber;
            var userByIdentifier = await _userManager.FindByNameAsync(identifier);
            if (userByIdentifier != null)
            {
                return BadRequest("User with this email or phone number already exists.");
            }

            if (await _userManager.FindByNameAsync(dto.Username) != null)
            {
                return BadRequest("Username is already taken.");
            }

            var otp = _otpService.GenerateOtp();
            var tempDataKeyOtp = identifier;
            _otpService.StoreOtp(tempDataKeyOtp, otp);

            var tempDataKey = $"TempRegister_{identifier}";
            _cache.Set(tempDataKey, dto, TimeSpan.FromMinutes(5)); //Luu tam du lieu dang ky trong 5 phut

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                await _smsService.SendSmsAsync(dto.PhoneNumber, $"Your OTP for registration is: {otp}");
                return Ok("OTP has been sent to your phone.");
            }
            else
            {
                await _emailSender.SendEmailAsync(dto.Email, "OTP for Registration", $"Your OTP for registration is: {otp}");
                return Ok("OTP has been sent to your email.");
            }



        }

        [HttpPost("verify-register")]
        public async Task<IActionResult> VerifyRegister(VerifyOtpDto dto)
        {
            try
            {
                dto.Validate();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            string identifier = dto.Identifier;
            var tempDataKeyOtp = identifier;
            if (!await _otpService.ValidateOtpAsync(tempDataKeyOtp, dto.Otp))
            {
                return BadRequest("Invalid or expired OTP.");
            }
            var tempDataKey = $"TempRegister_{identifier}";
            var tempDto = _cache.Get<RegisterDto>(tempDataKey);
            if (tempDto == null)
            {
                return BadRequest("Registration expired.");
            }

            //var email = string.IsNullOrWhiteSpace(tempDto.Email) ? null : tempDto.Email.Trim().ToLower();
            //var phone = string.IsNullOrWhiteSpace(tempDto.PhoneNumber) ? null : tempDto.PhoneNumber.Trim();

            var user = new ApplicationUser
            {
                UserName = tempDto.Username,
                Email = tempDto.Email,
                PhoneNumber = tempDto.PhoneNumber,
                FullName = tempDto.FullName,
                DateOfBirth = tempDto.DateOfBirth,
                ImageUrl = tempDto.ImageUrl

            };

            var result = await _userManager.CreateAsync(user, tempDto.PasswordHash);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            _cache.Remove(tempDataKey); //Xoa tam du lieu sau khi dang ky thanh cong
            _cache.Remove(tempDataKeyOtp);

            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                message = "Registration successful.",
                token
            });



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
            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
            }
            else if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            }

            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or phone number" });
            }

            //check PasswordHash
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.PasswordHash, false);
            if (!result.Succeeded) return Unauthorized("Invalid email or Password");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("forgot-Password")]
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
            if (!string.IsNullOrEmpty(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
            }
            else if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            }

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            //token cho reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //gui mail hoac sms
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                //Gui SMS o day (bo sung sau)
                var otp = _otpService.GenerateOtp();
                _otpService.StoreOtp(user.Id, otp);
                await _smsService.SendSmsAsync(dto.PhoneNumber, $"Your OTP for Password reset is: {otp}");
                return Ok("OTP has been sent to your phone.");

            }
            else
            {
                var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = dto.Email, token = encodedToken }, Request.Scheme);
                await _emailSender.SendEmailAsync(dto.Email, "Password Reset", $"Please reset your Password by clicking here: <a href='{callbackUrl}'>link</a>");
                return Ok("Password reset link has been sent to your email.");
            }
            //var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = dto.Email, token = encodedToken }, Request.Scheme);
            //var resetLink = Url.Action("ResetPassword", "Auth", new { token = token, email = dto.Email }, Request.Scheme);
            // Gửi email với liên kết đặt lại mật khẩu
            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-Password")]
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
                return BadRequest("Error resetting Password.");
            }
            return Ok("Password has been reset successfully.");
        }
    }

}
