CREATE DEFINER=`root`@`localhost` PROCEDURE `rollbackProject`(IN logID int, IN userID int, IN commentparam VARCHAR(512), out err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
	select activeProjectID into @activeID from projectLog where projectLog.projectLogID = logID;
	select projectID into @oldProjectID from activeProjects where ID = @activeID;
	select projectID into @ID from projectLog where projectLog.projectLogID = logID;

    
    update activeProjects set projectID = @ID where activeProjects.ID=@activeID;
    
    
    INSERT INTO projectLog
		VALUES (null, @activeID, @oldProjectID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'rollback'); 
END