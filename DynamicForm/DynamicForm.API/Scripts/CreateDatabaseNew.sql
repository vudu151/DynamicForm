-- ============================================
-- SCRIPT: TẠO DATABASE MỚI "DatabaseNew" VÀ TẤT CẢ CÁC BẢNG
-- ============================================
-- Mục đích:
-- 1. Tạo database mới tên "DatabaseNew"
-- 2. Tạo tất cả 7 bảng với cấu trúc mới:
--    - Id: INT (IDENTITY, Primary Key)
--    - PublicId: GUID (UNIQUE, INDEXED) - Dùng cho public API
--    - Foreign Keys: INT (thay vì GUID)
-- 3. SubmissionId trong FormDataValues là INT tự quản lý (không có FK)
--
-- LƯU Ý: Script này sẽ tạo database mới từ đầu.

-- ============================================
-- PHẦN 1: TẠO DATABASE MỚI
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DatabaseNew')
BEGIN
    CREATE DATABASE [DatabaseNew];
    PRINT 'Created database DatabaseNew';
END
ELSE
BEGIN
    PRINT 'Database DatabaseNew already exists';
END
GO

USE [DatabaseNew];
GO

-- ============================================
-- PHẦN 2: XÓA CÁC BẢNG CŨ (NẾU CÓ)
-- ============================================

PRINT '========================================';
PRINT 'XÓA CÁC BẢNG CŨ (NẾU CÓ)';
PRINT '========================================';

-- Xóa Foreign Key constraints trước
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormDataValues_FormVersions')
    ALTER TABLE [dbo].[FormDataValues] DROP CONSTRAINT [FK_FormDataValues_FormVersions];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormDataValues_FormFields')
    ALTER TABLE [dbo].[FormDataValues] DROP CONSTRAINT [FK_FormDataValues_FormFields];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FieldOptions_FormFields')
    ALTER TABLE [dbo].[FieldOptions] DROP CONSTRAINT [FK_FieldOptions_FormFields];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FieldConditions_FormFields')
    ALTER TABLE [dbo].[FieldConditions] DROP CONSTRAINT [FK_FieldConditions_FormFields];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FieldValidations_FormFields')
    ALTER TABLE [dbo].[FieldValidations] DROP CONSTRAINT [FK_FieldValidations_FormFields];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormFields_FormVersions')
    ALTER TABLE [dbo].[FormFields] DROP CONSTRAINT [FK_FormFields_FormVersions];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormFields_ParentField')
    ALTER TABLE [dbo].[FormFields] DROP CONSTRAINT [FK_FormFields_ParentField];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FormVersions_Forms')
    ALTER TABLE [dbo].[FormVersions] DROP CONSTRAINT [FK_FormVersions_Forms];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Forms_CurrentVersion')
    ALTER TABLE [dbo].[Forms] DROP CONSTRAINT [FK_Forms_CurrentVersion];

-- Xóa các bảng
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataValues]') AND type in (N'U'))
    DROP TABLE [dbo].[FormDataValues];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldOptions]') AND type in (N'U'))
    DROP TABLE [dbo].[FieldOptions];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldConditions]') AND type in (N'U'))
    DROP TABLE [dbo].[FieldConditions];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldValidations]') AND type in (N'U'))
    DROP TABLE [dbo].[FieldValidations];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormFields]') AND type in (N'U'))
    DROP TABLE [dbo].[FormFields];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormVersions]') AND type in (N'U'))
    DROP TABLE [dbo].[FormVersions];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Forms]') AND type in (N'U'))
    DROP TABLE [dbo].[Forms];

-- Xóa các bảng cũ không còn dùng (nếu có)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormData]') AND type in (N'U'))
    DROP TABLE [dbo].[FormData];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataHistory]') AND type in (N'U'))
    DROP TABLE [dbo].[FormDataHistory];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormPermissions]') AND type in (N'U'))
    DROP TABLE [dbo].[FormPermissions];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormPhysicalTables]') AND type in (N'U'))
    DROP TABLE [dbo].[FormPhysicalTables];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormFieldColumnMap]') AND type in (N'U'))
    DROP TABLE [dbo].[FormFieldColumnMap];
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Submissions]') AND type in (N'U'))
    DROP TABLE [dbo].[Submissions];

PRINT 'Đã xóa tất cả các bảng cũ (nếu có)';
GO

-- ============================================
-- PHẦN 3: TẠO LẠI TẤT CẢ CÁC BẢNG MỚI
-- ============================================
-- Tạo theo thứ tự: Parent tables trước, Child tables sau

PRINT '========================================';
PRINT 'BẮT ĐẦU TẠO CÁC BẢNG MỚI';
PRINT '========================================';

