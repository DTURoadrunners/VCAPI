DELIMITER $$

DROP PROCEDURE IF EXISTS grantUserProjectRole$$
CREATE PROCEDURE `grantUserProjectRole`(IN userid VARCHAR(32), IN projectid INT, IN roleid INT)
BEGIN
INSERT INTO projectRoles (userID, roleID, projectID)
	VALUES (userid, roleid, projectid) ON DUPLICATE KEY UPDATE roleID=roleid;    
END$$

DROP PROCEDURE IF EXISTS createCategory$$
CREATE PROCEDURE `createCategory`(IN nameparam VARCHAR(64), OUT id int)
BEGIN
	INSERT INTO category (name)
    VALUES (LOWER(nameparam));
    SELECT LAST_INSERT_ID() as id;
END$$

DROP PROCEDURE IF EXISTS createComponent$$
CREATE PROCEDURE `createComponent`(
	IN componentTypeId INT,
    IN _status VARCHAR(64),
	IN _type int,
	IN _comment VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
    OUT id int)
BEGIN
	START TRANSACTION;
		INSERT INTO componentJournal VALUES(NULL, _type ,_status, _comment);
		SET @journalEntry = LAST_INSERT_ID();
        INSERT INTO componentStaticId VALUES(NULL, @journalEntry, componentTypeId);
        SET id = LAST_INSERT_ID();
		INSERT INTO componentLog VALUES(NULL, @journalEntry, LAST_INSERT_ID(), userID, UNIX_TIMESTAMP(NOW()), logComment, "created");
	COMMIT;
END$$

-- Create new component type
DROP PROCEDURE IF EXISTS createComponentType$$
CREATE PROCEDURE `createComponentType`(
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
	START TRANSACTION;
		INSERT INTO componentTypeJournal (`name`, categoryID, `storage`, description)
		VALUES (nameparam, categoryId, `storageparam`, description);
		SET @journalEntry = LAST_INSERT_ID(); 
		INSERT INTO componentTypeStaticId VALUES (NULL, activeProjectIDParam, @journalEntry);
		set id = LAST_INSERT_ID();
		INSERT INTO componentTypeLog 
		VALUES (NULL, @journalEntry, id, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'created');
	COMMIT;
END$$

-- Create new document

DROP PROCEDURE IF EXISTS createDocument$$
CREATE PROCEDURE `createDocument`(
	IN _filename VARCHAR(64),
    IN _activeComponentTypeID INT(11),
    IN _bucketpath VARCHAR(1024),
    IN _description VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
	OUT id INT
)
BEGIN  
START TRANSACTION;
	INSERT INTO documentJournal VALUES (NULL, _bucketpath, _filename, _description);
	SET @newID = LAST_INSERT_ID();
	INSERT INTO documentStaticId VALUES(NULL, @newID, _activeComponentTypeID);
    set id = LAST_INSERT_ID();
	INSERT INTO documentLog VALUES(NULL, @newID, id, userID, UNIX_TIMESTAMP(NOW()), logComment, "created");
END$$

-- Create new project

DROP PROCEDURE IF EXISTS createProject$$
CREATE PROCEDURE `createProject`(IN nameparam VARCHAR(64), IN userid VARCHAR(32), IN commentparam VARCHAR(512), OUT id INT)
BEGIN 
START TRANSACTION;
	INSERT INTO projectJournal (name)
	VALUES (LOWER(nameparam));
    
    SET @newID = LAST_INSERT_ID();
    
    INSERT INTO projectStaticIds (projectID)
	VALUES (@newID);
	SET id = LAST_INSERT_ID();
    INSERT INTO projectLog 
    VALUES (NULL, id, @newID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'created'); 
END$$

-- Create new role
DROP PROCEDURE IF EXISTS createRole$$
CREATE PROCEDURE `createRole`(IN nameParam VARCHAR(32))
BEGIN
	INSERT INTO role (rolename)
	Values (LOWER(nameParam));    
END$$

-- Create new user

DROP PROCEDURE IF EXISTS `createUser`$$
CREATE PROCEDURE `createUser`(in ID varchar(32), in firstnameparam VARCHAR(32), in lastnameparam varchar(32), IN phonenumberparam INT, IN passwordparam VARCHAR(1024))
BEGIN
	START TRANSACTION;
		insert into user values(ID, firstnameparam, lastnameparam, phonenumberparam, passwordparam, 0);
    COMMIT;
END$$


-- delete component
DROP PROCEDURE IF EXISTS deleteComponent$$
CREATE PROCEDURE `deleteComponent`(
	IN ID INT,
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512))
BEGIN
	START TRANSACTION;
		SELECT activeComponentId into @oldID from componentStaticId where componentStaticId.ID = ID;
        update componentStaticId set activeComponentId = NULL WHERE componentJournal.ID = ID;
        INSERT INTO componentLog VALUES(NULL, @oldID, activeID, userID, UNIX_TIMESTAMP(NOW()), logComment, 'deleted');
	COMMIT;
