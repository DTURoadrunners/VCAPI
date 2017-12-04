CREATE DEFINER=`root`@`localhost` PROCEDURE `createDocument`(
	IN _filename VARCHAR(64),
    IN _activeComponentTypeID INT(11),
    IN _bucketpath VARCHAR(1024),
    IN _description VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
	OUT id INT
)
BEGIN 
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET id = 1;
ROLLBACK;
RESIGNAL;
END;  
START TRANSACTION;
	INSERT INTO document VALUES (NULL, _filename, _bucketpath, _description);
	SET @newID = LAST_INSERT_ID();
	INSERT INTO activeDocument VALUES(NULL, @newID, _activeComponentTypeID);
    set id = LAST_INSERT_ID();
	INSERT INTO documentLog VALUES(NULL, @newID, last_insert_id(), userID, UNIX_TIMESTAMP(NOW()), logComment, "created");
END