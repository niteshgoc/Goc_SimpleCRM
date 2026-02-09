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
        [State] VARCHAR(100) NULL,
        [PhoneNumber] VARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedBy] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,
        [UpdatedDate] DATETIME NULL,

        CONSTRAINT [PK_tblCustomerContact] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_tblCustomerContact_Customer] FOREIGN KEY ([CustomerId])
            REFERENCES [dbo].[tblCustomer]([Id])
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
-- Usage Example:
-- =============================================
-- INSERT INTO tblCustomerContact (CustomerId, MobileNo, Address, Email, City, State, PhoneNumber, CreatedBy)
-- VALUES (1, '9876543210', '123 Main St', 'john@example.com', 'Mumbai', 'Maharashtra', '022-12345678', 1)
