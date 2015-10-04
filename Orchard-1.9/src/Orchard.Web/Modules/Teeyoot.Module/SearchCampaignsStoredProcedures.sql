IF OBJECT_ID('SearchCampaigns', 'P') IS NOT NULL
	DROP PROCEDURE SearchCampaigns
GO

CREATE PROCEDURE SearchCampaigns
	@CurrentDate DATETIME,
	@Culture NVARCHAR(50),
	@Skip INT,
	@Take INT
AS
SET NOCOUNT ON
SELECT 
	CampaignRecord.Id Id,
	CampaignRecord.Title Title,
	CampaignRecord.Alias Alias,
	CampaignRecord.EndDate EndDate,
	CampaignRecord.URL URL,
	CampaignRecord.ProductCountSold ProductCountSold,
	CampaignRecord.ProductMinimumGoal ProductMinimumGoal,
	CampaignRecord.BackSideByDefault BackSideByDefault
FROM(
	SELECT
		CampaignRecord.Id Id,
		SUM(CASE WHEN OrderRecord.Created IS NOT NULL AND OrderRecord.Created >= DATEADD(HH, -24, @CurrentDate) 
			THEN LinkOrderCampaignProductRecord.Count 
			ELSE 0 
			END) SalesLast24Hours,
		SUM(LinkOrderCampaignProductRecord.Count) SalesAllPeriod
	FROM
		Teeyoot_Module_CampaignRecord CampaignRecord
		LEFT JOIN Teeyoot_Module_CampaignProductRecord CampaignProductRecord
		ON CampaignRecord.Id = CampaignProductRecord.CampaignRecord_Id
		LEFT JOIN Teeyoot_Module_LinkOrderCampaignProductRecord LinkOrderCampaignProductRecord
		ON CampaignProductRecord.Id = LinkOrderCampaignProductRecord.CampaignProductRecord_Id
		LEFT JOIN Teeyoot_Module_OrderRecord OrderRecord
		ON LinkOrderCampaignProductRecord.OrderRecord_Id = OrderRecord.Id
	WHERE 
		CampaignRecord.WhenDeleted IS NULL
		AND CampaignRecord.CampaignCulture = @Culture
		AND CampaignRecord.IsPrivate = 0
		AND CampaignRecord.IsActive = 1
		AND CampaignRecord.IsApproved = 1
	GROUP BY 
		CampaignRecord.Id
	) CampaignsTemp
	JOIN Teeyoot_Module_CampaignRecord CampaignRecord
	ON CampaignsTemp.Id = CampaignRecord.Id
ORDER BY 
	CampaignsTemp.SalesLast24Hours DESC, 
	CampaignsTemp.SalesAllPeriod DESC, 
	CampaignRecord.WhenApproved DESC
OFFSET 
	@Skip ROWS
FETCH NEXT 
	@Take ROWS ONLY
GO

IF OBJECT_ID('SearchCampaignsForFilter', 'P') IS NOT NULL
	DROP PROCEDURE SearchCampaignsForFilter
GO

CREATE PROCEDURE SearchCampaignsForFilter
	@CurrentDate DATETIME,
	@Culture NVARCHAR(50),
	@Filter NVARCHAR(4000),
	@Skip INT,
	@Take INT
AS
SET NOCOUNT ON
SELECT 
	CampaignRecord.Id Id,
	CampaignRecord.Title Title,
	CampaignRecord.Alias Alias,
	CampaignRecord.EndDate EndDate,
	CampaignRecord.URL URL,
	CampaignRecord.ProductCountSold ProductCountSold,
	CampaignRecord.ProductMinimumGoal ProductMinimumGoal,
	CampaignRecord.BackSideByDefault BackSideByDefault
