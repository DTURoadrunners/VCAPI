CREATE DEFINER=`root`@`localhost` PROCEDURE `getActiveComponents`(in componentTypeID int)
BEGIN
SELECT * from activeComponent where activeComponentType = componentTypeID;
END