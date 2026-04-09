-- Perfect interconnected test data for SPSCReadyDb - Run to clear & repopulate

BEGIN TRANSACTION;

-- Clear
DELETE FROM ExamPapers;
DELETE FROM ExamCycles;
DELETE FROM Posts;
DELETE FROM ExamStages;
DELETE FROM Subjects;
DELETE FROM Departments;

-- 1. Department
INSERT INTO Departments (Id, Name) VALUES ('11111111-1111-1111-1111-111111111111', 'Sindh Police');

-- 2. Post (FK to Department)
INSERT INTO Posts (Id, DepartmentId, Name) VALUES ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', '11111111-1111-1111-1111-111111111111', 'Sub Inspector (SI)');

-- 3. Exam Stages
INSERT INTO ExamStages (Id, Name) VALUES ('44444444-4444-4444-4444-444444444444', 'Prelims');

-- 4. Subjects
INSERT INTO Subjects (Id, Name) VALUES ('77777777-7777-7777-7777-777777777777', 'General Knowledge');

-- 5. Exam Cycle (FK to Post, DepartmentId required)
INSERT INTO ExamCycles (Id, PostId, DepartmentId, ExamYear) VALUES ('d1e2f3a4-b5c6-d7e8-f9a0-b1c2d3e4f5a6', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', '11111111-1111-1111-1111-111111111111', 2024);

COMMIT;

-- 6. ExamPaper (FK to Cycle, Stage, Subject)
INSERT INTO ExamPapers (Id, ExamCycleId, ExamStageId, SubjectId, Title, PdfUrl, UploadedAt) VALUES 
('11111111-2222-3333-4444-555555555555', 'd1e2f3a4-b5c6-d7e8-f9a0-b1c2d3e4f5a6', '44444444-4444-4444-4444-444444444444', '77777777-7777-7777-7777-777777777777', 'SI Prelims 2024 GK Test Paper', '/pdfs/SI_Police_Prelims_Paper_I.pdf', GETDATE());

COMMIT;

-- Verify full chain
SELECT 
  p.Title,
  d.Name as Department,
  po.Name as Post,
  ec.ExamYear as Cycle,
  es.Name as Stage,
  s.Name as Subject,
  p.PdfUrl
FROM ExamPapers p
JOIN ExamCycles ec ON p.ExamCycleId = ec.Id
JOIN Posts po ON ec.PostId = po.Id
JOIN Departments d ON po.DepartmentId = d.Id
JOIN ExamStages es ON p.ExamStageId = es.Id
