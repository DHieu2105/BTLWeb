# 🧪 STUDENT & SCORE MANAGEMENT MODULE - TEST GUIDE

**Module Owner**: Minh  
**Module**: Student Management + Score Management  
**Last Updated**: April 13, 2026  
**UI Language**: English

---

## **1. STUDENT MANAGEMENT (HocVien) - TEST SUITE**

### **1.1. Test CRUD Operations**

#### **Test 1.1.1: Create Student (Add Student)**

**Precondition**: User logged in as Admin or Staff  
**Steps**:

1. Navigate to: `/HocViens/Index`
2. Click "+ Add Student" button
3. Fill form with valid data:
   - Student ID (MaHocVien): `HV_TEST_001`
   - Full Name: `John Michael Doe`
   - Phone: `0987654321`
   - Registration Date: Today's date
   - Gender: Select "Nam" or "Nữ"
   - Manager (Nhân viên): Select from dropdown
4. Click "Create" or "Add" button
5. **Expected Results**:
   - ✅ Redirect to Student List (Index page)
   - ✅ New student appears in list with correct data
   - ✅ Success notification (if any)
   - ✅ Pagination updates if needed

**Test Data Variations**:
| Case | Full Name | Phone | Gender | Expected |
|------|-----------|-------|--------|----------|
| Valid | John Doe | 0987654321 | Nam | ✅ Create success |
| Valid | Jane Smith | 0123456789 | Nữ | ✅ Create success |
| Special chars | Nguyễn Văn Á | 0901234567 | Nam | ✅ Create success |

---

#### **Test 1.1.2: Edit Student**

**Precondition**: At least 1 student exists in system  
**Steps**:

1. Go to `/HocViens/Index`
2. Find a student row and click "Edit" button
3. Modify form fields:
   - Change Full Name: `Updated Name`
   - Change Phone: `0999999999`
   - Change Gender: Flip (Nam → Nữ or vice versa)
4. Click "Edit" or "Save" button
5. **Expected Results**:
   - ✅ Redirect to Student List
   - ✅ Updated data appears in list immediately
   - ✅ All fields updated correctly (no partial saves)

**Test Edit Scenarios**:
| Field | Original | New Value | Expected |
|-------|----------|-----------|----------|
| Phone | 0123456789 | 0987654321 | ✅ Updated |
| Full Name | John | Jane | ✅ Updated |
| Gender | Nam | Nữ | ✅ Updated |
| All fields | (old data) | (new data) | ✅ All updated |

---

#### **Test 1.1.3: View Student Details**

**Steps**:

1. Go to `/HocViens/Index`
2. Click "View" button on any student row
3. **Expected Results**:
   - ✅ Details page displays all student information
   - ✅ Data matches the row clicked
   - ✅ Links/buttons functional (Edit, Back, etc.)
   - ✅ Manager info displays if exists

---

#### **Test 1.1.4: Delete Student**

**Precondition**: Student has no critical dependencies  
**Steps**:

1. Go to `/HocViens/Index`
2. Click "Delete" button on any student
3. Verify confirmation page displays with warning
4. Click "Delete" to confirm
5. **Expected Results**:
   - ✅ Student removed from list
   - ✅ Redirect to Student List
   - ✅ Record no longer exists in database
   - ⚠️ If student has score records: Handle gracefully (show error or cascade delete)

---

#### **Test 1.1.5: Search Functionality**

**Steps**:

1. Go to `/HocViens/Index`
2. **Test Case A**: Search by student name
   - Type name in search box: `"John"`
   - Click "Search" button
   - **Expected**: Only students with "John" in name appear
3. **Test Case B**: Search by student ID
   - Type ID in search box: `"HV001"`
   - Click "Search" button
   - **Expected**: Only students with ID containing "HV001" appear
4. **Test Case C**: Search with special characters
   - Type: `"Nguyễn"`
   - **Expected**: ✅ Diacritics handled correctly
5. **Test Case D**: Case-insensitive search
   - Search: `"john"` and `"JOHN"`
   - **Expected**: Both return same results

---

#### **Test 1.1.6: Pagination**

**Steps**:

1. Go to `/HocViens/Index`
2. If student list has > 10 records:
   - Page 1 displays records 1-10
   - Click page "2" button
   - **Expected**: Records 11-20 display
3. Check pagination controls:
   - [ ] Previous button works
   - [ ] Next button works
   - [ ] Page numbers clickable
   - [ ] Current page highlighted
4. **Expected Results**:
   - ✅ 10 items max per page
   - ✅ Navigation between pages smooth
   - ✅ Total count accurate

---

### **1.2. Test Authorization & Security**

