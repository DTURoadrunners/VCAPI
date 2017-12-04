CREATE DEFINER=`root`@`localhost` PROCEDURE `createComponent`(
	IN activeComponentTypeID INT,
    IN _status VARCHAR(64),
	IN _comment VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
    OUT id int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET id = -1;
ROLLBACK;
resignal;
END;
START TRANSACTION;
		INSERT INTO component  VALUES(NULL, _status, _comment);
		SET @newID = LAST_INSERT_ID();
        INSERT INTO activeComponent VALUES(NULL, @newID, activeComponentTypeID);
        SET id = LAST_INSERT_ID();
		INSERT INTO componentLog VALUES(NULL, @newID, LAST_INSERT_ID(), userID, UNIX_TIMESTAMP(NOW()), logComment, "created");
	COMMIT;
END