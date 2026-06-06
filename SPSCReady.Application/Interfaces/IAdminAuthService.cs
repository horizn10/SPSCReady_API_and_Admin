using SPSCReady.Domain.Entities;

namespace SPSCReady.Application.Interfaces;

public interface IAdminAuthService
{
    Task<AdminUser?> ValidateAsync(string username, string password);
}