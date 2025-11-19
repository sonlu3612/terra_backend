// WebApi/Controllers/AuthController.cs
using Application.DTOs;
using Core.Interfaces;
using Core.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly IEmailSender _emailSender;
        private readonly ISmsService _smsService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IOtpService otpService,
            IEmailSender emailSender,
            ISmsService smsService,
            IMemoryCache cache,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _otpService = otpService;
            _emailSender = emailSender;
            _smsService = smsService;
            _cache = cache;
            _config = config;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            dto.Validate();

            ApplicationUser? user = null;
            if (!string.IsNullOrEmpty(dto.Email))
                user = await _userManager.FindByEmailAsync(dto.Email);
            else if (!string.IsNullOrEmpty(dto.PhoneNumber))
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _tokenService.CreateToken(user);
            return Ok(new { user = MapToUserResponse(user), token });
        }

        // POST /api/auth/signup → bước 1: gửi OTP
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] RegisterDto dto)
        {
            dto.Validate();

            var identifier = dto.Email ?? dto.PhoneNumber!;
            var exists = await _userManager.Users.AnyAsync(u =>
                u.Email == dto.Email || u.PhoneNumber == dto.PhoneNumber ||
                (!string.IsNullOrEmpty(dto.Username) && u.UserName == dto.Username));

            if (exists)
                return BadRequest(new { message = "Email, phone or username already taken" });

            var otp = _otpService.GenerateOtp();
            _otpService.StoreOtp(identifier, otp);
            _cache.Set($"TempSignup_{identifier}", dto, TimeSpan.FromMinutes(10));

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                await _smsService.SendSmsAsync(dto.PhoneNumber, $"Terra OTP: {otp}");
            else
                await _emailSender.SendEmailAsync(dto.Email!, "Terra – Your OTP", $"Your OTP is {otp}");

            return Ok(new { message = "OTP sent" });
        }

        // POST /api/auth/verify-signup → bước 2: hoàn tất đăng ký
        [HttpPost("verify-signup")]
        public async Task<IActionResult> VerifySignup([FromBody] VerifyOtpDto dto)
        {
            dto.Validate();

            if (!await _otpService.ValidateOtpAsync(dto.Identifier, dto.Otp))
                return BadRequest(new { message = "Invalid or expired OTP" });

            var tempDto = _cache.Get<RegisterDto>($"TempSignup_{dto.Identifier}");
            if (tempDto == null)
                return BadRequest(new { message = "Signup session expired" });

            var user = new ApplicationUser
            {
                UserName = tempDto.Username ?? tempDto.Email ?? tempDto.PhoneNumber,
                Email = tempDto.Email,
                PhoneNumber = tempDto.PhoneNumber,
                DisplayName = tempDto.FullName ?? tempDto.Username ?? "User",
                EmailConfirmed = !string.IsNullOrEmpty(tempDto.Email),
                PhoneNumberConfirmed = !string.IsNullOrEmpty(tempDto.PhoneNumber)
            };

            var result = await _userManager.CreateAsync(user, tempDto.PasswordHash);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors.First().Description });

            _cache.Remove($"TempSignup_{dto.Identifier}");

            var token = _tokenService.CreateToken(user);
            return Ok(new { user = MapToUserResponse(user), token });
        }

        // POST /api/auth/google
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleSignInDto dto)
        {
            if (string.IsNullOrEmpty(dto.Credential))
                return BadRequest(new { message = "Google credential required" });

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Credential, new()
                {
                    Audience = new[] { _config["Google:ClientId"] }
                });

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        DisplayName = payload.Name ?? payload.Email.Split('@')[0],
                        ImageUrl = payload.Picture,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(user);
                }
                else if (!string.IsNullOrEmpty(payload.Picture) && user.ImageUrl != payload.Picture)
                {
                    user.ImageUrl = payload.Picture;
                    await _userManager.UpdateAsync(user);
                }

                var token = _tokenService.CreateToken(user);
                return Ok(new { user = MapToUserResponse(user), token });
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { message = "Invalid Google token" });
            }
        }

        // POST /api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout() => Ok();

        // GET /api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? NotFound() : Ok(MapToUserResponse(user));
        }

        // POST /api/auth/refresh (placeholder)
        [HttpPost("refresh")]
        public IActionResult Refresh() => Ok(new { token = "new-jwt-placeholder" });

        // Helper: Map ApplicationUser to frontend User type
        // Frontend expects: displayName (not name), imageUrl (not photoURL)
        private object MapToUserResponse(ApplicationUser u)
        {
            return new
            {
                id = u.Id,
                username = u.UserName ?? string.Empty,
                displayName = string.IsNullOrEmpty(u.DisplayName) ? u.UserName ?? string.Empty : u.DisplayName,
                email = u.Email,
                bio = u.Bio,
                theme = u.Theme ?? "light",
                accent = u.Accent ?? "blue",
                website = u.Website,
                location = u.Location,
                imageUrl = u.ImageUrl ?? "/assets/twitter-avatar.jpg",
                coverPhotoURL = u.CoverPhotoURL,
                verified = u.Verified,
                totalTweets = u.TotalTweets,
                totalPhotos = u.TotalPhotos,
                pinnedTweetId = u.PinnedTweetId,
                following = new string[0], // Populated by separate endpoints
                followers = new string[0], // Populated by separate endpoints
                createdAt = u.CreatedAt.ToString("o"),    
                updatedAt = u.UpdatedAt.ToString("o")
            };
        }
    }
}