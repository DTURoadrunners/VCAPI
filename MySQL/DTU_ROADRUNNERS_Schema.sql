#CREATE DATABASE DTU_Roadrunners ;

USE DTU_Roadrunners;

## DROP EVERYTHING DON'T REMOVE USE DTU_Roadrnners!!!!
    SET FOREIGN_KEY_CHECKS = 0;
    SET GROUP_CONCAT_MAX_LEN=32768;
    SET @tables = NULL;
    SELECT GROUP_CONCAT('`', table_name, '`') INTO @tables
      FROM information_schema.tables
      WHERE table_schema = (SELECT DATABASE());
    SELECT IFNULL(@tables,'dummy') INTO @tables;

    SET @tables = CONCAT('DROP TABLE IF EXISTS ', @tables);
    PREPARE stmt FROM @tables;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
    SET FOREIGN_KEY_CHECKS = 1;
# END DROP EVERYTHING

CREATE TABLE `user` (
   userID       VARCHAR(32) NOT NULL,
   firstname    VARCHAR(32) NOT NULL, 
   lastname     VARCHAR(32) NOT NULL,
   phonenumber  INT NOT NULL,
   password 	VARCHAR(1024) NOT NULL,
   superuser    boolean NOT NULL DEFAULT(false), 
   PRIMARY KEY (userID)
);

CREATE TABLE `role` (
    roleID      INT NOT NULL AUTO_INCREMENT,
    rolename    VARCHAR(32) NOT NULL UNIQUE,
    PRIMARY KEY (roleID)
); 

CREATE TABLE `categories`(
    categoryID  INT NOT NULL AUTO_INCREMENT,
    `name`      VARCHAR(64) NOT NULL UNIQUE,
    PRIMARY KEY (categoryID)
);

CREATE TABLE `projectJournal`(
    projectID   INT NOT NULL AUTO_INCREMENT,
    `name`      VARCHAR(64) NOT NULL UNIQUE,
    PRIMARY KEY (projectID)
);


CREATE TABLE `projectStaticIds` (
    ID          INT NOT NULL AUTO_INCREMENT,
    activeProjectId   INT, 
    PRIMARY KEY (ID),
    FOREIGN KEY (activeProjectId) REFERENCES `projectJournal`(projectID)
);

