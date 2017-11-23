CREATE DEFINER=`root`@`localhost` PROCEDURE `rollbackComponent`(IN logID INT, in userID int, in commentParam VARCHAR(512), out err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
	select activeComponentID into @activeID from componentLog where componentLogID = logID;
	select componentID into @oldComponentID from activeComponent where ID = @activeID;
	select componentID into @ID from componentLog where componentLogID = logID;

    update activeComponent set componentID = @ID where ID=@activeID;
    
    INSERT INTO componentLog
		VALUES (null, @oldComponentID, @activeID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'rollback'); 

END