FROM(
	SELECT
		CampaignRecord.Id Id,
		SUM(CASE WHEN OrderRecord.Created IS NOT NULL AND OrderRecord.Created >= DATEADD(HH, -24, @CurrentDate) 
			THEN LinkOrderCampaignProductRecord.Count 
			ELSE 0 
			END) SalesLast24Hours,
		SUM(LinkOrderCampaignProductRecord.Count) SalesAllPeriod
	FROM(
		SELECT DISTINCT
			CampaignRecord.Id Id
		FROM 
			Teeyoot_Module_CampaignRecord CampaignRecord
			LEFT JOIN Teeyoot_Module_LinkCampaignAndCategoriesRecord LinkCampaignAndCategoriesRecord
			ON CampaignRecord.Id = LinkCampaignAndCategoriesRecord.CampaignRecord_Id
			LEFT JOIN Teeyoot_Module_CampaignCategoriesRecord CampaignCategoriesRecord
			ON LinkCampaignAndCategoriesRecord.CampaignCategoriesPartRecord_Id = CampaignCategoriesRecord.Id
		WHERE 
			CampaignRecord.WhenDeleted IS NULL
			AND CampaignRecord.CampaignCulture = @Culture
			AND CampaignRecord.IsPrivate = 0
			AND CampaignRecord.IsActive = 1
			AND CampaignRecord.IsApproved = 1
			AND (CampaignRecord.Title LIKE @Filter OR CampaignRecord.Description LIKE @Filter OR CampaignCategoriesRecord.Name LIKE @Filter)
		) FilteredCampaigns
		JOIN Teeyoot_Module_CampaignRecord CampaignRecord
		ON FilteredCampaigns.Id = CampaignRecord.Id
		LEFT JOIN Teeyoot_Module_CampaignProductRecord CampaignProductRecord
		ON CampaignRecord.Id = CampaignProductRecord.CampaignRecord_Id
		LEFT JOIN Teeyoot_Module_LinkOrderCampaignProductRecord LinkOrderCampaignProductRecord
		ON CampaignProductRecord.Id = LinkOrderCampaignProductRecord.CampaignProductRecord_Id
		LEFT JOIN Teeyoot_Module_OrderRecord OrderRecord
		ON LinkOrderCampaignProductRecord.OrderRecord_Id = OrderRecord.Id
	GROUP BY 
		CampaignRecord.Id
	) CampaignsTemp
	JOIN Teeyoot_Module_CampaignRecord CampaignRecord
	ON CampaignsTemp.Id = CampaignRecord.Id
ORDER BY 
	CampaignsTemp.SalesLast24Hours DESC, 
	CampaignsTemp.SalesAllPeriod DESC, 
	CampaignRecord.WhenApproved DESC
OFFSET 
	@Skip ROWS
FETCH NEXT 
	@Take ROWS ONLY
GO

IF OBJECT_ID('SearchCampaignsForTag', 'P') IS NOT NULL
	DROP PROCEDURE SearchCampaignsForTag
GO

CREATE PROCEDURE SearchCampaignsForTag
	@CurrentDate DATETIME,
	@Culture NVARCHAR(50),
	@Tag NVARCHAR(100),
	@Skip INT,
	@Take INT
AS
SET NOCOUNT ON
SELECT
	CampaignRecord.Id Id,
	CampaignRecord.Title Title,
	CampaignRecord.Alias Alias,
	CampaignRecord.EndDate EndDate,
	CampaignRecord.URL URL,
	CampaignRecord.ProductCountSold ProductCountSold,
	CampaignRecord.ProductMinimumGoal ProductMinimumGoal,
	CampaignRecord.BackSideByDefault BackSideByDefault
