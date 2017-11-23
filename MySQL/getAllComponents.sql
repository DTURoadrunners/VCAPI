CREATE DEFINER=`root`@`localhost` PROCEDURE `getAllComponents`(IN componentTypeID INT)
BEGIN
	SELECT component.componentID, componentTypeID, status, comment FROM activeComponent
    INNER JOIN component ON activeComponent.componentID = component.componentID
	WHERE activeComponent.componentTypeID = componentTypeID;
END