CREATE DEFINER=`root`@`localhost` PROCEDURE `getComponenttype`(in ID INT)
BEGIN
	Select * from componentType where componentTypeID = ID;
END