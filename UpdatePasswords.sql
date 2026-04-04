-- Setup tài khoản Admin
-- Chỉ cần INSERT hoặc UPDATE với password plaintext

-- Nếu admin chưa tồn tại thì tạo mới
IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE Username = 'admin')
BEGIN
    INSERT INTO TaiKhoan (Username, Password, Role)
    VALUES ('admin', '123', 'Admin');
END
ELSE
BEGIN
    UPDATE TaiKhoan 
    SET Password = '123', Role = 'Admin'
    WHERE Username = 'admin';
END


PRINT N'👤 Username: admin';
PRINT N'🔐 Password: 123';