END$$

-- delete component type
DROP PROCEDURE IF EXISTS deleteComponenttype$$
CREATE PROCEDURE `deleteComponenttype`(
	IN componentTypeId INT(11), 
    IN userid VARCHAR(32), 
    IN commentparam VARCHAR(512))
BEGIN
	START TRANSACTION;
        Select activeComponentID into @oldComponentTypeID from componentTypeStaticId where componentTypeStaticId.ID = componentTypeId;
        update componentTypeStaticId set activeComponentID = null where componentTypeStaticId.ID = componentTypeId;
        INSERT INTO componentTypeLog VALUES(NULL, @oldComponentTypeID, activeComponentTypeID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'deleted');
	COMMIT;
END$$

-- delete document
DROP PROCEDURE IF EXISTS deleteDocument$$
CREATE PROCEDURE `deleteDocument`(
	IN documentId INT,
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512))
BEGIN
	START TRANSACTION;
		Select activeDocumentID into @oldDocumentID from documentStaticId where ID = documentId;
		update documentStaticId set docactiveDocumentIDumentID = null where ID = documentId;
		INSERT INTO documentLog VALUES(NULL, @oldDocumentID, documentId, userID, UNIX_TIMESTAMP(NOW()), logComment, 'deleted');
	COMMIT;
END$$

-- delete project
DROP PROCEDURE IF EXISTS deleteProject$$
CREATE PROCEDURE `deleteProject`(IN projectId INT(11), IN userid VARCHAR(32), IN commentParam VARCHAR(512))
BEGIN 
	START TRANSACTION;
        Select projectID into @oldProjectID from projectStaticIds where ID = projectId;
		update projectStaticIds set activeProjectId = null where ID=projectId;
		INSERT INTO projectLog VALUES(null, projectId, @oldProjectID, userid, UNIX_TIMESTAMP(NOW()), commentParam, 'deleted');
	COMMIT;
END$$

DROP PROCEDURE IF EXISTS rollbackComponent$$
CREATE PROCEDURE `rollbackComponent`(IN revisionID INT, in userID int, in commentParam VARCHAR(512))
BEGIN
	START TRANSACTION;
		select activeComponentID into @staticId from componentLog where componentLogID = revisionID;
		select activeComponentId into @oldComponentID from componentStaticId where ID = @activeID;
		select affectedJournalEntry into @ID from componentLog where componentLogID = revisionID;

		update componentStaticId set activeComponentId = @ID where ID=@staticId;
		
		INSERT INTO componentLog VALUES (null, @oldComponentID, @staticId, userID, UNIX_TIMESTAMP(NOW()), commentParam, 'rollback'); 
	COMMIT;
END$$

DROP PROCEDURE rollbackComponentType$$
CREATE PROCEDURE `rollbackComponentType`(in revisionId int, in userID int, in commentParam VARCHAR(512))
BEGIN
	START TRANSACTION;
		select componentTypeID into @staticId from componentTypeLog where revision = revisionId;
		select activeComponentID into @oldComponentTypeID from componentTypeStaticId where ID = @staticId;
		select affectedJournalEntry into @ID from componentTypeLog where componentTypeLogID = revisionId;

		update componentTypeStaticId set activeComponentID = @ID where componentTypeStaticId.ID = @staticId;
			
		INSERT INTO componentTypeLog
		VALUES (null, @oldComponentTypeID, @staticId, userID, UNIX_TIMESTAMP(NOW()), commentParam, 'rollback'); 
	COMMIT;
END$$

DROP PROCEDURE rollbackDocument$$
CREATE PROCEDURE `rollbackDocument`(in logID int, in userID int, in commentParam VARCHAR(512), out err int)
BEGIN
START TRANSACTION;
	select activeDocumentID into @activeID from documentLog where documentLogID = logID;
	select activeDocumentID into @oldDocumentID from documentStaticId where ID = @activeID;
	select documentID into @ID from documentLog where documentLogID = logID;
    
    update documentStaticId set activeDocumentID = @ID where activeDocument.ID=@activeID;
    
    INSERT INTO documentLog
		VALUES (null, @oldDocumentID, @activeID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'rollback'); 
END$$

