DELETE FROM documentLog;
DELETE FROM documentStaticId;
DELETE FROM documentJournal;

DELETE FROM componentLog;
DELETE FROM componentStaticId;
DELETE FROM componentJournal;

DELETE FROM componentTypeLog;
DELETE FROM componentTypeStaticId;
DELETE FROM componentTypeJournal;

DELETE FROM projectRoles;

DELETE FROM projectLog;
DELETE FROM projectStaticIds;
DELETE FROM projectJournal;

DELETE FROM categories;

DELETE FROM `user`;



INSERT INTO categories VALUES(1, "Cat1");
INSERT INTO categories VALUES(2, "Cat2");
INSERT INTO categories VALUES(3, "Cat3");
INSERT INTO categories VALUES(4, "Cat4");

CALL createUser('SUPERADMIN', 'FirstName', 'LastName', 00000000, 'password');
UPDATE `user` SET superuser = '1' WHERE user.userID = 'SUPERADMIN';
CALL createUser("ADMIN", "FirstName", "LastName", 00000000, "password");
CALL createUser("STUDENT", "FirstName", "LastName", 00000000, "password");
CALL createUser("GUEST", "FirstName", "LastName", 00000000, "password");
CALL createUser("PROHIBITED", "FirstName", "LastName", 00000000, "password");

CALL createProject("MyProject", "SUPERADMIN", "Test project", @projectId);

CALL grantUserProjectRole("ADMIN", @projectId, 4);
CALL grantUserProjectRole("STUDENT", @projectId, 3);
CALL grantUserProjectRole("GUEST", @projectId, 2);
CALL grantUserProjectRole("PROHIBITED", @projectId, 1);

CALL createComponentType("Component1", @projectId, 1, 1, "Test Component Type", "STUDENT",  "Test Component Type", @componentTypeId);

CALL createComponent(@componentTypeId, "Available", "Test Component", "STUDENT", "Test Component", @componentId);
CALL createDocument("TestFile.dat", @componentTypeId, "Somewhere", "Test Document", "STUDENT", "Test Document", @documentId);