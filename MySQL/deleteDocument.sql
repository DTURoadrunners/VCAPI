CREATE DEFINER=`root`@`localhost` PROCEDURE `deleteDocument`(
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
	Select documentID into @oldDocumentID from activeDocument where ID = activeID;
    update activeDocument set documentID = null where ID=activeID;
    INSERT INTO documentLog VALUES(NULL, @oldDocumentID, activeID, userID, UNIX_TIMESTAMP(NOW()), logComment, 'deleted');
	COMMIT;
END