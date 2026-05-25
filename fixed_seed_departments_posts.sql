-- Fixed seed for testdb - INSERT all columns explicitly

USE [testdb]
GO

BEGIN TRANSACTION;

-- Clear FK-safe
DELETE FROM ExamPaperPosts;
DELETE FROM ExamPaperDepartments;
DELETE FROM ExamPaperStages;
DELETE FROM ExamPaperSubjects;
DELETE FROM ExamCycles;
DELETE FROM ExamPaperSubjects;
DELETE FROM Subjects;
DELETE FROM ExamStages;
DELETE FROM Posts;
DELETE FROM Departments;

-- Departments (Id IDENTITY auto, no Id needed)
INSERT INTO Departments (Name) VALUES ('Police Department');
INSERT INTO Departments (Name) VALUES ('Health Department');
INSERT INTO Departments (Name) VALUES ('Education Department');

-- Posts
INSERT INTO Posts (DepartmentId, Name) VALUES (1, 'Sub Inspector (SI)');
INSERT INTO Posts (DepartmentId, Name) VALUES (1, 'Police Constable');
INSERT INTO Posts (DepartmentId, Name) VALUES (2, 'Staff Nurse');
INSERT INTO Posts (DepartmentId, Name) VALUES (3, 'Assistant Teacher');

-- ExamStages
INSERT INTO ExamStages (Name) VALUES ('Prelims');
INSERT INTO ExamStages (Name) VALUES ('Mains');
INSERT INTO ExamStages (Name) VALUES ('Interview');

-- Subjects
INSERT INTO Subjects (Name, StageId) VALUES ('General Knowledge', 1);
INSERT INTO Subjects (Name, StageId) VALUES ('English', 2);
INSERT INTO Subjects (Name, StageId) VALUES ('Mathematics', 1);

COMMIT;

-- Verify
SELECT * FROM Departments;
SELECT * FROM Posts;
SELECT * FROM ExamStages;
SELECT COUNT(*) FROM Subjects;
