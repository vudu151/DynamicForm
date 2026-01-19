-- =============================================
-- Fix Vietnamese text for sample form PHIEU_KHAM
-- Use when data was inserted with wrong codepage (e.g. shown as "KhÃ¡m Bá»‡nh")
-- =============================================

USE DynamicFormDb;
GO

DECLARE @FormId UNIQUEIDENTIFIER = (
    SELECT TOP 1 [Id]
    FROM [dbo].[Forms]
    WHERE [Code] = 'PHIEU_KHAM'
);

IF @FormId IS NULL
BEGIN
    PRINT 'Form PHIEU_KHAM not found. Nothing to fix.';
    RETURN;
END

-- Fix Form text
UPDATE [dbo].[Forms]
SET
    [Name] = N'Phiếu Khám Bệnh',
    [Description] = N'Phiếu khám bệnh chuẩn cho bệnh nhân'
WHERE [Id] = @FormId;

-- Fix Version changelog
UPDATE [dbo].[FormVersions]
SET [ChangeLog] = N'Version đầu tiên của phiếu khám bệnh'
WHERE [FormId] = @FormId AND [Version] = '1.0.0';

-- Fix Field labels / placeholders / help texts (all versions of this form)
UPDATE ff
SET
    [Label] = CASE ff.[FieldCode]
        WHEN 'HO_TEN' THEN N'Họ và Tên'
        WHEN 'TUOI' THEN N'Tuổi'
        WHEN 'GIOI_TINH' THEN N'Giới tính'
        WHEN 'NGAY_KHAM' THEN N'Ngày khám'
        WHEN 'HUYET_AP' THEN N'Huyết áp (mmHg)'
        WHEN 'CHAN_DOAN' THEN N'Chẩn đoán'
        ELSE ff.[Label]
    END,
    [Placeholder] = CASE ff.[FieldCode]
        WHEN 'HO_TEN' THEN N'Nhập họ và tên đầy đủ'
        WHEN 'TUOI' THEN N'Nhập tuổi'
        WHEN 'HUYET_AP' THEN N'Nhập huyết áp'
        WHEN 'CHAN_DOAN' THEN N'Nhập chẩn đoán'
        ELSE ff.[Placeholder]
    END,
    [HelpText] = CASE ff.[FieldCode]
        WHEN 'HO_TEN' THEN N'Họ và tên bệnh nhân'
        WHEN 'TUOI' THEN N'Tuổi của bệnh nhân'
        WHEN 'GIOI_TINH' THEN N'Giới tính của bệnh nhân'
        WHEN 'NGAY_KHAM' THEN N'Ngày khám bệnh'
        WHEN 'HUYET_AP' THEN N'Huyết áp tâm thu'
        WHEN 'CHAN_DOAN' THEN N'Chẩn đoán ban đầu'
        ELSE ff.[HelpText]
    END
FROM [dbo].[FormFields] ff
INNER JOIN [dbo].[FormVersions] v ON v.[Id] = ff.[FormVersionId]
WHERE v.[FormId] = @FormId
  AND ff.[FieldCode] IN ('HO_TEN','TUOI','GIOI_TINH','NGAY_KHAM','HUYET_AP','CHAN_DOAN');

-- Fix Options label for GIOI_TINH
UPDATE fo
SET [Label] = CASE fo.[Value]
    WHEN 'Nam' THEN N'Nam'
    WHEN 'Nu' THEN N'Nữ'
    WHEN 'Khac' THEN N'Khác'
    ELSE fo.[Label]
END
FROM [dbo].[FieldOptions] fo
INNER JOIN [dbo].[FormFields] ff ON ff.[Id] = fo.[FieldId]
INNER JOIN [dbo].[FormVersions] v ON v.[Id] = ff.[FormVersionId]
WHERE v.[FormId] = @FormId
  AND ff.[FieldCode] = 'GIOI_TINH';

-- Fix Validation messages
UPDATE fv
SET [ErrorMessage] = CASE
    WHEN fv.[RuleType] = 1 AND ff.[FieldCode] = 'HO_TEN' THEN N'Họ tên là bắt buộc'
    WHEN fv.[RuleType] = 1 AND ff.[FieldCode] = 'TUOI' THEN N'Tuổi là bắt buộc'
    WHEN fv.[RuleType] = 6 AND ff.[FieldCode] = 'TUOI' THEN N'Tuổi phải từ 0 đến 150'
    WHEN fv.[RuleType] = 1 AND ff.[FieldCode] = 'GIOI_TINH' THEN N'Giới tính là bắt buộc'
    WHEN fv.[RuleType] = 6 AND ff.[FieldCode] = 'HUYET_AP' THEN N'Huyết áp phải từ 50 đến 250 mmHg'
    ELSE fv.[ErrorMessage]
END
FROM [dbo].[FieldValidations] fv
INNER JOIN [dbo].[FormFields] ff ON ff.[Id] = fv.[FieldId]
INNER JOIN [dbo].[FormVersions] v ON v.[Id] = ff.[FormVersionId]
WHERE v.[FormId] = @FormId;

-- Fix sample FormData JSON (BN001)
UPDATE fd
SET [DataJson] = N'{"HO_TEN":"Nguyễn Văn A","TUOI":25,"GIOI_TINH":"Nam","NGAY_KHAM":"2024-01-15","HUYET_AP":120,"CHAN_DOAN":"Cảm cúm"}'
FROM [dbo].[FormData] fd
INNER JOIN [dbo].[FormVersions] v ON v.[Id] = fd.[FormVersionId]
WHERE v.[FormId] = @FormId
  AND fd.[ObjectId] = 'BN001'
  AND fd.[ObjectType] = 'BENH_NHAN';

PRINT 'Fixed Vietnamese text for PHIEU_KHAM.';
GO

