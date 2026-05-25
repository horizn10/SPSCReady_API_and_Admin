-- =====================================================
-- SPSC READY - MOCK TEST DATA INSERTION SCRIPT
-- Sub Inspector Exam with General Knowledge Paper
-- =====================================================

-- TABLE RELATIONSHIPS DIAGRAM:
-- =============================
-- MockExams (Parent)
--     |
--     +--- MockTests (Child of Exam)
--              |
--              +--- MockSections (Child of MockTest)
--                       |
--                       +--- MockQuestions (Child of Section)
--
-- MockUserAttempts (Child of Exam via MockTest, tracks user test attempts)
--     |
--     +--- MockUserAnswers (Child of UserAttempt and Question, tracks individual answers)
--
-- KEY RELATIONSHIPS:
-- 1. Exam → MockTests: One exam has many mock tests (1:N)
-- 2. MockTest → Sections: One mock test has many sections (1:N)
-- 3. Section → Questions: One section has many questions (1:N)
-- 4. UserAttempt → MockTest: One user attempt belongs to one mock test (N:1)
-- 5. UserAttempt → UserAnswers: One attempt has many user answers (1:N)
-- 6. UserAnswer → Question: Each answer is for one question (N:1)
-- 7. UserAnswer → UserAttempt: Multiple answers belong to one attempt (N:1)

-- =====================================================
-- STEP 1: INSERT EXAM
-- =====================================================
INSERT INTO MockExams 
    (ExamName, ExamCode, ExamYear, Description, IsActive)
VALUES
    ('Sub Inspector Examination', 'SUB_INS_2026', 2026, 'Sub Inspector Competitive Examination for SPSC', 1);

-- Get the inserted ExamId (will be used as @ExamId)
-- SELECT @ExamId = ExamId FROM MockExams WHERE ExamCode = 'SUB_INS_2026';

-- =====================================================
-- STEP 2: INSERT MOCK TEST
-- =====================================================
-- Note: Replace @ExamId with the actual ExamId from Step 1 (e.g., 1)
DECLARE @ExamId INT = (SELECT TOP 1 ExamId FROM MockExams WHERE ExamCode = 'SUB_INS_2026');

INSERT INTO MockTests 
    (ExamId, Title, PaperType, PaperNumber, DurationMinutes, TotalMarks, TotalQuestions, PassingMarks, IsActive)
VALUES
    (@ExamId, 'Paper 1 - General Knowledge', 'Prelim', 1, 5, 100, 10, 60, 1);

-- =====================================================
-- STEP 3: INSERT SECTION
-- =====================================================
DECLARE @MockTestId INT = (SELECT TOP 1 MockTestId FROM MockTests WHERE Title = 'Paper 1 - General Knowledge');

INSERT INTO MockSections 
    (MockTestId, SectionName, SubjectTag, OrderIndex, QuestionCount, MarksPerQuestion, NegativeMarks)
VALUES
    (@MockTestId, 'General Knowledge - India & Sikkim', 'General Knowledge', 1, 10, 10, 0);

-- =====================================================
-- STEP 4: INSERT QUESTIONS
-- =====================================================
DECLARE @SectionId INT = (SELECT TOP 1 SectionId FROM MockSections WHERE SectionName = 'General Knowledge - India & Sikkim');

INSERT INTO MockQuestions 
    (SectionId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, Explanation, DifficultyLevel, OrderIndex, IsActive)
VALUES

-- Question 1
(
    @SectionId,
    'What is the capital of India?',
    'Mumbai',
    'New Delhi',
    'Kolkata',
    'Chennai',
    'B',
    'New Delhi is the capital and largest city of India.',
    'Easy',
    1,
    1
),

-- Question 2
(
    @SectionId,
    'Which state is the highest producer of tea in India?',
    'Tamil Nadu',
    'Assam',
    'Kerala',
    'Himachal Pradesh',
    'B',
    'Assam is the largest tea-producing state in India, producing over 50% of India''s total tea.',
    'Easy',
    2,
    1
),

-- Question 3
(
    @SectionId,
    'Sikkim shares its border with which of the following countries?',
    'Bangladesh and Bhutan',
    'Tibet and Nepal',
    'China, Bhutan, and Nepal',
    'Bhutan only',
    'C',
    'Sikkim shares borders with China (to the north and east), Bhutan (to the south), and Nepal (to the west).',
    'Medium',
    3,
    1
),

-- Question 4
(
    @SectionId,
    'What is the capital of Sikkim?',
    'Geyzing',
    'Gangtok',
    'Ravangla',
    'Lachung',
    'B',
    'Gangtok is the capital and largest city of Sikkim state.',
    'Easy',
    4,
    1
),

-- Question 5
(
    @SectionId,
    'Which river is the lifeline of India?',
    'Brahmaputra',
    'Ganges',
    'Yamuna',
    'Narmada',
    'B',
    'The River Ganges (Ganga) is considered the lifeline of India and has immense religious significance.',
    'Medium',
    5,
    1
),

-- Question 6
(
    @SectionId,
    'Sikkim became a full state of India in which year?',
    '1975',
    '1972',
    '1970',
    '1978',
    'A',
    'Sikkim was officially recognized as the 22nd state of India on May 16, 1975.',
    'Medium',
    6,
    1
),

