-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_GetById
-- Description: Retrieve a single customer contact record by ID
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_GetById]
    @Id INT
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
        WHERE cc.[Id] = @Id
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
-- Usage Example:
-- =============================================
-- EXEC ssp_tblCustomerContact_GetById @Id = 1
