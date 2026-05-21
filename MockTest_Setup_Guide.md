# Mock Test Data - Table Relationships & Setup Guide

## Quick Summary

You now have:
- ✅ **1 Exam**: "Sub Inspector Examination" 
- ✅ **1 Mock Test**: "Paper 1 - General Knowledge" (Prelims, 5 minutes, 10 questions)
- ✅ **1 Section**: "General Knowledge - India & Sikkim"
- ✅ **10 Questions**: All about India and Sikkim GK with options and explanations
- ✅ **All IDs use INT identity** (simple auto-incrementing numbers, not complex varchar)

---

## Table Hierarchy & Relationships

```
EXAM (Parent)
└── MOCK TEST (belongs to exam)
    └── SECTION (belongs to mock test)
        └── QUESTIONS (belong to section, each has 4 options + correct answer)

USER ATTEMPT (user takes test)
├── belongs to MOCK TEST
└── USER ANSWERS (one per question)
    └── references QUESTION (for validation)
```

### How Tables Link Together:

| From Table | To Table | Relationship | Cardinality |
|-----------|----------|--------------|------------|
| `MockTests` | `MockExams` | ExamId FK | Many tests per exam (1:N) |
| `MockSections` | `MockTests` | MockTestId FK | Many sections per test (1:N) |
| `MockQuestions` | `MockSections` | SectionId FK | Many questions per section (1:N) |
| `MockUserAttempts` | `MockTests` | MockTestId FK | Many attempts per test (1:N) |
| `MockUserAttempts` | `AspNetUsers` | UserId FK | Each user has many attempts (1:N) |
| `MockUserAnswers` | `MockUserAttempts` | AttemptId FK | Many answers per attempt (1:N) |
| `MockUserAnswers` | `MockQuestions` | QuestionId FK | Each answer is for one question (N:1) |

---

## Data Flow Example

When a student takes a test:

```
1. Student selects exam → finds Mock Test "Paper 1"
2. System loads Section "General Knowledge - India & Sikkim"
3. System displays 10 Questions (from MockQuestions table)
4. Student answers → Creates MockUserAttempt record
5. For each answer → Creates MockUserAnswer record
6. System compares SelectedOption with CorrectOption
7. Calculates score and marks
```

---

## How to Run the SQL Script

### Method 1: Run the entire script
1. Open SQL Server Management Studio (SSMS)
2. Open the file: `MockTest_InsertData.sql`
3. Execute the entire script (F5)
4. All data will be inserted automatically

### Method 2: Run step-by-step
1. Run STEP 1 (Insert Exam) first
2. Get the ExamId from the verification queries
3. Use that ExamId in STEP 2
4. Continue with STEP 3 & 4

> **NOTE**: The script uses subqueries to automatically get IDs, so you can run it all at once without issues.

---

## Primary Key Strategy

All tables use **INT with Identity(1,1)** as Primary Keys:
- ✅ Simple and clean (ExamId 1, 2, 3...)
- ✅ Auto-incrementing (no manual assignment needed)
- ✅ Best performance for foreign key relationships
- ✅ Easy to track and debug

---

## Data in This Package

### Exam Details:
- **Name**: Sub Inspector Examination
- **Code**: SUB_INS_2026
- **Year**: 2026

### Mock Test Details:
- **Title**: Paper 1 - General Knowledge
- **Type**: Prelim (Preliminary Round)
- **Paper Number**: 1
- **Duration**: 5 minutes
- **Total Questions**: 10
- **Total Marks**: 100 (10 marks per question)
- **Passing Marks**: 60

### 10 Questions Content:
All questions are multiple choice with 4 options (A, B, C, D):

1. ✓ Capital of India
2. ✓ Highest tea producer state
3. ✓ Sikkim borders
4. ✓ Capital of Sikkim
5. ✓ Lifeline of India (river)
6. ✓ When Sikkim became a full state
7. ✓ Literacy rate in Sikkim
8. ✓ Smallest union territory by population
9. ✓ Primary occupation in Sikkim
10. ✓ Mountain peak in Sikkim

### Difficulty Levels:
- Easy: 4 questions
- Medium: 5 questions
- Hard: 1 question

---

## Column Details

### MockExams Table
| Column | Type | Description |
|--------|------|-------------|
| ExamId | INT (PK) | Auto-incrementing primary key |
| ExamName | nvarchar(200) | Full name of the exam |
| ExamCode | nvarchar(50) UNIQUE | Unique code (e.g., SUB_INS_2026) |
| ExamYear | INT | Year of the exam |
| Description | nvarchar(max) | Optional description |
| IsActive | BIT | Enable/disable the exam (0=inactive, 1=active) |
| CreatedAt | datetime2 | Auto-set to UTC now |

### MockTests Table
| Column | Type | Description |
|--------|------|-------------|
| MockTestId | INT (PK) | Auto-incrementing primary key |
| ExamId | INT (FK) | Links to the exam |
| Title | nvarchar(200) | Name of the mock test |
| PaperType | nvarchar(50) | "Prelim" or "Mains" |
| PaperNumber | INT | Which paper (1, 2, 3...) |
| DurationMinutes | INT | Time allowed (in minutes) |
| TotalMarks | decimal(8,2) | Total marks for the test |
| TotalQuestions | INT | Number of questions |
| PassingMarks | decimal(8,2) | Passing threshold |
| IsActive | BIT | Enable/disable the test |

