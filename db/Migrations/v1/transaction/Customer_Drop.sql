-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer_Drop_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Customer_Drop_vtr
(
    @CustomerNo INT,
    @UpdatedDtm DATETIMEOFFSET
) AS
BEGIN
    -- Error state initialization --
    DECLARE @State TINYINT = CASE WHEN @@TRANCOUNT = 0 THEN 1
                                  ELSE 2
                             END;

    -- Validation checks --
    DECLARE @CurrentUpdatedDtm DATETIMEOFFSET;
    SELECT @CurrentUpdatedDtm = UpdatedDtm
    FROM Customer
    WHERE CustomerNo = @CustomerNo;

    IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR (70005, -1, @State, @CustomerNo);
        END
    IF @CurrentUpdatedDtm != @UpdatedDtm
        BEGIN
            DECLARE @UpdatedDtmString NVARCHAR(MAX) = CONVERT(NVARCHAR, @UpdatedDtm, 127);
            RAISERROR (70006, -1, @State, @CustomerNo, @UpdatedDtmString);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE Customer_Drop_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer_Drop_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Customer_Drop_tr
(
    @CustomerNo INT,
    @UpdatedDtm DATETIMEOFFSET
) AS
BEGIN
    DECLARE @ProcName SYSNAME = OBJECT_NAME(@@PROCID);

    ----------------------
    -- Validation block --
    ----------------------
    BEGIN TRY

        -- Transaction integrity check --
        EXEC Xact_Integrity_Check;

        -- Offline constraint validation (no locks held) --
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
        EXEC Customer_Drop_vtr @CustomerNo, @UpdatedDtm;

    END TRY
    BEGIN CATCH
        THROW;
    END CATCH

    -------------------
    -- Execute block --
    -------------------
    BEGIN TRY
        BEGIN TRANSACTION @ProcName;
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

        -- Online constraint validation (holding locks) --
        EXEC Customer_Drop_vtr @CustomerNo, @UpdatedDtm;

        -- Database updates --
        DECLARE @AuditedDtm DATETIMEOFFSET = SYSDATETIMEOFFSET();

        INSERT INTO CustomerHistory (CustomerNo, AuditedDtm, FirstName, LastName, DiscountPct, UpdatedDtm, IsObsolete)
        SELECT CustomerNo,
               @AuditedDtm AS AuditedDtm,
               FirstName,
               LastName,
               DiscountPct,
               UpdatedDtm,
               IsObsolete
        FROM Customer
        WHERE CustomerNo = @CustomerNo
          AND UpdatedDtm = @UpdatedDtm;

        UPDATE Customer
        SET UpdatedDtm = @AuditedDtm,
            IsObsolete = 1
        WHERE CustomerNo = @CustomerNo
          AND UpdatedDtm = @UpdatedDtm;

        -- Commit --
        COMMIT TRANSACTION @ProcName;
        RETURN 0;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION @ProcName;
        THROW;
    END CATCH
END
GO
-- rollback DROP PROCEDURE Customer_Drop_tr;
