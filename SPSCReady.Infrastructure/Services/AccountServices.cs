using Microsoft.AspNetCore.Identity;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPSCReady.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Inject Identity's UserManager
        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email, // Identity requires a UserName. Using Email is standard practice.
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Gender = request.Gender
            };

            // CreateAsync automatically hashes the password and saves the user
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return (true, Array.Empty<string>());
            }

            // If it fails (e.g., duplicate email, weak password), extract the errors
            var errors = result.Errors.Select(e => e.Description);
            return (false, errors);
        }
    }
}