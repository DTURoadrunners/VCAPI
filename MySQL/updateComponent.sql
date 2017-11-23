CREATE DEFINER=`root`@`localhost` PROCEDURE `updateComponent`(
	IN activeID INT,
    IN _status VARCHAR(64),
	IN _comment VARCHAR(512),
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
		INSERT INTO component VALUES(NULL, _status, _comment);
        SELECT componentID INTO @oldID from activeComponent WHERE ID = activeID;
        UPDATE activeComponent SET activeComponent.componentID = last_insert_id() WHERE activeComponent.ID = activeID;
		INSERT INTO componentLog VALUES(NULL, @oldID, activeID, userID, UNIX_TIMESTAMP(NOW()), logComment, 'updated');
	COMMIT;
END