#### **Test 1.2.1: Admin Role Access**

**Steps**:

1. Login as: Admin account
2. Navigate to: `/HocViens/Index`
3. **Expected**:
   - ✅ Full list visible
   - ✅ Can click Edit, Delete, View buttons
   - ✅ Can create new student
   - ✅ No access denied messages

---

#### **Test 1.2.2: Staff Role Access**

**Steps**:

1. Login as: Staff/NhanVien account
2. Navigate to: `/HocViens/Index`
3. **Expected**:
   - ✅ Full list visible
   - ✅ Can perform all CRUD operations
   - ✅ Same permissions as Admin

---

#### **Test 1.2.3: Teacher Role Access (SHOULD BE DENIED)**

**Steps**:

1. Login as: Teacher/GiaoVien account
2. Navigate to: `/HocViens/Index`
3. **Expected**:
   - ❌ Access Denied page (403 Forbidden)
   - ✅ Cannot access student management

---

#### **Test 1.2.4: Student Role Access (SHOULD BE DENIED)**

**Steps**:

1. Login as: Student/HocVien account
2. Navigate to: `/HocViens/Index`
3. **Expected**:
   - ❌ Access Denied page (403 Forbidden)
   - ✅ Cannot manage students

---

#### **Test 1.2.5: Anonymous User Access (NOT LOGGED IN)**

**Steps**:

1. Logout from application
2. Try to access: `/HocViens/Index`
3. **Expected**:
   - ✅ Redirect to login page
   - ✅ Cannot view student data

---

### **1.3. Test Data Validation**

#### **Test 1.3.1: Required Fields Validation**

**Steps**:

1. Go to `/HocViens/Create`
2. Leave Student ID empty, fill others
3. Click "Create"
4. **Expected**: ✅ Error message: Field required

**Repeat for**:

- [ ] Full Name (required)
- [ ] Manager/NhanVien (if required)

---

#### **Test 1.3.2: Gender Field Validation**

**Steps**:

1. Go to `/HocViens/Create`
2. Fill all fields except Gender
3. Click "Create"
4. **Expected**: ✅ Gender defaults or shows error if required

**Test Gender Normalization** (backend):

1. Create student with Gender: `"male"` (lowercase)
2. View details
3. **Expected**: ✅ Stored/Displayed as "Nam"

---

#### **Test 1.3.3: Phone Number Validation**

**Steps**:

1. Try phone: `"abc123"` (invalid format)
2. **Expected**: ✅ Accepted (if no format validation) OR ❌ Error message

**Note**: If validation rules exist, test against them

---

#### **Test 1.3.4: Duplicate Student ID**

**Steps**:

1. Create student: ID = `HV001`
2. Try to create another student with same ID: `HV001`
3. Click "Create"
4. **Expected**:
   - ❌ Error message: Duplicate key or validation error
   - ✅ Data not saved to database

---

#### **Test 1.3.5: Date Field Validation**

**Steps**:

1. Set Registration Date in future (e.g., 2027-12-31)
2. Click "Create"
3. **Expected**:
   - ✅ System accepts (if business allows)
   - ❌ OR Error if future dates not allowed

---

### **1.4. Test Concurrency & Data Integrity**

#### **Test 1.4.1: Concurrent Edit (2 Users)**

**Setup**: Open 2 browser windows, both logged in as Admin
**Steps**:

1. **Window 1**: Edit student HV001, change Name to "User1 Edit"
2. **Window 2**: Alt+Tab to Window 2, open same student HV001
3. **Window 2**: Change Name to "User2 Edit"
4. **Window 1**: Click Save
5. **Window 2**: Click Save
6. **Expected**:
   - ✅ Last save wins (last-write-wins pattern)
   - OR ❌ Concurrency exception handled gracefully
   - Data consistent in database

---

#### **Test 1.4.2: Delete While Editing**

**Setup**: 2 browser windows
**Steps**:

1. **Window 1**: Click Edit on student HV001
2. **Window 2**: Search, find HV001, click Delete and confirm
3. **Window 1**: Try to Save
4. **Expected**:
   - ❌ Error: Record not found
   - ✅ User informed clearly

---

### **1.5. Test UI/UX Elements**

#### **Test 1.5.1: Form Responsiveness**

**Devices**: Desktop (1920px), Tablet (768px), Mobile (375px)

1. Test form layout across devices
2. **Expected**:
   - ✅ Form readable and usable on all sizes
   - ✅ Buttons easily clickable
   - ✅ Labels properly aligned

---

#### **Test 1.5.2: Error Message Display**

