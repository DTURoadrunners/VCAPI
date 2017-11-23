CREATE DEFINER=`root`@`localhost` PROCEDURE `getComponent`(in ID int)
BEGIN
SELECT * FROM component WHERE componentID = ID;
END