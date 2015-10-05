UPDATE [dbo].[Teeyoot_Module_OrderRecord]
   SET [OrderStatusRecord_Id] = 6
 WHERE [OrderStatusRecord_Id] = 7
GO

UPDATE [dbo].[Teeyoot_Module_OrderStatusRecord]
   SET [Name] = 'Cancelled'
 WHERE [Id] = 6
GO

DELETE FROM [dbo].[Teeyoot_Module_OrderStatusRecord]
      WHERE [Id] = 7
GO
