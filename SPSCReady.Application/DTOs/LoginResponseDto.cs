// Path: SPSCReady.Application/DTOs/LoginResponseDto.cs
namespace SPSCReady.Application.DTOs
{
    public class LoginResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}