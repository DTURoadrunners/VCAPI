CREATE DEFINER=`root`@`localhost` PROCEDURE `deleteProject`(IN activeProjectID INT(11), IN userid VARCHAR(32), IN commentparam VARCHAR(512), OUT err INT)
BEGIN 
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
		
        Select projectID into @oldProjectID from activeProjects where ID = activeProjectID;

		update activeProjects set projectID = null where ID=activeProjectID;
	
		INSERT INTO projectLog
		VALUES (null, activeProjectID, @oldProjectID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'deleted');     
END