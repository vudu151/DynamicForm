-- ============================================
-- MIGRATION SCRIPT: CHUYỂN TỪ DataJson SANG FormDataValue
-- ============================================
-- Mục đích: 
-- 1. Tạo bảng FormDataValues để lưu từng giá trị field
-- 2. Xóa cột DataJson khỏi bảng FormData
-- Chạy script này sau khi deploy code mới

USE DynamicFormDb;
GO

-- 1. Tạo bảng FormDataValues
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataValues]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormDataValues] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormDataId] UNIQUEIDENTIFIER NOT NULL,
        [FormFieldId] UNIQUEIDENTIFIER NOT NULL,
        [FieldValue] NVARCHAR(4000) NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [SectionCode] NVARCHAR(50) NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        CONSTRAINT [FK_FormDataValues_FormData] FOREIGN KEY ([FormDataId]) REFERENCES [dbo].[FormData]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormDataValues_FormFields] FOREIGN KEY ([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_FormDataValues_FormDataId] ON [dbo].[FormDataValues]([FormDataId]);
    CREATE INDEX [IX_FormDataValues_FormFieldId] ON [dbo].[FormDataValues]([FormFieldId]);
    CREATE INDEX [IX_FormDataValues_FormDataId_FormFieldId_DisplayOrder] ON [dbo].[FormDataValues]([FormDataId], [FormFieldId], [DisplayOrder]);
    
    PRINT 'Created table FormDataValues';
END
ELSE
BEGIN
    PRINT 'Table FormDataValues already exists';
END
GO

-- 2. Migrate dữ liệu từ DataJson sang FormDataValues (nếu có dữ liệu cũ)
-- Lưu ý: Script này chỉ migrate nếu có dữ liệu trong DataJson
-- Nếu không có dữ liệu cũ, có thể bỏ qua bước này

DECLARE @hasData BIT = 0;
SELECT @hasData = 1 FROM [dbo].[FormData] WHERE [DataJson] IS NOT NULL AND [DataJson] != '{}' AND [DataJson] != '';

IF @hasData = 1
BEGIN
    PRINT 'Found existing data in DataJson. Starting migration...';
    PRINT 'NOTE: This is a complex migration. You may need to write custom script based on your data structure.';
    PRINT 'For now, old data will remain in DataJson column.';
    -- TODO: Implement migration logic based on your data structure
    -- This requires parsing JSON and creating FormDataValue records
END
ELSE
BEGIN
    PRINT 'No existing data in DataJson. Skipping data migration.';
END
GO

-- 3. Xóa cột DataJson (chỉ xóa nếu không còn dữ liệu quan trọng)
-- LƯU Ý: Chỉ chạy bước này sau khi đã migrate dữ liệu xong!

-- Uncomment dòng dưới để xóa cột DataJson:
-- ALTER TABLE [dbo].[FormData] DROP COLUMN [DataJson];
-- PRINT 'Dropped column DataJson from FormData';

PRINT 'Migration script completed!';
PRINT 'IMPORTANT: Review and migrate existing data before dropping DataJson column.';
GO
