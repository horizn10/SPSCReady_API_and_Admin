-- ========================================
-- DIAGNOSTIC: Check if Questions are properly linked to Sections and MockTests
-- ========================================
USE SPSCReadyDb;

PRINT '===== CHECK MOCK TESTS =====' ;
SELECT MockTestId, Title, TotalQuestions, IsActive FROM MockTests;

PRINT '';
PRINT '===== CHECK SECTIONS =====' ;
SELECT SectionId, MockTestId, SectionName, QuestionCount, OrderIndex FROM MockSections;

PRINT '';
PRINT '===== CHECK QUESTIONS =====' ;
SELECT QuestionId, SectionId, QuestionText, CorrectOption, IsActive, OrderIndex 
FROM MockQuestions 
ORDER BY SectionId, OrderIndex;

PRINT '';
PRINT '===== COUNT QUESTIONS PER SECTION =====' ;
SELECT 
    s.SectionId,
    s.SectionName,
    COUNT(q.QuestionId) AS QuestionCount,
    s.QuestionCount AS ExpectedCount
FROM MockSections s
LEFT JOIN MockQuestions q ON s.SectionId = q.SectionId
GROUP BY s.SectionId, s.SectionName, s.QuestionCount;

PRINT '';
PRINT '===== CHECK INACTIVE QUESTIONS =====' ;
SELECT COUNT(*) AS InactiveQuestions FROM MockQuestions WHERE IsActive = 0;

PRINT '';
PRINT '===== FULL MOCK TEST STRUCTURE =====' ;
SELECT 
    m.MockTestId,
    m.Title,
    m.DurationMinutes,
    s.SectionId,
    s.SectionName,
    COUNT(q.QuestionId) AS ActualQuestionCount
FROM MockTests m
LEFT JOIN MockSections s ON m.MockTestId = s.MockTestId
LEFT JOIN MockQuestions q ON s.SectionId = q.SectionId AND q.IsActive = 1
GROUP BY m.MockTestId, m.Title, m.DurationMinutes, s.SectionId, s.SectionName;
