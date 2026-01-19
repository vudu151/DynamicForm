-- =============================================
-- Script thêm dữ liệu mẫu cho Dynamic Form System
-- =============================================

USE DynamicFormDb;
GO

-- Xóa dữ liệu cũ nếu có (tùy chọn)
-- DELETE FROM [dbo].[FormDataHistory];
-- DELETE FROM [dbo].[FormData];
-- DELETE FROM [dbo].[FieldValidations];
-- DELETE FROM [dbo].[FieldConditions];
-- DELETE FROM [dbo].[FieldOptions];
-- DELETE FROM [dbo].[FormFields];
-- DELETE FROM [dbo].[FormVersions];
-- DELETE FROM [dbo].[FormPermissions];
-- DELETE FROM [dbo].[Forms];
-- GO

-- =============================================
-- Sample Form: PHIEU_KHAM
-- =============================================
DECLARE @FormId UNIQUEIDENTIFIER = NEWID();
DECLARE @VersionId UNIQUEIDENTIFIER = NEWID();

-- Tạo Form
INSERT INTO [dbo].[Forms] ([Id], [Code], [Name], [Description], [Status], [CreatedBy], [CreatedDate])
VALUES (@FormId, 'PHIEU_KHAM', N'Phiếu Khám Bệnh', N'Phiếu khám bệnh chuẩn cho bệnh nhân', 1, 'admin', GETUTCDATE());

-- Tạo Version
INSERT INTO [dbo].[FormVersions] ([Id], [FormId], [Version], [IsActive], [CreatedBy], [CreatedDate], [ApprovedDate], [ApprovedBy], [ChangeLog])
VALUES (@VersionId, @FormId, '1.0.0', 1, 'admin', GETUTCDATE(), GETUTCDATE(), 'admin', N'Version đầu tiên của phiếu khám bệnh');

-- Cập nhật CurrentVersionId
UPDATE [dbo].[Forms] SET [CurrentVersionId] = @VersionId WHERE [Id] = @FormId;

-- =============================================
-- Sample Fields
-- =============================================
DECLARE @FieldHoTen UNIQUEIDENTIFIER = NEWID();
DECLARE @FieldTuoi UNIQUEIDENTIFIER = NEWID();
DECLARE @FieldGioiTinh UNIQUEIDENTIFIER = NEWID();
DECLARE @FieldNgayKham UNIQUEIDENTIFIER = NEWID();
DECLARE @FieldHuyetAp UNIQUEIDENTIFIER = NEWID();
DECLARE @FieldChanDoan UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[FormFields] ([Id], [FormVersionId], [FieldCode], [FieldType], [Label], [DisplayOrder], [IsRequired], [IsVisible], [Placeholder], [HelpText])
VALUES 
    (@FieldHoTen, @VersionId, 'HO_TEN', 1, N'Họ và Tên', 1, 1, 1, N'Nhập họ và tên đầy đủ', N'Họ và tên bệnh nhân'),
    (@FieldTuoi, @VersionId, 'TUOI', 2, N'Tuổi', 2, 1, 1, N'Nhập tuổi', N'Tuổi của bệnh nhân'),
    (@FieldGioiTinh, @VersionId, 'GIOI_TINH', 6, N'Giới tính', 3, 1, 1, NULL, N'Giới tính của bệnh nhân'),
    (@FieldNgayKham, @VersionId, 'NGAY_KHAM', 3, N'Ngày khám', 4, 1, 1, NULL, N'Ngày khám bệnh'),
    (@FieldHuyetAp, @VersionId, 'HUYET_AP', 2, N'Huyết áp (mmHg)', 5, 0, 1, N'Nhập huyết áp', N'Huyết áp tâm thu'),
    (@FieldChanDoan, @VersionId, 'CHAN_DOAN', 10, N'Chẩn đoán', 6, 0, 1, N'Nhập chẩn đoán', N'Chẩn đoán ban đầu');

-- =============================================
-- Sample Options cho GIOI_TINH
-- =============================================
INSERT INTO [dbo].[FieldOptions] ([Id], [FieldId], [Value], [Label], [DisplayOrder], [IsDefault])
VALUES 
    (NEWID(), @FieldGioiTinh, 'Nam', N'Nam', 1, 0),
    (NEWID(), @FieldGioiTinh, 'Nu', N'Nữ', 2, 0),
    (NEWID(), @FieldGioiTinh, 'Khac', N'Khác', 3, 0);

-- =============================================
-- Sample Validations
-- =============================================
INSERT INTO [dbo].[FieldValidations] ([Id], [FieldId], [RuleType], [RuleValue], [ErrorMessage], [Priority], [IsActive])
VALUES 
    -- Required validation cho HO_TEN
    (NEWID(), @FieldHoTen, 1, NULL, N'Họ tên là bắt buộc', 0, 1),
    -- Required validation cho TUOI
    (NEWID(), @FieldTuoi, 1, NULL, N'Tuổi là bắt buộc', 0, 1),
    -- Range validation cho TUOI
    (NEWID(), @FieldTuoi, 6, '{"min": 0, "max": 150}', N'Tuổi phải từ 0 đến 150', 1, 1),
    -- Required validation cho GIOI_TINH
    (NEWID(), @FieldGioiTinh, 1, NULL, N'Giới tính là bắt buộc', 0, 1),
    -- Range validation cho HUYET_AP
    (NEWID(), @FieldHuyetAp, 6, '{"min": 50, "max": 250}', N'Huyết áp phải từ 50 đến 250 mmHg', 0, 1);

-- =============================================
-- Sample Form Data
-- =============================================
DECLARE @FormDataId UNIQUEIDENTIFIER = NEWID();
DECLARE @DataJson NVARCHAR(MAX) = '{"HO_TEN":"Nguyễn Văn A","TUOI":25,"GIOI_TINH":"Nam","NGAY_KHAM":"2024-01-15","HUYET_AP":120,"CHAN_DOAN":"Cảm cúm"}';

INSERT INTO [dbo].[FormData] ([Id], [FormVersionId], [ObjectId], [ObjectType], [DataJson], [CreatedBy], [CreatedDate], [Status])
VALUES (@FormDataId, @VersionId, 'BN001', 'BENH_NHAN', @DataJson, 'doctor1', GETUTCDATE(), 1);

PRINT 'Sample data inserted successfully!';
PRINT 'Form Code: PHIEU_KHAM';
PRINT 'Version: 1.0.0';
PRINT 'Form ID: ' + CAST(@FormId AS NVARCHAR(50));
PRINT 'Version ID: ' + CAST(@VersionId AS NVARCHAR(50));
GO
