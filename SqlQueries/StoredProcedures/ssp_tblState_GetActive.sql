-- =============================================
-- Stored Procedure: ssp_tblState_GetActive
-- Description: Retrieve all active states
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblState_GetActive]
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            [Id],
            [StateName],
            [StateCode],
            [IsActive],
            [CreatedBy],
            [CreatedDate],
            [UpdatedBy],
            [UpdatedDate]
        FROM [dbo].[tblState]
        WHERE [IsActive] = 1
        ORDER BY [StateName] ASC
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
-- EXEC ssp_tblState_GetActive
