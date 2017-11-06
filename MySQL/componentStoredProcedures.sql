DELIMITER //
CREATE PROCEDURE getAllComponents (IN componentTypeID INT)
BEGIN
	SELECT * FROM component
    WHERE component.componentTypeID = componentTypeID;
END //
DELIMITER ;
    
DELIMITER //
CREATE PROCEDURE getComponent (IN ID INT, IN componentTypeID INT)
BEGIN
	SELECT * FROM component
    WHERE component.componentID = ID &&
	component.componentTypeID = componentTypeID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE createComponent (
	IN componentTypeID INT,
    IN _status VARCHAR(64),
	IN _comment VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512))
BEGIN
	START TRANSACTION;
		INSERT INTO component VALUES(componentTypeID, _status, _comment);
		INSERT INTO componentLog VALUES(LAST_INSERT_ID(), userID, logComment, "CREATED");
	COMMIT;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE updateComponent (
	IN ID INT,
	IN componentTypeID INT,
    IN _status VARCHAR(64),
	IN _comment VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512))
BEGIN
	START TRANSACTION;
		UPDATE component
        SET component.componentTypeID = componentTypeID, component.status = _status, component.comment = _comment
        WHERE component.componentID = ID;
		INSERT INTO componentLog VALUES(ID, userID, logComment, "UPDATED");
	COMMIT;
END //
DELIMITER ;
/*
DELIMITER //
CREATE PROCEDURE deleteComponent (
	IN ID INT,
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512))
BEGIN
	START TRANSACTION;
		## Unsure how to proceed.
        ## Should update the componentType to no longer point to this component
		INSERT INTO componentLog VALUES(ID, userID, logComment, "DELETED");
	COMMIT;
END //
DELIMITER ;
*/