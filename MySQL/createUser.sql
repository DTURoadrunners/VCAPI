CREATE DEFINER=`root`@`localhost` PROCEDURE `createUser`(in ID varchar(32), in firstnameparam VARCHAR(32), in lastnameparam varchar(32), IN phonenumberparam varchar(32), IN passwordparam VARCHAR(512), out err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;  
SET err = 0;
START TRANSACTION;
	insert into user values(ID, firstnameparam, lastnameparam, phonenumberparam, passwordparam);
END