FROM(
	SELECT
		CampaignRecord.Id Id,
		SUM(CASE WHEN OrderRecord.Created IS NOT NULL AND OrderRecord.Created >= DATEADD(HH, -24, @CurrentDate) 
			THEN LinkOrderCampaignProductRecord.Count 
			ELSE 0 
			END) SalesLast24Hours,
		SUM(LinkOrderCampaignProductRecord.Count) SalesAllPeriod
	FROM(
		SELECT DISTINCT
			CampaignRecord.Id Id
		FROM
			Teeyoot_Module_CampaignRecord CampaignRecord
			JOIN Teeyoot_Module_LinkCampaignAndCategoriesRecord LinkCampaignAndCategoriesRecord
			ON CampaignRecord.Id = LinkCampaignAndCategoriesRecord.CampaignRecord_Id
			JOIN Teeyoot_Module_CampaignCategoriesRecord CampaignCategoriesRecord
			ON LinkCampaignAndCategoriesRecord.CampaignCategoriesPartRecord_Id = CampaignCategoriesRecord.Id
			AND LOWER(CampaignCategoriesRecord.Name) = @Tag
		WHERE 
			CampaignRecord.WhenDeleted IS NULL
			AND CampaignRecord.CampaignCulture = @Culture
			AND CampaignRecord.IsPrivate = 0
			AND CampaignRecord.IsActive = 1
			AND CampaignRecord.IsApproved = 1
		) FilteredCampaigns
		JOIN Teeyoot_Module_CampaignRecord CampaignRecord
		ON FilteredCampaigns.Id = CampaignRecord.Id
		LEFT JOIN Teeyoot_Module_CampaignProductRecord CampaignProductRecord
		ON CampaignRecord.Id = CampaignProductRecord.CampaignRecord_Id
		LEFT JOIN Teeyoot_Module_LinkOrderCampaignProductRecord LinkOrderCampaignProductRecord
		ON CampaignProductRecord.Id = LinkOrderCampaignProductRecord.CampaignProductRecord_Id
		LEFT JOIN Teeyoot_Module_OrderRecord OrderRecord
		ON LinkOrderCampaignProductRecord.OrderRecord_Id = OrderRecord.Id
	GROUP BY 
		CampaignRecord.Id
	) CampaignsTemp
	JOIN Teeyoot_Module_CampaignRecord CampaignRecord
	ON CampaignsTemp.Id = CampaignRecord.Id
ORDER BY 
	CampaignsTemp.SalesLast24Hours DESC, 
	CampaignsTemp.SalesAllPeriod DESC, 
	CampaignRecord.WhenApproved DESC
OFFSET 
	@Skip ROWS
FETCH NEXT 
	@Take ROWS ONLY
GO

IF TYPE_ID('INTEGER_LIST_TABLE_TYPE') IS NOT NULL
	/* Firts drop stored procedures that depends on this type */
	IF OBJECT_ID('GetCampaignsFirstProductData', 'P') IS NOT NULL
		DROP PROCEDURE GetCampaignsFirstProductData
	DROP TYPE INTEGER_LIST_TABLE_TYPE
GO

CREATE TYPE INTEGER_LIST_TABLE_TYPE AS TABLE(N INT NOT NULL PRIMARY KEY)
GO

CREATE PROCEDURE GetCampaignsFirstProductData
	/* http://www.sommarskog.se/arrays-in-sql-2008.html#TVP_in_TSQL */
	@CampaignIds INTEGER_LIST_TABLE_TYPE READONLY
AS
SET NOCOUNT ON
SELECT 
	CampaignRecord.Id CampaignRecordId,
	CampaignProductRecord.Id CampaignFirstProductId,
	CurrencyRecord.Code CampaignFirstProductCurrencyCode
FROM
	Teeyoot_Module_CampaignRecord CampaignRecord
	CROSS APPLY (
		SELECT TOP 1 
			Id, 
			CurrencyRecord_Id 
		FROM 
			Teeyoot_Module_CampaignProductRecord 
		WHERE 
			CampaignRecord_Id = CampaignRecord.Id 
			AND WhenDeleted IS NULL
	) CampaignProductRecord
	LEFT JOIN Teeyoot_Module_CurrencyRecord CurrencyRecord
	ON CampaignProductRecord.CurrencyRecord_Id = CurrencyRecord.Id
WHERE CampaignRecord.Id IN (SELECT N FROM @CampaignIds)
GO