CREATE DEFINER=`root`@`localhost` PROCEDURE `updateDocument`(
	IN activeID INT,
    IN filename VARCHAR(64),
	IN bucketPath VARCHAR(1024),
	IN description VARCHAR(512),
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
		INSERT INTO document VALUES(NULL, filename, bucketPath, description);
        select documentID into @oldID from activeDocument where ID = activeID;
        UPDATE activeDocument SET activeDocument.documentID = last_insert_id() WHERE ID = activeID;
		INSERT INTO documentLog VALUES(NULL, @oldID, activeID, userID, UNIX_TIMESTAMP(NOW()), logComment, 'updated');
	COMMIT;
END