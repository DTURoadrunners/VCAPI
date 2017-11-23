CREATE DEFINER=`root`@`localhost` PROCEDURE `rollbackComponenttype`(in logID int, in userID int, in commentParam VARCHAR(512), out err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
	select activeComponentTypeID into @activeID from componentTypeLog where componentTypeLogID = logID;
	select componentTypeID into @oldComponentTypeID from activeComponentType where ID = @activeID;
	select componentTypeID into @ID from componentTypeLog where componentTypeLogID = logID;

    
    update activeComponentType set componentTypeID = @ID where activeComponentType.ID=@activeID;
    
    
    INSERT INTO componentTypeLog
		VALUES (null, @oldComponentTypeID, @activeID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'rollback'); 
END