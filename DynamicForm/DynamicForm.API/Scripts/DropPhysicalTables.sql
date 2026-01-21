-- ============================================
-- SCRIPT: XÓA CÁC BẢNG PHYSICAL TABLES
-- ============================================
-- Mục đích: Xóa các bảng vật lý được generate tự động
-- Chạy script này sau khi deploy code mới

USE DynamicFormDb;
GO

-- 1. Drop FormFieldColumnMaps table
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormFieldColumnMaps]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[FormFieldColumnMaps];
    PRINT 'Dropped table FormFieldColumnMaps';
END
ELSE
BEGIN
    PRINT 'Table FormFieldColumnMaps does not exist';
END
GO

-- 2. Drop FormPhysicalTables table
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormPhysicalTables]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[FormPhysicalTables];
    PRINT 'Dropped table FormPhysicalTables';
END
ELSE
BEGIN
    PRINT 'Table FormPhysicalTables does not exist';
END
GO

-- 3. Drop all dynamically generated physical tables (FORM_*_V*)
DECLARE @sql NVARCHAR(MAX) = '';
DECLARE @tableName NVARCHAR(255);

DECLARE table_cursor CURSOR FOR
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME LIKE 'FORM_%_V%'
  AND TABLE_TYPE = 'BASE TABLE';

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = 'DROP TABLE [dbo].[' + @tableName + '];';
    EXEC sp_executesql @sql;
    PRINT 'Dropped table ' + @tableName;
    FETCH NEXT FROM table_cursor INTO @tableName;
END;

CLOSE table_cursor;
DEALLOCATE table_cursor;

PRINT 'Script completed successfully!';
GO
