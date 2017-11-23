CREATE DEFINER=`root`@`localhost` PROCEDURE `updateUser`(IN userID VARCHAR(32), IN firstnameparam VARCHAR(32), IN lastnameparam VARCHAR(32), IN phonenumberparam VARCHAR(32))
BEGIN
UPDATE user
SET firstname = firstnameparam, lastname = lastnameparam, phonenumber = phonenumberparam
WHERE user.userID = userID;
END