# 🧪 Hướng Dẫn Test Module Học Viên & Điểm (Minh)

## 📋 Phạm Vi Test
- **Module 1**: Quản lý Học Viên (CRUD)
- **Module 2**: Quản lý Điểm (CRUD)
- **Ghi Chú**: Đã fix lỗi `GioiTinh` constraint và `DiemTong` computed column

---

## 🎯 PHẦN 1: MODULE HỌC VIÊN (HocViens)

### 1.1 Test CREATE - Thêm Học Viên Mới

#### Test Case 1: Thêm học viên hợp lệ (Nam)
```
Bước 1: Vào trang /HocViens/Create hoặc nhấn "Add Student"
Bước 2: Điền thông tin:
  - Student ID: HV001
  - Full Name: Nguyễn Văn A
  - Phone Number: 0912345678
  - Gender: Nam (Male) ← CHỌN từ dropdown
  - Registration Date: 01/01/2024
  - Staff In Charge: Chọn từ list
Bước 3: Nhấn "Add Student"
Kết quả mong đợi: ✅ Học viên được lưu, quay về trang Index
```

#### Test Case 2: Thêm học viên với Nữ
```
Giống Test Case 1 nhưng Gender chọn: Nữ (Female)
Kết quả mong đợi: ✅ Lưu thành công
```

#### Test Case 3: Không chọn Giới Tính
```
Bước 1: Điền đầy đủ thông tin nhưng để Gender trống
Bước 2: Nhấn "Add Student"
Kết quả mong đợi: ❌ Lỗi validation "Giới tính không hợp lệ"
```

#### Test Case 4: Mã học viên trùng lặp
```
Bước 1: Thêm học viên HV001 (nếu chưa có)
Bước 2: Thêm lại học viên cùng mã HV001
Kết quả mong đợi: ❌ Lỗi DB constraint (Primary Key violation)
```

#### Test Case 5: Auto-generate Mã Học Viên
```
Bước 1: Để trống Student ID
Bước 2: Điền đầy đủ thông tin khác
Bước 3: Nhấn "Add Student"
Kết quả mong đợi: ✅ Hệ thống tự sinh mã (HV + timestamp)
```

---

### 1.2 Test READ - Xem Danh Sách Học Viên

#### Test Case 1: Hiển thị danh sách phân trang
```
Bước 1: Vào trang /HocViens/Index
Kết quả mong đợi: ✅ Hiển thị tối đa 10 học viên/trang
```

#### Test Case 2: Tìm kiếm theo tên
```
Bước 1: Vào Index, điền "Nguyễn" vào ô search
Bước 2: Nhấn "Search"
Kết quả mong đợi: ✅ Hiển thị chỉ các HV có tên chứa "Nguyễn"
```

#### Test Case 3: Tìm kiếm theo mã
```
Bước 1: Vào Index, điền "HV001" vào ô search
Bước 2: Nhấn "Search"
Kết quả mong đợi: ✅ Hiển thị HV có mã "HV001"
```

#### Test Case 4: Xem chi tiết học viên
```
Bước 1: Vào Index
Bước 2: Nhấn icon "Eye" hoặc vào tên học viên
Kết quả mong đợi: ✅ Hiển thị trang Details với đầy đủ thông tin
```

---

### 1.3 Test UPDATE - Chỉnh Sửa Học Viên

#### Test Case 1: Chỉnh sửa tên học viên
```
Bước 1: Vào Index → Nhấn "Edit" hoặc vào tên HV
Bước 2: Sửa "Full Name": "Nguyễn Văn A" → "Nguyễn Văn B"
Bước 3: Nhấn "Save Changes"
Kết quả mong đợi: ✅ Tên cập nhật, quay về Index
```

#### Test Case 2: Chỉnh sửa giới tính
```
Bước 1: Vào Edit của một HV
Bước 2: Thay Gender: Nam → Nữ
Bước 3: Nhấn "Save Changes"
Kết quả mong đợi: ✅ Giới tính cập nhật (phải là dropdown)
```

#### Test Case 3: Chỉnh sửa số điện thoại
```
Bước 1: Vào Edit
Bước 2: Sửa Phone: "0912345678" → "0987654321"
Bước 3: Nhấn "Save Changes"
Kết quả mong đợi: ✅ Số điện thoại cập nhật
```

#### Test Case 4: Xóa thông tin bắt buộc
```
Bước 1: Vào Edit
Bước 2: Xóa "Full Name", để trống
Bước 3: Nhấn "Save Changes"
Kết quả mong đợi: ❌ Validation error (nếu field bắt buộc)
```

---

### 1.4 Test DELETE - Xóa Học Viên

