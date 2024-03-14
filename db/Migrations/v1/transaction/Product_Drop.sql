-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Product_Drop_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Product_Drop_vtr
(
    @ProductNo  INT,
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
    FROM Product
    WHERE ProductNo = @ProductNo;

    IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR (60005, -1, @State, @ProductNo);
        END
    IF @CurrentUpdatedDtm != @UpdatedDtm
        BEGIN
            DECLARE @UpdatedDtmString NVARCHAR(MAX) = CONVERT(NVARCHAR, @UpdatedDtm, 127);
            RAISERROR (60006, -1, @State, @ProductNo, @UpdatedDtmString);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE Product_Drop_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Product_Drop_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Product_Drop_tr
(
    @ProductNo  INT,
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
        EXEC Product_Drop_vtr @ProductNo, @UpdatedDtm;

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
        EXEC Product_Drop_vtr @ProductNo, @UpdatedDtm;

        -- Database updates --
        DECLARE @AuditedDtm DATETIMEOFFSET = SYSDATETIMEOFFSET();

        INSERT INTO ProductHistory (ProductNo, AuditedDtm, Name, Description, Price, UpdatedDtm, IsObsolete)
        SELECT ProductNo,
               @AuditedDtm AS AuditedDtm,
               Name,
               Description,
               Price,
               UpdatedDtm,
               IsObsolete
        FROM Product
        WHERE ProductNo = @ProductNo
          AND UpdatedDtm = @UpdatedDtm;

        UPDATE Product
        SET UpdatedDtm = @AuditedDtm,
            IsObsolete = 1
        WHERE ProductNo = @ProductNo
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
-- rollback DROP PROCEDURE Product_Drop_tr;
