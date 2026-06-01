using SPSCReady.Application.DTOs.MockTest;

namespace SPSCReady.Application.Interfaces;

public interface IAttemptService
{
    Task<StartAttemptResponseDto> StartAttemptAsync(string userId, int mockTestId);
    Task<AttemptResultDto> SubmitAttemptAsync(SubmitAttemptRequestDto dto, string userId);
    Task<AttemptResultDto?> GetAttemptResultAsync(int attemptId, string userId);
    Task<List<AttemptSummaryDto>> GetUserAttemptsAsync(string userId);
}
