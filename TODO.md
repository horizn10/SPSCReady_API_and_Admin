# TODO

## Task: Remove subject dropdown; treat paper name as subject name for uploads

### Step 1 — UI cleanup (ManagePaper)
- Update `MyApp.Admin/MyApp.Admin/Views/ManagePaper/Create.cshtml`
- Remove Subject dropdown + header column
- Keep only: Paper Name (as subject name), Phase, PDF

### Step 2 — Remove SubjectId from upload model
- Update `MyApp.Admin/MyApp.Admin/Models/ExamPaperCreateModel.cs`
- Remove `SubjectId` from `SubjectPaperViewModel`

### Step 3 — Update controller JSON payload
- Update `MyApp.Admin/MyApp.Admin/Controllers/ManagePaperController.cs`
- Change `SubjectPapersJson` to include `Title` + `StageId` only

### Step 4 — Update API DTO / parsing
- Update `SPSCReady.Application/DTOs/SubjectPaperDto.cs`
- Update `SPSCReady.API/Controllers/PapersController.cs` deserialization accordingly

### Step 5 — Update service logic
- Update `SPSCReady.Infrastructure/Services/PaperService.cs`
- For each uploaded paper:
  - Use `Title` as subject name
  - Find `Subject` by name; if missing create it
  - Populate `ExamPaperSubject.SubjectId` and `SubjectName`

### Step 6 — Verification
- Upload one or more PDFs
- Confirm PDFs still saved under `wwwroot/pdf*`
- Confirm Subject links use created/found subjects

