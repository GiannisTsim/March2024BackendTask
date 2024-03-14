-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Product stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE Product
(
    ProductNo   INT            NOT NULL,
    Name        VARCHAR(30)    NOT NULL,
    Description VARCHAR(100)   NOT NULL,
    Price       DECIMAL(7, 2)  NOT NULL,
    UpdatedDtm  DATETIMEOFFSET NOT NULL,
    IsObsolete  BIT            NOT NULL,
    CONSTRAINT UC_Product_PK PRIMARY KEY CLUSTERED (ProductNo),
    CONSTRAINT UC_Product_AK UNIQUE (Name),
    CONSTRAINT Product_Price_GT_0_ck CHECK (Price > 0)
);
-- rollback DROP TABLE Product;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:ProductHistory stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE ProductHistory
(
    ProductNo   INT            NOT NULL,
    AuditedDtm  DATETIMEOFFSET NOT NULL,
    Name        VARCHAR(30)    NOT NULL,
    Description VARCHAR(100)   NOT NULL,
    Price       DECIMAL(7, 2)  NOT NULL,
    UpdatedDtm  DATETIMEOFFSET NOT NULL,
    IsObsolete  BIT            NOT NULL,
    CONSTRAINT UC_ProductHistory_PK PRIMARY KEY CLUSTERED (ProductNo, AuditedDtm),
    CONSTRAINT Product_Was_ProductHistory_fk FOREIGN KEY (ProductNo) REFERENCES Product (ProductNo)
);
-- rollback DROP TABLE ProductHistory;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE Customer
(
    CustomerNo  INT            NOT NULL,
    FirstName   VARCHAR(30)    NOT NULL,
    LastName    VARCHAR(30)    NOT NULL,
    DiscountPct DECIMAL(3, 2)  NOT NULL,
    UpdatedDtm  DATETIMEOFFSET NOT NULL,
    IsObsolete  BIT            NOT NULL,
    CONSTRAINT UC_Customer_PK PRIMARY KEY CLUSTERED (CustomerNo),
    CONSTRAINT UC_Customer_AK UNIQUE (FirstName, LastName),
    CONSTRAINT Customer_DiscountPct_IsValidPct_ck CHECK (DiscountPct >= 0 AND DiscountPct <= 1)
);
-- rollback DROP TABLE Customer;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:CustomerHistory stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE CustomerHistory
(
    CustomerNo  INT            NOT NULL,
    AuditedDtm  DATETIMEOFFSET NOT NULL,
    FirstName   VARCHAR(30)    NOT NULL,
    LastName    VARCHAR(30)    NOT NULL,
    DiscountPct DECIMAL(3, 2)  NOT NULL,
    UpdatedDtm  DATETIMEOFFSET NOT NULL,
    IsObsolete  BIT            NOT NULL,
    CONSTRAINT UC_CustomerHistory_PK PRIMARY KEY CLUSTERED (CustomerNo, AuditedDtm),
    CONSTRAINT Customer_Was_CustomerHistory_fk FOREIGN KEY (CustomerNo) REFERENCES Customer (CustomerNo)
);
-- rollback DROP TABLE CustomerHistory;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Purchase stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE Purchase
(
    CustomerNo  INT            NOT NULL,
    PurchaseDtm DATETIMEOFFSET NOT NULL,
    CONSTRAINT UC_Purchase_PK PRIMARY KEY CLUSTERED (CustomerNo, PurchaseDtm),
    CONSTRAINT Customer_Executes_Purchase_fk FOREIGN KEY (CustomerNo) REFERENCES Customer (CustomerNo)
);
-- rollback DROP TABLE Purchase;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseItem stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE PurchaseItem
(
    CustomerNo  INT            NOT NULL,
    PurchaseDtm DATETIMEOFFSET NOT NULL,
    ProductNo   INT            NOT NULL,
    Quantity    INT            NOT NULL,
    CONSTRAINT UC_PurchaseItem_PK PRIMARY KEY CLUSTERED (CustomerNo, PurchaseDtm, ProductNo),
    CONSTRAINT Purchase_Comprises_PurchaseItem_fk FOREIGN KEY (CustomerNo, PurchaseDtm) REFERENCES Purchase (CustomerNo, PurchaseDtm),
    CONSTRAINT Product_IsPurchasedAs_PurchaseItem_fk FOREIGN KEY (ProductNo) REFERENCES Product (ProductNo),
    CONSTRAINT PurchaseItem_Quantity_GT_0_ck CHECK (Quantity > 0)
);
-- rollback DROP TABLE PurchaseItem;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseSubmit stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE PurchaseSubmit
(
    CustomerNo    INT            NOT NULL,
    PurchaseDtm   DATETIMEOFFSET NOT NULL,
    SubmissionDtm DATETIMEOFFSET NOT NULL,
    CONSTRAINT UC_PurchaseSubmit_PK PRIMARY KEY CLUSTERED (CustomerNo, PurchaseDtm),
    CONSTRAINT Purchase_Is_PurchaseSubmit_fk FOREIGN KEY (CustomerNo, PurchaseDtm) REFERENCES Purchase (CustomerNo, PurchaseDtm)
);
-- rollback DROP TABLE PurchaseSubmit;

    
-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseSubmitNotification stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE TABLE PurchaseSubmitNotification
(
    CustomerNo      INT            NOT NULL,
    PurchaseDtm     DATETIMEOFFSET NOT NULL,
    NotificationDtm DATETIMEOFFSET NOT NULL,
    CONSTRAINT UC_PurchaseSubmitNotification_PK PRIMARY KEY CLUSTERED (CustomerNo, PurchaseDtm),
    CONSTRAINT PurchaseSubmit_Is_PurchaseSubmitNotification_fk FOREIGN KEY (CustomerNo, PurchaseDtm) REFERENCES PurchaseSubmit (CustomerNo, PurchaseDtm)
);
-- rollback DROP TABLE PurchaseSubmitNotification;

        