1. Trigger validation error
2. **Expected**:
   - ✅ Clear, red error messages
   - ✅ Error highlighting problematic fields
   - ✅ Suggestions for correction

---

---

## **2. SCORE MANAGEMENT (KetQua) - TEST SUITE**

### **2.1. Test CRUD Operations**

#### **Test 2.1.1: Create Score (Add Score)**

**Precondition**:

- User logged in (Admin/Teacher/Staff)
- Students and Courses exist in system

**Steps**:

1. Navigate to: `/KetQuas/Index`
2. Click "+ Add Score" button
3. Fill form:
   - Student: Select "John Michael Doe (HV001)"
   - Course: Select "IELTS 6.5+"
   - Listening Score: `85`
   - Reading Score: `90`
4. Click "Create" button
5. **Expected Results**:
   - ✅ Redirect to Score List
   - ✅ New score appears in list
   - ✅ Total Score auto-calculated: 85 + 90 = 175
   - ✅ Stats update (Total Records, Average Score)

**Test Data Sets**:
| Listening | Reading | Total | Notes |
|-----------|---------|-------|-------|
| 50 | 60 | 110 | ✅ Valid |
| 100 | 100 | 200 | ✅ Perfect scores |
| 0 | 0 | 0 | ✅ Minimum |
| 75 | 85 | 160 | ✅ Normal |

---

#### **Test 2.1.2: Edit Score**

**Precondition**: Score record exists
**Steps**:

1. Go to `/KetQuas/Index`
2. Find a score row, click "Edit" button
3. Change values:
   - Listening: 85 → 92
   - Reading: 90 → 87
4. Click "Edit" or "Save" button
5. **Expected Results**:
   - ✅ Redirect to Score List
   - ✅ New values display in list
   - ✅ Total Score recalculated: 92 + 87 = 179
   - ✅ Only Listening & Reading editable (Total is read-only)

**Edit Edge Cases**:
| From | To | Expected |
|------|----|----|
| 85 | 92 | ✅ Update |
| 50 | 0 | ✅ Set to 0 |
| 75 | 100 | ✅ Max value |
| (existing) | (same) | ✅ No change, still save |

---

#### **Test 2.1.3: View Score Details**

**Steps**:

1. Go to `/KetQuas/Index`
2. Click "View" button on any score record
3. **Expected Results**:
   - ✅ Details page displays:
     - Student name & ID
     - Course name
     - Listening, Reading scores
     - Total Score (auto-calculated)
   - ✅ Data matches the row
   - ✅ Edit, Delete buttons available

---

#### **Test 2.1.4: Delete Score**

**Steps**:

1. Go to `/KetQuas/Index`
2. Click "Delete" button on any score record
3. Verify confirmation page and warning
4. Click "Delete" to confirm
5. **Expected Results**:
   - ✅ Record removed from list
   - ✅ Redirect to Score List
   - ✅ Stats updated (Total Records decreases)

---

#### **Test 2.1.5: Search by Student**

**Steps**:

1. Go to `/KetQuas/Index`
2. In search box, type: `"John"` or `"HV001"`
3. Click "Search" button
4. **Expected**:
   - ✅ Only scores for students matching name/ID appear
   - ✅ Case-insensitive search works
   - ✅ Partial name matches work (search "Joh" finds "John")

**Search Test Cases**:
| Search Term | Expected Results |
|------------|------------------|
| "John" | All scores for students named John |
| "HV001" | All scores for student HV001 |
| "Nguyễn" | Diacritics handled correctly |
| "" (empty) | All scores |
| "NonExistent" | No results, empty list |

---

#### **Test 2.1.6: Filter by Course**

**Steps**:

1. Go to `/KetQuas/Index`
2. **Filter Dropdown**: Select "IELTS 6.5+"
3. **Expected**:
   - ✅ Only scores for IELTS course appear
   - ✅ Stats recalculate for filtered set
4. Change filter to "TOEFL 500+"
5. **Expected**:
   - ✅ List updates to show only TOEFL scores
6. **Filter to**: "All Courses" (default)
7. **Expected**:
   - ✅ All scores reappear

---

#### **Test 2.1.7: Combined Search + Filter**

**Steps**:

1. Go to `/KetQuas/Index`
2. Enter search: `"John"`
3. Select filter: `"IELTS 6.5+"`
4. Click "Search"
5. **Expected**:
   - ✅ Only John's IELTS scores appear
   - ✅ Both search and filter applied simultaneously

---

### **2.2. Test Statistics & Calculations**

#### **Test 2.2.1: Total Records Count**

**Steps**:

1. Count visible records in table
2. Compare with "Total Records" stat card
3. **Expected**: ✅ Stat card count == actual record count

