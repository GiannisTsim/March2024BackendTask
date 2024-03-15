-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:WebAppWriterRole stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE ROLE WebAppWriterRole;
GRANT EXECUTE ON Customer_Add_tr TO WebAppWriterRole;
GRANT EXECUTE ON Customer_Drop_tr TO WebAppWriterRole;
GRANT EXECUTE ON Customer_Modify_tr TO WebAppWriterRole;
GRANT EXECUTE ON Product_Add_tr TO WebAppWriterRole;
GRANT EXECUTE ON Product_Drop_tr TO WebAppWriterRole;
GRANT EXECUTE ON Product_Modify_tr TO WebAppWriterRole;
GRANT EXECUTE ON PurchaseItem_Add_tr TO WebAppWriterRole;
GRANT EXECUTE ON PurchaseSubmit_Add_tr TO WebAppWriterRole;
GRANT EXECUTE ON PurchaseSubmitNotification_Add_tr TO WebAppWriterRole;
-- rollback DROP ROLE WebAppWriterRole;


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:WebApp stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE LOGIN WebAppLogin WITH PASSWORD = '${WEB_APP_PASSWORD}';
CREATE USER WebApp FOR LOGIN WebAppLogin;
ALTER ROLE WebAppWriterRole ADD MEMBER WebApp;
ALTER ROLE db_datareader ADD MEMBER WebApp;
-- rollback DROP USER WebApp;
-- rollback DROP LOGIN WebAppLogin;