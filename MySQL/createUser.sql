DELIMITER \\

DROP PROCEDURE IF EXISTS `createUser`\\

CREATE DEFINER=`root`@`localhost` PROCEDURE `createUser`(in ID varchar(32), in firstnameparam VARCHAR(32), in lastnameparam varchar(32), IN phonenumberparam INT, IN passwordparam VARCHAR(1024))
BEGIN
DECLARE EXIT HANDLER FOR 1062
SIGNAL SQLSTATE '45000'
SET MESSAGE_TEXT = 'User already exists';
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
ROLLBACK;
RESIGNAL;
END;
START TRANSACTION;
	insert into user values(ID, firstnameparam, lastnameparam, phonenumberparam, passwordparam, 0);
    COMMIT;
END\\	
