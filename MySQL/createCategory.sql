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
END IF;
END