#### Test Case 1: Xóa học viên thành công
```
Bước 1: Vào Index
Bước 2: Nhấn icon "Trash" hoặc vào "Delete"
Bước 3: Xác nhận "Yes, delete"
Kết quả mong đợi: ✅ HV bị xóa, quay về Index
```

#### Test Case 2: Xóa học viên có liên kết (Foreign Key)
```
Bước 1: Tìm học viên có điểm (KetQua) liên kết
Bước 2: Cố gắng xóa
Kết quả mong đợi: ❌ Lỗi FK constraint (nếu có cascade delete: ✅ xóa hết)
```

---

## 🎯 PHẦN 2: MODULE ĐIỂM (KetQuas)

### 2.1 Test CREATE - Thêm Điểm

#### Test Case 1: Thêm điểm hợp lệ
```
Bước 1: Vào /KetQuas/Index → Nhấn "Add Score"
Bước 2: Điền:
  - Student: HV001 (chọn dropdown)
  - Course: KH1 (chọn dropdown)
  - Listening Score: 200
  - Reading Score: 200
Bước 3: Quan sát "Total Score (Auto-calculated)" → Phải hiển thị 400
Bước 4: Nhấn "Add Score"
Kết quả mong đợi: ✅ Điểm lưu thành công, Total = 400 (từ DB computed column)
```

#### Test Case 2: Listening Score = 250, Reading Score = 250
```
Giống Test Case 1 nhưng điểm 250 + 250 = 500
Kết quả mong đợi: ✅ Lưu thành công, Total = 500
```

#### Test Case 3: Không nhập Listening Score
```
Bước 1: Chỉ nhập Reading Score = 100
Bước 2: Nhấn "Add Score"
Kết quả mong đợi: 
  - Total phải hiển thị 100 (0 + 100)
  - Lưu thành công
```

#### Test Case 4: Trùng khóa chính (Student + Course)
```
Bước 1: Thêm điểm cho HV001 + KH1 (nếu chưa có)
Bước 2: Cố gắng thêm lại HV001 + KH1 với điểm khác
Kết quả mong đợi: ❌ Lỗi Primary Key violation
```

#### Test Case 5: Modal AJAX (nếu có)
```
Bước 1: Vào Index → Nhấn "Add Score" từ modal
Bước 2: Điền thông tin và submit
Kết quả mong đợi: ✅ Thêm thành công, modal đóng, danh sách cập nhật
```

---

### 2.2 Test READ - Xem Danh Sách Điểm

#### Test Case 1: Hiển thị danh sách điểm
```
Bước 1: Vào /KetQuas/Index
Kết quả mong đợi: ✅ Hiển thị danh sách điểm với cột:
  - Student (Mã HV + Tên)
  - Course
  - Listening Score
  - Reading Score
  - Total Score (tính từ DB)
```

#### Test Case 2: Tìm kiếm theo tên/mã học viên
```
Bước 1: Nhập "Nguyễn" hoặc "HV001" vào ô search
Bước 2: Nhấn "Search"
Kết quả mong đợi: ✅ Hiển thị chỉ điểm của HV tìm được
```

#### Test Case 3: Lọc theo khóa học
```
Bước 1: Chọn khóa học từ dropdown filter
Bước 2: Nhấn "Filter"
Kết quả mong đợi: ✅ Hiển thị chỉ điểm của khóa học đó
```

#### Test Case 4: Xem chi tiết điểm
```
Bước 1: Nhấn icon "Eye" hoặc "View"
Kết quả mong đợi: ✅ Hiển thị trang Details với đầy đủ:
  - Student info
  - Course info
  - All scores
```

---

### 2.3 Test UPDATE - Chỉnh Sửa Điểm

#### Test Case 1: Chỉnh sửa Listening Score
```
Bước 1: Vào Edit điểm (HV001 + KH1)
Bước 2: Sửa Listening: 200 → 180
Bước 3: Quan sát Total Score (preview) → Phải thay đổi
Bước 4: Nhấn "Save" hoặc update
Kết quả mong đợi: ✅ Listening = 180, Total tính lại từ DB
```

#### Test Case 2: Chỉnh sửa Reading Score
```
Giống Test Case 1 nhưng sửa Reading Score
Kết quả mong đợi: ✅ Reading cập nhật, Total tính lại
```

#### Test Case 3: Cập nhật cả hai điểm
```
Bước 1: Sửa Listening: 200 → 100 AND Reading: 200 → 150
Bước 2: Nhấn save
Kết quả mong đợi: ✅ Cả hai cập nhật, Total = 250 (từ DB)
```

