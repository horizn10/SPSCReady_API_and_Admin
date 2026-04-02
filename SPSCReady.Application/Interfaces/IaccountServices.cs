using SPSCReady.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSCReady.Application.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request);

        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}