CREATE DEFINER=`root`@`localhost` PROCEDURE `rollbackDocument`(in logID int, in userID int, in commentParam VARCHAR(512), out err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
	select activeDocumentID into @activeID from documentLog where documentLogID = logID;
	select documentID into @oldDocumentID from activeDocument where ID = @activeID;
	select documentID into @ID from documentLog where documentLogID = logID;
    
    update activeDocument set documentID = @ID where activeDocument.ID=@activeID;
    
    INSERT INTO documentLog
		VALUES (null, @oldDocumentID, @activeID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'rollback'); 
END