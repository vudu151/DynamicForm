-- =============================================
-- Script tạo Database cho Dynamic Form System
-- =============================================

USE master;
GO

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DynamicFormDb')
BEGIN
    CREATE DATABASE DynamicFormDb;
END
GO

USE DynamicFormDb;
GO

-- =============================================
-- Bảng FORM
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Forms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Forms] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Code] NVARCHAR(50) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] INT NOT NULL DEFAULT 0,
        [CurrentVersionId] UNIQUEIDENTIFIER NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        CONSTRAINT [UK_Forms_Code] UNIQUE ([Code])
    );

    CREATE INDEX [IX_Forms_Status] ON [dbo].[Forms]([Status]);
    CREATE INDEX [IX_Forms_CurrentVersionId] ON [dbo].[Forms]([CurrentVersionId]);
END
GO

-- =============================================
-- Bảng FORM_VERSION
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormVersions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormVersions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormId] UNIQUEIDENTIFIER NOT NULL,
        [Version] NVARCHAR(20) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [ApprovedDate] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(100) NULL,
        [ChangeLog] NVARCHAR(1000) NULL,
        CONSTRAINT [FK_FormVersions_Forms] FOREIGN KEY ([FormId]) REFERENCES [dbo].[Forms]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UK_FormVersions_FormId_Version] UNIQUE ([FormId], [Version])
    );

    CREATE INDEX [IX_FormVersions_FormId] ON [dbo].[FormVersions]([FormId]);
    CREATE INDEX [IX_FormVersions_IsActive] ON [dbo].[FormVersions]([IsActive]);
    CREATE INDEX [IX_FormVersions_FormId_IsActive] ON [dbo].[FormVersions]([FormId], [IsActive]);
END
GO

-- =============================================
-- Bảng FORM_FIELD
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormFields]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormFields] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormVersionId] UNIQUEIDENTIFIER NOT NULL,
        [FieldCode] NVARCHAR(50) NOT NULL,
        [FieldType] INT NOT NULL,
        [Label] NVARCHAR(200) NOT NULL,
        [DisplayOrder] INT NOT NULL,
        [IsRequired] BIT NOT NULL DEFAULT 0,
        [IsVisible] BIT NOT NULL DEFAULT 1,
        [DefaultValue] NVARCHAR(500) NULL,
        [Placeholder] NVARCHAR(200) NULL,
        [HelpText] NVARCHAR(500) NULL,
        [CssClass] NVARCHAR(200) NULL,
        [PropertiesJson] NVARCHAR(MAX) NULL,
        [ParentFieldId] UNIQUEIDENTIFIER NULL,
        [MinOccurs] INT NULL,
        [MaxOccurs] INT NULL,
        CONSTRAINT [FK_FormFields_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormFields_ParentField] FOREIGN KEY ([ParentFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UK_FormFields_FormVersionId_FieldCode] UNIQUE ([FormVersionId], [FieldCode])
    );

    CREATE INDEX [IX_FormFields_FormVersionId] ON [dbo].[FormFields]([FormVersionId]);
    CREATE INDEX [IX_FormFields_DisplayOrder] ON [dbo].[FormFields]([FormVersionId], [DisplayOrder]);
    CREATE INDEX [IX_FormFields_ParentFieldId] ON [dbo].[FormFields]([ParentFieldId]);
END
GO

-- =============================================
-- Bảng FIELD_VALIDATION
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldValidations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FieldValidations] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FieldId] UNIQUEIDENTIFIER NOT NULL,
        [RuleType] INT NOT NULL,
        [RuleValue] NVARCHAR(500) NULL,
        [ErrorMessage] NVARCHAR(500) NOT NULL,
        [Priority] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_FieldValidations_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FieldValidations_FieldId] ON [dbo].[FieldValidations]([FieldId]);
    CREATE INDEX [IX_FieldValidations_Priority] ON [dbo].[FieldValidations]([FieldId], [Priority]);
END
GO

-- =============================================
-- Bảng FIELD_CONDITION
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldConditions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FieldConditions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FieldId] UNIQUEIDENTIFIER NOT NULL,
        [ConditionType] INT NOT NULL,
        [Expression] NVARCHAR(1000) NOT NULL,
        [ActionsJson] NVARCHAR(MAX) NULL,
        [Priority] INT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_FieldConditions_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FieldConditions_FieldId] ON [dbo].[FieldConditions]([FieldId]);
END
GO

-- =============================================
-- Bảng FIELD_OPTION
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldOptions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FieldOptions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FieldId] UNIQUEIDENTIFIER NOT NULL,
        [Value] NVARCHAR(100) NOT NULL,
        [Label] NVARCHAR(200) NOT NULL,
        [DisplayOrder] INT NOT NULL,
        [IsDefault] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_FieldOptions_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FieldOptions_FieldId] ON [dbo].[FieldOptions]([FieldId]);
    CREATE INDEX [IX_FieldOptions_DisplayOrder] ON [dbo].[FieldOptions]([FieldId], [DisplayOrder]);
END
GO

