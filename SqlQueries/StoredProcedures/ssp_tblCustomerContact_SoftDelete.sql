-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_SoftDelete
-- Description: Soft delete a customer contact record (set IsActive = 0)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_SoftDelete]
    @Id INT,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if record exists
        IF EXISTS (SELECT 1 FROM [dbo].[tblCustomerContact] WHERE [Id] = @Id)
        BEGIN
            UPDATE [dbo].[tblCustomerContact]
            SET
                [IsActive] = 0,
                [UpdatedBy] = @UpdatedBy,
                [UpdatedDate] = GETDATE()
            WHERE [Id] = @Id

            -- Return updated record
            SELECT
                [Id], [CustomerId], [MobileNo], [Address], [Email],
                [City], [State], [PhoneNumber], [IsActive],
                [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate],
                'Record soft deleted successfully' AS Message,
                1 AS Success
            FROM [dbo].[tblCustomerContact]
            WHERE [Id] = @Id
        END
        ELSE
        BEGIN
            -- Return not found message
            SELECT
                @Id AS Id,
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
-- EXEC ssp_tblCustomerContact_SoftDelete @Id = 1, @UpdatedBy = 1
