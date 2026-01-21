-- ============================================
-- MIGRATION SCRIPT: CHUYỂN SANG INT PK + GUID PublicId
-- ============================================
-- Mục đích:
-- 1. Thêm cột PublicId (GUID, unique, indexed) cho tất cả bảng
-- 2. Thay đổi Id từ GUID sang INT với IDENTITY
-- 3. Cập nhật foreign keys từ GUID sang INT
-- 4. Tạo bảng Submissions mới
-- 
-- LƯU Ý: Script này chỉ tạo cấu trúc mới, KHÔNG migrate dữ liệu cũ.
-- Nếu có dữ liệu cũ, cần script migrate riêng.

USE DynamicFormDb;
GO

-- ============================================
-- 1. TẠO BẢNG SUBMISSIONS
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Submissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Submissions] (
        [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [FormVersionId] INT NOT NULL,
        [ObjectId] NVARCHAR(100) NOT NULL,
        [ObjectType] NVARCHAR(50) NOT NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Submitted, 2=Approved
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        CONSTRAINT [FK_Submissions_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE NO ACTION
    );

    -- Indexes
    CREATE UNIQUE INDEX [IX_Submissions_PublicId] ON [dbo].[Submissions]([PublicId]);
    CREATE INDEX [IX_Submissions_ObjectId_ObjectType_FormVersionId] ON [dbo].[Submissions]([ObjectId], [ObjectType], [FormVersionId]);
    CREATE INDEX [IX_Submissions_FormVersionId] ON [dbo].[Submissions]([FormVersionId]);
    CREATE INDEX [IX_Submissions_CreatedDate] ON [dbo].[Submissions]([CreatedDate]);
    CREATE INDEX [IX_Submissions_Status] ON [dbo].[Submissions]([Status]);

    PRINT 'Created table Submissions';
END
ELSE
BEGIN
    PRINT 'Table Submissions already exists';
END
GO

-- ============================================
-- 2. THÊM CỘT PublicId CHO TẤT CẢ BẢNG
-- ============================================

-- Forms
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Forms') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[Forms] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_Forms_PublicId] ON [dbo].[Forms]([PublicId]);
    PRINT 'Added PublicId column to Forms';
END
GO

-- FormVersions
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormVersions') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[FormVersions] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_FormVersions_PublicId] ON [dbo].[FormVersions]([PublicId]);
    PRINT 'Added PublicId column to FormVersions';
END
GO

-- FormFields
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormFields') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[FormFields] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_FormFields_PublicId] ON [dbo].[FormFields]([PublicId]);
    PRINT 'Added PublicId column to FormFields';
END
GO

-- FieldValidations
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FieldValidations') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[FieldValidations] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_FieldValidations_PublicId] ON [dbo].[FieldValidations]([PublicId]);
    PRINT 'Added PublicId column to FieldValidations';
END
GO

-- FieldConditions
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FieldConditions') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[FieldConditions] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_FieldConditions_PublicId] ON [dbo].[FieldConditions]([PublicId]);
    PRINT 'Added PublicId column to FieldConditions';
END
GO

-- FieldOptions
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FieldOptions') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[FieldOptions] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_FieldOptions_PublicId] ON [dbo].[FieldOptions]([PublicId]);
    PRINT 'Added PublicId column to FieldOptions';
END
GO

-- FormDataValues
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'PublicId')
BEGIN
    ALTER TABLE [dbo].[FormDataValues] ADD [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    CREATE UNIQUE INDEX [IX_FormDataValues_PublicId] ON [dbo].[FormDataValues]([PublicId]);
    PRINT 'Added PublicId column to FormDataValues';
END
GO

-- ============================================
-- 3. CẬP NHẬT FormDataValues: SubmissionId
-- ============================================
-- LƯU Ý: Cần migrate dữ liệu từ SubmissionId (GUID) sang SubmissionId (INT) trước khi thay đổi kiểu dữ liệu.
-- Script này chỉ thêm FK constraint nếu SubmissionId đã là INT.

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'SubmissionId')
BEGIN
    -- Kiểm tra xem SubmissionId đã là INT chưa
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormDataValues') AND name = 'SubmissionId' AND system_type_id = 56) -- INT
    BEGIN
        -- Thêm FK constraint nếu chưa có
        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormDataValues_Submissions')
        BEGIN
            ALTER TABLE [dbo].[FormDataValues]
            ADD CONSTRAINT [FK_FormDataValues_Submissions] 
            FOREIGN KEY ([SubmissionId]) REFERENCES [dbo].[Submissions]([Id]) ON DELETE CASCADE;
            PRINT 'Added FK_FormDataValues_Submissions constraint';
        END
    END
    ELSE
    BEGIN
        PRINT 'WARNING: FormDataValues.SubmissionId is not INT. Need to migrate data first.';
    END
END
GO

PRINT 'Migration script completed!';
PRINT 'IMPORTANT: Review and migrate existing data before changing Id columns from GUID to INT.';
GO
