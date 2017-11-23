CREATE DEFINER=`root`@`localhost` PROCEDURE `createRole`(IN nameParam VARCHAR(32))
BEGIN
if(SELECT NOT EXISTS  (
		SELECT * 
		FROM role
		WHERE LOWER(nameParam) LIKE rolename
	)
)
	THEN
		INSERT INTO role (rolename)
		Values (LOWER(nameParam));    
END IF;
    
END