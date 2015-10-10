﻿IF OBJECT_ID('GetCampaigns', 'P') IS NOT NULL
	DROP PROCEDURE GetCampaigns
GO

CREATE PROCEDURE GetCampaigns
	@CurrentDate DATETIME,
	@Culture NVARCHAR(50),
	@CurrencyId INT = NULL,
	@SortColumn NVARCHAR(100) = NULL,
	@SortDirection NVARCHAR(50) = NULL,
	@Skip INT,
	@Take INT
AS
SET NOCOUNT ON
DECLARE @SQLQuery NVARCHAR(MAX)
DECLARE @ParamDefinition NVARCHAR(MAX)

SET @SQLQuery = N'
SELECT
	CampaignTemp.Profit Profit,
	CampaignTemp.Last24HoursSold Last24HoursSold,
	CampaignRecord.Id Id,
	CampaignRecord.Title Title,
	CampaignRecord.ProductCountGoal Goal,
	CampaignRecord.ProductCountSold Sold,
	CampaignRecord.IsApproved IsApproved,
	CampaignRecord.EndDate EndDate,
	CampaignRecord.Alias Alias,
	CampaignRecord.IsActive IsActive,
	CampaignRecord.ProductMinimumGoal Minimum,
	CampaignRecord.StartDate CreatedDate,
	CampaignStatusRecord.Name Status,
	TeeyootUserPartRecord.PhoneNumber PhoneNumber,
	UserPartRecord.UserName Email,
	CurrencyRecord.ShortName Currency
FROM(
	SELECT
		CampaignRecord.Id Id,
		MAX(CampaignProductRecord.CurrencyRecord_Id) CampaignAnyProductCurrencyId,
		SUM(CASE WHEN
				OrderRecord.IsActive = 1
				AND OrderStatusRecord.Name != ''Cancelled''
				AND OrderStatusRecord.Name != ''Unapproved''
			THEN
				LinkOrderCampaignProductRecord.Count * (CampaignProductRecord.Price - CampaignProductRecord.BaseCost)
			ELSE
				0
			END) Profit,
		SUM(CASE WHEN
				OrderRecord.IsActive = 1
				AND OrderStatusRecord.Name != ''Cancelled''
				AND	OrderStatusRecord.Name != ''Unapproved''
				AND OrderRecord.Created >= DATEADD(HH, -24, @CurrentDate)
			THEN
				LinkOrderCampaignProductRecord.Count
			ELSE
				0
			END) Last24HoursSold
	FROM
		Teeyoot_Module_CampaignRecord CampaignRecord
		LEFT JOIN Teeyoot_Module_CampaignProductRecord CampaignProductRecord
		ON CampaignRecord.Id = CampaignProductRecord.CampaignRecord_Id
		LEFT JOIN Teeyoot_Module_LinkOrderCampaignProductRecord LinkOrderCampaignProductRecord
		ON CampaignProductRecord.Id = LinkOrderCampaignProductRecord.CampaignProductRecord_Id
		LEFT JOIN Teeyoot_Module_OrderRecord OrderRecord
		ON LinkOrderCampaignProductRecord.OrderRecord_Id = OrderRecord.Id
		LEFT JOIN Teeyoot_Module_OrderStatusRecord OrderStatusRecord
		ON OrderRecord.OrderStatusRecord_Id = OrderStatusRecord.Id
	WHERE
		CampaignRecord.WhenDeleted IS NULL
		AND CampaignRecord.CampaignCulture = @Culture
	GROUP BY
		CampaignRecord.Id
	) CampaignTemp
	JOIN Teeyoot_Module_CampaignRecord CampaignRecord
	ON CampaignTemp.Id = CampaignRecord.Id
	LEFT JOIN Teeyoot_Module_CampaignStatusRecord CampaignStatusRecord
	ON CampaignRecord.CampaignStatusRecord_Id = CampaignStatusRecord.Id
	LEFT JOIN Teeyoot_Module_TeeyootUserPartRecord TeeyootUserPartRecord
	ON CampaignRecord.TeeyootUserId = TeeyootUserPartRecord.Id
	LEFT JOIN Orchard_Users_UserPartRecord UserPartRecord
	ON TeeyootUserPartRecord.Id = UserPartRecord.Id
	LEFT JOIN Teeyoot_Module_CurrencyRecord CurrencyRecord
	ON CampaignTemp.CampaignAnyProductCurrencyId = CurrencyRecord.Id'

IF @CurrencyId IS NOT NULL
BEGIN
	SET @SQLQuery = @SQLQuery + N'
WHERE 
	CampaignTemp.CampaignAnyProductCurrencyId = @CurrencyId'
END

IF @SortColumn IS NOT NULL
BEGIN
	SET @SQLQuery = @SQLQuery + N'
ORDER BY ' + 
	@SortColumn + N' ' + @SortDirection
END
ELSE
BEGIN
	SET @SQLQuery = @SQLQuery + N'
ORDER BY 
	CampaignRecord.Id ASC' 
END

SET @SQLQuery = @SQLQuery + N'
OFFSET
	@Skip ROWS
FETCH NEXT
	@Take ROWS ONLY'

PRINT @SQLQuery

SET @ParamDefinition = N'
	@CurrentDate DATETIME,
	@Culture NVARCHAR(50),
	@CurrencyId INT,
	@Skip INT,
	@Take INT'

EXECUTE sp_executesql @SQLQuery, @ParamDefinition,
	@CurrentDate,
	@Culture,
	@CurrencyId,
	@Skip,
	@Take
GO