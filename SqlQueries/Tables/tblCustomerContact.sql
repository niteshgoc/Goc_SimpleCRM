-- =============================================
-- Table: tblCustomerContact
-- Description: Stores customer contact information including phone, email, and address details
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblCustomerContact]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblCustomerContact] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CustomerId] INT NOT NULL,
        [MobileNo] VARCHAR(100) NULL,
        [Address] VARCHAR(100) NULL,
        [Email] VARCHAR(100) NULL,
        [City] VARCHAR(100) NULL,
        [State] INT NULL,
        [PhoneNumber] VARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedBy] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,
        [UpdatedDate] DATETIME NULL,

        CONSTRAINT [PK_tblCustomerContact] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_tblCustomerContact_Customer] FOREIGN KEY ([CustomerId])
            REFERENCES [dbo].[Customer]([Id]),
        CONSTRAINT [FK_tblCustomerContact_State] FOREIGN KEY ([State])
            REFERENCES [dbo].[tblState]([Id])
    )

    PRINT 'Table tblCustomerContact created successfully.'
END
ELSE
BEGIN
    PRINT 'Table tblCustomerContact already exists.'
END
GO

-- Create index on CustomerId for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblCustomerContact_CustomerId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tblCustomerContact_CustomerId]
    ON [dbo].[tblCustomerContact] ([CustomerId])
    INCLUDE ([IsActive])
END
GO

-- =============================================
-- ALTER TABLE: Change State from VARCHAR to INT
-- =============================================

-- Check if State column is VARCHAR
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'tblCustomerContact'
    AND COLUMN_NAME = 'State'
    AND DATA_TYPE = 'varchar'
)
BEGIN
    -- Step 1: Add new StateId column as INT
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'tblCustomerContact'
        AND COLUMN_NAME = 'StateId'
    )
    BEGIN
        ALTER TABLE [dbo].[tblCustomerContact]
        ADD [StateId] INT NULL

        PRINT 'StateId column added to tblCustomerContact.'
    END

    -- Step 2: Drop the old State VARCHAR column
    ALTER TABLE [dbo].[tblCustomerContact]
    DROP COLUMN [State]

    PRINT 'Old State (VARCHAR) column dropped from tblCustomerContact.'

    -- Step 3: Rename StateId to State
    EXEC sp_rename 'tblCustomerContact.StateId', 'State', 'COLUMN'

    PRINT 'StateId renamed to State in tblCustomerContact.'

    -- Step 4: Add foreign key constraint
    IF NOT EXISTS (
        SELECT * FROM sys.foreign_keys
        WHERE name = 'FK_tblCustomerContact_State'
    )
    BEGIN
        ALTER TABLE [dbo].[tblCustomerContact]
        ADD CONSTRAINT [FK_tblCustomerContact_State] FOREIGN KEY ([State])
            REFERENCES [dbo].[tblState]([Id])

        PRINT 'Foreign key constraint added to tblCustomerContact.State.'
    END
END
ELSE
BEGIN
    PRINT 'State column is already INT or does not exist.'
END
GO

-- =============================================
-- Usage Example:
-- =============================================
-- INSERT INTO tblCustomerContact (CustomerId, MobileNo, Address, Email, City, State, PhoneNumber, CreatedBy)
-- VALUES (1, '9876543210', '123 Main St', 'john@example.com', 'Mumbai', 1, '022-12345678', 1)
