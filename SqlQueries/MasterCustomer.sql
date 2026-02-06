-- =============================================
-- Master Customer Table and Stored Procedures
-- =============================================

-- =============================================
-- Create Customer Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customer]
    (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyName] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL,
        [Phone] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1
    )
END
GO

-- =============================================
-- Stored Procedure: Insert or Update Customer
-- Parameters: All columns except CreatedAt
-- Logic: If @Id = 0 then INSERT, else UPDATE
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ssp_Customer_InsertUpdate]
    @Id INT = 0,
    @CompanyName NVARCHAR(200),
    @Email NVARCHAR(255),
    @Phone NVARCHAR(50) = NULL,
    @Address NVARCHAR(500) = NULL,
    @City NVARCHAR(100) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            -- INSERT new customer
            INSERT INTO [dbo].[Customer]
            (
                [CompanyName],
                [Email],
                [Phone],
                [Address],
                [City],
                [IsActive]
            )
            VALUES
            (
                @CompanyName,
                @Email,
                @Phone,
                @Address,
                @City,
                @IsActive
            )

            -- Return the newly created Id
            SELECT SCOPE_IDENTITY() AS Id
        END
        ELSE
        BEGIN
            -- UPDATE existing customer
            UPDATE [dbo].[Customer]
            SET
                [CompanyName] = @CompanyName,
                [Email] = @Email,
                [Phone] = @Phone,
                [Address] = @Address,
                [City] = @City,
                [IsActive] = @IsActive
            WHERE [Id] = @Id

            -- Return the updated Id
            SELECT @Id AS Id
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
GO

-- =============================================
-- Stored Procedure: Get Active Customers
-- Returns: All active customers
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ssp_Customer_GetActive]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [CompanyName],
        [Email],
        [Phone],
        [Address],
        [City],
        [CreatedAt],
        [IsActive]
    FROM [dbo].[Customer]
    WHERE [IsActive] = 1
    ORDER BY [CompanyName]
END
GO

-- =============================================
-- Stored Procedure: Get Customer by Id
-- Parameters: @Id
-- Returns: Single customer record
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ssp_Customer_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [CompanyName],
        [Email],
        [Phone],
        [Address],
        [City],
        [CreatedAt],
        [IsActive]
    FROM [dbo].[Customer]
    WHERE [Id] = @Id
END
GO

-- =============================================
-- Stored Procedure: Get All Customers
-- Returns: All customers (active and inactive)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ssp_Customer_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [CompanyName],
        [Email],
        [Phone],
        [Address],
        [City],
        [CreatedAt],
        [IsActive]
    FROM [dbo].[Customer]
    ORDER BY [CompanyName]
END
GO

-- =============================================
-- Stored Procedure: Delete Customer by Id
-- Parameters: @Id
-- Logic: Physical delete (removes record)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ssp_Customer_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM [dbo].[Customer]
        WHERE [Id] = @Id

        -- Return rows affected
        SELECT @@ROWCOUNT AS RowsAffected
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
GO

-- =============================================
-- Stored Procedure: Soft Delete (Mark as Inactive)
-- Parameters: @Id
-- Logic: Sets IsActive = 0
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ssp_Customer_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE [dbo].[Customer]
        SET [IsActive] = 0
        WHERE [Id] = @Id

        -- Return rows affected
        SELECT @@ROWCOUNT AS RowsAffected
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
GO
