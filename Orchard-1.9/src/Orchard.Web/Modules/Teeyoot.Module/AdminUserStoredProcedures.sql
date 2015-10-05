IF OBJECT_ID('GetUsers', 'P') IS NOT NULL
	DROP PROCEDURE GetUsers
GO

CREATE PROCEDURE GetUsers
	@Skip INT,
	@Take INT
AS
SET NOCOUNT ON
SELECT 
	UserPartRecord.UserName UserName,
	UserPartRecord.Id UserId,
	CurrencyRecord.Name CurrencyName,
	CAST(CASE WHEN TeeyootUserPartRecord.Id IS NOT NULL 
		THEN 1 
		ELSE 0 
		END AS BIT) IsTeeyootUser
FROM 
	Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord
	JOIN Orchard_Framework_ContentItemRecord ContentItemRecord
	ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id
	JOIN Orchard_Users_UserPartRecord UserPartRecord
	ON ContentItemRecord.Id = UserPartRecord.Id
	LEFT JOIN Teeyoot_Module_TeeyootUserPartRecord TeeyootUserPartRecord
	ON UserPartRecord.Id = TeeyootUserPartRecord.Id
	LEFT JOIN Teeyoot_Module_CurrencyRecord CurrencyRecord
	ON TeeyootUserPartRecord.CurrencyRecord_Id = CurrencyRecord.Id
WHERE 
	ContentItemVersionRecord.Published = 1
ORDER BY 
	UserPartRecord.Id
OFFSET 
	@Skip ROWS
FETCH NEXT 
	@Take ROWS ONLY
GO

IF OBJECT_ID('GetUsersCount', 'P') IS NOT NULL
	DROP PROCEDURE GetUsersCount
GO

CREATE PROCEDURE GetUsersCount
AS
SET NOCOUNT ON
SELECT 
	COUNT(*)
FROM 
	Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord
	JOIN Orchard_Framework_ContentItemRecord ContentItemRecord
	ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id
	JOIN Orchard_Users_UserPartRecord UserPartRecord
	ON ContentItemRecord.Id = UserPartRecord.Id
WHERE 
	ContentItemVersionRecord.Published = 1
GO