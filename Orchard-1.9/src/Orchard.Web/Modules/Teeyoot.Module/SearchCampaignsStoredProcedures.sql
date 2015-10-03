IF OBJECT_ID('SearchCampaigns', 'P') IS NOT NULL
	DROP PROCEDURE SearchCampaigns
GO

CREATE PROCEDURE SearchCampaigns
	@CurrentDate DATETIME
AS
SET NOCOUNT ON
SELECT 
	CampaignRecord.Id CampaignRecordId,
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
	AND CampaignRecord.IsPrivate = 0
	AND CampaignRecord.IsActive = 1
	AND CampaignRecord.IsApproved = 1
GROUP BY CampaignRecord.Id
ORDER BY SalesLast24Hours DESC, SalesAllPeriod DESC, MAX(CampaignRecord.StartDate) DESC
GO

IF OBJECT_ID('SearchCampaignsForTag', 'P') IS NOT NULL
	DROP PROCEDURE SearchCampaignsForTag
GO

CREATE PROCEDURE SearchCampaignsForTag
	@CurrentDate DATETIME,
	@Tag NVARCHAR(100)
AS
SET NOCOUNT ON
SELECT 
	CampaignRecord.Id CampaignRecordId,
	SUM(CASE WHEN OrderRecord.Created IS NOT NULL AND OrderRecord.Created >= DATEADD(HH, -24, @CurrentDate) 
		THEN LinkOrderCampaignProductRecord.Count 
		ELSE 0 
		END) SalesLast24Hours,
	SUM(LinkOrderCampaignProductRecord.Count) SalesAllPeriod
FROM 
	Teeyoot_Module_CampaignRecord CampaignRecord
	JOIN Teeyoot_Module_LinkCampaignAndCategoriesRecord LinkCampaignAndCategoriesRecord
	ON CampaignRecord.Id = LinkCampaignAndCategoriesRecord.CampaignRecord_Id
	JOIN Teeyoot_Module_CampaignCategoriesRecord CampaignCategoriesRecord
	ON LinkCampaignAndCategoriesRecord.CampaignCategoriesPartRecord_Id = CampaignCategoriesRecord.Id
	AND LOWER(CampaignCategoriesRecord.Name) = @Tag
	LEFT JOIN Teeyoot_Module_CampaignProductRecord CampaignProductRecord
	ON CampaignRecord.Id = CampaignProductRecord.CampaignRecord_Id
	LEFT JOIN Teeyoot_Module_LinkOrderCampaignProductRecord LinkOrderCampaignProductRecord
	ON CampaignProductRecord.Id = LinkOrderCampaignProductRecord.CampaignProductRecord_Id
	LEFT JOIN Teeyoot_Module_OrderRecord OrderRecord
	ON LinkOrderCampaignProductRecord.OrderRecord_Id = OrderRecord.Id
WHERE 
	CampaignRecord.WhenDeleted IS NULL
	AND CampaignRecord.IsPrivate = 0
	AND CampaignRecord.IsActive = 1
	AND CampaignRecord.IsApproved = 1
GROUP BY CampaignRecord.Id
ORDER BY SalesLast24Hours DESC, SalesAllPeriod DESC, MAX(CampaignRecord.StartDate) DESC
GO

IF OBJECT_ID('SearchCampaignsForFilter', 'P') IS NOT NULL
	DROP PROCEDURE SearchCampaignsForFilter
GO

CREATE PROCEDURE SearchCampaignsForFilter
	@CurrentDate DATETIME,
	@Filter NVARCHAR(4000)
AS
SET NOCOUNT ON
SELECT 
	CampaignTemp.CampaignRecordId CampaignRecordId,
	SUM(CASE WHEN OrderRecord.Created IS NOT NULL AND OrderRecord.Created >= DATEADD(HH, -24, @CurrentDate) 
		THEN LinkOrderCampaignProductRecord.Count 
		ELSE 0 
		END) SalesLast24Hours,
	SUM(LinkOrderCampaignProductRecord.Count) SalesAllPeriod
FROM(
	SELECT DISTINCT 
		CampaignRecord.Id CampaignRecordId
	FROM 
		Teeyoot_Module_CampaignRecord CampaignRecord
		LEFT JOIN Teeyoot_Module_LinkCampaignAndCategoriesRecord LinkCampaignAndCategoriesRecord
		ON CampaignRecord.Id = LinkCampaignAndCategoriesRecord.CampaignRecord_Id
		LEFT JOIN Teeyoot_Module_CampaignCategoriesRecord CampaignCategoriesRecord
		ON LinkCampaignAndCategoriesRecord.CampaignCategoriesPartRecord_Id = CampaignCategoriesRecord.Id
	WHERE 
		CampaignRecord.WhenDeleted IS NULL
		AND CampaignRecord.IsPrivate = 0
		AND CampaignRecord.IsActive = 1
		AND CampaignRecord.IsApproved = 1
		AND (CampaignRecord.Title LIKE @Filter OR CampaignRecord.Description LIKE @Filter OR CampaignCategoriesRecord.Name LIKE @Filter)
) CampaignTemp
	JOIN Teeyoot_Module_CampaignRecord CampaignRecord
	ON CampaignTemp.CampaignRecordId = CampaignRecord.Id
	LEFT JOIN Teeyoot_Module_CampaignProductRecord CampaignProductRecord
	ON CampaignRecord.Id = CampaignProductRecord.CampaignRecord_Id
	LEFT JOIN Teeyoot_Module_LinkOrderCampaignProductRecord LinkOrderCampaignProductRecord
	ON CampaignProductRecord.Id = LinkOrderCampaignProductRecord.CampaignProductRecord_Id
	LEFT JOIN Teeyoot_Module_OrderRecord OrderRecord
	ON LinkOrderCampaignProductRecord.OrderRecord_Id = OrderRecord.Id
GROUP BY CampaignRecordId
ORDER BY SalesLast24Hours DESC, SalesAllPeriod DESC, MAX(CampaignRecord.StartDate) DESC
GO