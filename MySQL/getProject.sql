CREATE DEFINER=`root`@`localhost` PROCEDURE `getProject`(IN projectID int)
BEGIN
	Select * from project where project.projectID = projectID;
END