using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPSCReady.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMockTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Exam
            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "ExamName", "ExamCode", "ExamYear", "Description", "IsActive", "CreatedAt" },
                values: new object[] { "Staff Selection Commission Combined", "SI-2025", 2025, "SCC Combined Graduate Level Exam 2025", true, DateTime.UtcNow }
            );

            // Get the ExamId (should be 1)
            migrationBuilder.Sql("DECLARE @ExamId INT = (SELECT ExamId FROM Exams WHERE ExamCode = 'SI-2025');" +
                "INSERT INTO MockTests (ExamId, Title, PaperType, PaperNumber, DurationMinutes, TotalMarks, TotalQuestions, PassingMarks, IsActive, CreatedAt) " +
                "VALUES (@ExamId, 'SSC CGL 2025 - Preliminary Mock Test 1', 'Prelim', 1, 120, 200, 100, 100, 1, GETUTCDATE()); " +
                "INSERT INTO MockTests (ExamId, Title, PaperType, PaperNumber, DurationMinutes, TotalMarks, TotalQuestions, PassingMarks, IsActive, CreatedAt) " +
                "VALUES (@ExamId, 'SSC CGL 2025 - Preliminary Mock Test 2', 'Prelim', 2, 120, 200, 100, 100, 1, GETUTCDATE()); " +
                "DECLARE @MockTestId INT = (SELECT TOP 1 MockTestId FROM MockTests WHERE ExamId = @ExamId AND PaperType = 'Prelim' ORDER BY PaperNumber); " +
                "INSERT INTO Sections (MockTestId, SectionName, SubjectTag, OrderIndex, QuestionCount, MarksPerQuestion, NegativeMarks) " +
                "VALUES (@MockTestId, 'General Awareness', 'GA', 1, 25, 2.0, 0.5); " +
                "INSERT INTO Sections (MockTestId, SectionName, SubjectTag, OrderIndex, QuestionCount, MarksPerQuestion, NegativeMarks) " +
                "VALUES (@MockTestId, 'English Language', 'ENG', 2, 25, 2.0, 0.5); " +
                "INSERT INTO Sections (MockTestId, SectionName, SubjectTag, OrderIndex, QuestionCount, MarksPerQuestion, NegativeMarks) " +
                "VALUES (@MockTestId, 'Quantitative Aptitude', 'QA', 3, 25, 2.0, 0.5); " +
                "INSERT INTO Sections (MockTestId, SectionName, SubjectTag, OrderIndex, QuestionCount, MarksPerQuestion, NegativeMarks) " +
                "VALUES (@MockTestId, 'Reasoning', 'REASON', 4, 25, 2.0, 0.5);"
            );

            // Add sample questions
            migrationBuilder.Sql(
                "DECLARE @SectionId INT, @QuestionIndex INT = 1; " +
                "SET @SectionId = (SELECT TOP 1 SectionId FROM Sections WHERE SectionName = 'General Awareness' ORDER BY SectionId); " +
                "INSERT INTO Questions (SectionId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, OrderIndex, IsActive) " +
                "VALUES (@SectionId, 'What is the capital of India?', 'Delhi', 'Mumbai', 'Bangalore', 'Kolkata', 'A', 'Easy', 1, 1); " +
                "INSERT INTO Questions (SectionId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, OrderIndex, IsActive) " +
                "VALUES (@SectionId, 'Who was the first Prime Minister of India?', 'Jawaharlal Nehru', 'Indira Gandhi', 'Rajiv Gandhi', 'Atal Bihari Vajpayee', 'A', 'Easy', 2, 1);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete in reverse order due to FK constraints
            migrationBuilder.Sql("DELETE FROM UserAnswers; DELETE FROM UserAttempts; DELETE FROM Questions; DELETE FROM Sections; DELETE FROM MockTests; DELETE FROM Exams WHERE ExamCode = 'SI-2025';");
        }
    }
}
