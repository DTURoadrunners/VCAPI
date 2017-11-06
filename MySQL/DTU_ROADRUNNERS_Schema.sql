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
    rolename    VARCHAR(32) NOT NULL,
    PRIMARY KEY (roleID)
); 

CREATE TABLE `project`(
    projectID   INT NOT NULL AUTO_INCREMENT,
    `name`      VARCHAR(64) NOT NULL,
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
    `name`      VARCHAR(64) NOT NULL,
    PRIMARY KEY (categoryID)
);


CREATE TABLE `componentType`(
    componentTypeID INT NOT NULL AUTO_INCREMENT,
    `name`          VARCHAR(64) NOT NULL,
    categoryID      INT NOT NULL,
    `storage`       INT NOT NULL,
    `description`   VARCHAR(512) NOT NULL,
    PRIMARY KEY (componentTypeID),
    FOREIGN KEY (categoryID) REFERENCES `category`(categoryID)
);

CREATE TABLE `component` (
    componentID         INT NOT NULL AUTO_INCREMENT,
    componentTypeID     INT NOT NULL,
    `status`            VARCHAR(64) NOT NULL,
    `comment`           VARCHAR(512) NOT NULL,
    PRIMARY KEY (componentID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID)
);

CREATE TABLE `document`(
    documentID      INT NOT NULL AUTO_INCREMENT,
    componentTypeID INT NOT NULL,
    filename        VARCHAR(64) NOT NULL,
    bucketpath      VARCHAR(1024) NOT NULL,
    description     VARCHAR(512) NOT NULL,
    PRIMARY KEY (documentID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID)
    
);

CREATE TABLE `projectLog` (
    projectLogID    INT NOT NULL AUTO_INCREMENT,
    projectID       INT NOT NULL,
    userID          VARCHAR(32) NOT NULL,
    `timestamp`     datetime NOT NULL,
    `comment`       VARCHAR(512) NOT NULL,
    `type`          VARCHAR(16) NOT NULL,
    PRIMARY KEY (projectLogID),
    FOREIGN KEY (projectID) REFERENCES `project`(projectID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE `componentTypeLog` (
    componentTypeLogID  INT NOT NULL AUTO_INCREMENT,
    componentTypeID     INT NOT NULL,
    oldComponentTypeID  INT NOT NULL,
    userID              VARCHAR(32) NOT NULL,
    `timestamp`         datetime NOT NULL,
    `comment`           VARCHAR(512) NOT NULL,
    `type`              VARCHAR(16) NOT NULL,
    PRIMARY KEY (componentTypeLogID),
    FOREIGN KEY (oldComponentTypeID) REFERENCES `componentType`(componentTypeID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE `componentLog` (
    componentLogID  INT NOT NULL AUTO_INCREMENT,
    componentID         INT NOT NULL,
    oldComponentID INT NOT NULL,
    userID          VARCHAR(32) NOT NULL,
    `timestamp`     datetime NOT NULL,
    `comment`       VARCHAR(512) NOT NULL,
    `type`          VARCHAR(16) NOT NULL,
    PRIMARY KEY (componentLogID),
    FOREIGN KEY (componentID) REFERENCES `component`(componentID),
    FOREIGN KEY (oldComponentID) REFERENCES `component`(componentID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE `documentLog` (
    documentLogID   INT NOT NULL AUTO_INCREMENT,
    documentID      INT NOT NULL,
    oldDocumentID      INT NOT NULL,
    userID          VARCHAR(32) NOT NULL,
    `timestamp`     datetime NOT NULL,
    `comment`       VARCHAR(512) NOT NULL,
    `type`          VARCHAR(16) NOT NULL,
    PRIMARY KEY (documentLogID),
    FOREIGN KEY (oldDocumentID) REFERENCES `document`(documentID),
    FOREIGN KEY (userID) REFERENCES `user`(userID)
);

CREATE TABLE activeComponentType (
    projectID INT NOT NULL,
    componentTypeID INT NOT NULL,
    FOREIGN KEY (projectID) REFERENCES `project`(projectID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID)
);

CREATE TABLE activeDocument (
    documentID INT NOT NULL,
    componentTypeID INT NOT NULL,
    FOREIGN KEY (documentID) REFERENCES `document`(documentID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID)
);

CREATE TABLE activeComponent (
    componentID INT NOT NULL,
    componentTypeID INT NOT NULL,
    FOREIGN KEY (componentID) REFERENCES `component`(componentID),
    FOREIGN KEY (componentTypeID) REFERENCES `componentType`(componentTypeID)
);

    
