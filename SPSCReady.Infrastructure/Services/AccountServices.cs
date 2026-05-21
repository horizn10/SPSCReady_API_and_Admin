using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SPSCReady.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailService _emailService;

        public AccountService(
            UserManager<ApplicationUser> userManager, 
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _emailService = emailService;
        }

        // --- THE SIGNATURE HERE MUST MATCH THE INTERFACE EXACTLY ---
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Gender = request.Gender
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return (true, Array.Empty<string>());
            }

            var errors = result.Errors.Select(e => e.Description);
            return (false, errors);
        }

        // --- LOGIN METHOD ---
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new LoginResponseDto
                {
                    IsSuccessful = false,
                    Message = "Invalid email or password."
                };
            }

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                IsSuccessful = true,
                Message = "Login successful.",
                Token = token
            };
        }

        // --- OTP LOGIN METHODS ---
        public async Task<(bool Success, string Message)> SendOtpAsync(OtpRequestDto request)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return (false, "Email is required.");
            }

            // Check if user exists
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // For security, don't reveal if user exists or not
                return (false, "If an account with this email exists, an OTP will be sent.");
            }

            // Generate OTP (6-digit code)
            var otpCode = GenerateOtp();

            // Delete any existing unexpired OTPs for this email
            var existingOtps = await _dbContext.OtpTokens
                .Where(o => o.Email == request.Email && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            if (existingOtps.Any())
            {
                _dbContext.OtpTokens.RemoveRange(existingOtps);
            }

            // Create new OTP token (valid for 10 minutes)
            var otpToken = new OtpToken
            {
                Email = request.Email,
                Code = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.OtpTokens.Add(otpToken);
            await _dbContext.SaveChangesAsync();

            // Send OTP via email
            var emailSent = await _emailService.SendOtpEmailAsync(request.Email, otpCode);

            if (!emailSent)
            {
                // Delete the OTP if email couldn't be sent
                _dbContext.OtpTokens.Remove(otpToken);
                await _dbContext.SaveChangesAsync();
                return (false, "Failed to send OTP. Please try again.");
            }

            return (true, "OTP sent successfully to your email.");
        }

        public async Task<LoginResponseDto> VerifyOtpAndLoginAsync(OtpVerifyDto request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
            {
                return new LoginResponseDto
                {
                    IsSuccessful = false,
                    Message = "Email and OTP code are required."
                };
            }

            // Find the OTP token
            var otpToken = await _dbContext.OtpTokens
                .FirstOrDefaultAsync(o => 
                    o.Email == request.Email && 
                    o.Code == request.Code && 
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow);

            if (otpToken == null)
            {
                return new LoginResponseDto
                {
                    IsSuccessful = false,
                    Message = "Invalid or expired OTP."
                };
            }

            // Mark OTP as used
            otpToken.IsUsed = true;
            _dbContext.OtpTokens.Update(otpToken);
            await _dbContext.SaveChangesAsync();

            // Find the user
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new LoginResponseDto
                {
                    IsSuccessful = false,
                    Message = "User not found."
                };
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                IsSuccessful = true,
                Message = "Login successful.",
                Token = token
            };
        }

        // --- HELPER METHODS ---
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}