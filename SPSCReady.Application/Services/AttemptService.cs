using SPSCReady.Application.DTOs.MockTest;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Domain.Enums;

namespace SPSCReady.Application.Services;

public class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _attemptRepository;
    private readonly IMockTestRepository _mockTestRepository;
    private readonly IQuestionRepository _questionRepository;

    public AttemptService(
        IAttemptRepository attemptRepository,
        IMockTestRepository mockTestRepository,
        IQuestionRepository questionRepository)
    {
        _attemptRepository = attemptRepository;
        _mockTestRepository = mockTestRepository;
        _questionRepository = questionRepository;
    }

    public async Task<StartAttemptResponseDto> StartAttemptAsync(string userId, int mockTestId)
    {
        // Get the mock test
        var mockTest = await _mockTestRepository.GetByIdAsync(mockTestId)
            ?? throw new InvalidOperationException("MockTest not found");

        // Check if user already has an attempt for this test
        var existingAttempt = await _attemptRepository.GetExistingAttemptAsync(userId, mockTestId);
        if (existingAttempt != null && existingAttempt.Status == AttemptStatus.InProgress)
            throw new InvalidOperationException("User already has an in-progress attempt for this test");

        // Create new attempt
        var attempt = new UserAttempt
        {
            UserId = userId,
            MockTestId = mockTestId,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(mockTest.DurationMinutes),
            Status = AttemptStatus.InProgress
        };

        var created = await _attemptRepository.CreateAsync(attempt);

        return new StartAttemptResponseDto(
            created.AttemptId,
            created.ExpiresAt,
            mockTest.DurationMinutes
        );
    }

    public async Task<AttemptResultDto> SubmitAttemptAsync(SubmitAttemptRequestDto dto, string userId)
    {
        // Get the attempt with all data
        var attempt = await _attemptRepository.GetWithAnswersAsync(dto.AttemptId)
            ?? throw new InvalidOperationException("Attempt not found");

        if (attempt.UserId != userId)
            throw new UnauthorizedAccessException("User can only submit their own attempts");

        // Check if already submitted
        if (attempt.Status == AttemptStatus.Submitted)
            throw new InvalidOperationException("Attempt already submitted");

        // Create UserAnswer records and calculate scores
        var userAnswers = new List<UserAnswer>();
        decimal totalScore = 0;
        int correctCount = 0;
        int wrongCount = 0;
        int skippedCount = 0;

        foreach (var answerDto in dto.Answers)
        {
            // Get the question with section info
            var question = await _questionRepository.GetByIdAsync(answerDto.QuestionId)
                ?? throw new InvalidOperationException($"Question {answerDto.QuestionId} not found");

            var section = question.Section;

            // Calculate marks for this answer
            decimal marksAwarded = 0;
            bool isCorrect = false;

            if (answerDto.SelectedOption == null)
            {
                // Skipped
                marksAwarded = 0;
                skippedCount++;
            }
            else if (answerDto.SelectedOption == question.CorrectOption)
            {
                // Correct answer
                marksAwarded = section.MarksPerQuestion;
                isCorrect = true;
                correctCount++;
            }
            else
            {
                // Wrong answer - apply negative marking
                marksAwarded = -section.NegativeMarks;
                wrongCount++;
            }

            totalScore += marksAwarded;

            // Create UserAnswer record
            var userAnswer = new UserAnswer
            {
                AttemptId = attempt.AttemptId,
                QuestionId = answerDto.QuestionId,
                SelectedOption = answerDto.SelectedOption,
                IsCorrect = isCorrect,
                MarksAwarded = marksAwarded,
                IsMarkedForReview = answerDto.IsMarkedForReview,
                AnsweredAt = DateTime.UtcNow
            };

            userAnswers.Add(userAnswer);
        }

        // Ensure total score is not negative
        if (totalScore < 0) totalScore = 0;

        // Save all answers
        await _attemptRepository.AddAnswersAsync(userAnswers);

        // Update attempt with results
        attempt.SubmittedAt = DateTime.UtcNow;
        attempt.Status = AttemptStatus.Submitted;
        attempt.TotalScore = totalScore;
        attempt.CorrectCount = correctCount;
        attempt.WrongCount = wrongCount;
        attempt.SkippedCount = skippedCount;
        attempt.Percentage = (totalScore / attempt.MockTest.TotalMarks) * 100;

        await _attemptRepository.UpdateAsync(attempt);

        // Get updated attempt with answers for result building
        var resultAttempt = await _attemptRepository.GetWithAnswersAsync(attempt.AttemptId)
            ?? throw new InvalidOperationException("Failed to retrieve updated attempt");

        return BuildAttemptResultDto(resultAttempt);
    }

    public async Task<AttemptResultDto?> GetAttemptResultAsync(int attemptId, string userId)
    {
        var attempt = await _attemptRepository.GetWithAnswersAsync(attemptId);
        if (attempt == null || attempt.UserId != userId || attempt.Status != AttemptStatus.Submitted)
            return null;

        return BuildAttemptResultDto(attempt);
    }

    public async Task<List<AttemptSummaryDto>> GetUserAttemptsAsync(string userId)
    {
        var attempts = await _attemptRepository.GetByUserIdAsync(userId);
        return attempts.Select(a => new AttemptSummaryDto(
            a.AttemptId,
            a.MockTestId,
            a.MockTest.Title,
            a.StartedAt,
            a.SubmittedAt,
            a.TotalScore,
            (int)a.MockTest.TotalMarks,
            a.Percentage,
            a.Status.ToString()
        )).ToList();
    }

    private AttemptResultDto BuildAttemptResultDto(UserAttempt attempt)
    {
        var mockTest = attempt.MockTest;
        var passingMarks = mockTest.PassingMarks ?? 0;
        var isPassed = attempt.Percentage.HasValue && attempt.Percentage >= passingMarks;

        // Build section breakdown
        var sectionBreakdown = new List<SectionResultDto>();
        foreach (var section in mockTest.Sections.OrderBy(s => s.OrderIndex))
        {
            var sectionAnswers = attempt.UserAnswers
                .Where(ua => ua.Question.SectionId == section.SectionId)
                .ToList();

            if (sectionAnswers.Any())
            {
                int sectionCorrect = sectionAnswers.Count(ua => ua.IsCorrect == true);
                int sectionWrong = sectionAnswers.Count(ua => ua.IsCorrect == false);
                int sectionSkipped = sectionAnswers.Count(ua => ua.SelectedOption == null);

                decimal sectionScore = sectionAnswers.Sum(ua => ua.MarksAwarded ?? 0);

                sectionBreakdown.Add(new SectionResultDto(
                    section.SectionId,
                    section.SectionName,
                    sectionCorrect,
                    sectionWrong,
                    sectionSkipped,
                    sectionScore
                ));
            }
        }

        // Build answer review
        var answerReview = attempt.UserAnswers
            .OrderBy(ua => ua.Question.Section.OrderIndex)
            .ThenBy(ua => ua.Question.OrderIndex)
            .Select(ua => new AnswerReviewDto(
                ua.QuestionId,
                ua.Question.QuestionText,
                ua.SelectedOption,
                ua.Question.CorrectOption,
                ua.IsCorrect ?? false,
                ua.Question.Explanation
            )).ToList();

        return new AttemptResultDto(
            attempt.AttemptId,
            mockTest.Title,
            attempt.TotalScore ?? 0,
            (int)mockTest.TotalMarks,
            attempt.Percentage ?? 0,
            attempt.CorrectCount ?? 0,
            attempt.WrongCount ?? 0,
            attempt.SkippedCount ?? 0,
            isPassed,
            sectionBreakdown,
            answerReview
        );
    }
}
