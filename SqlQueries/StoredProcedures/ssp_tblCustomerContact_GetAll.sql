-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_GetAll
-- Description: Retrieve all customer contact records (with optional filters)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_GetAll]
    @CustomerId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            cc.[Id],
            cc.[CustomerId],
            cc.[MobileNo],
            cc.[Address],
            cc.[Email],
            cc.[City],
            cc.[State],
            cc.[PhoneNumber],
            cc.[IsActive],
            cc.[CreatedBy],
            cc.[CreatedDate],
            cc.[UpdatedBy],
            cc.[UpdatedDate],
            -- Join with Customer table for additional context
            c.[CustomerName],
            c.[CustomerCode]
        FROM [dbo].[tblCustomerContact] cc
        LEFT JOIN [dbo].[tblCustomer] c ON cc.[CustomerId] = c.[Id]
        WHERE
            (@CustomerId IS NULL OR cc.[CustomerId] = @CustomerId)
            AND (@IsActive IS NULL OR cc.[IsActive] = @IsActive)
        ORDER BY cc.[CreatedDate] DESC
    END TRY
    BEGIN CATCH
        -- Return error information
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine
    END CATCH
END
GO

-- =============================================
-- Usage Examples:
-- =============================================
-- Get all contacts:
-- EXEC ssp_tblCustomerContact_GetAll

-- Get all active contacts:
-- EXEC ssp_tblCustomerContact_GetAll @IsActive = 1

-- Get all contacts for a specific customer:
-- EXEC ssp_tblCustomerContact_GetAll @CustomerId = 1

-- Get all active contacts for a specific customer:
-- EXEC ssp_tblCustomerContact_GetAll @CustomerId = 1, @IsActive = 1
