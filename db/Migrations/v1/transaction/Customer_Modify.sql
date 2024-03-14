-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer_Modify_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Customer_Modify_vtr
(
    @CustomerNo INT,
    @UpdatedDtm DATETIMEOFFSET,
    @FirstName  VARCHAR(30),
    @LastName   VARCHAR(30)
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
    IF EXISTS
        (
            SELECT 1
            FROM Customer
            WHERE FirstName = @FirstName
              AND LastName = @LastName
              AND CustomerNo != @CustomerNo
        )
        BEGIN
            RAISERROR (70004, -1, @State, @FirstName, @LastName);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE Customer_Modify_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer_Modify_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Customer_Modify_tr
(
    @CustomerNo  INT,
    @UpdatedDtm  DATETIMEOFFSET,
    @FirstName   VARCHAR(30),
    @LastName    VARCHAR(30),
    @DiscountPct DECIMAL(3, 2)
) AS
BEGIN
    DECLARE @ProcName SYSNAME = OBJECT_NAME(@@PROCID);

    ----------------------
    -- Validation block --
    ----------------------
    BEGIN TRY

        -- Transaction integrity check --
        EXEC Xact_Integrity_Check;

        -- Parameter checks --
        IF @FirstName IS NULL OR @FirstName = ''
            BEGIN
                RAISERROR (70001, -1 , 1);
            END
        IF @LastName IS NULL OR @LastName = ''
            BEGIN
                RAISERROR (70001, -1 , 1);
            END
        IF @DiscountPct IS NULL OR @DiscountPct < 0 OR @DiscountPct > 1
            BEGIN
                RAISERROR (70003, -1 , 1);
            END

        -- Offline constraint validation (no locks held) --
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
        EXEC Customer_Modify_vtr @CustomerNo, @UpdatedDtm, @FirstName, @LastName;

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
        EXEC Customer_Modify_vtr @CustomerNo, @UpdatedDtm, @FirstName, @LastName;

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
        SET FirstName   = @FirstName,
            LastName    = @LastName,
            DiscountPct = @DiscountPct,
            UpdatedDtm  = @AuditedDtm
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
-- rollback DROP PROCEDURE Customer_Modify_tr;
