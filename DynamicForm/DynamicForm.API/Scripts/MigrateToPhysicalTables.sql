-- ============================================
-- MIGRATION SCRIPT: CHUYỂN SANG PHYSICAL TABLES
-- ============================================
-- Mục đích: Thêm Status, PublishedDate, PublishedBy vào FormVersions
-- Chạy script này trước khi deploy code mới

USE DynamicFormDb;
GO

-- 1. Add Status column (0=Draft, 1=Published, 2=Archived)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormVersions') AND name = 'Status')
BEGIN
    ALTER TABLE FormVersions
    ADD Status INT NOT NULL DEFAULT 0;
    
    PRINT 'Added Status column to FormVersions';
END
GO

-- 2. Add PublishedDate, PublishedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FormVersions') AND name = 'PublishedDate')
BEGIN
    ALTER TABLE FormVersions
    ADD PublishedDate DATETIME2 NULL,
        PublishedBy NVARCHAR(100) NULL;
    
    PRINT 'Added PublishedDate and PublishedBy columns to FormVersions';
END
GO

-- 3. Migrate existing data: IsActive = true → Status = 1 (Published)
UPDATE FormVersions
SET Status = 1,
    PublishedDate = COALESCE(ApprovedDate, CreatedDate),
    PublishedBy = COALESCE(ApprovedBy, CreatedBy)
WHERE IsActive = 1 AND Status = 0;

PRINT 'Migrated existing active versions to Published status';
GO

-- 4. Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FormVersions_Status' AND object_id = OBJECT_ID('FormVersions'))
BEGIN
    CREATE INDEX IX_FormVersions_Status ON FormVersions(Status);
    PRINT 'Created index IX_FormVersions_Status';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FormVersions_FormId_Status' AND object_id = OBJECT_ID('FormVersions'))
BEGIN
    CREATE INDEX IX_FormVersions_FormId_Status ON FormVersions(FormId, Status);
    PRINT 'Created index IX_FormVersions_FormId_Status';
END
GO

-- 5. Verify migration
SELECT 
    COUNT(*) as TotalVersions,
    SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) as Draft,
    SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) as Published,
    SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) as Archived
FROM FormVersions;

PRINT 'Migration completed successfully!';
GO