DROP PROCEDURE rollbackProject$$
CREATE PROCEDURE `rollbackProject`(IN logID int, IN userID int, IN commentparam VARCHAR(512), out err int)
BEGIN
START TRANSACTION;
	select projectId into @staticId from projectLog where projectLog.revisionNumber = logID;
	select activeProjectId into @oldProjectID from projectStaticIds where ID = @staticId;
	select affectedJournalEntry into @newActiveId from projectLog where projectLog.projectLogID = logID;

    update projectStaticIds set activeProjectId = @newActiveId where activeProprojectStaticIdsjects.ID=@staticId;
    
    INSERT INTO projectLog
		VALUES (null, @staticId, @oldProjectID, userID, UNIX_TIMESTAMP(NOW()), commentparam, 'rollback'); 
END$$

DROP PROCEDURE updateComponent$$
CREATE PROCEDURE `updateComponent`(
	IN componentId INT,
    IN _status VARCHAR(64),
	IN associatedComponentType INT,
	IN _comment VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
    out err int)
BEGIN
	START TRANSACTION;
		INSERT INTO componentJournal VALUES(NULL, associatedComponentType, _status, _comment);
		set @newEntry = last_insert_id();
        SELECT activeComponentId INTO @oldID from componentStaticId WHERE ID = componentId;
        UPDATE componentStaticId SET activeComponent.activeComponentId = @newEntry WHERE activeComponent.ID = componentId;
		INSERT INTO componentLog VALUES(NULL, @oldID, componentId, userID, UNIX_TIMESTAMP(NOW()), logComment, 'updated');
	COMMIT;
END$$

DROP PROCEDURE updateComponentType$$
CREATE PROCEDURE `updateComponentType`(IN nameparam VARCHAR(64), IN activeID int(11), IN categoryId int(11), 
									   IN storageparam int(11), IN descriptionparam varchar(512), IN userid varchar(32), 
									   IN commentparam varchar(512), OUT err int)
BEGIN
	START TRANSACTION;
		INSERT INTO componentTypeJournal VALUES(NULL, nameparam, categoryId, storageparam, descriptionparam);
		SELECT activeComponentID INTO @oldID from componentTypeStaticId WHERE ID = activeID;
		set @newEntry = LAST_INSERT_ID();
		UPDATE componentTypeStaticId SET componentTypeStaticId.activeComponentID = @newEntry WHERE componentTypeStaticId.ID = activeID;
		INSERT INTO componentTypeLog VALUES(NULL, @newEntry, @oldID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'updated');
	COMMIT;
END$$

DROP PROCEDURE updateDocument$$
CREATE PROCEDURE `updateDocument`(
	IN activeID INT,
    IN filename VARCHAR(64),
	IN bucketPath VARCHAR(1024),
	IN description VARCHAR(512),
    IN userID VARCHAR(32),
    IN logComment VARCHAR(512),
    out err int)
BEGIN
	START TRANSACTION;
		INSERT INTO documentJournal VALUES(NULL, filename, bucketPath, description);
        SELECT documentID INTO @oldID FROM documentStaticId WHERE ID = activeID;
        UPDATE documentStaticId SET documentStaticId.activeDocumentID = last_insert_id() WHERE ID = activeID;
		INSERT INTO documentLog VALUES(NULL, @oldID, activeID, userID, UNIX_TIMESTAMP(NOW()), logComment, 'updated');
	COMMIT;
END$$

DROP PROCEDURE updateProject$$
CREATE PROCEDURE `updateProject`(IN nameparam VARCHAR(64), IN activeProjectID int(11), IN userid varchar(32), IN commentparam varchar(512), OUT err INT)
BEGIN 
	START TRANSACTION;
	    insert into projectJournal Values (NULL, nameparam);
		set @newProjectID = LAST_INSERT_ID();
		select activeProjectId into @oldProjectID from projectStaticIds where ID = activeProjectID;

		update activeProjects SET projectID = @newProjectID where ID = activeProjectID; 

		INSERT INTO projectLog
		VALUES (NULL, activeProjectID, @oldProjectID, userid, UNIX_TIMESTAMP(NOW()), commentparam, 'updated');
	COMMIT;
END$$

DROP PROCEDURE updateUser$$
CREATE PROCEDURE `updateUser`(IN userID VARCHAR(32), IN firstnameparam VARCHAR(32), IN lastnameparam VARCHAR(32), IN phonenumberparam VARCHAR(32))
BEGIN
UPDATE user
SET firstname = firstnameparam, lastname = lastnameparam, phonenumber = phonenumberparam
WHERE user.userID = userID;
END$$