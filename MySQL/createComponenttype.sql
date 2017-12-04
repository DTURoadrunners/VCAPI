CREATE DEFINER=`root`@`localhost` PROCEDURE `createComponenttype`(
	IN nameparam VARCHAR(64),
	IN activeProjectIDParam INT,
	IN categoryId int(11), 
	IN `storageparam` int(11), 
	IN description varchar(512), 
	IN userid varchar(32), 
	IN commentparam varchar(512),
    OUT id int
)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
SET id = -1;
RESIGNAL;
ROLLBACK;
END;
START TRANSACTION;
	INSERT INTO componentType (`name`, categoryID, `storage`, description)
    VALUES (nameparam, categoryId, storageparam, description);
	SET @newID = LAST_INSERT_ID(); 
    INSERT INTO activeComponentType VALUES (NULL, activeProjectIDParam, @newID);
    set id = LAST_INSERT_ID();
    INSERT INTO componentTypeLog 
    VALUES (NULL, @newID, LAST_INSERT_ID(), userid, UNIX_TIMESTAMP(NOW()), commentparam, 'created');
	
END