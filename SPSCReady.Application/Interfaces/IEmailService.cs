using System.Threading.Tasks;

namespace SPSCReady.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string email, string otpCode);
    }
}
