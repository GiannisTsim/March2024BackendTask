-- liquibase formatted sql

-- ------------------------------------------------------------------------------------------------------------------ --
-- changeset ${AUTHOR}:Xact_Integrity_Check stripComments:false endDelimiter:GO
-- ------------------------------------------------------------------------------------------------------------------ --
CREATE PROCEDURE Xact_Integrity_Check
AS
BEGIN
    IF @@TRANCOUNT > 0
        RAISERROR (50001, -1, 1);
    IF @@OPTIONS & 2 != 0
        RAISERROR (50002, -1, 1);
    RETURN 0;
END
GO
-- rollback DROP PROCEDURE Xact_Integrity_Check;