-- =============================================
-- Bảng FORM_DATA
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormData] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormVersionId] UNIQUEIDENTIFIER NOT NULL,
        [ObjectId] NVARCHAR(100) NOT NULL,
        [ObjectType] NVARCHAR(50) NOT NULL,
        [DataJson] NVARCHAR(MAX) NOT NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        [Status] INT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_FormData_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_FormData_FormVersionId] ON [dbo].[FormData]([FormVersionId]);
    CREATE INDEX [IX_FormData_ObjectId] ON [dbo].[FormData]([ObjectId], [ObjectType]);
    CREATE INDEX [IX_FormData_CreatedDate] ON [dbo].[FormData]([CreatedDate]);
    CREATE INDEX [IX_FormData_CreatedBy] ON [dbo].[FormData]([CreatedBy]);
END
GO

-- =============================================
-- Bảng FORM_DATA_HISTORY
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormDataHistory] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormDataId] UNIQUEIDENTIFIER NOT NULL,
        [DataJson] NVARCHAR(MAX) NOT NULL,
        [ChangedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ChangedBy] NVARCHAR(100) NOT NULL,
        [ChangeReason] NVARCHAR(500) NULL,
        CONSTRAINT [FK_FormDataHistory_FormData] FOREIGN KEY ([FormDataId]) REFERENCES [dbo].[FormData]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FormDataHistory_FormDataId] ON [dbo].[FormDataHistory]([FormDataId]);
    CREATE INDEX [IX_FormDataHistory_ChangedDate] ON [dbo].[FormDataHistory]([ChangedDate]);
END
GO

-- =============================================
-- Bảng FORM_PERMISSION
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormPermissions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormId] UNIQUEIDENTIFIER NOT NULL,
        [RoleCode] NVARCHAR(50) NOT NULL,
        [PermissionType] INT NOT NULL,
        [CanView] BIT NOT NULL DEFAULT 1,
        [CanEdit] BIT NOT NULL DEFAULT 0,
        [CanDelete] BIT NOT NULL DEFAULT 0,
        [CanApprove] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_FormPermissions_Forms] FOREIGN KEY ([FormId]) REFERENCES [dbo].[Forms]([Id]) ON DELETE CASCADE,
        CONSTRAINT [UK_FormPermissions_FormId_RoleCode] UNIQUE ([FormId], [RoleCode])
    );

    CREATE INDEX [IX_FormPermissions_FormId] ON [dbo].[FormPermissions]([FormId]);
END
GO

-- =============================================
-- Foreign Key cho Forms.CurrentVersionId
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Forms_CurrentVersionId')
BEGIN
    ALTER TABLE [dbo].[Forms]
    ADD CONSTRAINT [FK_Forms_CurrentVersionId] 
    FOREIGN KEY ([CurrentVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE SET NULL;
END
GO

-- =============================================
-- Sample Data (Tùy chọn)
-- =============================================
-- Uncomment để thêm dữ liệu mẫu

/*
-- Sample Form
DECLARE @FormId UNIQUEIDENTIFIER = NEWID();
DECLARE @VersionId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Forms] ([Id], [Code], [Name], [Description], [Status], [CreatedBy])
VALUES (@FormId, 'PHIEU_KHAM', N'Phiếu Khám Bệnh', N'Phiếu khám bệnh chuẩn', 1, 'admin');

INSERT INTO [dbo].[FormVersions] ([Id], [FormId], [Version], [IsActive], [CreatedBy], [ApprovedDate], [ApprovedBy])
VALUES (@VersionId, @FormId, '1.0.0', 1, 'admin', GETUTCDATE(), 'admin');

UPDATE [dbo].[Forms] SET [CurrentVersionId] = @VersionId WHERE [Id] = @FormId;

-- Sample Fields
DECLARE @Field1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Field2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Field3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[FormFields] ([Id], [FormVersionId], [FieldCode], [FieldType], [Label], [DisplayOrder], [IsRequired], [IsVisible])
VALUES 
    (@Field1, @VersionId, 'HO_TEN', 1, N'Họ và Tên', 1, 1, 1),
    (@Field2, @VersionId, 'TUOI', 2, N'Tuổi', 2, 1, 1),
    (@Field3, @VersionId, 'GIOI_TINH', 6, N'Giới tính', 3, 1, 1);

-- Sample Options for GIOI_TINH
INSERT INTO [dbo].[FieldOptions] ([Id], [FieldId], [Value], [Label], [DisplayOrder], [IsDefault])
VALUES 
    (NEWID(), @Field3, 'Nam', N'Nam', 1, 0),
    (NEWID(), @Field3, 'Nu', N'Nữ', 2, 0);

-- Sample Validation
INSERT INTO [dbo].[FieldValidations] ([Id], [FieldId], [RuleType], [RuleValue], [ErrorMessage], [Priority])
VALUES 
    (NEWID(), @Field1, 1, NULL, N'Họ tên là bắt buộc', 0),
    (NEWID(), @Field2, 4, '{"min": 0, "max": 150}', N'Tuổi phải từ 0 đến 150', 0);
*/

PRINT 'Database schema created successfully!';
GO