### MockSections Table
| Column | Type | Description |
|--------|------|-------------|
| SectionId | INT (PK) | Auto-incrementing primary key |
| MockTestId | INT (FK) | Links to the mock test |
| SectionName | nvarchar(200) | Name of the section |
| SubjectTag | nvarchar(100) | Category tag (e.g., "General Knowledge") |
| OrderIndex | INT | Display order (1, 2, 3...) |
| QuestionCount | INT | Number of questions in section |
| MarksPerQuestion | decimal(4,2) | Marks for each question |
| NegativeMarks | decimal(4,2) | Negative marking (0 = no negative) |

### MockQuestions Table
| Column | Type | Description |
|--------|------|-------------|
| QuestionId | INT (PK) | Auto-incrementing primary key |
| SectionId | INT (FK) | Links to the section |
| QuestionText | nvarchar(max) | The question itself |
| OptionA/B/C/D | nvarchar(max) | Multiple choice options |
| CorrectOption | nvarchar(1) | 'A', 'B', 'C', or 'D' |
| Explanation | nvarchar(max) | Why this is correct |
| DifficultyLevel | nvarchar(50) | "Easy", "Medium", or "Hard" |
| OrderIndex | INT | Question display order (1-10) |
| IsActive | BIT | Enable/disable question |

### MockUserAttempts Table
| Column | Type | Description |
|--------|------|-------------|
| AttemptId | INT (PK) | Auto-incrementing primary key |
| UserId | nvarchar(450) (FK) | Links to AspNetUsers |
| MockTestId | INT (FK) | Which test the user took |
| StartedAt | datetime2 | When user started |
| SubmittedAt | datetime2 | When user finished (nullable) |
| ExpiresAt | datetime2 | When the test expires |
| TotalScore | decimal(8,2) | Final score (nullable until submitted) |
| CorrectCount | INT | Number of correct answers |
| WrongCount | INT | Number of wrong answers |
| SkippedCount | INT | Number of skipped questions |
| Percentage | decimal(5,2) | Score percentage (nullable) |
| Status | nvarchar(50) | "InProgress", "Submitted", "Expired" |

### MockUserAnswers Table
| Column | Type | Description |
|--------|------|-------------|
| AnswerId | INT (PK) | Auto-incrementing primary key |
| AttemptId | INT (FK) | Links to the attempt |
| QuestionId | INT (FK) | Which question was answered |
| SelectedOption | nvarchar(1) | 'A', 'B', 'C', 'D', or null (skipped) |
| IsCorrect | BIT | Whether answer was correct (nullable) |
| MarksAwarded | decimal(6,2) | Marks obtained (nullable) |
| IsMarkedForReview | BIT | User marked for review |
| AnsweredAt | datetime2 | When the answer was submitted |

---

## Next Steps

1. ✅ **Run the SQL script** to insert the data
2. ✅ **Run verification queries** to confirm data was inserted
3. ✅ **Test the API** with these IDs:
   - Exam ID: 1 (or whatever was inserted)
   - MockTest ID: 1 (or whatever was inserted)
   - Section ID: 1 (or whatever was inserted)
   - Question IDs: 1-10

4. Create a user attempt when testing
5. Submit answers and verify scoring logic

---

## Important Notes

- ⚠️ All IDs are **simple INT (auto-incrementing)** - no complex strings
- ✅ **Cascading delete** is enabled for sections and questions (if you delete a test, sections and questions are deleted too)
- ✅ **Restrict delete** for exam and user attempts (can't delete if there are related records)
- ✅ **All correct options are marked** (CorrectOption column has A, B, C, or D)
- ✅ **PaperType uses enum value**: "Prelim" (stored as string in DB)
- ✅ **DifficultyLevel uses enum**: "Easy", "Medium", "Hard"

---

## Example Query to Get Full Test Structure

```sql
-- Get complete test structure
SELECT 
    e.ExamId, e.ExamName,
    m.MockTestId, m.Title, m.DurationMinutes, m.TotalQuestions,
    s.SectionId, s.SectionName,
    q.QuestionId, q.QuestionText, q.CorrectOption, q.DifficultyLevel
FROM MockExams e
JOIN MockTests m ON e.ExamId = m.ExamId
JOIN MockSections s ON m.MockTestId = s.MockTestId
JOIN MockQuestions q ON s.SectionId = q.SectionId
WHERE e.ExamCode = 'SUB_INS_2026'
ORDER BY e.ExamId, m.MockTestId, s.SectionId, q.OrderIndex;
```

This gives you the entire hierarchy in one result set!

---

## Troubleshooting

**Issue**: "Violation of FOREIGN KEY constraint" when inserting MockTests
- **Solution**: Make sure the ExamId exists. Run STEP 1 first.

**Issue**: Questions not appearing in sections
- **Solution**: Verify SectionId is correct. Run verification query for sections first.

**Issue**: Duplicate key error
- **Solution**: The script uses auto-increment. If you run it twice, you'll get duplicates. Delete the data and re-run.

**To delete all test data:**
```sql
DELETE FROM MockUserAnswers WHERE AttemptId IN (SELECT AttemptId FROM MockUserAttempts WHERE MockTestId = @MockTestId);
DELETE FROM MockUserAttempts WHERE MockTestId = @MockTestId;
DELETE FROM MockQuestions WHERE SectionId IN (SELECT SectionId FROM MockSections WHERE MockTestId = @MockTestId);
DELETE FROM MockSections WHERE MockTestId = @MockTestId;
DELETE FROM MockTests WHERE MockTestId = @MockTestId;
DELETE FROM MockExams WHERE ExamCode = 'SUB_INS_2026';
```
