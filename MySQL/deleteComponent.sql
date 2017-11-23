CREATE DEFINER=`root`@`localhost` PROCEDURE `deleteComponent`(
	IN activeID INT,
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
    out err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
		SELECT componentID into @oldID from activeComponent where ID = activeID;
        update activeComponent set componentID = NULL WHERE activeComponent.ID = activeID;
        INSERT INTO componentLog VALUES(NULL, @oldID, activeID, userID, UNIX_TIMESTAMP(NOW()), logComment, 'deleted');
	COMMIT;
END