**After operations**:

- Create new score → Total Records +1
- Delete score → Total Records -1

---

#### **Test 2.2.2: Average Score Calculation**

**Setup**: Scores with known totals
| Student | Total |
|---------|-------|
| John | 175 |
| Jane | 180 |
| Mike | 160 |

**Expected Average**: (175 + 180 + 160) / 3 = 171.67

**Steps**:

1. Go to `/KetQuas/Index`
2. Check "Average Score" stat card
3. **Expected**: ✅ Shows correct average (171.67 or 171.7)

---

#### **Test 2.2.3: Total Score Auto-Calculation**

**Steps**:

1. Create score: Listening 75, Reading 85
2. Total Score field should show: 160
3. Check database directly to verify computed column working
4. **Expected**: ✅ Total = Listening + Reading

---

#### **Test 2.2.4: Stats Update on Filter**

**Steps**:

1. View all scores — note Total Records: 90
2. Filter by course "IELTS"
3. **Expected**: ✅ Total Records updates to IELTS count only
4. Average Score recalculates for filtered set
5. Remove filter
6. **Expected**: ✅ Stats return to full dataset

---

### **2.3. Test Authorization & Security**

#### **Test 2.3.1: Admin Access**

**Steps**:

1. Login as: Admin
2. Go to: `/KetQuas/Index`
3. **Expected**:
   - ✅ Full list visible
   - ✅ All CRUD buttons available
   - ✅ Can create, edit, delete scores

---

#### **Test 2.3.2: Teacher (Giáo Viên) Access**

**Steps**:

1. Login as: Teacher
2. Go to: `/KetQuas/Index`
3. **Expected**:
   - ✅ Same access as Admin
   - ✅ Can create scores (for grade entry)
   - ✅ Can edit and delete scores
   - ✅ Can see all students' scores

---

#### **Test 2.3.3: Staff (Nhân Viên) Access**

**Steps**:

1. Login as: Staff
2. Go to: `/KetQuas/Index`
3. **Expected**:
   - ✅ Same access as Admin & Teacher
   - ✅ Full CRUD capability

---

#### **Test 2.3.4: Student (Học Viên) Access (DENIED)**

**Steps**:

1. Login as: Student
2. Try to access: `/KetQuas/Index`
3. **Expected**:
   - ❌ Access Denied (403)
   - ✅ Cannot see other students' scores
   - ✅ Cannot edit/delete scores
   - Note: Students might have separate view for own scores

---

#### **Test 2.3.5: Anonymous User (NOT LOGGED IN)**

**Steps**:

1. Logout
2. Try to access: `/KetQuas/Index`
3. **Expected**:
   - ✅ Redirect to login page
   - ✅ Cannot view any score data

---

### **2.4. Test Data Validation**

#### **Test 2.4.1: Score Range Validation**

| Score     | Value | Expected                              |
| --------- | ----- | ------------------------------------- |
| Listening | -5    | ❌ Error: Min 0                       |
| Listening | 101   | ❌ Error: Max 100                     |
| Reading   | 0     | ✅ Valid (minimum)                    |
| Reading   | 100   | ✅ Valid (maximum)                    |
| Reading   | 50.5  | ✅ Accept decimal OR ❌ Integers only |

**Steps**:

1. Try to create score with Listening: `-10`
2. **Expected**: ❌ Validation error message
3. Try with Reading: `150`
4. **Expected**: ❌ Validation error: "Score must be 0-100"

---

#### **Test 2.4.2: Composite Key Validation (Student + Course)**

**Precondition**: Student HV001 already has score for IELTS course

**Steps**:

1. Try to create duplicate: Student HV001 + Course IELTS
2. **Expected**:
   - ❌ Error: Duplicate key constraint
   - ✅ Record not saved

**Test Valid Composite Keys**:
| Student | Course | Expected |
|---------|--------|----------|
| HV001 | IELTS | ✅ Create |
| HV001 | TOEFL | ✅ Create (different course) |
| HV002 | IELTS | ✅ Create (different student) |
| HV001 | IELTS (again) | ❌ Duplicate |

---

#### **Test 2.4.3: Required Fields**

**Steps**:

1. Try to create score with Student = empty
2. **Expected**: ❌ Error: "Student required"
3. Try with Course = empty
4. **Expected**: ❌ Error: "Course required"
5. Try with Listening = empty (if required)
6. **Expected**: ❌ OR ✅ (depends on business logic)

---

#### **Test 2.4.4: Data Type Validation**

