CREATE DEFINER=`root`@`localhost` PROCEDURE `connectUserProject`(IN userid VARCHAR(32), IN projectid INT, IN roleid INT, OUT iserror INT)
BEGIN
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET iserror = 1;
INSERT INTO project_relationship (userID, roleID, projectID)
	VALUES (userid, roleid, projectid);    
END