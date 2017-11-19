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
   phonenumber  VARCHAR(32) NOT NULL,
   PRIMARY KEY (userID)
);

CREATE TABLE `role` (
    roleID      INT NOT NULL AUTO_INCREMENT,
    rolename    VARCHAR(32) NOT NULL UNIQUE,
    PRIMARY KEY (roleID)
); 

CREATE TABLE `project`(
    projectID   INT NOT NULL AUTO_INCREMENT,
    `name`      VARCHAR(64) NOT NULL UNIQUE,
    PRIMARY KEY (projectID)
);

CREATE TABLE `project_relationship` (
    userID      VARCHAR(32) NOT NULL,
    roleID      INT NOT NULL,
    projectID   INT NOT NULL,
    FOREIGN KEY (userID) REFERENCES `user`(userID),
    FOREIGN KEY (roleID) REFERENCES `role`(roleID),
    FOREIGN KEY (projectID) REFERENCES `project`(projectID)
);

CREATE TABLE `category`(
    categoryID  INT NOT NULL AUTO_INCREMENT,
    `name`      VARCHAR(64) NOT NULL UNIQUE,
    PRIMARY KEY (categoryID)
);


CREATE TABLE `componentType`(
    componentTypeID INT NOT NULL AUTO_INCREMENT,
    `name`          VARCHAR(64) NOT NULL UNIQUE,
    categoryID      INT NOT NULL,
    `storage`       INT NOT NULL,
    `description`   VARCHAR(512) NOT NULL,
    PRIMARY KEY (componentTypeID),
    FOREIGN KEY (categoryID) REFERENCES `category`(categoryID)
);

CREATE TABLE `component` (
    componentID         INT NOT NULL AUTO_INCREMENT,
    `status`            VARCHAR(64) NOT NULL,
    `comment`           VARCHAR(512) NOT NULL,
    PRIMARY KEY (componentID)
);

CREATE TABLE `document`(
    documentID              INT NOT NULL AUTO_INCREMENT,
    filename                VARCHAR(64) NOT NULL,
    bucketpath              VARCHAR(1024) NOT NULL,
    description             VARCHAR(512) NOT NULL,
    PRIMARY KEY (documentID)
    
);


CREATE TABLE activeProjects (
    ID          INT NOT NULL AUTO_INCREMENT,
    projectID   INT, 
    PRIMARY KEY (ID),
    FOREIGN KEY (projectID) REFERENCES `project`(projectID)
);

CREATE TABLE `projectLog` (
    projectLogID        INT NOT NULL AUTO_INCREMENT,
    activeProjectID	 	INT NOT NULL,
    projectID           INT,
    userID              VARCHAR(32) NOT NULL,
    `timestamp`         INT NOT NULL,
    `comment`           VARCHAR(512) NOT NULL,
    `type`              ENUM('created', 'deleted', 'updated', 'rollback'),
    PRIMARY KEY (projectLogID),
    FOREIGN KEY (projectID) REFERENCES `project`(projectID),
	FOREIGN KEY (activeProjectID) REFERENCES `activeProjects`(ID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE activeComponentType (
    ID                  INT NOT NULL AUTO_INCREMENT,
    activeProjectID     INT NOT NULL,
    componentTypeID     INT,
    PRIMARY KEY (ID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID),
    FOREIGN KEY (activeProjectID) REFERENCES `activeProjects`(ID)
);

CREATE TABLE `componentTypeLog` (
    componentTypeLogID      INT NOT NULL AUTO_INCREMENT,
    componentTypeID         INT,
    activeComponentTypeID   INT NOT NULL,
    userID                  VARCHAR(32) NOT NULL,
    `timestamp`             INT NOT NULL,
    `comment`               VARCHAR(512) NOT NULL,
    `type`                  ENUM('created', 'deleted', 'updated', 'rollback'),
    PRIMARY KEY (componentTypeLogID),
    FOREIGN KEY (activeComponentTypeID) REFERENCES `activeComponentType`(ID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE activeComponent (
    ID                  INT NOT NULL AUTO_INCREMENT,
    componentID         INT,
    activeComponentTypeID     INT NOT NULL,
    PRIMARY KEY (ID),
    FOREIGN KEY (componentID) REFERENCES `component`(componentID),
    FOREIGN KEY (activeComponentTypeID) REFERENCES `activeComponentType`(ID)
);

CREATE TABLE `componentLog` (
    componentLogID          INT NOT NULL AUTO_INCREMENT,
    componentID             INT,
    activeComponentID      INT NOT NULL,
    userID                  VARCHAR(32) NOT NULL,
    `timestamp`             INT NOT NULL,
    `comment`               VARCHAR(512) NOT NULL,
    `type`                  ENUM('created', 'deleted', 'updated',  'rollback'),
    PRIMARY KEY (componentLogID),
    FOREIGN KEY (componentID) REFERENCES `component`(componentID),
    FOREIGN KEY (activeComponentID) REFERENCES `activeComponent`(ID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE activeDocument (
    ID                  INT NOT NULL AUTO_INCREMENT,
    documentID          INT,
    activeComponentTypeID     INT NOT NULL,
    PRIMARY KEY (ID),
    FOREIGN KEY (documentID) REFERENCES `document`(documentID),
    FOREIGN KEY (activeComponentTypeID) REFERENCES `activeComponentType`(ID)
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
    FOREIGN KEY (activeDocumentID) REFERENCES `activeDocument`(ID),
	FOREIGN KEY (documentID) REFERENCES `document`(documentID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);


CREATE TABLE friendshipComponent (
    ID          INT NOT NULL AUTO_INCREMENT,
    friendLeft  INT NOT NULL, 
    friendRight INT NOT NULL, 
    projectID   INT NOT NULL,   
    PRIMARY KEY (ID),
    FOREIGN KEY (friendLeft) REFERENCES `activeComponent`(ID),
    FOREIGN KEY (friendRight) REFERENCES `activeComponent`(ID),
    FOREIGN KEY (projectID) REFERENCES `project`(projectID)
);
    
