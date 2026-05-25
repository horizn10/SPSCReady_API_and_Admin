USE [testdb];

-- =============================================
-- Insert Test Data: Police Department + SI + Prelims + GK
-- Single batch - no GO statements to preserve variable scope
-- =============================================

BEGIN TRANSACTION;

-- 1. Insert Department
INSERT INTO Departments (Name) VALUES ('Police Department');
DECLARE @DeptId INT = SCOPE_IDENTITY();
PRINT 'Department ID: ' + CAST(@DeptId AS VARCHAR);

-- 2. Insert Post (linked to Department)
INSERT INTO Posts (DepartmentId, Name) VALUES (@DeptId, 'Sub Inspector (SI)');
DECLARE @PostId INT = SCOPE_IDENTITY();
PRINT 'Post ID: ' + CAST(@PostId AS VARCHAR);

-- 3. Insert Exam Stage
INSERT INTO ExamStages (Name) VALUES ('Prelims');
DECLARE @StageId INT = SCOPE_IDENTITY();
PRINT 'Stage ID: ' + CAST(@StageId AS VARCHAR);

-- 4. Insert Subject (linked to Stage - required FK)
INSERT INTO Subjects (Name, StageId) VALUES ('General Knowledge', @StageId);
DECLARE @SubjectId INT = SCOPE_IDENTITY();
PRINT 'Subject ID: ' + CAST(@SubjectId AS VARCHAR);

-- =============================================
-- Insert a sample Exam Paper + junction records
-- =============================================

INSERT INTO ExamPapers (Title, Description, ExamDate, UploadedBy, UploadedAt) 
VALUES (
    'SI Police Prelims GK Paper', 
    'Sample test paper for Sub Inspector Prelims General Knowledge', 
    GETDATE(), 
    'test_user', 
    GETDATE()
);
DECLARE @ExamId INT = SCOPE_IDENTITY();

-- Link the exam paper to all reference data via junction tables
INSERT INTO ExamPaperDepartments (ExamId, DepartmentId) VALUES (@ExamId, @DeptId);
INSERT INTO ExamPaperPosts (ExamId, PostId) VALUES (@ExamId, @PostId);
INSERT INTO ExamPaperStages (ExamId, StageId) VALUES (@ExamId, @StageId);
INSERT INTO ExamPaperSubjects (ExamId, StageId, SubjectId, SubjectName, Url, Date) 
VALUES (@ExamId, @StageId, @SubjectId, 'General Knowledge', '/pdfs/SI_Police_Prelims_Paper_I.pdf', GETDATE());

COMMIT;

-- =============================================
-- Verify inserted data
-- =============================================
SELECT 'Departments' AS TableName, * FROM Departments;
SELECT 'Posts' AS TableName, * FROM Posts;
SELECT 'ExamStages' AS TableName, * FROM ExamStages;
SELECT 'Subjects' AS TableName, * FROM Subjects;
SELECT 'ExamPapers' AS TableName, * FROM ExamPapers;
SELECT 'ExamPaperDepartments' AS TableName, * FROM ExamPaperDepartments;
SELECT 'ExamPaperPosts' AS TableName, * FROM ExamPaperPosts;
SELECT 'ExamPaperStages' AS TableName, * FROM ExamPaperStages;
SELECT 'ExamPaperSubjects' AS TableName, * FROM ExamPaperSubjects;
