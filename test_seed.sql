-- Test Seed for SPSCReadyDb - Run in SSMS
-- Clear existing (optional)
DELETE FROM ExamPapers;
DELETE FROM ExamCycles;
DELETE FROM Posts;
DELETE FROM ExamStages;
DELETE FROM Subjects;
DELETE FROM Departments;

-- Departments
INSERT INTO Departments (Id, Name) VALUES 
('11111111-1111-1111-1111-111111111111', 'Sindh Police'),
('22222222-2222-2222-2222-222222222222', 'Health Department'),
('33333333-3333-3333-3333-333333333333', 'Education Department');

-- Posts
INSERT INTO Posts (Id, DepartmentId, Name) VALUES 
('a1b2c3d4-e5f6-7890-abcd-ef1234567890', '11111111-1111-1111-1111-111111111111', 'Sub Inspector'),
('b2c3d4e5-f6g7-8901-bcde-f23456789012', '11111111-1111-1111-1111-111111111111', 'Assistant Sub Inspector'),
('d4e5f6g7-h8i9-0123-defg-456789012345', '22222222-2222-2222-2222-222222222222', 'Medical Officer');

-- Exam Stages
INSERT INTO ExamStages (Id, Name) VALUES 
('44444444-4444-4444-4444-444444444444', 'Prelims'),
('55555555-5555-5555-5555-555555555555', 'Mains');

-- Subjects
INSERT INTO Subjects (Id, Name) VALUES 
('77777777-7777-7777-7777-777777777777', 'General Knowledge'),
('88888888-8888-8888-8888-888888888888', 'Reasoning');

-- Exam Cycles (2020-2024 for SI)
INSERT INTO ExamCycles (Id, PostId, ExamYear) VALUES 
('d1e2f3a4-b5c6-d7e8-f9a0-b1c2d3e4f5a6', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 2024),
('e2f3g4b5-c6d7-e8f9-a0b1-c2d3e4f5a6b7', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 2023),
('f3g4h5c6-d7e8-f9a0-b1c2-d3e4f5a6b7c8', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 2022),
('11112222-3333-4444-5555-666677778888', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 2021),
('22223333-4444-5555-6666-777788889999', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 2020);

-- Verify
SELECT 'Departments' as TableName, COUNT(*) as Count FROM Departments
UNION ALL SELECT 'Posts', COUNT(*) FROM Posts
UNION ALL SELECT 'ExamCycles', COUNT(*) FROM ExamCycles;
