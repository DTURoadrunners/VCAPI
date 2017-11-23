CREATE DEFINER=`root`@`localhost` PROCEDURE `deleteComponenttype`(
	IN activeComponentTypeID INT(11), 
    IN userid VARCHAR(32), 
    IN commentparam VARCHAR(512), 
    OUT err INT)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
        Select componentTypeID into @oldComponentTypeID from activeComponentType where ID = activeComponentTypeID;
        update activeComponentType set componentTypeID = null where ID=activeComponentTypeID;
        INSERT INTO componentTypeLog VALUES(NULL, @oldComponentTypeID, activeComponentTypeID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'deleted');
	COMMIT;
END