| Field           | Invalid Input | Expected                          |
| --------------- | ------------- | --------------------------------- |
| Listening Score | "abc"         | ❌ Error or converts to 0         |
| Reading Score   | "@#$"         | ❌ Error                          |
| Score           | "100.99"      | ✅ Accept decimal OR integer only |

---

### **2.5. Test Concurrency & Integrity**

#### **Test 2.5.1: Concurrent Score Edit (AsNoTracking)**

**Setup**: 2 browser windows, both logged in

**Steps**:

1. **Window 1**: Edit score HV001/IELTS, change Listening to 92
2. **Window 2**: Open same score HV001/IELTS in edit form
3. **Window 1**: Click Save
4. **Window 2**: Change Reading to 87, click Save
5. **Expected**:
   - ✅ Both saves succeed (no tracking conflict due to AsNoTracking)
   - ✅ Final data: Listening=92, Reading=87

---

#### **Test 2.5.2: Delete During Edit**

**Steps**:

1. **Window 1**: Click Edit on score HV001/IELTS
2. **Window 2**: Delete score HV001/IELTS
3. **Window 1**: Try to Save
4. **Expected**:
   - ❌ Error: Record not found
   - ✅ User informed clearly

---

### **2.6. Test Score Sidebar/Dashboard**

#### **Test 2.6.1: Auto-Update Score in Details**

**Steps**:

1. Open Details for a score
2. Check if sidebar shows:
   - [ ] Listening Score: 85
   - [ ] Reading Score: 90
   - [ ] Total Score: 175 (auto-calculated)
3. **Expected**: ✅ All values display correctly

---

#### **Test 2.6.2: Details Page Edit Integration**

**Steps**:

1. View score details
2. Update Listening: 85 → 95
3. Sidebar Total should update: 175 → 185
4. **Expected**: ✅ Real-time calculation

---

### **2.7. Test UI/UX**

#### **Test 2.7.1: Table Responsiveness**

| Device  | Width  | Expected                         |
| ------- | ------ | -------------------------------- |
| Desktop | 1920px | ✅ All columns visible           |
| Tablet  | 768px  | ✅ Columns stack or scroll       |
| Mobile  | 375px  | ✅ Table scrollable horizontally |

---

#### **Test 2.7.2: Filter Dropdown Behavior**

**Steps**:

1. Go to `/KetQuas/Index`
2. Click Filter dropdown
3. **Expected**:
   - ✅ All courses listed
   - ✅ Currently selected highlighted
   - ✅ "All Courses" option available

---

#### **Test 2.7.3: Search Input Behavior**

**Steps**:

1. Type in search box: "J"
2. **Expected**: ✅ Suggestion or live search (if implemented)
3. Type more: "Jo"
4. Continue typing: "John"
5. **Expected**: ✅ Results refine as you type (or on button click)

---

---

## **3. CROSS-MODULE TESTS**

### **Test 3.1: Student-Score Relationship**

**Precondition**: Student HV001 exists
**Steps**:

1. Create score for HV001
2. Go to Student Details for HV001
3. **Expected**:
   - ✅ Score records displayed/linked (if implemented)
   - ✅ No orphaned records

---

### **Test 3.2: Delete Student with Scores**

**Precondition**: Student HV001 has 3 score records
**Steps**:

1. Go to HocViens, find HV001
2. Click Delete
3. **Expected**:
   - ❌ Error: "Cannot delete, has score records"
   - OR ✅ Cascade delete with warning
   - Verify behavior matches requirements

---

### **Test 3.3: Course-Score Relationship**

**Precondition**: Course IELTS has scores
**Steps**:

1. Filter scores by IELTS
2. Count records
3. Go to Course Management (if exists)
4. Check if course shows linked score count
5. **Expected**: ✅ Consistent data

---

---

## **4. PERFORMANCE TESTS**

### **Test 4.1: Page Load Time**

| Page           | Expected Load Time |
| -------------- | ------------------ |
| HocViens Index | < 2 seconds        |
| KetQuas Index  | < 2 seconds        |
| Create Form    | < 1 second         |
| Edit Form      | < 1 second         |

**Method**: Use browser DevTools > Performance tab

---

### **Test 4.2: Search Performance**

| Scenario                      | Data Size | Expected Speed |
| ----------------------------- | --------- | -------------- |
| Search in 100 records         | 100       | < 500ms        |
| Search in 1000 records        | 1000      | < 1 second     |
| Filter 1000 records by course | 1000      | < 500ms        |

---

### **Test 4.3: Pagination Performance**

**Steps**:

1. Go to page 1 (10 records)
2. Click page 10 (with 1000 total records)
3. **Expected**: ✅ Load < 1 second

---

---

## **5. SECURITY TESTS**

