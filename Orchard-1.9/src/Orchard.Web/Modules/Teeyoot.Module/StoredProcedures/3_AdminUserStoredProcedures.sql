IF OBJECT_ID('GetUsers', 'P') IS NOT NULL
	DROP PROCEDURE GetUsers
GO

CREATE PROCEDURE GetUsers	
	@RoleId INT = NULL,
	@Skip INT,
	@Take INT
AS
SET NOCOUNT ON
DECLARE @SQLQuery NVARCHAR(MAX)
DECLARE @ParamDefinition NVARCHAR(MAX)

SET @SQLQuery = ' SELECT' +
	' UserPartRecord.UserName UserName,' +
	' UserPartRecord.Id UserId,' +
	' CurrencyRecord.Name CurrencyName,' +
	' CAST(' +
		' CASE WHEN TeeyootUserPartRecord.Id IS NOT NULL' +
		' THEN 1' +
		' ELSE 0' +
		' END AS BIT) IsTeeyootUser' +
	' FROM' +
		' Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord' +
		' JOIN Orchard_Framework_ContentItemRecord ContentItemRecord' +
		' ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id' +
		' JOIN Orchard_Users_UserPartRecord UserPartRecord' +
		' ON ContentItemRecord.Id = UserPartRecord.Id'
IF @RoleId IS NOT NULL
BEGIN
	SET @SQLQuery = @SQLQuery +
		' JOIN Orchard_Roles_UserRolesPartRecord UserRolesPartRecord' +
		' ON UserPartRecord.Id = UserRolesPartRecord.UserId' +
		' AND UserRolesPartRecord.Role_id = @RoleId'
END
SET @SQLQuery = @SQLQuery +
		' LEFT JOIN Teeyoot_Module_TeeyootUserPartRecord TeeyootUserPartRecord' +
		' ON UserPartRecord.Id = TeeyootUserPartRecord.Id' +
		' LEFT JOIN Teeyoot_Module_CurrencyRecord CurrencyRecord' +
		' ON TeeyootUserPartRecord.CurrencyRecord_Id = CurrencyRecord.Id' +
	' WHERE' +
		' ContentItemVersionRecord.Published = 1'+
	' ORDER BY' +
		' UserPartRecord.Id' +
	' OFFSET' +
		' @Skip ROWS' +
	' FETCH NEXT' +
		' @Take ROWS ONLY'

SET @ParamDefinition = 
	N'@RoleId INT,
	@Skip INT,
	@Take INT'

EXECUTE sp_executesql @SQLQuery, @ParamDefinition,
	@RoleId,
	@Skip,
	@Take
GO

IF OBJECT_ID('GetUsersCount', 'P') IS NOT NULL
	DROP PROCEDURE GetUsersCount
GO

CREATE PROCEDURE GetUsersCount
	@RoleId INT = NULL
AS
SET NOCOUNT ON
DECLARE @SQLQuery NVARCHAR(MAX)
DECLARE @ParamDefinition NVARCHAR(MAX)

SET @SQLQuery = ' SELECT' +
	' COUNT(*)' +
	' FROM' +
		' Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord' +
		' JOIN Orchard_Framework_ContentItemRecord ContentItemRecord' +
		' ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id' +
		' JOIN Orchard_Users_UserPartRecord UserPartRecord' +
		' ON ContentItemRecord.Id = UserPartRecord.Id'
IF @RoleId IS NOT NULL
BEGIN
	SET @SQLQuery = @SQLQuery +
		' JOIN Orchard_Roles_UserRolesPartRecord UserRolesPartRecord' +
		' ON UserPartRecord.Id = UserRolesPartRecord.UserId' +
		' AND UserRolesPartRecord.Role_id = @RoleId'
END
SET @SQLQuery = @SQLQuery +
	' WHERE' +
		' ContentItemVersionRecord.Published = 1'

SET @ParamDefinition = 
	N'@RoleId INT'

EXECUTE sp_executesql @SQLQuery, @ParamDefinition,
	@RoleId
GO

IF OBJECT_ID('GetUsersRoles', 'P') IS NOT NULL
	DROP PROCEDURE GetUsersRoles
GO

CREATE PROCEDURE GetUsersRoles
	@UserIds INTEGER_LIST_TABLE_TYPE READONLY
AS
SET NOCOUNT ON
SELECT
	N UserId,
	RoleRecord.Name RoleName
FROM
	@UserIds Users
	JOIN Orchard_Roles_UserRolesPartRecord UserRolesPartRecord
	ON Users.N = UserRolesPartRecord.UserId
	JOIN Orchard_Roles_RoleRecord RoleRecord
	ON UserRolesPartRecord.Role_id = RoleRecord.Id
ORDER BY
	RoleRecord.Id
GO