-- ============================================
-- 1. BẢNG Forms
-- ============================================
CREATE TABLE [dbo].[Forms] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Code] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Active, 2=Inactive
    [CurrentVersionId] INT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL
);

-- Indexes cho Forms
CREATE UNIQUE INDEX [IX_Forms_PublicId] ON [dbo].[Forms]([PublicId]);
CREATE UNIQUE INDEX [IX_Forms_Code] ON [dbo].[Forms]([Code]);
CREATE INDEX [IX_Forms_Status] ON [dbo].[Forms]([Status]);

PRINT 'Created table Forms';
GO

-- ============================================
-- 2. BẢNG FormVersions
-- ============================================
CREATE TABLE [dbo].[FormVersions] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FormId] INT NOT NULL,
    [Version] NVARCHAR(20) NOT NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Published, 2=Archived
    [IsActive] BIT NOT NULL DEFAULT 0, -- Deprecated, kept for backward compatibility
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
    [PublishedDate] DATETIME2 NULL,
    [PublishedBy] NVARCHAR(100) NULL,
    [ApprovedDate] DATETIME2 NULL, -- Deprecated, kept for backward compatibility
    [ApprovedBy] NVARCHAR(100) NULL, -- Deprecated, kept for backward compatibility
    [ChangeLog] NVARCHAR(1000) NULL,
    CONSTRAINT [FK_FormVersions_Forms] FOREIGN KEY ([FormId]) REFERENCES [dbo].[Forms]([Id]) ON DELETE NO ACTION
);

-- Indexes cho FormVersions
CREATE UNIQUE INDEX [IX_FormVersions_PublicId] ON [dbo].[FormVersions]([PublicId]);
CREATE UNIQUE INDEX [IX_FormVersions_FormId_Version] ON [dbo].[FormVersions]([FormId], [Version]);
CREATE INDEX [IX_FormVersions_FormId_Status] ON [dbo].[FormVersions]([FormId], [Status]);
CREATE INDEX [IX_FormVersions_Status] ON [dbo].[FormVersions]([Status]);

-- Foreign Key: Forms.CurrentVersionId -> FormVersions.Id
ALTER TABLE [dbo].[Forms]
ADD CONSTRAINT [FK_Forms_CurrentVersion] 
FOREIGN KEY ([CurrentVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE SET NULL;

PRINT 'Created table FormVersions';
GO

-- ============================================
-- 3. BẢNG FormFields
-- ============================================
CREATE TABLE [dbo].[FormFields] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FormVersionId] INT NOT NULL,
    [FieldCode] NVARCHAR(50) NOT NULL,
    [FieldType] INT NOT NULL, -- 1=Text, 2=Number, 3=Date, 4=Select, etc.
    [Label] NVARCHAR(200) NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [IsRequired] BIT NOT NULL DEFAULT 0,
    [IsVisible] BIT NOT NULL DEFAULT 1,
    [DefaultValue] NVARCHAR(500) NULL,
    [Placeholder] NVARCHAR(200) NULL,
    [HelpText] NVARCHAR(500) NULL,
    [CssClass] NVARCHAR(200) NULL,
    [PropertiesJson] NVARCHAR(MAX) NULL, -- JSON for dynamic properties
    [ParentFieldId] INT NULL, -- For nested/repeater fields
    [MinOccurs] INT NULL,
    [MaxOccurs] INT NULL,
    [SectionCode] NVARCHAR(50) NULL,
    CONSTRAINT [FK_FormFields_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FormFields_ParentField] FOREIGN KEY ([ParentFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE SET NULL
);

-- Indexes cho FormFields
CREATE UNIQUE INDEX [IX_FormFields_PublicId] ON [dbo].[FormFields]([PublicId]);
CREATE UNIQUE INDEX [IX_FormFields_FormVersionId_FieldCode] ON [dbo].[FormFields]([FormVersionId], [FieldCode]);
CREATE INDEX [IX_FormFields_FormVersionId_DisplayOrder] ON [dbo].[FormFields]([FormVersionId], [DisplayOrder]);

PRINT 'Created table FormFields';
GO

-- ============================================
-- 4. BẢNG FieldValidations
-- ============================================
CREATE TABLE [dbo].[FieldValidations] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FieldId] INT NOT NULL,
    [RuleType] INT NOT NULL, -- 1=Required, 2=Min, 3=Max, 4=Range, 5=Regex, etc.
    [RuleValue] NVARCHAR(500) NULL,
    [ErrorMessage] NVARCHAR(500) NOT NULL,
    [Priority] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_FieldValidations_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
);

-- Indexes cho FieldValidations
CREATE UNIQUE INDEX [IX_FieldValidations_PublicId] ON [dbo].[FieldValidations]([PublicId]);
CREATE INDEX [IX_FieldValidations_FieldId] ON [dbo].[FieldValidations]([FieldId]);

