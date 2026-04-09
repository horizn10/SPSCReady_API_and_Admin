-- Test ExamPaper entries - PdfUrl is web path (not file system path). Copy your local PDFs to SPSCReady.API/wwwroot/pdfs/ then run
INSERT INTO ExamPapers (Id, ExamCycleId, ExamStageId, SubjectId, Title, PdfUrl, UploadedAt) VALUES 
('paper1-aaaa-bbbb-cccc-dddd-eeeeeeeeeeee', 'd1e2f3a4-b5c6-d7e8-f9a0-b1c2d3e4f5a6', '44444444-4444-4444-4444-444444444444', '77777777-7777-7777-7777-777777777777', 'SI Prelims 2024 GK', '/pdfs/your-local-pdf1.pdf', GETDATE()),
('paper2-bbbb-cccc-dddd-eeee-ffffffffffff', 'e2f3g4b5-c6d7-e8f9-a0b1-c2d3e4f5a6b7', '55555555-5555-5555-5555-555555555555', '88888888-8888-8888-8888-888888888888', 'SI Mains 2023 Reasoning', '/pdfs/your-local-pdf2.pdf', GETDATE());

-- Copy sample PDFs to SPSCReady.API/wwwroot/pdfs/ or update PdfUrl after upload
-- Verify
SELECT * FROM ExamPapers ORDER BY UploadedAt DESC;
