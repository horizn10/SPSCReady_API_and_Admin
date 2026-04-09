-- SPSCReady Test Data Seed Script for SSMS
-- Run in your SPSCReadyDb database
-- Generates GUIDs - copy them for dropdown testing

-- 1. Departments
INSERT INTO Departments (Id, Name) VALUES 
('11111111-1111-1111-1111-111111111111', 'Sindh Police'),
('22222222-2222-2222-2222-222222222222', 'Health Department'),
('33333333-3333-3333-3333-333333333333', 'Education');

-- 2. Posts (requires Dept FK)
INSERT INTO Posts (Id, DepartmentId, Name) VALUES 
('a1b2c3d4-e5f6-7890-abcd-ef1234567890', '11111111-1111-1111-1111-111111111111', 'Sub Inspector (SI)'),
('b2c3d4e5-f6g7-8901-bcde-f2345678901', '11111111-1111-1111-1111-111111111111', 'Assistant Sub Inspector (ASI)'),
('c3d4e5f6-g7h8-9012-cdef-3456789012', '22222222-2222-2222-2222-222222222222', 'Medical Officer');

-- 3. Exam Stages
INSERT INTO ExamStages (Id, Name) VALUES 
('44444444-4444-4444-4444-444444444444', 'Prelims'),
('55555555-5555-5555-5555-555555555555', 'Mains'),
('66666666-6666-6666-6666-666666666666', 'Interview');

-- 4. Subjects
INSERT INTO Subjects (Id, Name) VALUES 
('77777777-7777-7777-7777-777777777777', 'General Knowledge'),
('88888888-8888-8888-8888-888888888888', 'Reasoning'),
('99999999-9999-9999-9999-999999999999', 'English');

-- 5. Exam Cycles (requires Post FK)
INSERT INTO ExamCycles (Id, PostId, DepartmentId, ExamYear) VALUES 
('d1e2f3a4-b5c6-d7e8-f9a0-b1c2d3e4f5a6', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', '11111111-1111-1111-1111-111111111111', 2024),
('e2f3g4b5-c6d7-e8f9-a0b1-c2d3e4f5a6b7', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', '11111111-1111-1111-1111-111111111111', 2023),
('f3g4h5c6-d7e8-f9a0-b1c2-d3e4f5a6b7c8', 'b2c3d4e5-f6g7-8901-bcde-f2345678901', '11111111-1111-1111-1111-111111111111', 2024);

-- Copy these GUIDs for testing:
-- Departments: 1111... (Police), 2222... (Health), 3333... (Education)
-- Posts: a1b2... (SI Police), b2c3... (ASI Police)
-- Cycles: d1e2... (SI 2024), e2f3... (SI 2023)
-- Stages: 4444... (Prelims), 5555... (Mains)
-- Subjects: 7777... (GK), 8888... (Reasoning)

-- Verify:
SELECT * FROM Departments;
SELECT * FROM Posts;
SELECT * FROM ExamCycles;
SELECT * FROM ExamStages;
SELECT * FROM Subjects;

-- Now test upload: Police → SI → 2024 Cycle → Prelims → GK + PDF

