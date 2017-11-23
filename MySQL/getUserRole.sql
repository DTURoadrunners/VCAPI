CREATE DEFINER=`root`@`localhost` PROCEDURE `getUserRole`(IN userID INT, IN projectID INT)
BEGIN
	SELECT roleID FROM project_relationship WHERE project_relationship.userID = userID AND project_relationship.projectID = ID;
END