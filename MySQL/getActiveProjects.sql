CREATE DEFINER=`root`@`localhost` PROCEDURE `getActiveProjects`()
BEGIN
	select * from activeProjects;
END