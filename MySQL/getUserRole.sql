DROP PROCEDURE IF EXISTS `getUserRole`;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `getUserRole`(IN userID INT, IN projectID INT)
BEGIN
	DECLARE SuperAdmin BOOL DEFAULT FALSE;
    SELECT `superuser` INTO SuperAdmin FROM `user` where `user`.`userID` = userID;
    IF SuperAdmin = TRUE THEN
		SELECT roleID FROM role where role.rolename = 'superuser';
	ELSE
		SELECT roleID FROM project_relationship WHERE project_relationship.userID = userID AND project_relationship.projectID = ID;
	END IF;
END$$
DELIMITER ;
