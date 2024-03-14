-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Product_Add_vtr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Product_Add_vtr
(
    @Name VARCHAR(30)
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
            FROM Product
            WHERE Name = @Name
              AND IsObsolete = 0
        )
        BEGIN
            RAISERROR (60004, -1, @State, @Name);
        END

    -- Validation successful--
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE Product_Add_vtr;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Product_Add_tr stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Product_Add_tr
(
    @Name        VARCHAR(30),
    @Description VARCHAR(100),
    @Price       DECIMAL(7, 2),
    @ProductNo   INT = NULL OUTPUT
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
        IF @Name IS NULL OR @Name = ''
            BEGIN
                RAISERROR (60001, -1 , 1);
            END
        IF @Description IS NULL
            BEGIN
                RAISERROR (60002, -1 , 1);
            END
        IF @Price IS NULL OR @Price <= 0
            BEGIN
                RAISERROR (60003, -1 , 1);
            END

        -- Offline constraint validation (no locks held) --
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
        EXEC Product_Add_vtr @Name;

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
        EXEC Product_Add_vtr @Name;

        -- Database updates --
        SELECT @ProductNo = ProductNo
        FROM Product
        WHERE Name = @Name
          AND IsObsolete = 1;

        IF @@ROWCOUNT != 0
            BEGIN
                -- If an obsolete product with the same name exists, restore the product with the necessary modifications
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
                WHERE ProductNo = @ProductNo;

                UPDATE Product
                SET Description = @Description,
                    Price       = @Price,
                    UpdatedDtm  = @AuditedDtm,
                    IsObsolete  = 0
                WHERE ProductNo = @ProductNo;
            END
        ELSE
            BEGIN
                -- Else add new product
                SET @ProductNo = (
                                     SELECT COALESCE(MAX(ProductNo) + 1, 1)
                                     FROM Product
                );
                INSERT INTO Product (ProductNo, Name, Description, Price, UpdatedDtm, IsObsolete)
                VALUES (@ProductNo, @Name, @Description, @Price, SYSDATETIMEOFFSET(), 0);
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
-- rollback DROP PROCEDURE Product_Add_tr;
