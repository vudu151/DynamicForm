-- =============================================
-- Script tạo tất cả các bảng cho Dynamic Form System
-- Sử dụng trong SQL Server Management Studio
-- =============================================

USE master;
GO

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DynamicFormDb')
BEGIN
    CREATE DATABASE DynamicFormDb;
    PRINT 'Database DynamicFormDb đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Database DynamicFormDb đã tồn tại.';
END
GO

USE DynamicFormDb;
GO

-- =============================================
-- Bảng FORMS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Forms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Forms] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Code] NVARCHAR(50) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Active, 2=Inactive
        [CurrentVersionId] UNIQUEIDENTIFIER NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        CONSTRAINT [UK_Forms_Code] UNIQUE ([Code])
    );

    CREATE INDEX [IX_Forms_Status] ON [dbo].[Forms]([Status]);
    CREATE INDEX [IX_Forms_CurrentVersionId] ON [dbo].[Forms]([CurrentVersionId]);
    
    PRINT 'Bảng Forms đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng Forms đã tồn tại.';
END
GO

-- =============================================
-- Bảng FORMVERSIONS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormVersions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormVersions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormId] UNIQUEIDENTIFIER NOT NULL,
        [Version] NVARCHAR(20) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ApprovedDate] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(100) NULL,
        [ChangeLog] NVARCHAR(1000) NULL,
        CONSTRAINT [FK_FormVersions_Forms] FOREIGN KEY ([FormId]) REFERENCES [dbo].[Forms]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UK_FormVersions_FormId_Version] UNIQUE ([FormId], [Version])
    );

    CREATE INDEX [IX_FormVersions_FormId] ON [dbo].[FormVersions]([FormId]);
    CREATE INDEX [IX_FormVersions_IsActive] ON [dbo].[FormVersions]([IsActive]);
    CREATE INDEX [IX_FormVersions_FormId_IsActive] ON [dbo].[FormVersions]([FormId], [IsActive]);
    
    PRINT 'Bảng FormVersions đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FormVersions đã tồn tại.';
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
    
    PRINT 'Foreign Key FK_Forms_CurrentVersionId đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Foreign Key FK_Forms_CurrentVersionId đã tồn tại.';
END
GO

-- =============================================
-- Bảng FORMFIELDS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormFields]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormFields] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormVersionId] UNIQUEIDENTIFIER NOT NULL,
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
    
    PRINT 'Bảng FormFields đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FormFields đã tồn tại.';
END
GO

-- =============================================
-- Bảng FIELDVALIDATIONS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldValidations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FieldValidations] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FieldId] UNIQUEIDENTIFIER NOT NULL,
        [RuleType] INT NOT NULL, -- 1=Required, 2=Min, 3=Max, 4=Range, 5=Regex, etc.
        [RuleValue] NVARCHAR(500) NULL,
        [ErrorMessage] NVARCHAR(500) NOT NULL DEFAULT '',
        [Priority] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_FieldValidations_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FieldValidations_FieldId] ON [dbo].[FieldValidations]([FieldId]);
    CREATE INDEX [IX_FieldValidations_Priority] ON [dbo].[FieldValidations]([FieldId], [Priority]);
    
    PRINT 'Bảng FieldValidations đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FieldValidations đã tồn tại.';
END
GO

-- =============================================
-- Bảng FIELDCONDITIONS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldConditions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FieldConditions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FieldId] UNIQUEIDENTIFIER NOT NULL,
        [ConditionType] INT NOT NULL, -- 1=Show, 2=Hide, 3=Enable, 4=Disable
        [Expression] NVARCHAR(1000) NOT NULL DEFAULT '', -- JSON expression
        [ActionsJson] NVARCHAR(MAX) NULL,
        [Priority] INT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_FieldConditions_FormFields] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FieldConditions_FieldId] ON [dbo].[FieldConditions]([FieldId]);
    
    PRINT 'Bảng FieldConditions đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FieldConditions đã tồn tại.';
END
GO

-- =============================================
-- Bảng FIELDOPTIONS
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
    
    PRINT 'Bảng FieldOptions đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FieldOptions đã tồn tại.';
END
GO

-- =============================================
-- Bảng FORMDATA
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormData] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormVersionId] UNIQUEIDENTIFIER NOT NULL,
        [ObjectId] NVARCHAR(100) NOT NULL,
        [ObjectType] NVARCHAR(50) NOT NULL,
        [DataJson] NVARCHAR(MAX) NOT NULL DEFAULT '{}',
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Submitted, 2=Approved
        CONSTRAINT [FK_FormData_FormVersions] FOREIGN KEY ([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_FormData_FormVersionId] ON [dbo].[FormData]([FormVersionId]);
    CREATE INDEX [IX_FormData_ObjectId_ObjectType] ON [dbo].[FormData]([ObjectId], [ObjectType]);
    CREATE INDEX [IX_FormData_CreatedDate] ON [dbo].[FormData]([CreatedDate]);
    
    PRINT 'Bảng FormData đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FormData đã tồn tại.';
END
GO

-- =============================================
-- Bảng FORMDATAHISTORY
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormDataHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormDataHistory] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormDataId] UNIQUEIDENTIFIER NOT NULL,
        [DataJson] NVARCHAR(MAX) NOT NULL DEFAULT '{}',
        [ChangedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ChangedBy] NVARCHAR(100) NOT NULL DEFAULT '',
        [ChangeReason] NVARCHAR(500) NULL,
        CONSTRAINT [FK_FormDataHistory_FormData] FOREIGN KEY ([FormDataId]) REFERENCES [dbo].[FormData]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_FormDataHistory_FormDataId] ON [dbo].[FormDataHistory]([FormDataId]);
    CREATE INDEX [IX_FormDataHistory_ChangedDate] ON [dbo].[FormDataHistory]([ChangedDate]);
    
    PRINT 'Bảng FormDataHistory đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FormDataHistory đã tồn tại.';
END
GO

-- =============================================
-- Bảng FORMPERMISSIONS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FormPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FormPermissions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FormId] UNIQUEIDENTIFIER NOT NULL,
        [RoleCode] NVARCHAR(50) NOT NULL,
        [PermissionType] INT NOT NULL, -- 1=Form, 2=Field
        [CanView] BIT NOT NULL DEFAULT 1,
        [CanEdit] BIT NOT NULL DEFAULT 0,
        [CanDelete] BIT NOT NULL DEFAULT 0,
        [CanApprove] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_FormPermissions_Forms] FOREIGN KEY ([FormId]) REFERENCES [dbo].[Forms]([Id]) ON DELETE CASCADE,
        CONSTRAINT [UK_FormPermissions_FormId_RoleCode] UNIQUE ([FormId], [RoleCode])
    );

    CREATE INDEX [IX_FormPermissions_FormId] ON [dbo].[FormPermissions]([FormId]);
    
    PRINT 'Bảng FormPermissions đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng FormPermissions đã tồn tại.';
END
GO

-- =============================================
-- Hoàn tất
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT 'TẤT CẢ CÁC BẢNG ĐÃ ĐƯỢC TẠO THÀNH CÔNG!';
PRINT '=============================================';
PRINT '';
PRINT 'Danh sách các bảng đã tạo:';
PRINT '1. Forms';
PRINT '2. FormVersions';
PRINT '3. FormFields';
PRINT '4. FieldValidations';
PRINT '5. FieldConditions';
PRINT '6. FieldOptions';
PRINT '7. FormData';
PRINT '8. FormDataHistory';
PRINT '9. FormPermissions';
PRINT '';
GO
