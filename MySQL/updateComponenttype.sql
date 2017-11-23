CREATE DEFINER=`root`@`localhost` PROCEDURE `updateComponenttype`(IN nameparam VARCHAR(64), IN activeID int(11), IN categoryId int(11), IN storageparam int(11), IN descriptionparam varchar(512), IN userid varchar(32), IN commentparam varchar(512), OUT err int)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET err = 1;
ROLLBACK;
END;
SET err = 0;
START TRANSACTION;
	INSERT INTO componentType VALUES(NULL, nameparam, categoryId, storageparam, descriptionparam);
    SELECT componentTypeID INTO @oldID from activeComponentType WHERE ID = activeID;
    UPDATE activeComponentType SET activeComponentType.componentTypeID = LAST_INSERT_ID() WHERE activeComponentType.ID = activeID;
	INSERT INTO componentTypeLog VALUES(NULL, LAST_INSERT_ID(), @oldID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'updated');
	COMMIT;
	
END