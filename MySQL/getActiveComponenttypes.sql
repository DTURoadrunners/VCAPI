CREATE DEFINER=`root`@`localhost` PROCEDURE `getActiveComponenttypes`(IN projectID int)
BEGIN
	Select * from activeComponentType where activeprojectID = projectID;
END