#### Test Case 4: Xóa điểm (đặt thành NULL/0)
```
Bước 1: Vào Edit
Bước 2: Xóa giá trị Listening (để trống)
Bước 3: Nhấn save
Kết quả mong đợi: ✅ Lưu thành công (NULL được phép)
  Total = 0 + Reading (tính từ DB)
```

---

### 2.4 Test DELETE - Xóa Điểm

#### Test Case 1: Xóa điểm thành công
```
Bước 1: Vào Index → Nhấn icon "Trash" hoặc "Delete"
Bước 2: Xác nhận "Yes, delete"
Kết quả mong đợi: ✅ Điểm bị xóa, quay về Index
```

#### Test Case 2: Xóa từ Details
```
Bước 1: Vào Details của một điểm
Bước 2: Nhấn "Delete"
Bước 3: Xác nhận
Kết quả mong đợi: ✅ Xóa thành công
```

---

## 🐛 TEST LỖI & EDGE CASES

### Test Lỗi Constraint GioiTinh
```
Scénario: Cố gắng nhập giới tính không hợp lệ
Cách Test:
  - Modify browser DevTools để gửi gender = "Unknown"
  - Hoặc sửa HTML input thành text
Kết quả mong đợi: ❌ Server reject với lỗi validation
  (NormalizeGender() sẽ trả về null)
```

### Test Computed Column DiemTong
```
Scénario: Kiểm tra DiemTong tính toán từ DB, không từ client
Cách Test:
  1. Thêm điểm Listening = 200, Reading = 200
  2. Kiểm tra SQL: SELECT * FROM KetQua WHERE MaHocVien = 'HV001'
  3. Verify: DiemTong = 400 (từ formula, không từ hidden input)
Kết quả mong đợi: ✅ DB tính chính xác, không phụ thuộc client
```

### Test Authorization
```
Scénario: Kiểm tra quyền truy cập
Test Case 1: Login as Admin → Có quyền CRUD HocVien + KetQua ✅
Test Case 2: Login as GiaoVien → Có quyền xem + thêm/sửa điểm ✅
Test Case 3: Login as Student → Không có quyền ❌ (Redirect 403)
```

---

## 📝 CHECKLIST TEST

### ✅ HocViens Module
- [ ] CREATE - Thêm mới với gender dropdown
- [ ] CREATE - Validation gender
- [ ] CREATE - Auto-generate mã
- [ ] READ - Danh sách phân trang
- [ ] READ - Tìm kiếm tên/mã
- [ ] READ - Xem chi tiết
- [ ] UPDATE - Sửa mọi field
- [ ] UPDATE - Validation error
- [ ] DELETE - Xóa thành công
- [ ] DELETE - FK constraint check

### ✅ KetQuas Module
- [ ] CREATE - Thêm điểm hợp lệ
- [ ] CREATE - Total Score preview (client)
- [ ] CREATE - Total Score từ DB (server)
- [ ] CREATE - Trùng khóa chính
- [ ] READ - Danh sách hiển thị
- [ ] READ - Tìm kiếm HV
- [ ] READ - Lọc khóa học
- [ ] UPDATE - Sửa Listening
- [ ] UPDATE - Sửa Reading
- [ ] UPDATE - Total Score tính lại
- [ ] DELETE - Xóa thành công

### ✅ Bug Fixes Verified
- [ ] GioiTinh constraint fixed (dropdown + server validation)
- [ ] DiemTong computed column working (DB tính, không client)
- [ ] No DiemTong hidden input sent to server

---

## 🚀 Cách Chạy Test

### Option 1: Manual Test (Recommended cho UI)
```powershell
# Terminal 1: Chạy app
cd F:\ASP.NET\Ltweb-ttan
dotnet run

# Trình duyệt: http://localhost:44349
# Test từng chức năng theo guide trên
```

### Option 2: Automated Test (Recommended cho Logic)
```powershell
# Terminal: Chạy unit test
cd F:\ASP.NET\Ltweb-ttan
dotnet test

# Hoặc từ Visual Studio: Test Explorer
```

### Option 3: Check Database
```sql
-- SQL Server
SELECT * FROM HocVien;
SELECT * FROM KetQua;

-- Verify Computed Column
SELECT MaHocVien, MaKhoaHoc, DiemListening, DiemReading, DiemTong
FROM KetQua
WHERE MaHocVien = 'HV001'
AND MaKhoaHoc = 'KH1';
-- DiemTong phải = DiemListening + DiemReading
```

---

## 📞 Contact & Support

Nếu gặp lỗi:
1. Kiểm tra **Output Window** → Build output
2. Kiểm tra **SQL Server Logs**
3. Kiểm tra **Browser Console** (F12 → Console)
4. Kiểm tra **Network Tab** (F12 → Network)

---

**Last Updated**: 2025-01-XX  
**Status**: ✅ Ready for Testing
