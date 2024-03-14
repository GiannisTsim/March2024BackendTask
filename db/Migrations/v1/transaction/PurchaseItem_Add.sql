-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseItem_Add_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE PurchaseItem_Add_vtr
(
    @CustomerNo  INT,
    @PurchaseDtm DATETIMEOFFSET,
    @ProductNo   INT
) AS
BEGIN
    -- Error state initialization --
    DECLARE @State TINYINT = CASE WHEN @@TRANCOUNT = 0 THEN 1
                                  ELSE 2
                             END;

    -- Validation checks --
    DECLARE @PurchaseDtmString NVARCHAR(MAX) = CONVERT(NVARCHAR, @PurchaseDtm, 127);

    -- If Purchase does not already exist, ensure Customer exists
    IF NOT EXISTS
           (
               SELECT 1
               FROM Purchase
               WHERE CustomerNo = @CustomerNo
                 AND PurchaseDtm = @PurchaseDtm
           )
        AND NOT EXISTS
           (
               SELECT 1
               FROM Customer
               WHERE CustomerNo = @CustomerNo
           )
        BEGIN
            RAISERROR (70005, -1, @State, @CustomerNo);
        END
    -- Else if Purchase exists, ensure it is not submitted
    ELSE IF EXISTS
        (
            SELECT 1
            FROM PurchaseSubmit
            WHERE CustomerNo = @CustomerNo
              AND PurchaseDtm = @PurchaseDtm
        )
        BEGIN
            RAISERROR (80007, -1, @State, @CustomerNo, @PurchaseDtmString);
        END

    -- Ensure Product exists
    IF NOT EXISTS
        (
            SELECT 1
            FROM Product
            WHERE ProductNo = @ProductNo
        )
        BEGIN
            RAISERROR (60005, -1, @State, @ProductNo);
        END

    -- Ensure PurchaseItem does not already exist
    IF EXISTS
        (
            SELECT 1
            FROM PurchaseItem
            WHERE CustomerNo = @CustomerNo
              AND PurchaseDtm = @PurchaseDtm
              AND ProductNo = @ProductNo
        )
        BEGIN
            RAISERROR (80003,-1, @State, @CustomerNo, @PurchaseDtmString, @ProductNo);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE PurchaseItem_Add_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseItem_Add_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE PurchaseItem_Add_tr
(
    @CustomerNo  INT,
    @PurchaseDtm DATETIMEOFFSET,
    @ProductNo   INT,
    @Quantity    INT
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
        IF @PurchaseDtm IS NULL
            BEGIN
                RAISERROR (80008, -1, 1);
            END

        IF @Quantity IS NULL OR @Quantity <= 0
            BEGIN
                RAISERROR (80001, -1 , 1);
            END

        -- Offline constraint validation (no locks held) --
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
        EXEC PurchaseItem_Add_vtr @CustomerNo, @PurchaseDtm, @ProductNo;

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
        EXEC PurchaseItem_Add_vtr @CustomerNo, @PurchaseDtm, @ProductNo;

        -- Database updates --
        IF NOT EXISTS
            (
                SELECT 1 FROM Purchase WHERE CustomerNo = @CustomerNo AND PurchaseDtm = @PurchaseDtm
            )
            BEGIN
                INSERT INTO Purchase (CustomerNo, PurchaseDtm) VALUES (@CustomerNo, @PurchaseDtm);
            END

        INSERT INTO PurchaseItem (CustomerNo, PurchaseDtm, ProductNo, Quantity)
        VALUES (@CustomerNo, @PurchaseDtm, @ProductNo, @Quantity);

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
-- rollback DROP PROCEDURE PurchaseItem_Add_tr;
