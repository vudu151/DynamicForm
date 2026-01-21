-- ============================================
-- SCRIPT: XÓA BẢNG FormData
-- ============================================
-- Mục đích: Xóa bảng FormData (dữ liệu là rác, không cần migrate)

USE DynamicFormDb;
GO

-- 1. Xóa foreign key constraints từ FormDataValues (nếu có)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormDataValues_FormData')
BEGIN
    ALTER TABLE [dbo].[FormDataValues] DROP CONSTRAINT [FK_FormDataValues_FormData];
    PRINT 'Dropped foreign key FK_FormDataValues_FormData';
END
GO

-- 2. Xóa bảng FormData
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormData]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[FormData];
    PRINT 'Dropped table FormData successfully!';
END
ELSE
BEGIN
    PRINT 'Table FormData does not exist.';
END
GO

PRINT 'Script completed!';
GO