### **Test 5.1: SQL Injection Prevention**

**Steps**:

1. Go to Search box
2. Enter: `'; DROP TABLE HocViens; --`
3. Click Search
4. **Expected**:
   - ✅ No error, treated as literal string
   - ✅ Table still exists (not dropped)
   - ✅ Returns empty or matching results

---

### **Test 5.2: XSS (Cross-Site Scripting) Prevention**

**Steps**:

1. Create student with name: `<script>alert('XSS')</script>`
2. **Expected**:
   - ✅ Script NOT executed
   - ✅ Displayed as text: `<script>alert('XSS')</script>`
   - ✅ HTML escaped properly

---

### **Test 5.3: CSRF Token**

**Steps**:

1. Check form source code
2. **Expected**: ✅ CSRF token present in form

```html
<input type="hidden" name="__RequestVerificationToken" value="..." />
```

---

### **Test 5.4: Authorization Bypass**

**Steps**:

1. Logout
2. Manually type URL: `/HocViens/Delete/HV001`
3. **Expected**:
   - ✅ Redirect to login, cannot bypass auth
   - ✅ Cannot delete student without authorization

---

---

## **6. BROWSER COMPATIBILITY**

### **Test 6.1: Cross-Browser Testing**

| Browser | Version | Status  |
| ------- | ------- | ------- |
| Chrome  | Latest  | ✅ Test |
| Firefox | Latest  | ✅ Test |
| Edge    | Latest  | ✅ Test |
| Safari  | Latest  | ✅ Test |

**Test**: Check for layout issues, console errors, missing features

---

### **Test 6.2: Mobile Browser**

| Browser | Device  | Status  |
| ------- | ------- | ------- |
| Chrome  | Android | ✅ Test |
| Safari  | iOS     | ✅ Test |

---

---

## **7. DATA INTEGRITY TESTS**

### **Test 7.1: Database Consistency**

**Steps**:

1. Create student HV001
2. Create score for HV001
3. Check database directly (SQL Server Management Studio)
4. **Expected**: ✅ Data exists in tables
   - HocViens table has HV001
   - KetQuas table has record with MaHocVien=HV001

---

### **Test 7.2: Transaction Rollback**

**Steps**:

1. Create student (fills form)
2. Network error occurs or server timeout
3. **Expected**:
   - ✅ Record not partially saved
   - ✅ All or nothing transaction

---

---

## **8. EDGE CASES & STRESS TESTS**

### **Test 8.1: Large Dataset**

**Precondition**: 10,000+ student records, 50,000+ score records
**Steps**:

1. Load Student Index
2. Search for specific student
3. Apply pagination
4. **Expected**: ✅ System handles large data gracefully

---

### **Test 8.2: Empty Database**

**Precondition**: No students, no scores
**Steps**:

1. Go to HocViens Index
2. **Expected**:
   - ✅ Page loads without error
   - ✅ Shows "No records found" or similar
   - ✅ "Add Student" button still available

---

### **Test 8.3: Null/Empty Values**

| Field             | Null Value | Expected             |
| ----------------- | ---------- | -------------------- |
| Phone             | NULL/empty | ✅ Acceptable        |
| Manager           | NULL       | ❌ Error if required |
| Registration Date | NULL       | ❌ Error if required |

---

### **Test 8.4: Special Characters in Name**

**Names to test**:

- `Nguyễn Văn Á` (Vietnamese diacritics) ✅
- `José María` (Spanish) ✅
- `O'Brien` (Apostrophe) ✅
- `Mary-Jane` (Hyphen) ✅
- `李明` (Chinese) ✅
- `محمد` (Arabic) ✅

**Expected**: ✅ All display and save correctly

---

### **Test 8.5: Maximum String Lengths**

| Field      | Max Length | Test           |
| ---------- | ---------- | -------------- |
| Full Name  | 255        | Send 500 chars |
| Phone      | 20         | Send 50 digits |
| Student ID | 50         | Send 100 chars |

**Expected**: ✅ Truncation or validation error

---

---

## **9. AJAX/MODAL TESTS** (If implemented)

### **Test 9.1: AJAX Add Student (Modal)**

**Steps**:

1. Click "+ Add Student"
2. If modal appears (AJAX):
   - Fill form in modal
   - Click "Add"
3. **Expected**:
   - ✅ Modal closes
   - ✅ New record appears in list without page reload
   - ✅ JSON response successful

---

### **Test 9.2: AJAX Add Score (Modal)**

**Steps**:

1. Click "+ Add Score"
2. Fill modal form
3. Click "Add"
4. **Expected**:
   - ✅ Modal closes
   - ✅ Score appears in table
   - ✅ Stats updated immediately