CREATE TABLE `projectLog` (
    revisionNumber          INT NOT NULL AUTO_INCREMENT,
    projectId	 	        INT NOT NULL,
    affectedJournalEntry    INT,
    userID                  VARCHAR(32) NOT NULL,
    `timestamp`             INT NOT NULL,
    `comment`               VARCHAR(512) NOT NULL,
    `type`                  ENUM('created', 'deleted', 'updated', 'rollback'),
    PRIMARY KEY (revisionNumber),
    FOREIGN KEY (affectedJournalEntry) REFERENCES `projectJournal`(projectID),
	FOREIGN KEY (projectId) REFERENCES `projectStaticIds`(ID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE `projectRoles` (
    userID      VARCHAR(32) NOT NULL,
    roleID      INT NOT NULL,
    projectID   INT NOT NULL,
    FOREIGN KEY (userID) REFERENCES `user`(userID),
    FOREIGN KEY (roleID) REFERENCES `role`(roleID),
    FOREIGN KEY (projectID) REFERENCES `projectStaticIds`(ID)
);

CREATE view `project` AS SELECT `projectStaticIds`.`ID`, `name` 
FROM `projectStaticIds` join `projectJournal` ON `projectStaticIds`.`ID` = `projectJournal`.`projectID`;

CREATE TABLE `componentTypeJournal`(
    componentTypeID INT NOT NULL AUTO_INCREMENT,
    `name`          VARCHAR(64) NOT NULL UNIQUE,
    categoryID      INT NOT NULL,
    `storage`       INT NOT NULL,
    `description`   VARCHAR(512) NOT NULL,
    PRIMARY KEY (componentTypeID),
    FOREIGN KEY (categoryID) REFERENCES `categories`(categoryID)
);

CREATE TABLE `componentTypeStaticId` (
    ID                  INT NOT NULL AUTO_INCREMENT,
    activeComponentID   INT,
    associatedProject   INT NOT NULL,   
    PRIMARY KEY (ID),
    FOREIGN KEY (associatedProject) REFERENCES `projectStaticIds`(ID),
    FOREIGN KEY (activeComponentID) REFERENCES `componentTypeJournal`(componentTypeID)
);

CREATE TABLE `componentTypeLog` (
    revision                INT NOT NULL AUTO_INCREMENT,
    componentTypeID         INT,
    affectedJournalEntry    INT NOT NULL,
    userID                  VARCHAR(32) NOT NULL,
    `timestamp`             INT NOT NULL,
    `comment`               VARCHAR(512) NOT NULL,
    `type`                  ENUM('created', 'deleted', 'updated', 'rollback'),
    PRIMARY KEY (revision),
    FOREIGN KEY (componentTypeID) REFERENCES `componentTypeStaticId`(ID),
    FOREIGN KEY (affectedJournalEntry) REFERENCES `componentTypeJournal`(componentTypeID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE view `componentTypes` AS SELECT `ID`, `name`, `categoryID`, `storage`, `description` 
FROM `componentTypeStaticId` join `componentTypeJournal` ON `componentTypeStaticId`.`ID` = `componentTypeJournal`.`componentTypeID`;

CREATE TABLE `componentJournal` (
    componentID         INT NOT NULL AUTO_INCREMENT,
    typeID              INT NOT NULL,
    `status`            VARCHAR(64) NOT NULL,
    `comment`           VARCHAR(512) NOT NULL,
    PRIMARY KEY (componentID),
    FOREIGN KEY (typeID) REFERENCES `componentTypeStaticId`(ID)
);

CREATE TABLE `componentStaticId` (
    ID                  INT NOT NULL AUTO_INCREMENT,
    activeComponentId   INT,
    componentTypeId     INT NOT NULL,
    PRIMARY KEY (ID),
    FOREIGN KEY (componentTypeId) REFERENCES `componentTypeStaticId`(ID),
    FOREIGN KEY (activeComponentId) REFERENCES `componentJournal`(componentID)
);

CREATE TABLE `componentLog` (
    componentLogID          INT NOT NULL AUTO_INCREMENT,
    affectedJournalEntry    INT,
    activeComponentID      	INT NOT NULL,
    userID                  VARCHAR(32) NOT NULL,
    `timestamp`             INT NOT NULL,
    `comment`               VARCHAR(512) NOT NULL,
    `type`                  ENUM('created', 'deleted', 'updated',  'rollback'),
    PRIMARY KEY (componentLogID),
    FOREIGN KEY (affectedJournalEntry) REFERENCES `componentJournal`(componentID),
    FOREIGN KEY (activeComponentID) REFERENCES `componentStaticId`(ID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE view `components` AS SELECT 'ID', `typeID`, `status`, `comment` 
FROM `componentStaticId` join `componentJournal` ON `componentStaticId`.`ID` = `componentJournal`.`componentTypeID`;

CREATE TABLE `documentJournal`(
    documentID              INT NOT NULL AUTO_INCREMENT,
    bucketpath              VARCHAR(1024) NOT NULL,
    filename                VARCHAR(64) NOT NULL,
    description             VARCHAR(512) NOT NULL,
    PRIMARY KEY (documentID) 
);

CREATE TABLE documentStaticId (
    ID                  INT NOT NULL AUTO_INCREMENT,
    activeDocumentID          INT,
    associatedComponentTypeID    INT NOT NULL,
    PRIMARY KEY (ID),
    FOREIGN KEY (activeDocumentID) REFERENCES `documentJournal`(documentID),
    FOREIGN KEY (associatedComponentTypeID) REFERENCES `componentTypeStaticId`(ID)
);

CREATE TABLE `documentLog` (
    documentLogID       INT NOT NULL AUTO_INCREMENT,
    documentID          INT,
    activeDocumentID    INT NOT NULL,
    userID              VARCHAR(32) NOT NULL,
    `timestamp`         INT NOT NULL,
    `comment`           VARCHAR(512) NOT NULL,
    `type`              ENUM('created', 'deleted', 'updated', 'rollback'),
    PRIMARY KEY (documentLogID),
    FOREIGN KEY (activeDocumentID) REFERENCES `documentStaticId`(ID),
	FOREIGN KEY (documentID) REFERENCES `documentJournal`(documentID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE view `documents` AS SELECT 'ID', `bucketpath`, `filename`, `description` 
FROM `documentStaticId` join `documentJournal` ON `documentStaticId`.`ID` = `documentJournal`.`documentID`;

insert into role VALUES(NULL, 'PROHIBITED');
insert into role VALUES(NULL, 'GUEST');
insert into role VALUES(NULL, 'STUDENT');
insert into role VALUES(NULL, 'ADMIN');
insert into role VALUES(NULL, 'SUPERADMIN');
