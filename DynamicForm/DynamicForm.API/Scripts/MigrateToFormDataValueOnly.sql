-- ============================================
-- MIGRATION SCRIPT: BỎ BẢNG FormData, CHỈ DÙNG FormDataValue
-- ============================================
-- Mục đích: 
-- 1. Tạo bảng FormDataValues với đầy đủ trường (SubmissionId, ObjectId, ObjectType, Status, ...)
-- 2. Xóa bảng FormData
-- Chạy script này sau khi deploy code mới

USE DynamicFormDb;
GO

-- 1. Tạo bảng FormDataValues mới (nếu chưa có)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataValues]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormDataValues] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [SubmissionId] UNIQUEIDENTIFIER NOT NULL,
        [FormVersionId] UNIQUEIDENTIFIER NOT NULL,
        [FormFieldId] UNIQUEIDENTIFIER NOT NULL,
        [ObjectId] NVARCHAR(100) NOT NULL,
        [ObjectType] NVARCHAR(50) NOT NULL,
        [FieldValue] NVARCHAR(4000) NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [SectionCode] NVARCHAR(50) NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Submitted, 2=Approved
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        CONSTRAINT [FK_FormDataValues_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FormDataValues_FormFields] FOREIGN KEY ([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE NO ACTION
    );

    -- Indexes cho performance
    CREATE INDEX [IX_FormDataValues_SubmissionId] ON [dbo].[FormDataValues]([SubmissionId]);
    CREATE INDEX [IX_FormDataValues_ObjectId_ObjectType_FormVersionId] ON [dbo].[FormDataValues]([ObjectId], [ObjectType], [FormVersionId]);
    CREATE INDEX [IX_FormDataValues_FormVersionId] ON [dbo].[FormDataValues]([FormVersionId]);
    CREATE INDEX [IX_FormDataValues_FormFieldId] ON [dbo].[FormDataValues]([FormFieldId]);
    CREATE INDEX [IX_FormDataValues_SubmissionId_FormFieldId_DisplayOrder] ON [dbo].[FormDataValues]([SubmissionId], [FormFieldId], [DisplayOrder]);
    CREATE INDEX [IX_FormDataValues_CreatedDate] ON [dbo].[FormDataValues]([CreatedDate]);
    CREATE INDEX [IX_FormDataValues_Status] ON [dbo].[FormDataValues]([Status]);
    
    PRINT 'Created table FormDataValues';
END
ELSE
BEGIN
    -- Nếu bảng đã tồn tại, kiểm tra và thêm các cột mới nếu thiếu
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'SubmissionId')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] ADD [SubmissionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
        CREATE INDEX [IX_FormDataValues_SubmissionId] ON [dbo].[FormDataValues]([SubmissionId]);
        PRINT 'Added SubmissionId column to FormDataValues';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'FormVersionId')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] ADD [FormVersionId] UNIQUEIDENTIFIER NOT NULL;
        -- Cần set giá trị từ FormData trước khi add FK
        PRINT 'Added FormVersionId column to FormDataValues';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'ObjectId')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] ADD [ObjectId] NVARCHAR(100) NOT NULL DEFAULT '';
        PRINT 'Added ObjectId column to FormDataValues';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'ObjectType')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] ADD [ObjectType] NVARCHAR(50) NOT NULL DEFAULT '';
        PRINT 'Added ObjectType column to FormDataValues';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'Status')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] ADD [Status] INT NOT NULL DEFAULT 0;
        CREATE INDEX [IX_FormDataValues_Status] ON [dbo].[FormDataValues]([Status]);
        PRINT 'Added Status column to FormDataValues';
    END

    -- Xóa các cột cũ nếu có
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'FormDataId')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] DROP CONSTRAINT [FK_FormDataValues_FormData];
        ALTER TABLE [dbo].[FormDataValues] DROP COLUMN [FormDataId];
        PRINT 'Removed FormDataId column from FormDataValues';
    END
END
GO

-- 2. Migrate dữ liệu từ FormData + FormDataValues cũ (nếu có)
-- Lưu ý: Script này chỉ migrate nếu có dữ liệu cũ
-- Nếu không có dữ liệu cũ, có thể bỏ qua bước này

DECLARE @hasOldData BIT = 0;
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormData]') AND type in (N'U'))
BEGIN
    SELECT @hasOldData = 1 FROM [dbo].[FormData] WHERE EXISTS (SELECT 1 FROM [dbo].[FormDataValues] WHERE [FormDataValues].[FormDataId] = [FormData].[Id]);
END

IF @hasOldData = 1
BEGIN
    PRINT 'Found existing data. Starting migration...';
    PRINT 'NOTE: This migration requires mapping FormDataId to SubmissionId.';
    PRINT 'For now, old data migration logic needs to be implemented based on your data structure.';
    -- TODO: Implement migration logic:
    -- 1. Lấy tất cả FormData
    -- 2. Với mỗi FormData, tạo SubmissionId mới
    -- 3. Update FormDataValues: Set SubmissionId, FormVersionId, ObjectId, ObjectType, Status từ FormData
END
ELSE
BEGIN
    PRINT 'No existing data to migrate.';
END
GO

-- 3. Xóa bảng FormData (dữ liệu là rác, không cần migrate)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormData]') AND type in (N'U'))
BEGIN
    -- Xóa foreign key constraints từ FormDataValues (nếu có)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormDataValues_FormData')
    BEGIN
        ALTER TABLE [dbo].[FormDataValues] DROP CONSTRAINT [FK_FormDataValues_FormData];
        PRINT 'Dropped foreign key FK_FormDataValues_FormData';
    END
    
    DROP TABLE [dbo].[FormData];
    PRINT 'Dropped table FormData';
END
ELSE
BEGIN
    PRINT 'Table FormData does not exist.';
END

PRINT 'Migration script completed!';
GO