---

---

## **10. REGRESSION TESTS**

### **After Each Code Change, Verify**:

- [ ] All CRUD operations still work
- [ ] Search/Filter functional
- [ ] Pagination works
- [ ] Authorization enforced
- [ ] Validation messages display
- [ ] No console errors (F12 DevTools)
- [ ] Database changes persist
- [ ] Stats calculate correctly

---

---

## **✅ FINAL CHECKLIST**

### **Functionality**:

- [ ] Create Student ✅
- [ ] Edit Student ✅
- [ ] Delete Student ✅
- [ ] View Student ✅
- [ ] Search Students ✅
- [ ] Create Score ✅
- [ ] Edit Score ✅
- [ ] Delete Score ✅
- [ ] View Score Details ✅
- [ ] Search Scores ✅
- [ ] Filter Scores by Course ✅
- [ ] Stats Display & Calculate ✅

### **Security**:

- [ ] SQL Injection prevented ✅
- [ ] XSS prevented ✅
- [ ] CSRF token present ✅
- [ ] Authorization enforced ✅
- [ ] No access without login ✅

### **UI/UX**:

- [ ] Responsive design works ✅
- [ ] Error messages clear ✅
- [ ] Buttons functional ✅
- [ ] Forms usable ✅
- [ ] No broken links ✅

### **Data Integrity**:

- [ ] Database changes persist ✅
- [ ] No orphaned records ✅
- [ ] Composite keys enforced ✅
- [ ] Validation works ✅

### **Performance**:

- [ ] Pages load < 2s ✅
- [ ] Search responds quickly ✅
- [ ] Pagination smooth ✅
- [ ] No lag on large datasets ✅

### **Browser Compatibility**:

- [ ] Chrome ✅
- [ ] Firefox ✅
- [ ] Edge ✅
- [ ] Safari ✅
- [ ] Mobile browsers ✅

---

---

## **🚀 DEPLOYMENT READINESS**

When all tests pass:

1. ✅ Code review completed
2. ✅ All test cases passed
3. ✅ No console errors
4. ✅ All features working
5. ✅ Database backup created
6. ✅ Ready to merge to main branch

**Status**: Ready for Production ✨

---

_Last Updated: April 13, 2026_  
_Module Owner: Minh_  
_Test Guide Version: 2.0 (English)_

---

### **1.2. Test Authorization**

#### **Test Without Login**

1. Logout from app
2. Try to access: `/HocViens/Index`
3. **Expected**: Redirect to login page

#### **Test Without Admin Role**

1. Login as Teacher/Giáo Viên
2. Try to access: `/HocViens/Index`
3. **Expected**: Access Denied page

#### **Test With Admin Role**

1. Login as Admin
2. Access: `/HocViens/Index`
3. **Expected**: Full access to all CRUD operations

---

### **1.3. Test Validation**

#### **Test Empty MaHocVien**

1. Go to Create page
2. Leave Mã học viên empty
3. Click "Thêm học viên"
4. **Expected**: Validation error message

#### **Test Duplicate MaHocVien**

1. Go to Create page
2. Enter existing MaHocVien (e.g., HV001)
3. Click "Thêm học viên"
4. **Expected**: Database error or duplicate key error

---

## **2. TEST ĐIỂM (KetQua)**

### **2.1. Test CRUD Operations**

#### **Test Create (Thêm Điểm)**

1. Navigate to: `/KetQuas/Create`
2. Fill form:
   - Học viên: Select from dropdown
   - Khóa học: Select from dropdown
   - Điểm Listening: `85`
   - Điểm Reading: `90`
3. Click "Thêm điểm"
4. **Expected**: Redirect to Index
   - DiemTong automatically = 85 + 90 = 175 ❌ (Wait, should be max 100 each)
   - Actually DiemTong = 175 which seems high, but formula is correct

#### **Test Search & Filter**

1. Go to `/KetQuas/Index`
2. **Test Search**:
   - Type student name or ID in search box
   - **Expected**: Only matching records
3. **Test Filter by Course**:
   - Select course from dropdown
   - **Expected**: Only records for that course

#### **Test Statistics**

1. Go to `/KetQuas/Index`
2. Check stat cards at top:
   - Tổng bản ghi: Count should match
   - Đã hoàn thành: Count should be records with DiemTong > 0
   - Điểm trung bình: Should calculate correctly

#### **Test Details**

1. Click "Xem" button on any score record
2. **Expected**: Full details displayed including student name and course

#### **Test Edit**

1. Click "Sửa" button
2. Change listening/reading score (e.g., 75 → 80)
3. Click "Cập nhật"
4. **Expected**: DiemTong recalculated and displayed

