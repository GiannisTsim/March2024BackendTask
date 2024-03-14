-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:PurchaseItem_V stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE VIEW PurchaseItem_V AS
SELECT CustomerNo,
       PurchaseDtm,
       ProductNo,
       Quantity,
       PriceUnit,
       (Quantity * PriceUnit) AS PriceBatch
FROM (
         SELECT CustomerNo,
                PurchaseDtm,
                ProductNo,
                Quantity,
                CASE WHEN PurchaseDtm >= (
                                             SELECT UpdatedDtm
                                             FROM Product
                                             WHERE ProductNo = PurchaseItem.ProductNo
                ) THEN (
                           SELECT Price
                           FROM Product
                           WHERE ProductNo = PurchaseItem.ProductNo
                )
                     ELSE (
                              SELECT Price
                              FROM ProductHistory
                              WHERE PurchaseDtm < AuditedDtm
                                AND PurchaseDtm >= UpdatedDtm
                     )
                END AS PriceUnit
         FROM PurchaseItem
) AS T;
-- rollback DROP VIEW PurchaseItem_V;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Purchase_V stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE VIEW Purchase_V AS
SELECT Purchase.CustomerNo,
       Purchase.PurchaseDtm,
       PriceInitial,
       DiscountPct,
       (PriceInitial * DiscountPct)                  AS Discount,
       (PriceInitial - (PriceInitial * DiscountPct)) AS PriceFinal,
       SubmissionDtm,
       NotificationDtm
FROM (
         SELECT CustomerNo,
                PurchaseDtm,
                SUM(PriceBatch) AS PriceInitial,
                CASE WHEN PurchaseDtm >= (
                                             SELECT UpdatedDtm
                                             FROM Customer
                                             WHERE CustomerNo = PurchaseItem_V.CustomerNo
                ) THEN (
                           SELECT DiscountPct
                           FROM Customer
                           WHERE CustomerNo = PurchaseItem_V.CustomerNo
                )
                     ELSE (
                              SELECT DiscountPct
                              FROM CustomerHistory
                              WHERE PurchaseDtm < AuditedDtm
                                AND PurchaseDtm >= UpdatedDtm
                     )
                END             AS DiscountPct
         FROM PurchaseItem_V
         GROUP BY CustomerNo, PurchaseDtm
) AS Purchase
LEFT JOIN PurchaseSubmit
ON Purchase.CustomerNo = PurchaseSubmit.CustomerNo
    AND Purchase.PurchaseDtm = PurchaseSubmit.PurchaseDtm
LEFT JOIN PurchaseSubmitNotification
ON PurchaseSubmit.CustomerNo = PurchaseSubmitNotification.CustomerNo
    AND PurchaseSubmit.PurchaseDtm = PurchaseSubmitNotification.PurchaseDtm;
-- rollback DROP VIEW Purchase_V;