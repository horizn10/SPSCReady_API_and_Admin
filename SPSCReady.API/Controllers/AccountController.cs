using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
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
    }
}