PRINT 'Created table FieldValidations';
GO

-- ============================================
-- 5. BẢNG FieldConditions
-- ============================================
CREATE TABLE [dbo].[FieldConditions] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FieldId] INT NOT NULL,
    [ConditionType] INT NOT NULL, -- 1=Show, 2=Hide, 3=Enable, 4=Disable
    [Expression] NVARCHAR(1000) NOT NULL, -- JSON expression
    [ActionsJson] NVARCHAR(MAX) NULL, -- JSON for actions
    [Priority] INT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_FieldConditions_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
);

-- Indexes cho FieldConditions
CREATE UNIQUE INDEX [IX_FieldConditions_PublicId] ON [dbo].[FieldConditions]([PublicId]);
CREATE INDEX [IX_FieldConditions_FieldId] ON [dbo].[FieldConditions]([FieldId]);

PRINT 'Created table FieldConditions';
GO

-- ============================================
-- 6. BẢNG FieldOptions
-- ============================================
CREATE TABLE [dbo].[FieldOptions] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FieldId] INT NOT NULL,
    [Value] NVARCHAR(100) NOT NULL,
    [Label] NVARCHAR(200) NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_FieldOptions_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
);

-- Indexes cho FieldOptions
CREATE UNIQUE INDEX [IX_FieldOptions_PublicId] ON [dbo].[FieldOptions]([PublicId]);
CREATE INDEX [IX_FieldOptions_FieldId_DisplayOrder] ON [dbo].[FieldOptions]([FieldId], [DisplayOrder]);

PRINT 'Created table FieldOptions';
GO

-- ============================================
-- 7. BẢNG FormDataValues
-- ============================================
CREATE TABLE [dbo].[FormDataValues] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [SubmissionId] INT NOT NULL, -- Tự quản lý, không có FK constraint
    [FormVersionId] INT NOT NULL,
    [FormFieldId] INT NOT NULL,
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
    -- Note: SubmissionId không có FK constraint, tự quản lý
    CONSTRAINT [FK_FormDataValues_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FormDataValues_FormFields] FOREIGN KEY ([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE NO ACTION
);

-- Indexes cho FormDataValues
CREATE UNIQUE INDEX [IX_FormDataValues_PublicId] ON [dbo].[FormDataValues]([PublicId]);
CREATE INDEX [IX_FormDataValues_SubmissionId] ON [dbo].[FormDataValues]([SubmissionId]);
CREATE INDEX [IX_FormDataValues_ObjectId_ObjectType_FormVersionId] ON [dbo].[FormDataValues]([ObjectId], [ObjectType], [FormVersionId]);
CREATE INDEX [IX_FormDataValues_FormVersionId] ON [dbo].[FormDataValues]([FormVersionId]);
CREATE INDEX [IX_FormDataValues_FormFieldId] ON [dbo].[FormDataValues]([FormFieldId]);
CREATE INDEX [IX_FormDataValues_SubmissionId_FormFieldId_DisplayOrder] ON [dbo].[FormDataValues]([SubmissionId], [FormFieldId], [DisplayOrder]);
CREATE INDEX [IX_FormDataValues_CreatedDate] ON [dbo].[FormDataValues]([CreatedDate]);
CREATE INDEX [IX_FormDataValues_Status] ON [dbo].[FormDataValues]([Status]);

PRINT 'Created table FormDataValues';
GO

-- ============================================
-- HOÀN TẤT
-- ============================================
PRINT '========================================';
PRINT 'HOÀN TẤT TẠO DATABASE VÀ CÁC BẢNG';
PRINT '========================================';
PRINT 'Database: DatabaseNew';
PRINT 'Đã tạo thành công 7 bảng:';
PRINT '  1. Forms';
PRINT '  2. FormVersions';
PRINT '  3. FormFields';
PRINT '  4. FieldValidations';
PRINT '  5. FieldConditions';
PRINT '  6. FieldOptions';
PRINT '  7. FormDataValues';
PRINT '';
PRINT 'Tất cả các bảng đều có:';
PRINT '  - Id: INT (IDENTITY, Primary Key)';
PRINT '  - PublicId: GUID (UNIQUE, INDEXED) - Dùng cho public API';
PRINT '  - Foreign Keys: INT (thay vì GUID)';
PRINT '';
PRINT 'SubmissionId trong FormDataValues:';
PRINT '  - INT tự quản lý (không có FK constraint)';
PRINT '  - Dùng để nhóm các FormDataValue của cùng 1 lần submit';
PRINT '';
PRINT 'Database đã sẵn sàng để sử dụng!';
GO
