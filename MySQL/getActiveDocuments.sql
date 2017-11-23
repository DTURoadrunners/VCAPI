CREATE DEFINER=`root`@`localhost` PROCEDURE `getActiveDocuments`(in componentTypeID int)
BEGIN
	Select * from activeDocument where activeComponentTypeID = componentTypeID;
END