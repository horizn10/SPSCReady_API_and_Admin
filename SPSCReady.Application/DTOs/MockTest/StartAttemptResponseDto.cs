namespace SPSCReady.Application.DTOs.MockTest;

public record StartAttemptResponseDto(
    int AttemptId,
    DateTime ExpiresAt,
    int DurationMinutes
);
