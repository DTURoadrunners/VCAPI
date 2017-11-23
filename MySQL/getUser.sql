CREATE DEFINER=`root`@`localhost` PROCEDURE `getUser`(IN userID INT)
BEGIN
	SELECT * FROM user WHERE user.userID = userID;
END