-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_InsertUpdate
-- Description: Insert or update customer contact information
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_InsertUpdate]
    @Id INT = 0,
    @CustomerId INT,
    @MobileNo VARCHAR(100) = NULL,
    @Address VARCHAR(100) = NULL,
    @Email VARCHAR(100) = NULL,
    @City VARCHAR(100) = NULL,
    @State VARCHAR(100) = NULL,
    @PhoneNumber VARCHAR(100) = NULL,
    @IsActive BIT = 1,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- UPDATE existing record
        IF @Id > 0
        BEGIN
            UPDATE [dbo].[tblCustomerContact]
            SET
                [CustomerId] = @CustomerId,
                [MobileNo] = @MobileNo,
                [Address] = @Address,
                [Email] = @Email,
                [City] = @City,
                [State] = @State,
                [PhoneNumber] = @PhoneNumber,
                [IsActive] = @IsActive,
                [UpdatedBy] = @UpdatedBy,
                [UpdatedDate] = GETDATE()
            WHERE [Id] = @Id

            -- Return updated record
            SELECT
                [Id], [CustomerId], [MobileNo], [Address], [Email],
                [City], [State], [PhoneNumber], [IsActive],
                [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
            FROM [dbo].[tblCustomerContact]
            WHERE [Id] = @Id
        END
        -- INSERT new record
        ELSE
        BEGIN
            INSERT INTO [dbo].[tblCustomerContact] (
                [CustomerId], [MobileNo], [Address], [Email],
                [City], [State], [PhoneNumber], [IsActive],
                [CreatedBy], [CreatedDate]
            )
            VALUES (
                @CustomerId, @MobileNo, @Address, @Email,
                @City, @State, @PhoneNumber, @IsActive,
                @CreatedBy, GETDATE()
            )

            -- Return newly inserted record
            SELECT
                [Id], [CustomerId], [MobileNo], [Address], [Email],
                [City], [State], [PhoneNumber], [IsActive],
                [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
            FROM [dbo].[tblCustomerContact]
            WHERE [Id] = SCOPE_IDENTITY()
        END
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
-- INSERT:
-- EXEC ssp_tblCustomerContact_InsertUpdate
--      @Id = 0, @CustomerId = 1, @MobileNo = '9876543210',
--      @Address = '123 Main St', @Email = 'john@example.com',
--      @City = 'Mumbai', @State = 'Maharashtra', @PhoneNumber = '022-12345678',
--      @CreatedBy = 1

-- UPDATE:
-- EXEC ssp_tblCustomerContact_InsertUpdate
--      @Id = 1, @CustomerId = 1, @MobileNo = '9876543211',
--      @Address = '456 New St', @Email = 'john.doe@example.com',
--      @UpdatedBy = 1
