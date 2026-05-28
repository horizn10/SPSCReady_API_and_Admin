using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPSCReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors) = await _accountService.RegisterAsync(request);

            if (succeeded)
            {
                return Ok(new { message = "Registration Successful!" });
            }

            // Return the specific Identity errors (e.g., to display in your Flutter UI)
            return BadRequest(new { message = "Registration failed", errors });
        }
        [HttpPost("login")] // Routes to /api/account/login
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.LoginAsync(request);

            if (response.IsSuccessful)
            {
                return Ok(response); // Returns the token to Flutter
            }

            return Unauthorized(new { message = response.Message });
        }

        /// <summary>
        /// Step 1: Send OTP to email for login
        /// </summary>
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.SendOtpAsync(request);

            if (success)
            {
                return Ok(new { success = true, message });
            }

            return BadRequest(new { success = false, message });
        }

        /// <summary>
        /// Step 2: Verify OTP and login user
        /// </summary>
        [HttpPost("verify-otp-login")]
        public async Task<IActionResult> VerifyOtpLogin([FromBody] OtpVerifyDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.VerifyOtpAndLoginAsync(request);

            if (response.IsSuccessful)
            {
                return Ok(response); // Returns the token
            }

            return Unauthorized(new { message = response.Message });
        }

        /// <summary>
        /// Get current user's profile information
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User ID not found in token." });
            }

            var profile = await _accountService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return Ok(profile);
        }
    }
}