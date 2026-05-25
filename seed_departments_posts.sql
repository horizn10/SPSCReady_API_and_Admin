-- Fixed seed for testdb with int Ids for Departments, Posts, ExamStages

USE [testdb]

-- Clear
DELETE FROM ExamPapers;
DELETE FROM ExamPaperPosts;
DELETE FROM ExamPaperDepartments;
DELETE FROM ExamPaperStages;
DELETE FROM ExamPaperSubjects;
DELETE FROM ExamCycles;
DELETE FROM Posts;
DELETE FROM Subjects;
DELETE FROM ExamStages;
DELETE FROM Departments;

-- Departments
INSERT INTO Departments (Id, Name) VALUES (1, 'Police Department');
INSERT INTO Departments (Id, Name) VALUES (2, 'Health Department');
INSERT INTO Departments (Id, Name) VALUES (3, 'Education Department');

-- Posts
INSERT INTO Posts (Id, DepartmentId, Name) VALUES (1, 1, 'Sub Inspector (SI)');
INSERT INTO Posts (Id, Name) VALUES (2, 1, 'Police Constable');
INSERT INTO Posts (Id, Name) VALUES (3, 2, 'Staff Nurse');
INSERT INTO Posts (Id, Name) VALUES (4, 3, 'Assistant Teacher');

-- ExamStages
INSERT INTO ExamStages (Id, Name) VALUES (1, 'Prelims');
INSERT INTO ExamStages (Id, Name) VALUES (2, 'Mains');
INSERT INTO ExamStages (Id, Name) VALUES (3, 'Interview');

-- Subjects (optional for paper)
INSERT INTO Subjects (Id, Name) VALUES (1, 'General Knowledge');
INSERT INTO Subjects (Id, Name) VALUES (2, 'English');
INSERT INTO Subjects (Id, Name) VALUES (3, 'Mathematics');

-- Verify
SELECT 'Departments' as TableName, COUNT(*) as Count FROM Departments
UNION ALL SELECT 'Posts', COUNT(*) FROM Posts
UNION ALL SELECT 'ExamStages', COUNT(*) FROM ExamStages;
