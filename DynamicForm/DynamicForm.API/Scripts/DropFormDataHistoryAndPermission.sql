-- ============================================
-- SCRIPT: XÓA 2 BẢNG FormDataHistory VÀ FormPermissions
-- ============================================
-- Mục đích: Xóa 2 bảng không còn sử dụng
-- Chạy script này sau khi deploy code mới

USE DynamicFormDb;
GO

-- 1. Drop FormDataHistory table
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataHistory]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[FormDataHistory];
    PRINT 'Dropped table FormDataHistory';
END
ELSE
BEGIN
    PRINT 'Table FormDataHistory does not exist';
END
GO

-- 2. Drop FormPermissions table
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormPermissions]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[FormPermissions];
    PRINT 'Dropped table FormPermissions';
END
ELSE
BEGIN
    PRINT 'Table FormPermissions does not exist';
END
GO

PRINT 'Script completed successfully!';
GO
