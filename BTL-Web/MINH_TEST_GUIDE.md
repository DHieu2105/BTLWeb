# 🧪 HƯỚNG DẪN TEST MODULE HỌC VIÊN + ĐIỂM

## **1. TEST HỌC VIÊN (HocVien)**

### **1.1. Test CRUD Operations**

#### **Test Create (Thêm Học Viên)**
1. Navigate to: `/HocViens/Create`
2. Fill form:
   - Mã học viên: `HV999`
   - Họ và tên: `Nguyễn Văn Test`
   - SDT: `0123456789`
   - Ngày đăng ký: Select today
   - Giới tính: Select male/female
   - Nhân viên quản lý: Select from dropdown
3. Click "Thêm học viên"
4. **Expected**: Redirect to Index, new student appears in list

#### **Test Search**
1. Go to `/HocViens/Index`
2. Search box: Type `Nguyễn` or `HV999`
3. **Expected**: Only matching records displayed

#### **Test Pagination**
1. Go to `/HocViens/Index`
2. **Expected**: Max 10 items per page
3. Click next page if available

#### **Test Details**
1. From Index page, click "Xem" button on any row
2. **Expected**: Full student details displayed

#### **Test Edit**
1. From Index page, click "Sửa" button
2. Change any field (e.g., phone number)
3. Click "Cập nhật"
4. **Expected**: Changes saved and reflected in list

#### **Test Delete**
1. From Index page, click "Xóa" button
2. Confirm on delete page
3. Click "Xóa" button
4. **Expected**: Student removed from list

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

