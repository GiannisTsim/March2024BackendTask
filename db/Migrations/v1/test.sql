-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Purchase_V.test stripComments:false endDelimiter:GO runOnChange:true
-- ------------------------------------------------------------------------------------------------------------------ --
EXEC tSQLt.NewTestClass Purchase_V;
GO

CREATE OR ALTER PROCEDURE Purchase_V.[Test computed columns validity with historical data]
AS
BEGIN
    -- Product initialization
    DECLARE @ProductNo INT = 1;
    DECLARE @ProductUpdatedDtm DATETIMEOFFSET = SYSDATETIMEOFFSET();
    INSERT INTO Product (ProductNo, Name, Description, Price, UpdatedDtm, IsObsolete)
    VALUES (@ProductNo, 'Foo', 'Lorem ipsum dolor sit amet.', 200, @ProductUpdatedDtm, 0);
    INSERT INTO ProductHistory (ProductNo, AuditedDtm, Name, Description, Price, UpdatedDtm, IsObsolete)
    VALUES (@ProductNo, @ProductUpdatedDtm, 'Foo', 'Lorem ipsum dolor sit amet.', 400,
            DATEADD(DAY, -30, SYSDATETIMEOFFSET()), 0);

    -- Customer initialization
    DECLARE @CustomerNo INT = 1;
    DECLARE @CustomerUpdatedDtm DATETIMEOFFSET = SYSDATETIMEOFFSET();
    INSERT INTO Customer (CustomerNo, FirstName, LastName, DiscountPct, UpdatedDtm, IsObsolete)
    VALUES (@CustomerNo, 'John', 'Doe', 0.10, @CustomerUpdatedDtm, 0);
    INSERT INTO CustomerHistory (CustomerNo, AuditedDtm, FirstName, LastName, DiscountPct, UpdatedDtm, IsObsolete)
    VALUES (@CustomerNo, @CustomerUpdatedDtm, 'John', 'Doe', 0.20, DATEADD(DAY, -30, SYSDATETIMEOFFSET()), 0);

    -- Purchase initialization
    DECLARE @Purchase1Dtm DATETIMEOFFSET = SYSDATETIMEOFFSET();
    INSERT INTO Purchase (CustomerNo, PurchaseDtm) VALUES (@CustomerNo, @Purchase1Dtm);
    INSERT INTO PurchaseItem (CustomerNo, PurchaseDtm, ProductNo, Quantity)
    VALUES (@CustomerNo, @Purchase1Dtm, @ProductNo, 10);

    DECLARE @Purchase2Dtm DATETIMEOFFSET = DATEADD(DAY, -20, SYSDATETIMEOFFSET());
    INSERT INTO Purchase (CustomerNo, PurchaseDtm) VALUES (@CustomerNo, @Purchase2Dtm);
    INSERT INTO PurchaseItem (CustomerNo, PurchaseDtm, ProductNo, Quantity)
    VALUES (@CustomerNo, @Purchase2Dtm, @ProductNo, 10);

    -- Execution
    SELECT CustomerNo, PurchaseDtm, PriceInitial, DiscountPct, Discount, PriceFinal
    INTO #Actual
    FROM Purchase_V;

    -- Assertion
    SELECT TOP 0 * INTO #Expected FROM #Actual;
    INSERT INTO #Expected (CustomerNo, PurchaseDtm, PriceInitial, DiscountPct, Discount, PriceFinal)
    VALUES (@CustomerNo, @Purchase1Dtm, (200 * 10), 0.10, (200 * 10 * 0.10), ((200 * 10) - (200 * 10 * 0.10))),
           (@CustomerNo, @Purchase2Dtm, (400 * 10), 0.20, (400 * 10 * 0.20), ((400 * 10) - (400 * 10 * 0.20)));

    EXEC tSQLt.AssertEqualsTable #Expected, #Actual;
END
GO

-- rollback EXEC tSQLt.DropClass 'Purchase_V'