-- Question 7
(
    @SectionId,
    'What is the literacy rate trend in Sikkim?',
    'Below 60%',
    'Between 60-70%',
    'Between 80-90%',
    'Above 90%',
    'C',
    'Sikkim has one of the highest literacy rates in India at approximately 87.4%.',
    'Hard',
    7,
    1
),

-- Question 8
(
    @SectionId,
    'Which is the smallest union territory of India by population?',
    'Dadra and Nagar Haveli',
    'Puducherry',
    'Lakshadweep',
    'Andaman and Nicobar Islands',
    'C',
    'Lakshadweep is the smallest union territory of India in terms of population.',
    'Hard',
    8,
    1
),

-- Question 9
(
    @SectionId,
    'What is the primary occupation in Sikkim?',
    'Mining',
    'Manufacturing',
    'Agriculture',
    'Tourism',
    'C',
    'Agriculture, particularly the cultivation of cardamom, ginger, and other spices, is the primary occupation in Sikkim.',
    'Medium',
    9,
    1
),

-- Question 10
(
    @SectionId,
    'Which mountain peak is located in Sikkim?',
    'K2',
    'Mount Kanchenjunga',
    'Mount Everest',
    'Mount Nanda Devi',
    'B',
    'Mount Kanchenjunga (8,586 m) is the third-highest peak in the world and is located on the border of Sikkim with Nepal.',
    'Medium',
    10,
    1
);

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Run these to verify the data was inserted correctly:

-- Check Exam Data
SELECT 'EXAM DATA' AS Section;
SELECT ExamId, ExamName, ExamCode, ExamYear, IsActive FROM MockExams 
WHERE ExamCode = 'SUB_INS_2026';

-- Check Mock Test Data
SELECT 'MOCK TEST DATA' AS Section;
SELECT MockTestId, ExamId, Title, PaperType, PaperNumber, DurationMinutes, TotalMarks, TotalQuestions, PassingMarks 
FROM MockTests 
WHERE Title = 'Paper 1 - General Knowledge';

-- Check Section Data
SELECT 'SECTION DATA' AS Section;
SELECT SectionId, MockTestId, SectionName, SubjectTag, QuestionCount, MarksPerQuestion, NegativeMarks 
FROM MockSections 
WHERE SectionName = 'General Knowledge - India & Sikkim';

-- Check Questions Data
SELECT 'QUESTIONS DATA' AS Section;
SELECT QuestionId, SectionId, QuestionText, CorrectOption, DifficultyLevel, OrderIndex 
FROM MockQuestions 
WHERE SectionId = (SELECT TOP 1 SectionId FROM MockSections WHERE SectionName = 'General Knowledge - India & Sikkim')
ORDER BY OrderIndex;

-- Check Total Count
SELECT 'TOTAL QUESTION COUNT' AS Section;
SELECT COUNT(*) AS TotalQuestions 
FROM MockQuestions 
WHERE SectionId = (SELECT TOP 1 SectionId FROM MockSections WHERE SectionName = 'General Knowledge - India & Sikkim');

-- =====================================================
-- DATABASE SCHEMA SUMMARY
-- =====================================================
-- Table: MockExams
-- PK: ExamId (INT, Identity)
-- Columns: ExamName (nvarchar 200), ExamCode (nvarchar 50 UNIQUE), ExamYear (int), Description (nvarchar max), IsActive (bit)
--
-- Table: MockTests
-- PK: MockTestId (INT, Identity)
-- FK: ExamId → MockExams.ExamId
-- Columns: Title (nvarchar 200), PaperType (nvarchar 50), PaperNumber (int), DurationMinutes (int), 
--          TotalMarks (decimal 8,2), TotalQuestions (int), PassingMarks (decimal 8,2), IsActive (bit)
--
-- Table: MockSections
-- PK: SectionId (INT, Identity)
-- FK: MockTestId → MockTests.MockTestId
-- Columns: SectionName (nvarchar 200), SubjectTag (nvarchar 100), OrderIndex (int), QuestionCount (int),
--          MarksPerQuestion (decimal 4,2), NegativeMarks (decimal 4,2)
--
-- Table: MockQuestions
-- PK: QuestionId (INT, Identity)
-- FK: SectionId → MockSections.SectionId
-- Columns: QuestionText (nvarchar max), OptionA-D (nvarchar max), CorrectOption (nvarchar 1),
--          Explanation (nvarchar max), DifficultyLevel (nvarchar 50), OrderIndex (int), IsActive (bit)
--
-- Table: MockUserAttempts
-- PK: AttemptId (INT, Identity)
-- FK: UserId → AspNetUsers.Id, MockTestId → MockTests.MockTestId
-- Columns: StartedAt (datetime2), SubmittedAt (datetime2), ExpiresAt (datetime2), TotalScore (decimal 8,2),
--          CorrectCount (int), WrongCount (int), SkippedCount (int), Percentage (decimal 5,2), Status (nvarchar 50)
--
-- Table: MockUserAnswers
-- PK: AnswerId (INT, Identity)
-- FK: AttemptId → MockUserAttempts.AttemptId, QuestionId → MockQuestions.QuestionId
-- Columns: SelectedOption (nvarchar 1), IsCorrect (bit), MarksAwarded (decimal 6,2), IsMarkedForReview (bit), AnsweredAt (datetime2)
