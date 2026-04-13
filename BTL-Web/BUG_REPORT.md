# 🐛 BUG REPORT - Student & Score Management Module

**Date**: April 13, 2026  
**Tester**: Minh  
**Priority**: HIGH  
**Status**: Analysis Complete

---

## **ISSUES IDENTIFIED & FIXED**

### **🔴 ISSUE #1: Edit Score - Missing Max Value Validation**

**Location**: `Views/KetQuas/Edit.cshtml` (Line: Input fields)

**Description**:
Edit form for score has `min="0"` but **MISSING** `max="100"` attribute on input fields.

**Current Code**:

```html
<input asp-for="DiemListening" class="form-control" type="number" min="0" />
<input asp-for="DiemReading" class="form-control" type="number" min="0" />
```

**Issue**: User can enter 300, 500, 999 - completely invalid! Score should be 0-100 only.

**Evidence**: Edit Score page shows "Reading: 300", "Total: 600" (screenshot)

**Severity**: 🔴 CRITICAL - Data Validation Failed

**Fix**:

```html
<input
  asp-for="DiemListening"
  class="form-control"
  type="number"
  min="0"
  max="100"
/>
<input
  asp-for="DiemReading"
  class="form-control"
  type="number"
  min="0"
  max="100"
/>
```

---

### **🔴 ISSUE #2: Create Score - Total Score Shows 0 Instead of Sum**

**Location**: `Views/KetQuas/Create.cshtml` (Line: Total Score display)

**Description**:
When creating new score with Listening=85 and Reading=90, the Total Score display shows **0** instead of 175.

**Current Code**:

```html
<span id="DiemTong">0</span> / 200
```

This is hardcoded to 0 and never updates.

**Expected**:
Should show: `85 + 90 = 175 / 200`

**Root Cause**:
No JavaScript event listener to update `#DiemTong` when user inputs listening/reading scores.

**Severity**: 🔴 CRITICAL - UX Issue

**Status**: ✅ FIXED

- Added JavaScript calculation function for real-time total score update
- Integrated auto-calculation with input event listeners

---

### **🔴 ISSUE #3: Create Score - Missing JavaScript for Real-time Calculation**

**Location**: `Views/KetQuas/Create.cshtml` (Missing Scripts section)

**Description**:
Create form has no JavaScript to:

1. Listen to input change events
2. Calculate total = listening + reading
3. Update the display in real-time

**Edit form HAS this** (lines 66-95 in Edit.cshtml), but Create form DOES NOT.

**Severity**: 🔴 CRITICAL - Inconsistent UX

**Missing Code**:

```javascript
<script>
    $(document).ready(function () {
        function updateTotal() {
            let listening = parseInt($("#DiemListening").val()) || 0;
            let reading = parseInt($("#DiemReading").val()) || 0;
            let total = listening + reading;
            $("#DiemTong").text(total);
        }

        $("#DiemListening, #DiemReading").on("input", updateTotal);
    });
</script>
```

---

### **🟠 ISSUE #4: Backend Validation Missing for Score Range**

**Location**: `Controllers/KetQuasController.cs`

**Description**:
Neither Create nor Edit action validates that scores are within 0-100 range.

**Current**:

- Only checks if ModelState.IsValid
- No custom validation for score range

**Issue**:
If user disables JavaScript validation (F12 DevTools) or sends invalid data via API, system accepts scores like 300, 500, etc.

**Severity**: 🟠 HIGH - Security/Data Integrity

**Fix**: Add model validation in KetQua.cs or controller

**Code Location**: Need to add in KetQuasController.cs Create/Edit methods:

```csharp
if ((ketQua.DiemListening < 0 || ketQua.DiemListening > 100) ||
    (ketQua.DiemReading < 0 || ketQua.DiemReading > 100))
{
    ModelState.AddModelError("", "Scores must be between 0 and 100");
    // reload view...
}
```

---

### **🟠 ISSUE #5: Language Mixing - Sidebar Contains Vietnamese Text**

**Location**: Multiple views (Delete Student, Details pages show mixed UI)

**Description**:
UI has mixed English and Vietnamese:

- Main content: English ✅
- Sidebar/Help text: Contains Vietnamese ❌
  - Example: "LỰA CHỌN KHÁC" (Vietnamese)
  - "Khóa tài khoản" (Vietnamese)
  - "Lưu trữ" (Vietnamese)

**Severity**: 🟠 MEDIUM - UX Consistency

**Status**: ✅ FIXED

- Replaced "Khóa tài khoản" → "Lock Account"
- Replaced "Lưu trữ" → "Archive"
- Replaced "Lựa chọn khác" → "Alternative Options"
- Replaced all Vietnamese instructions with English equivalents
- Ensured consistent UI language across all views

**Fixed Files**:

- `Views/HocViens/Delete.cshtml` (Sidebar buttons)
- `Views/Auth/AccessDenied.cshtml` (All Vietnamese text)
- `Views/HocViens/Create.cshtml` (Notes section)
- `Views/KetQuas/Create.cshtml` (Score Scale label)

---

---

## **SUMMARY TABLE**

| Issue                     | Severity    | Type       | File                 | Status    |
| ------------------------- | ----------- | ---------- | -------------------- | --------- |
| Edit Score - No max="100" | 🔴 CRITICAL | Validation | Edit.cshtml          | NEEDS FIX |
| Create Score - Total = 0  | 🔴 CRITICAL | UX         | Create.cshtml        | NEEDS FIX |
| Missing JS calculation    | 🔴 CRITICAL | Feature    | Create.cshtml        | NEEDS FIX |
| No backend validation     | 🟠 HIGH     | Security   | KetQuasController.cs | NEEDS FIX |
| Language mixing           | 🟠 MEDIUM   | UX         | Multiple             | NEEDS FIX |

---

## **TESTING BREAKDOWN**

### **Test Case That Failed**:

**Test 2.1.1: Create Score**

- Input: Listening=85, Reading=90
- **Expected**: Total=175
- **Actual**: Total=0 ❌

### **Test Case That Failed**:

**Test 2.4.2: Score Range Validation**

- Input: Listening=300, Reading=300
- **Expected**: Error message
- **Actual**: Accepted & saved ❌

---

## **ACTION ITEMS**

### **PRIORITY 1 - CRITICAL (Fix Immediately)**:

- [ ] Add `max="100"` to Edit Score form inputs
- [ ] Add JavaScript calculation for Create Score form
- [ ] Add backend validation for score range (0-100)

### **PRIORITY 2 - HIGH**:

- [ ] Add server-side validation before saving to DB
- [ ] Test with and without JavaScript enabled

### **PRIORITY 3 - MEDIUM**:

- [ ] Remove all Vietnamese text from UI
- [ ] Ensure all UI elements are in English only

---

## **NEXT STEPS**

1. **Immediately Fix Issues #1, #2, #3** → Update Views
2. **Then Fix Issue #4** → Update Controller + Model
3. **Then Fix Issue #5** → Replace all Vietnamese text
4. **Re-test all test cases** from MINH_TEST_GUIDE.md
5. **Verify database** has no invalid data (300+ scores)

---

_Report Generated: April 13, 2026_  
_Analyzed by: GitHub Copilot_
