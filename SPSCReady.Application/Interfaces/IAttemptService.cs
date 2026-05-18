using SPSCReady.Application.DTOs.MockTest;

namespace SPSCReady.Application.Interfaces;

public interface IAttemptService
{
    Task<StartAttemptResponseDto> StartAttemptAsync(int userId, int mockTestId);
    Task<AttemptResultDto> SubmitAttemptAsync(SubmitAttemptRequestDto dto, int userId);
    Task<AttemptResultDto?> GetAttemptResultAsync(int attemptId, int userId);
    Task<List<AttemptSummaryDto>> GetUserAttemptsAsync(int userId);
}
