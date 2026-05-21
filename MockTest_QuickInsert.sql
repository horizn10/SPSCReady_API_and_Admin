-- ========================================
-- QUICK COPY-PASTE SQL SCRIPT
-- Sub Inspector Exam - General Knowledge Mock Test
-- ========================================
-- This script has no variables - just copy and paste everything!
-- All IDs are INT with auto-increment (1, 2, 3, etc.)

-- STEP 1: Insert Exam
INSERT INTO MockExams (ExamName, ExamCode, ExamYear, Description, IsActive)
VALUES ('Sub Inspector Examination', 'SUB_INS_2026', 2026, 'Sub Inspector Competitive Examination for SPSC', 1);

-- STEP 2: Insert Mock Test
INSERT INTO MockTests (ExamId, Title, PaperType, PaperNumber, DurationMinutes, TotalMarks, TotalQuestions, PassingMarks, IsActive)
VALUES (1, 'Paper 1 - General Knowledge', 'Prelim', 1, 5, 100, 10, 60, 1);

-- STEP 3: Insert Section
INSERT INTO MockSections (MockTestId, SectionName, SubjectTag, OrderIndex, QuestionCount, MarksPerQuestion, NegativeMarks)
VALUES (1, 'General Knowledge - India & Sikkim', 'General Knowledge', 1, 10, 10, 0);

-- STEP 4: Insert All 10 Questions
INSERT INTO MockQuestions (SectionId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, Explanation, DifficultyLevel, OrderIndex, IsActive)
VALUES
(1, 'What is the capital of India?', 'Mumbai', 'New Delhi', 'Kolkata', 'Chennai', 'B', 'New Delhi is the capital and largest city of India.', 'Easy', 1, 1),
(1, 'Which state is the highest producer of tea in India?', 'Tamil Nadu', 'Assam', 'Kerala', 'Himachal Pradesh', 'B', 'Assam is the largest tea-producing state in India, producing over 50% of India''s total tea.', 'Easy', 2, 1),
(1, 'Sikkim shares its border with which of the following countries?', 'Bangladesh and Bhutan', 'Tibet and Nepal', 'China, Bhutan, and Nepal', 'Bhutan only', 'C', 'Sikkim shares borders with China (to the north and east), Bhutan (to the south), and Nepal (to the west).', 'Medium', 3, 1),
(1, 'What is the capital of Sikkim?', 'Geyzing', 'Gangtok', 'Ravangla', 'Lachung', 'B', 'Gangtok is the capital and largest city of Sikkim state.', 'Easy', 4, 1),
(1, 'Which river is the lifeline of India?', 'Brahmaputra', 'Ganges', 'Yamuna', 'Narmada', 'B', 'The River Ganges (Ganga) is considered the lifeline of India and has immense religious significance.', 'Medium', 5, 1),
(1, 'Sikkim became a full state of India in which year?', '1975', '1972', '1970', '1978', 'A', 'Sikkim was officially recognized as the 22nd state of India on May 16, 1975.', 'Medium', 6, 1),
(1, 'What is the literacy rate trend in Sikkim?', 'Below 60%', 'Between 60-70%', 'Between 80-90%', 'Above 90%', 'C', 'Sikkim has one of the highest literacy rates in India at approximately 87.4%.', 'Hard', 7, 1),
(1, 'Which is the smallest union territory of India by population?', 'Dadra and Nagar Haveli', 'Puducherry', 'Lakshadweep', 'Andaman and Nicobar Islands', 'C', 'Lakshadweep is the smallest union territory of India in terms of population.', 'Hard', 8, 1),
(1, 'What is the primary occupation in Sikkim?', 'Mining', 'Manufacturing', 'Agriculture', 'Tourism', 'C', 'Agriculture, particularly the cultivation of cardamom, ginger, and other spices, is the primary occupation in Sikkim.', 'Medium', 9, 1),
(1, 'Which mountain peak is located in Sikkim?', 'K2', 'Mount Kanchenjunga', 'Mount Everest', 'Mount Nanda Devi', 'B', 'Mount Kanchenjunga (8,586 m) is the third-highest peak in the world and is located on the border of Sikkim with Nepal.', 'Medium', 10, 1);

-- VERIFICATION - Run these to check if data was inserted correctly
PRINT '======= EXAM DATA ======='
SELECT ExamId, ExamName, ExamCode, ExamYear, IsActive FROM MockExams WHERE ExamCode = 'SUB_INS_2026';

PRINT '======= MOCK TEST DATA ======='
SELECT MockTestId, Title, PaperType, DurationMinutes, TotalQuestions, PassingMarks FROM MockTests WHERE Title = 'Paper 1 - General Knowledge';

PRINT '======= SECTION DATA ======='
SELECT SectionId, SectionName, QuestionCount, MarksPerQuestion FROM MockSections WHERE SectionName = 'General Knowledge - India & Sikkim';

PRINT '======= QUESTIONS DATA ======='
SELECT QuestionId, QuestionText, CorrectOption, DifficultyLevel FROM MockQuestions WHERE SectionId = 1 ORDER BY OrderIndex;

PRINT '======= TOTAL QUESTION COUNT ======='
SELECT COUNT(*) AS TotalQuestions FROM MockQuestions WHERE SectionId = 1;
