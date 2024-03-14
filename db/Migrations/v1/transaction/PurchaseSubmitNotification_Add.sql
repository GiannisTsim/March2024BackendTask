-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseSubmitNotification_Add_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE PurchaseSubmitNotification_Add_vtr
(
    @CustomerNo  INT,
    @PurchaseDtm DATETIMEOFFSET
) AS
BEGIN
    -- Error state initialization --
    DECLARE @State TINYINT = CASE WHEN @@TRANCOUNT = 0 THEN 1
                                  ELSE 2
                             END;

    -- Validation checks --
    DECLARE @PurchaseDtmString NVARCHAR(MAX) = CONVERT(NVARCHAR, @PurchaseDtm, 127);
    
    IF NOT EXISTS
        (
            SELECT 1
            FROM PurchaseSubmit
            WHERE CustomerNo = @CustomerNo
              AND PurchaseDtm = @PurchaseDtm
        )
        BEGIN
            RAISERROR (80005, -1, @State, @CustomerNo, @PurchaseDtmString);
        END

    IF EXISTS
        (
            SELECT 1
            FROM PurchaseSubmitNotification
            WHERE CustomerNo = @CustomerNo
              AND PurchaseDtm = @PurchaseDtm
        )
        BEGIN
            RAISERROR (80006,-1, @State, @CustomerNo, @PurchaseDtmString);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE PurchaseSubmitNotification_Add_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseSubmitNotification_Add_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE PurchaseSubmitNotification_Add_tr
(
    @CustomerNo  INT,
    @PurchaseDtm DATETIMEOFFSET
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
        EXEC PurchaseSubmitNotification_Add_vtr @CustomerNo, @PurchaseDtm;

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
        EXEC PurchaseSubmitNotification_Add_vtr @CustomerNo, @PurchaseDtm;

        -- Database updates --
        INSERT INTO PurchaseSubmitNotification (CustomerNo, PurchaseDtm, NotificationDtm)
        VALUES (@CustomerNo, @PurchaseDtm, SYSDATETIMEOFFSET());

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
-- rollback DROP PROCEDURE PurchaseSubmitNotification_Add_tr;
