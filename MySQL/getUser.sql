CREATE DEFINER=`root`@`localhost` PROCEDURE `getUser`(IN userID VARCHAR(64))
BEGIN
	SELECT * FROM user WHERE user.userID = userID;
END