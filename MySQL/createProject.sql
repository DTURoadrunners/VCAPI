CREATE DEFINER=`root`@`localhost` PROCEDURE `createProject`(IN nameparam VARCHAR(64), IN userid VARCHAR(32), IN commentparam VARCHAR(512), OUT id INT)
BEGIN 
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET id = 1;
ROLLBACK;
RESIGNAL;
END;
START TRANSACTION;
	INSERT INTO project (name)
	VALUES (LOWER(nameparam));
    
    SET @newID = LAST_INSERT_ID();
    
    INSERT INTO activeProjects (projectID)
	VALUES (@newID);
	SET id = LAST_INSERT_ID();
    INSERT INTO projectLog 
    VALUES (NULL, LAST_INSERT_ID(), @newID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'created'); 


END