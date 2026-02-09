-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_Delete
-- Description: Hard delete a customer contact record (permanent removal)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if record exists
        IF EXISTS (SELECT 1 FROM [dbo].[tblCustomerContact] WHERE [Id] = @Id)
        BEGIN
            DELETE FROM [dbo].[tblCustomerContact]
            WHERE [Id] = @Id

            -- Return success message
            SELECT
                @Id AS DeletedId,
                'Record deleted successfully' AS Message,
                1 AS Success
        END
        ELSE
        BEGIN
            -- Return not found message
            SELECT
                @Id AS DeletedId,
                'Record not found' AS Message,
                0 AS Success
        END
    END TRY
    BEGIN CATCH
        -- Return error information
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine,
            0 AS Success
    END CATCH
END
GO

-- =============================================
-- Usage Example:
-- =============================================
-- EXEC ssp_tblCustomerContact_Delete @Id = 1
