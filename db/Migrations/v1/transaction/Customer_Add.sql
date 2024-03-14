-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer_Add_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Customer_Add_vtr
(
    @FirstName VARCHAR(30),
    @LastName  VARCHAR(30)
) AS
BEGIN
    -- Error state initialization --
    DECLARE @State TINYINT = CASE WHEN @@TRANCOUNT = 0 THEN 1
                                  ELSE 2
                             END;

    -- Validation checks --
    IF EXISTS
        (
            SELECT 1
            FROM Customer
            WHERE FirstName = @FirstName
              AND LastName = @LastName
              AND IsObsolete = 0
        )
        BEGIN
            RAISERROR (70004, -1, @State, @FirstName, @LastName);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE Customer_Add_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer_Add_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Customer_Add_tr
(
    @FirstName   VARCHAR(30),
    @LastName    VARCHAR(30),
    @DiscountPct DECIMAL(3, 2),
    @CustomerNo  INT = NULL OUTPUT
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
        EXEC Customer_Add_vtr @FirstName, @LastName;

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
        EXEC Customer_Add_vtr @FirstName, @LastName;

        -- Database updates --
        SELECT @CustomerNo = CustomerNo
        FROM Customer
        WHERE FirstName = @FirstName
          AND LastName = @LastName
          AND IsObsolete = 1;
        
        IF @@ROWCOUNT != 0
            BEGIN
                -- If an obsolete customer with the same name exists, restore the customer with the necessary modifications
                DECLARE @AuditedDtm DATETIMEOFFSET = SYSDATETIMEOFFSET();

                INSERT INTO CustomerHistory (CustomerNo, AuditedDtm, FirstName, LastName, DiscountPct, UpdatedDtm,
                                             IsObsolete)
                SELECT CustomerNo,
                       @AuditedDtm AS AuditedDtm,
                       FirstName,
                       LastName,
                       DiscountPct,
                       UpdatedDtm,
                       IsObsolete
                FROM Customer
                WHERE CustomerNo = @CustomerNo;

                UPDATE Customer
                SET DiscountPct = @DiscountPct,
                    UpdatedDtm  = @AuditedDtm,
                    IsObsolete  = 0
                WHERE CustomerNo = @CustomerNo;
            END
        ELSE
            BEGIN
                -- Else add new customer
                SET @CustomerNo = (
                                      SELECT COALESCE(MAX(CustomerNo) + 1, 1)
                                      FROM Customer
                );
                INSERT INTO Customer (CustomerNo, FirstName, LastName, DiscountPct, UpdatedDtm, IsObsolete)
                VALUES (@CustomerNo, @FirstName, @LastName, @DiscountPct, SYSDATETIMEOFFSET(), 0);
            END

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
-- rollback DROP PROCEDURE Customer_Add_tr;
