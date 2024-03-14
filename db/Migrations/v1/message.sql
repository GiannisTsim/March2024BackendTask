-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:_Generic stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
EXEC sp_addmessage 50001, 16, 'Transaction cannot be called from within an open transaction.';
-- rollback EXEC sp_dropmessage 50001, 'all';

EXEC sp_addmessage 50002, 16, 'Implicit transactions are not allowed.';
-- rollback EXEC sp_dropmessage 50002, 'all';

EXEC sp_addmessage 50003, 16, 'Utility transaction must be called from within an open transaction.';
-- rollback EXEC sp_dropmessage 50003, 'all';


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Product stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
EXEC sp_addmessage 60001, 16, 'Product.Name cannot be null or whitespace.';
-- rollback EXEC sp_dropmessage 60001, 'all';

EXEC sp_addmessage 60002, 16, 'Product.Description cannot be null.';
-- rollback EXEC sp_dropmessage 60002, 'all';

EXEC sp_addmessage 60003, 16, 'Product.Price must be a number greater than 0.';
-- rollback EXEC sp_dropmessage 60003, 'all';

EXEC sp_addmessage 60004, 16, 'Product[Name=''%s''] already exists.';
-- rollback EXEC sp_dropmessage 60004, 'all';

EXEC sp_addmessage 60005, 16, 'Product[ProductNo=%d] does not exist.';
-- rollback EXEC sp_dropmessage 60005, 'all';

EXEC sp_addmessage 60006, 16, 'Product[ProductNo=%d,UpdatedDtm=''%s''] currency is lost.';
-- rollback EXEC sp_dropmessage 60006, 'all';


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Customer stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
EXEC sp_addmessage 70001, 16, 'Customer.FirstName cannot be null or whitespace.';
-- rollback EXEC sp_dropmessage 70001, 'all';

EXEC sp_addmessage 70002, 16, 'Customer.LastName cannot be null or whitespace.';
-- rollback EXEC sp_dropmessage 70002, 'all';

EXEC sp_addmessage 70003, 16, 'Customer.Discount must be a number between 0 and 1 (inclusive).';
-- rollback EXEC sp_dropmessage 70003, 'all';

EXEC sp_addmessage 70004, 16, 'Customer[FirstName=''%s'',LastName=''%s''] already exists.';
-- rollback EXEC sp_dropmessage 70004, 'all';

EXEC sp_addmessage 70005, 16, 'Customer[CustomerNo=%d] does not exist.';
-- rollback EXEC sp_dropmessage 70005, 'all';

EXEC sp_addmessage 70006, 16, 'Customer[CustomerNo=%d,UpdatedDtm=''%s''] currency is lost.';
-- rollback EXEC sp_dropmessage 70006, 'all';


-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Purchase stripComments:false
-- ------------------------------------------------------------------------------------------------------------------ --
EXEC sp_addmessage 80001, 16, 'PurchaseItem.Quantity must be a number greater than 0.';
-- rollback EXEC sp_dropmessage 80001, 'all';

EXEC sp_addmessage 80002, 16, 'Purchase[CustomerNo=%d,PurchaseDtm=''%s''] does not exist.';
-- rollback EXEC sp_dropmessage 80002, 'all';

EXEC sp_addmessage 80003, 16, 'PurchaseItem[CustomerNo=%d,PurchaseDtm=''%s'',ProductNo=%d] already exists.';
-- rollback EXEC sp_dropmessage 80003, 'all';

EXEC sp_addmessage 80004, 16, 'PurchaseSubmit[CustomerNo=%d,PurchaseDtm=''%s''] already exists.';
-- rollback EXEC sp_dropmessage 80004, 'all';

EXEC sp_addmessage 80005, 16, 'PurchaseSubmit[CustomerNo=%d,PurchaseDtm=''%s''] does not exist.';
-- rollback EXEC sp_dropmessage 80005, 'all';

EXEC sp_addmessage 80006, 16, 'PurchaseSubmitNotification[CustomerNo=%d,PurchaseDtm=''%s''] already exists.';
-- rollback EXEC sp_dropmessage 80006, 'all';

EXEC sp_addmessage 80007, 16, 'Purchase[CustomerNo=%d,PurchaseDtm=''%s''] is submitted and cannot be changed.';
-- rollback EXEC sp_dropmessage 80007, 'all';

EXEC sp_addmessage 80008, 16, 'Purchase.PurchaseDtm cannot be null.';
-- rollback EXEC sp_dropmessage 80008, 'all';