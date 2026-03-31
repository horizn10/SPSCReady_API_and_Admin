using SPSCReady.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSCReady.Application.Interfaces
{
    public interface IAccountService
    {
        // Returning a tuple so we can pass back specific Identity errors
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request);
    }
}