IF TYPE_ID('INTEGER_LIST_TABLE_TYPE') IS NOT NULL
BEGIN
	/* Firts drop stored procedures that depends on this type */
	IF OBJECT_ID('GetCampaignsFirstProductData', 'P') IS NOT NULL
		DROP PROCEDURE GetCampaignsFirstProductData
	IF OBJECT_ID('GetUsersRoles', 'P') IS NOT NULL
		DROP PROCEDURE GetUsersRoles
	/* Drop type itself */
	DROP TYPE INTEGER_LIST_TABLE_TYPE
END
GO

CREATE TYPE INTEGER_LIST_TABLE_TYPE AS TABLE(N INT NOT NULL PRIMARY KEY)
GO