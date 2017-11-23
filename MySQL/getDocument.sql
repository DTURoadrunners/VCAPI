CREATE DEFINER=`root`@`localhost` PROCEDURE `getDocument`(in ID int)
BEGIN
	Select * from document where documentID = ID;
END