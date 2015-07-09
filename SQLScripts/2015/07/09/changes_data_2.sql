/*
This script was created by Visual Studio on 7/9/2015 at 10:19 AM.
Run this script on bqt.me\sqlexpress, 1433.teebay-dev (sa) to make it the same as .\SQL2008Dev.teebay (sa).
This script performs its actions in the following order:
1. Disable foreign-key constraints.
2. Perform DELETE commands. 
3. Perform UPDATE commands.
4. Perform INSERT commands.
5. Re-enable foreign-key constraints.
Please back up your target database before running this script.
*/
SET NUMERIC_ROUNDABORT OFF
GO
SET XACT_ABORT, ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
/*Pointer used for text / image updates. This might not be needed, but is declared here just in case*/
DECLARE @pv binary(16)
BEGIN TRANSACTION
DELETE FROM [dbo].[Orchard_Indexing_IndexingTaskRecord] WHERE [Id]=724
UPDATE [dbo].[Common_CommonPartRecord] SET [PublishedUtc]='20150709 07:12:14.000', [ModifiedUtc]='20150709 07:12:13.000' WHERE [Id]=56
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="466f984bcc6b44428378c1f8f8f7932b" /><CommonPart CreatedUtc="2015-07-03T11:55:56.5271774Z" ModifiedUtc="2015-07-09T07:12:13.9777144Z" PublishedUtc="2015-07-09T07:12:14.0377167Z" /><WidgetPart RenderTitle="false" Title="Create Search Buttons" Position="3" Zone="BeforeMain" Name="createSearchButtons" /><LayoutPart SessionKey="c8dd4c39-3da9-4a4a-bb46-3ef65befdfd8" /></Data>' WHERE [Id]=56
UPDATE [dbo].[Orchard_Framework_ContentItemVersionRecord] SET [Published]=0, [Latest]=0 WHERE [Id]=421
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (422, 56, NULL)
SET IDENTITY_INSERT [dbo].[Orchard_Indexing_IndexingTaskRecord] ON
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (726, 0, '20150709 07:12:14.000', 56)
SET IDENTITY_INSERT [dbo].[Orchard_Indexing_IndexingTaskRecord] OFF
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentItemVersionRecord] ON
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (422, 40, 1, 1, N'<Data><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Grid&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=12&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3cdiv%3e%0d%0a%3cdiv+style%3d%22text-align%3a+center%3b%22%3eCreate+%26amp%3b+Sell+T-shirts%3c%2fdiv%3e%0d%0a%3cdiv+class%3d%22title-logo%22%3eNo+upfront+cost.+No+risk.+No+hassle%3c%2fdiv%3e%0d%0a%3c%2fdiv%3e&amp;Content=%3cdiv%3e%0d%0a%3cdiv+style%3d%22text-align%3a+center%3b%22%3eCreate+%26amp%3b+Sell+T-shirts%3c%2fdiv%3e%0d%0a%3cdiv+class%3d%22title-logo%22%3eNo+upfront+cost.+No+risk.+No+hassle%3c%2fdiv%3e%0d%0a%3c%2fdiv%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;banner-title&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:1,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=12&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3cdiv+class%3d%22button-create-search-container%22+style%3d%22text-align%3a+center%3b%22%3e%3cbutton+onclick%3d%22redirectToStartCampaign()%22+class%3d%22tb-button%22%3eCREATE%3c%2fbutton%3e%3cspan%3eor%3c%2fspan%3e%3cbutton+class%3d%22tb-button%22%3eSEARCH%3c%2fbutton%3e%3c%2fdiv%3e&amp;Content=%3cdiv+class%3d%22button-create-search-container%22+style%3d%22text-align%3a+center%3b%22%3e%3cbutton+onclick%3d%22redirectToStartCampaign()%22+class%3d%22tb-button%22%3eCREATE%3c%2fbutton%3e%3cspan%3eor%3c%2fspan%3e%3cbutton+class%3d%22tb-button%22%3eSEARCH%3c%2fbutton%3e%3c%2fdiv%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 56)
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentItemVersionRecord] OFF
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (422, 56, '20150709 07:12:13.000', '20150709 07:12:14.000', '20150709 07:12:13.000')
COMMIT TRANSACTION
