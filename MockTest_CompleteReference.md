# Mock Test Database - Complete Reference

## Quick Navigation
- **File 1**: `MockTest_QuickInsert.sql` ← **Use this to run the SQL** (simple, no variables)
- **File 2**: `MockTest_InsertData.sql` ← Has detailed comments and variable handling
- **File 3**: `MockTest_Setup_Guide.md` ← Complete documentation (you're reading this area)
- **File 4**: `MockTest_CompleteReference.md` ← This file

---

## Data Summary Table

### What Will Be Created:

| Entity | Count | Details |
|--------|-------|---------|
| **Exams** | 1 | Sub Inspector Examination (ExamId = 1) |
| **Mock Tests** | 1 | Paper 1 - General Knowledge, 5 mins, 10 questions (MockTestId = 1) |
| **Sections** | 1 | General Knowledge - India & Sikkim (SectionId = 1) |
| **Questions** | 10 | All about India & Sikkim GK (QuestionIds = 1-10) |

---

## Question Details

| # | Question | Answer | Difficulty | Topic |
|----|----------|--------|-----------|-------|
| 1 | What is the capital of India? | **B** (New Delhi) | Easy | Geography |
| 2 | Highest tea producer state? | **B** (Assam) | Easy | Agriculture |
| 3 | Sikkim borders with? | **C** (China, Bhutan, Nepal) | Medium | Geography |
| 4 | Capital of Sikkim? | **B** (Gangtok) | Easy | Geography |
| 5 | Lifeline of India (river)? | **B** (Ganges) | Medium | Geography |
| 6 | Sikkim became full state in? | **A** (1975) | Medium | History |
| 7 | Literacy rate in Sikkim? | **C** (80-90%) | Hard | Demographics |
| 8 | Smallest UT by population? | **C** (Lakshadweep) | Hard | Geography |
| 9 | Primary occupation in Sikkim? | **C** (Agriculture) | Medium | Economics |
| 10 | Mountain peak in Sikkim? | **B** (Mount Kanchenjunga) | Medium | Geography |

---

## Exam Specifications

```
┌─────────────────────────────────────────────────┐
│           SUB INSPECTOR EXAMINATION             │
├─────────────────────────────────────────────────┤
│ Exam Code:      SUB_INS_2026                    │
│ Year:           2026                            │
│ Status:         Active                          │
└─────────────────────────────────────────────────┘
    │
    └─ MOCK TEST: Paper 1 - General Knowledge
       ├─ Paper Type:    Prelim (Preliminary)
       ├─ Paper Number:  1
       ├─ Duration:      5 minutes
       ├─ Total Marks:   100
       ├─ Total Qs:      10
       ├─ Pass Marks:    60
       └─ Status:        Active
           │
           └─ SECTION: General Knowledge - India & Sikkim
              ├─ Subject Tag:       General Knowledge
              ├─ Question Count:    10
              ├─ Marks/Question:    10
              ├─ Negative Marks:    0
              └─ Total Marks:       100
                  │
                  └─ QUESTIONS 1-10 (as per table above)
                     ├─ Each has 4 options (A, B, C, D)
                     ├─ Marked correct option
                     ├─ Explanation provided
                     ├─ Difficulty level set
                     └─ All Active & Enabled
```

---

## Primary Keys (INT Auto-Increment)

| Table | ID Column | Starting Value | Auto-Increment | Notes |
|-------|-----------|---------------|--------------------|-------|
| MockExams | ExamId | 1 | +1 | Simple clean IDs |
| MockTests | MockTestId | 1 | +1 | No complex varchar |
| MockSections | SectionId | 1 | +1 | Auto-assigned |
| MockQuestions | QuestionId | 1 | +1 | Sequential 1-10 |
| MockUserAttempts | AttemptId | 1 | +1 | For future attempts |
| MockUserAnswers | AnswerId | 1 | +1 | For future answers |

**All use SQL Identity property for auto-increment**

---

## Foreign Key Relationships

```
MockExams (ExamId)
    ↓ (one-to-many)
MockTests (MockTestId, ExamId FK)
    ├─ (one-to-many)
    │ MockSections (SectionId, MockTestId FK)
    │     ↓ (one-to-many)
    │     MockQuestions (QuestionId, SectionId FK)
    │
    └─ (one-to-many)
      MockUserAttempts (AttemptId, MockTestId FK)
          ↓ (one-to-many)
          MockUserAnswers (AnswerId, AttemptId FK, QuestionId FK)
```

---

## How to Use the SQL Files

### Option 1: Use QuickInsert (Recommended for Testing)
```sql
-- Copy all content from MockTest_QuickInsert.sql
-- Paste into SQL Server Management Studio
-- Press F5 to execute
-- Done! All data is inserted
```

### Option 2: Use Detailed Version (For Learning)
```sql
-- Open MockTest_InsertData.sql
-- Read through the comments to understand each step
-- Execute step by step if you want to understand the process
-- Or just run all at once (the script handles everything)
```

---

## Testing the Data

### To verify the exam structure:
```sql
SELECT * FROM MockExams WHERE ExamCode = 'SUB_INS_2026';
```
**Expected**: 1 row with ExamId = 1

### To verify the mock test:
```sql
SELECT * FROM MockTests WHERE Title = 'Paper 1 - General Knowledge';
```
**Expected**: 1 row with MockTestId = 1, DurationMinutes = 5

### To verify all questions:
```sql
SELECT QuestionId, QuestionText, CorrectOption FROM MockQuestions WHERE SectionId = 1 ORDER BY OrderIndex;
```
**Expected**: 10 rows, all with IsActive = 1

### To verify correct option is in correct format:
```sql
SELECT DISTINCT CorrectOption FROM MockQuestions WHERE SectionId = 1;
```
**Expected**: A, B, C (no D in this set)

---

## Marking Scheme

| Aspect | Details |
|--------|---------|
| **Total Marks** | 100 |
| **Number of Questions** | 10 |
| **Marks per Question** | 10 |
| **Negative Marks** | 0 (no negative marking) |
| **Passing Marks** | 60 (60%) |
| **Time Limit** | 5 minutes |

### Score Calculation Example:
- If student answers 8 questions correctly:
  - Correct: 8 × 10 = 80 marks
  - Wrong: 2 × 0 = 0 (no negative)
  - **Total: 80 marks (80%)**
  - **Result: PASS** (>60)

---

## Database Constraints Implemented

1. **PRIMARY KEY**
   - All tables have INT identity PK
   - Auto-increment from 1

2. **FOREIGN KEYS**
   - MockTests.ExamId → MockExams.ExamId (ON DELETE RESTRICT)
   - MockSections.MockTestId → MockTests.MockTestId (ON DELETE CASCADE)
   - MockQuestions.SectionId → MockSections.SectionId (ON DELETE CASCADE)
   - MockUserAttempts.MockTestId → MockTests.MockTestId (ON DELETE RESTRICT)
   - MockUserAnswers.AttemptId → MockUserAttempts.AttemptId (ON DELETE CASCADE)
   - MockUserAnswers.QuestionId → MockQuestions.QuestionId (ON DELETE RESTRICT)

3. **UNIQUE CONSTRAINTS**
   - MockExams.ExamCode must be unique

4. **DEFAULT VALUES**
   - IsActive defaults to 1 (true)
   - CreatedAt defaults to GETUTCDATE()

---

## Enums Used

### PaperType
- `Prelim` = 1 (used in our test)
- `Mains` = 2

### DifficultyLevel
- `Easy` = 1 (4 questions in our set)
- `Medium` = 2 (5 questions in our set)
- `Hard` = 3 (1 question in our set)

### AttemptStatus (for future use)
- `InProgress` - Test is being taken
- `Submitted` - Test completed and submitted
- `Expired` - Time limit exceeded

---

## Important Notes

✅ **What's Included**:
- Exam record
- Mock test with correct duration (5 mins)
- Section with General Knowledge content
- 10 fully detailed questions with explanations
- All correct options marked (A, B, C as answers)
- Difficulty levels assigned
- Simple INT primary keys

⚠️ **What's NOT Included** (for future addition):
- User attempts (will be created when users take the test)
- User answers (will be created when users submit answers)
- Student accounts (use your user registration system)

🔄 **To Create a User Attempt**:
```sql
INSERT INTO MockUserAttempts 
(UserId, MockTestId, StartedAt, ExpiresAt, CorrectCount, WrongCount, SkippedCount, Status)
VALUES 
('user-id-here', 1, GETUTCDATE(), DATEADD(MINUTE, 5, GETUTCDATE()), 0, 0, 10, 'InProgress');
```

---

## Sample API Calls (After Data is Inserted)

### Get Exam by ID
```
GET /api/exams/1
Response: { ExamId: 1, ExamName: "Sub Inspector Examination", ... }
```

### Get Mock Test
```
GET /api/mockTests/1
Response: { MockTestId: 1, Title: "Paper 1 - General Knowledge", ... }
```

### Get Section with Questions
```
GET /api/sections/1/questions
Response: [ {QuestionId: 1, QuestionText: "...", OptionA: "...", ...}, ... ]
```

### Submit Test Answer
```
POST /api/attempts/1/answers
Body: { QuestionId: 1, SelectedOption: "B" }
```

---

## Troubleshooting Guide

| Problem | Cause | Solution |
|---------|-------|----------|
| "Violation of FOREIGN KEY" | ExamId doesn't exist | Run Step 1 (Exam insert) first |
| "Duplicate Key Error" | Primary key conflict | Data already exists; delete and re-run |
| Questions not showing | Wrong SectionId | Verify Section was created with SectionId = 1 |
| Wrong number of records | Partial execution | Run the complete script again |
| Strings have extra quotes | Copy-paste issue | Use the QuickInsert version instead |

---

## Files Provided

1. **MockTest_QuickInsert.sql**
   - Simplest version
   - Copy and paste everything
   - No variable handling needed
   - Best for quick testing

2. **MockTest_InsertData.sql**
   - Full detailed version
   - Includes variable handling
   - Verification queries included
   - Good for understanding the process

3. **MockTest_Setup_Guide.md**
   - Complete documentation
   - Column descriptions
   - Table relationships explained
   - Step-by-step instructions

4. **MockTest_CompleteReference.md**
   - This file
   - Quick reference
   - Troubleshooting guide
   - API call examples

---

## Next Steps

1. ✅ Choose one SQL file (QuickInsert recommended)
2. ✅ Copy all content
3. ✅ Open SQL Server Management Studio
4. ✅ Paste and execute (F5)
5. ✅ Run verification queries to confirm
6. ✅ Test with your API
7. ✅ Create a user attempt to test the full flow

---

## Questions & Answers About the Data

**Q: Why are all correct answers different (A, B, C)?**
A: This makes the test fair and prevents pattern recognition. No single option appears too many times.

**Q: Why 10 questions for a 5-minute test?**
A: That's 30 seconds per question, which is reasonable for multiple-choice GK questions.

**Q: Can I add more questions later?**
A: Yes! Insert more rows into MockQuestions with SectionId = 1 and increment OrderIndex.

**Q: Can I modify difficulty levels?**
A: Yes! Update the DifficultyLevel column (Easy, Medium, Hard).

**Q: What if a student's time expires?**
A: Set Status = 'Expired' and don't allow further answers. Partial marks can be calculated.

**Q: Are negative marks enabled?**
A: No, NegativeMarks = 0. Wrong answers don't reduce score.

---

## Database Diagram (Text Format)

```
┌──────────────────────────────────┐
│         MOCKEXAMS                │
├──────────────────────────────────┤
│ PK: ExamId (INT)                 │
│ ExamName: nvarchar(200)          │
│ ExamCode: nvarchar(50) [UNIQUE]  │
│ ExamYear: INT                    │
│ Description: nvarchar(max)       │
│ IsActive: BIT (1)                │
│ CreatedAt: datetime2             │
└──────────────────────────────────┘
          ↓ 1:N
┌──────────────────────────────────┐
│       MOCKTESTS                  │
├──────────────────────────────────┤
│ PK: MockTestId (INT)             │
│ FK: ExamId → MockExams           │
│ Title: nvarchar(200)             │
│ PaperType: nvarchar(50)          │
│ PaperNumber: INT                 │
│ DurationMinutes: INT             │
│ TotalMarks: decimal(8,2)         │
│ TotalQuestions: INT              │
│ PassingMarks: decimal(8,2)       │
│ IsActive: BIT (1)                │
└──────────────────────────────────┘
          ↓ 1:N
┌──────────────────────────────────┐
│      MOCKSECTIONS                │
├──────────────────────────────────┤
│ PK: SectionId (INT)              │
│ FK: MockTestId → MockTests       │
│ SectionName: nvarchar(200)       │
│ SubjectTag: nvarchar(100)        │
│ OrderIndex: INT                  │
│ QuestionCount: INT               │
│ MarksPerQuestion: decimal(4,2)   │
│ NegativeMarks: decimal(4,2)      │
└──────────────────────────────────┘
          ↓ 1:N
┌──────────────────────────────────┐
│      MOCKQUESTIONS               │
├──────────────────────────────────┤
│ PK: QuestionId (INT)             │
│ FK: SectionId → MockSections     │
│ QuestionText: nvarchar(max)      │
│ OptionA/B/C/D: nvarchar(max)     │
│ CorrectOption: nvarchar(1)       │
│ Explanation: nvarchar(max)       │
│ DifficultyLevel: nvarchar(50)    │
│ OrderIndex: INT                  │
│ IsActive: BIT (1)                │
└──────────────────────────────────┘
```

---

End of Reference Guide. Happy Testing! 🚀
