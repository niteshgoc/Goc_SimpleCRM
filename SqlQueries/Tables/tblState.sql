-- =============================================
-- Table: tblState
-- Description: Master table for storing state information
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblState]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblState] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [StateName] VARCHAR(100) NOT NULL,
        [StateCode] VARCHAR(10) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedBy] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,
        [UpdatedDate] DATETIME NULL,

        CONSTRAINT [PK_tblState] PRIMARY KEY CLUSTERED ([Id] ASC)
    )

    PRINT 'Table tblState created successfully.'
END
ELSE
BEGIN
    PRINT 'Table tblState already exists.'
END
GO

-- Create index on StateName for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblState_StateName')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tblState_StateName]
    ON [dbo].[tblState] ([StateName])
    INCLUDE ([IsActive])
END
GO

-- =============================================
-- Sample Data: Insert Common Indian States
-- =============================================
IF NOT EXISTS (SELECT * FROM tblState)
BEGIN
    INSERT INTO tblState (StateName, StateCode, IsActive, CreatedBy)
    VALUES
        ('Maharashtra', 'MH', 1, 1),
        ('Gujarat', 'GJ', 1, 1),
        ('Karnataka', 'KA', 1, 1),
        ('Tamil Nadu', 'TN', 1, 1),
        ('Delhi', 'DL', 1, 1),
        ('Uttar Pradesh', 'UP', 1, 1),
        ('West Bengal', 'WB', 1, 1),
        ('Rajasthan', 'RJ', 1, 1),
        ('Madhya Pradesh', 'MP', 1, 1),
        ('Telangana', 'TS', 1, 1),
        ('Andhra Pradesh', 'AP', 1, 1),
        ('Kerala', 'KL', 1, 1),
        ('Punjab', 'PB', 1, 1),
        ('Haryana', 'HR', 1, 1),
        ('Bihar', 'BR', 1, 1),
        ('Odisha', 'OR', 1, 1),
        ('Jharkhand', 'JH', 1, 1),
        ('Assam', 'AS', 1, 1),
        ('Chhattisgarh', 'CG', 1, 1),
        ('Uttarakhand', 'UK', 1, 1)

    PRINT 'Sample state data inserted successfully.'
END
GO
