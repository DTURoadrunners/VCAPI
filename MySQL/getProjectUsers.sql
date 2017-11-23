CREATE DEFINER=`root`@`localhost` PROCEDURE `getProjectUsers`(IN projectID INT)
BEGIN
	SELECT userID FROM project_relationship
    WHERE project_relationship.projectID = projectID;
    ## return actual users, union?
END