#### **Test Delete**

1. Click "Xóa" button
2. Confirm deletion with warning message
3. Click "Xóa"
4. **Expected**: Record removed from list

---

### **2.2. Test Validation**

#### **Test Score Range**

1. Go to Create page
2. Enter Listening: `-5`
3. **Expected**: Validation error "Điểm phải từ 0 đến 100"

#### **Test Score Range > 100**

1. Go to Create page
2. Enter Reading: `150`
3. **Expected**: Validation error

#### **Test Empty Fields**

1. Go to Create page
2. Leave Học viên empty
3. Click submit
4. **Expected**: Required field error

---

### **2.3. Test Authorization**

#### **Test Admin Access**

1. Login as Admin
2. Access `/KetQuas/Index`
3. **Expected**: Full access, can CRUD

#### **Test Teacher Access**

1. Login as Teacher/Giáo Viên
2. Access `/KetQuas/Index`
3. **Expected**: Full access, can CRUD

#### **Test Student Access**

1. Login as Student/Học Viên
2. Access `/KetQuas/Index`
3. **Expected**: Access Denied page

#### **Test No Login**

1. Logout
2. Try to access `/KetQuas/Index`
3. **Expected**: Redirect to login

---

### **2.4. Test Composite Key (2 Keys)**

#### **Test Same Student, Different Courses**

1. Create record: Student HV001, Course IELTS
   - Listening: 80, Reading: 75
2. Create another: Student HV001, Course TOEFL
   - Listening: 85, Reading: 90
3. **Expected**: Both records exist in database (different courses)

#### **Test Edit with Composite Key**

1. Edit: HV001 + IELTS record
2. Change listening to 85
3. Click update
4. **Expected**: Only IELTS record updated, TOEFL unchanged

#### **Test Delete with Composite Key**

1. Delete: HV001 + IELTS record
2. **Expected**: Only IELTS record deleted, TOEFL still exists

---

## **3. TEST RESPONSIVE DESIGN**

### **Desktop View (1920px)**

1. Open in full screen
2. Check:
   - [ ] Form layout proper
   - [ ] Table displays all columns
   - [ ] Stats grid shows 3 columns

### **Tablet View (768px)**

1. Resize browser to 768px width
2. Check:
   - [ ] Responsive form layout
   - [ ] Table scrollable if needed
   - [ ] Navigation accessible

### **Mobile View (375px)**

1. Resize browser to 375px width
2. Check:
   - [ ] Single column layout
   - [ ] Touch-friendly buttons
   - [ ] Form readable
   - [ ] Table scrollable

---

## **4. TEST EDGE CASES**

### **Test Null/Empty Values**

- [ ] Empty search string → show all records
- [ ] Filter dropdown "Tất cả khóa học" → show all courses
- [ ] Delete confirm page displays correctly

### **Test Concurrent Users**

1. Open 2 browser windows, login as Admin
2. In window 1, edit student HV001
3. In window 2, also edit same student HV001
4. **Expected**: Last save wins (or concurrency error handled)

### **Test Large Data**

1. If 1000+ students exist
2. Search should still be fast
3. Pagination should work smoothly

---

## **5. BROWSER COMPATIBILITY TEST**

- [ ] Chrome/Edge (Latest)
- [ ] Firefox (Latest)
- [ ] Safari (if Mac)
- [ ] Mobile browsers (iOS Safari, Chrome Mobile)

---

## **6. PERFORMANCE TEST**

- [ ] Index page loads < 2 seconds
- [ ] Search responds immediately
- [ ] Filter changes load < 1 second
- [ ] Create/Edit form loads < 1 second

---

## **7. SECURITY TEST**

### **Test SQL Injection**

1. Search: `'; DROP TABLE HocViens; --`
2. **Expected**: No error, treated as string

### **Test XSS (Cross-Site Scripting)**

1. Create student name: `<script>alert('xss')</script>`
2. **Expected**: Displayed as text, not executed

### **Test Authorization Bypass**

1. Try to access `/KetQuas/Index` without login by typing URL
2. **Expected**: Redirect to login

---

## **✅ CHECKLIST**

- [ ] All CRUD operations work
- [ ] Search & Filter functional
- [ ] Validation working
- [ ] Authorization enforced
- [ ] Responsive design verified
- [ ] No console errors
- [ ] No broken links
- [ ] Database changes persisted
- [ ] Statistics calculated correctly
- [ ] Composite key handled properly
- [ ] Build successful without errors

---

## **🚀 Ready for Production**

If all tests pass, module is ready to merge! ✅
