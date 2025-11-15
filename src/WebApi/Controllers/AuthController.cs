using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsService _smsService;
        private readonly IOtpService _otpService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IOtpService otpService,
            ISmsService smsService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsService = smsService;
            _otpService = otpService;
        }

        // === REGISTER: Tạo user + gửi OTP ===
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try { dto.Validate(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }

            string identifier = dto.Email ?? dto.PhoneNumber!;
            var existingUser = await _userManager.FindByNameAsync(identifier);
            if (existingUser != null)
                return BadRequest("User with this email or phone already exists.");

            var user = new ApplicationUser
            {
                UserName = identifier,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Gửi OTP
            var otp = _otpService.GenerateOtp();
            _otpService.StoreOtp(user.Id, otp);

            if (!string.IsNullOrEmpty(dto.Email))
            {
                await _emailSender.SendEmailAsync(dto.Email, "Xác minh tài khoản", $"Mã OTP của bạn: {otp}");
            }
            else
            {
                await _smsService.SendSmsAsync(dto.PhoneNumber!, $"Mã OTP: {otp}");
            }

            return Ok(new { Message = "OTP sent. Verify to activate.", UserId = user.Id });
        }

        // === XÁC MINH OTP SAU REGISTER → TRẢ JWT ===
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return NotFound("User not found");

            var isValid = await _otpService.ValidateOtpAsync(dto.UserId, dto.Otp);
            if (!isValid) return BadRequest("Invalid or expired OTP");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        // === ĐĂNG NHẬP ===
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try { dto.Validate(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }

            string identifier = dto.Email ?? dto.PhoneNumber!;
            var user = await _userManager.FindByNameAsync(identifier);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        // === QUÊN MẬT KHẨU → GỬI OTP ===
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try { dto.Validate(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }

            ApplicationUser user = null;

            if (!string.IsNullOrEmpty(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
            }
            else if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber || u.UserName == dto.PhoneNumber);
            }

            if (user == null)
                return NotFound("User not found");

            var otp = _otpService.GenerateOtp();
            _otpService.StoreOtp(user.Id, otp);

            if (!string.IsNullOrEmpty(dto.Email))
            {
                await _emailSender.SendEmailAsync(dto.Email, "Đặt lại mật khẩu", $"Mã OTP: {otp}");
            }
            else
            {
                await _smsService.SendSmsAsync(dto.PhoneNumber!, $"Mã OTP đặt lại mật khẩu: {otp}");
            }

            return Ok(new { Message = "OTP sent", UserId = user.Id });
        }

        // === ĐẶT LẠI MẬT KHẨU BẰNG OTP ===
        [HttpPost("reset-password/otp")]
        public async Task<IActionResult> ResetPasswordByOtp([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return NotFound("User not found");

            var isValid = await _otpService.ValidateOtpAsync(dto.UserId, dto.Otp);
            if (!isValid) return BadRequest("Invalid or expired OTP");

            // Tạo token reset từ Identity (bắt buộc)
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok("Password reset successfully");
        }
    }
}