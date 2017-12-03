CREATE DEFINER=`root`@`localhost` PROCEDURE `createCategory`(IN nameparam VARCHAR(64))
BEGIN
IF(SELECT NOT EXISTS (
	SELECT * 
    FROM category
    WHERE LOWER(nameparam) LIKE (name)
))
THEN
	INSERT INTO category (name)
    VALUES (LOWER(nameparam));
    SELECT LAST_INSERT_ID() as ID;
ELSEIF
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = "Such a catagory already exists";
END IF;
END