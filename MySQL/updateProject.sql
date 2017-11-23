CREATE DEFINER=`root`@`localhost` PROCEDURE `updateProject`(IN nameparam VARCHAR(64), IN activeProjectID int(11), IN userid varchar(32), IN commentparam varchar(512), OUT err INT)
BEGIN 
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
		select projectID into @oldProjectID from activeProjects where ID = activeProjectID;
        
        Insert into project Values (NULL, nameparam);
        
        set @newProjectID = LAST_INSERT_ID();
	
		INSERT INTO projectLog
		VALUES (NULL, activeProjectID, @oldProjectID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'updated');
        
        update activeProjects SET projectID = @newProjectID where ID = activeProjectID; 
END