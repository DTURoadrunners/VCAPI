CREATE DEFINER=`root`@`localhost` PROCEDURE `createComponent`(
	IN activeComponentTypeID INT,
    IN _status VARCHAR(64),
	IN _comment VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
    OUT err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
		INSERT INTO component  VALUES(NULL, _status, _comment);
		SET @newID = LAST_INSERT_ID();
        INSERT INTO activeComponent VALUES(NULL, @newID, activeComponentTypeID);
		SET @newNewID = LAST_INSERT_ID();
		INSERT INTO componentLog VALUES(NULL, @newID, activeComponentTypeID, userID, UNIX_TIMESTAMP(NOW()), logComment, "created");
		SELECT @newNewID as ID;
	COMMIT;
END