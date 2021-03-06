/*
This script was created by Visual Studio on 05.07.2015 at 17:39.
Run this script on .\SQL2008Dev.teebay-test (sa) to make it the same as .\SQL2008Dev.teebay (sa).
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
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1929
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1930
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1931
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1932
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1933
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1934
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1935
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1936
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1937
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1938
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1939
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1940
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1941
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1942
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1943
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1944
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1945
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1946
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1947
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1948
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1949
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1950
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1951
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1952
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1953
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1954
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1955
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1956
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1957
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1958
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1959
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1960
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1961
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1962
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1963
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1964
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1965
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1966
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1967
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1968
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1969
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1970
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1971
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1972
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1973
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1974
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1975
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1976
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1977
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1978
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1979
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1980
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1981
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1982
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1983
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1984
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1985
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1986
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1987
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1988
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1989
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1990
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1991
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1992
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1993
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1994
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1995
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1996
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1997
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1998
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=1999
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2000
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2001
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2002
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2003
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2004
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2005
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2006
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2007
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2008
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2009
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2010
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2011
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2012
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2013
DELETE FROM [dbo].[Settings_ShellFeatureRecord] WHERE [Id]=2014
DELETE FROM [dbo].[Orchard_Indexing_IndexingTaskRecord] WHERE [Id]=203
UPDATE [dbo].[Settings_ShellDescriptorRecord] SET [SerialNumber]=26 WHERE [Id]=1
UPDATE [dbo].[Settings_ShellFeatureStateRecord] SET [EnableState]=N'Up' WHERE [Id]=48
UPDATE [dbo].[Orchard_Widgets_LayerPartRecord] SET [Name]=N'The Homepage' WHERE [Id]=7
UPDATE [dbo].[Orchard_Widgets_WidgetPartRecord] SET [Position]=N'2' WHERE [Id]=36
UPDATE [dbo].[Orchard_Widgets_WidgetPartRecord] SET [Position]=N'3' WHERE [Id]=56
UPDATE [dbo].[Orchard_Widgets_WidgetPartRecord] SET [Position]=N'5' WHERE [Id]=60
UPDATE [dbo].[Orchard_Widgets_WidgetPartRecord] SET [Position]=N'4' WHERE [Id]=63
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><CommonPart CreatedUtc="2015-07-02T07:23:55.2361049Z" ModifiedUtc="2015-07-05T13:50:25.5742964Z" PublishedUtc="2015-07-02T07:23:55.2431053Z" /><LayerPart Name="The Homepage" LayerRule="url ''~/''" Description="The widgets in this layer are displayed on the home page" /></Data>' WHERE [Id]=7
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="ab439a048b1f47249c83d7af4746d1de" /><CommonPart CreatedUtc="2015-07-03T10:26:57.6208095Z" ModifiedUtc="2015-07-03T11:53:19.5381982Z" PublishedUtc="2015-07-03T11:53:19.5491988Z" /><WidgetPart RenderTitle="false" Title="Big Banner" Position="2" Zone="BeforeMain" Name="bigbanner" /><LayoutPart SessionKey="49d9d7ff-15d5-42a4-b508-aae4c6ca2c9e" /></Data>' WHERE [Id]=36
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="466f984bcc6b44428378c1f8f8f7932b" /><CommonPart CreatedUtc="2015-07-03T11:55:56.5271774Z" ModifiedUtc="2015-07-04T11:00:45.5966004Z" PublishedUtc="2015-07-04T11:00:45.6086011Z" /><WidgetPart RenderTitle="false" Title="Create Search Buttons" Position="3" Zone="BeforeMain" Name="createSearchButtons" /><LayoutPart SessionKey="c8dd4c39-3da9-4a4a-bb46-3ef65befdfd8" /></Data>' WHERE [Id]=56
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="ef42a98c8c86441da0231a17a9c1734b" /><CommonPart CreatedUtc="2015-07-03T13:14:29.1347232Z" ModifiedUtc="2015-07-04T14:15:54.6823218Z" PublishedUtc="2015-07-04T14:15:54.6963226Z" /><WidgetPart RenderTitle="false" Title="Get Started Button" Position="5" Zone="Header" Name="getStartedButton" /></Data>' WHERE [Id]=60
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="2ddb24b6186e4d1a811bcfb238434529" /><CommonPart CreatedUtc="2015-07-03T13:25:28.0824128Z" ModifiedUtc="2015-07-05T13:12:13.3609092Z" PublishedUtc="2015-07-05T13:12:13.3759093Z" /><MenuItemPart Url="/Users/Account/LogOn" /></Data>' WHERE [Id]=62
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="0c4b1836b60d430da5c4eb2ae3dea8ce" /><CommonPart CreatedUtc="2015-07-03T13:26:06.7146225Z" ModifiedUtc="2015-07-03T13:26:06.775626Z" PublishedUtc="2015-07-03T13:26:06.7846265Z" /><WidgetPart RenderTitle="false" Title="Anonimous Menu" Position="4" Zone="Header" Name="anonimousMenu" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="8" /></Data>' WHERE [Id]=63
UPDATE [dbo].[Orchard_Framework_ContentItemRecord] SET [Data]=N'<Data><IdentityPart Identifier="53760aad85054db5827e082852c845ae" /><CommonPart CreatedUtc="2015-07-04T14:16:31.7284407Z" ModifiedUtc="2015-07-05T09:25:29.1747842Z" PublishedUtc="2015-07-04T14:18:18.8785693Z" /></Data>' WHERE [Id]=97
UPDATE [dbo].[Orchard_Framework_ContentItemVersionRecord] SET [Data]=N'<Data />' WHERE [Id]=4
UPDATE [dbo].[Orchard_Framework_ContentItemVersionRecord] SET [Published]=0, [Latest]=0 WHERE [Id]=123
UPDATE [dbo].[Orchard_Framework_ContentItemVersionRecord] SET [Published]=0, [Latest]=0 WHERE [Id]=212
UPDATE [dbo].[Common_CommonPartRecord] SET [ModifiedUtc]='20150705 13:50:25.000' WHERE [Id]=7
UPDATE [dbo].[Common_CommonPartRecord] SET [PublishedUtc]='20150705 13:12:13.000', [ModifiedUtc]='20150705 13:12:13.000' WHERE [Id]=62
UPDATE [dbo].[Common_CommonPartRecord] SET [Container_id]=5 WHERE [Id]=63
UPDATE [dbo].[Common_CommonPartRecord] SET [ModifiedUtc]='20150705 09:25:29.000' WHERE [Id]=97
UPDATE [dbo].[Common_CommonPartVersionRecord] SET [ModifiedUtc]='20150705 13:50:25.000' WHERE [Id]=7
UPDATE [dbo].[Common_CommonPartVersionRecord] SET [ModifiedUtc]='20150705 09:25:29.000' WHERE [Id]=212
SET IDENTITY_INSERT [dbo].[Orchard_Alias_AliasRecord] ON
INSERT INTO [dbo].[Orchard_Alias_AliasRecord] ([Id], [Path], [Action_id], [RouteValues], [Source]) VALUES (6, N'design', 1, N'<v Id="99" />', N'Autoroute:View')
INSERT INTO [dbo].[Orchard_Alias_AliasRecord] ([Id], [Path], [Action_id], [RouteValues], [Source]) VALUES (7, N'details', 1, N'<v Id="100" />', N'Autoroute:View')
INSERT INTO [dbo].[Orchard_Alias_AliasRecord] ([Id], [Path], [Action_id], [RouteValues], [Source]) VALUES (8, N'pricing', 1, N'<v Id="101" />', N'Autoroute:View')
INSERT INTO [dbo].[Orchard_Alias_AliasRecord] ([Id], [Path], [Action_id], [RouteValues], [Source]) VALUES (9, N'LearnMore', 1, N'<v Id="133" />', N'Autoroute:View')
INSERT INTO [dbo].[Orchard_Alias_AliasRecord] ([Id], [Path], [Action_id], [RouteValues], [Source]) VALUES (10, N'TrackYourOrder', 1, N'<v Id="136" />', N'Autoroute:View')
SET IDENTITY_INSERT [dbo].[Orchard_Alias_AliasRecord] OFF
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (99, NULL, NULL, 0)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (100, NULL, NULL, 0)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (101, NULL, NULL, 0)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (102, N'Get Started', N'3', 8)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (104, N'Get Started', N'1', 103)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (106, N'Create your tee', N'1', 105)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (107, N'Set a goal', N'2', 105)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (108, N'Add a description', N'3', 105)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (110, N'NEXT PAGE', N'1', 34)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (111, N'NEXT PAGE', N'1', 109)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (113, N'LOUNCH', N'1', 112)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (115, N'NEXT PAGE', N'1', 114)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (133, NULL, NULL, 0)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (136, NULL, NULL, 0)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (140, N'Learn More', N'1', 139)
INSERT INTO [dbo].[Navigation_MenuPartRecord] ([Id], [MenuText], [MenuPosition], [MenuId]) VALUES (141, N'Track Your Order', N'2', 139)
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (214, 99, N'GetStartedDesign')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (215, 100, N'GetStartedDetails')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (216, 101, N'GetStartedPricing')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (218, 103, N'GetStartedMenu')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (220, 105, N'TeeWizardMenu')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (224, 109, N'Wizard Design Next Button')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (227, 112, N'Wizard Details Launch Button')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (229, 114, N'Wizard Pricing Next Button')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (236, 103, N'Get Started Menu')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (237, 103, N'Get Started Menu')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (238, 105, N'Tee Wizard Menu')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (243, 100, N'GetStartedDetails')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (244, 100, N'GetStartedDetails')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (258, 133, N'Learn More')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (261, 136, N'Track Your Order')
INSERT INTO [dbo].[Title_TitlePartRecord] ([Id], [ContentItemRecord_id], [Title]) VALUES (266, 139, N'Authenticated Menu')
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (214, 99, NULL)
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (215, 100, NULL)
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (216, 101, NULL)
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (243, 100, NULL)
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (244, 100, NULL)
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (258, 133, NULL)
INSERT INTO [dbo].[Orchard_Layouts_LayoutPartRecord] ([Id], [ContentItemRecord_id], [TemplateId]) VALUES (261, 136, NULL)
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (102, N'ef10eab7accc43bb8d9309ca2e30a79e')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (103, N'f17f81082d7d49de9c7844278ace9f71')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (104, N'12e5d43b7c974907920b317ea63e9f19')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (105, N'd539ccec113445fdbe6444ca299debd8')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (106, N'3dd7224ebf504a9a87ea7168d0213387')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (107, N'75bd45e197894a02a27e9c41295538ec')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (108, N'f706b2bdc67545ea8976d16435e9ff7e')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (109, N'e8fa2be34dd148daba1067fd1fd4d188')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (110, N'5aebe0686c6a46ccafd4c50f3a52eb3d')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (111, N'c71a0e6545a248b0834c5014921942dd')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (112, N'1bdf41ffcbe7464fabe71bfe7cd35403')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (113, N'e984891477814ef1bec911bb9c23c67d')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (114, N'b1116dfd60244895b8d22f9758ac239d')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (115, N'cd2218af974e46f8bf19afbea4861d3e')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (120, N'b045595134544b0da78054660c6c1b46')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (121, N'9a9a4f2559c941459d429d97ca6d40b0')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (122, N'd740c1711f214f439df09d7f6353d839')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (123, N'9c58a951f5444ba69df93ea95d64125e')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (124, N'2f6b0e26642e4c6a82a5f4fbf6623a10')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (125, N'4fdde536035a442d8ac81af57e4bd862')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (127, N'3edaedb0bfe2444a87a91fa4b05e3a28')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (128, N'1bbcfe78985743448684dc20683e2ef9')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (130, N'aa5a0cfb02154340b7fde9323c4f792f')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (131, N'26c358b5151b428f84e91872b6fccab0')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (132, N'511ce46312b64e918b749c9d2cae7143')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (135, N'4ebf6cb78c124932b7a25de8e3438a0f')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (138, N'f4947a38cd2c4563bfad8972fb08f88c')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (139, N'7438c630c2884b3882bf05f6bdacd265')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (140, N'450120e9ca4d4342a38ce12c631e89c3')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (141, N'8dd67e37ee3f4efd88ed672e9689c65c')
INSERT INTO [dbo].[Common_IdentityPartRecord] ([Id], [Identifier]) VALUES (142, N'234c59ee28654224a0227096ed98d198')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (214, 99, '20150705 09:12:17.000', '20150705 09:12:39.000', '20150705 09:12:39.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (215, 100, '20150705 09:17:06.000', '20150705 09:17:16.000', '20150705 09:17:16.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (216, 101, '20150705 09:22:27.000', '20150705 09:22:27.000', '20150705 09:22:27.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (217, 102, '20150705 09:25:56.000', '20150705 09:25:56.000', '20150705 09:26:43.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (218, 103, '20150705 09:27:19.000', '20150705 09:27:19.000', '20150705 09:27:19.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (219, 104, '20150705 09:27:55.000', '20150705 09:27:55.000', '20150705 09:27:55.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (220, 105, '20150705 09:29:20.000', '20150705 09:29:20.000', '20150705 09:29:20.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (221, 106, '20150705 09:30:06.000', '20150705 09:30:06.000', '20150705 09:30:06.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (222, 107, '20150705 09:31:20.000', '20150705 09:31:20.000', '20150705 09:31:20.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (223, 108, '20150705 09:32:08.000', '20150705 09:32:08.000', '20150705 09:32:08.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (224, 109, '20150705 09:33:32.000', '20150705 09:33:32.000', '20150705 12:31:24.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (225, 110, '20150705 09:34:00.000', '20150705 09:34:00.000', '20150705 09:34:15.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (226, 111, '20150705 09:34:54.000', '20150705 09:34:54.000', '20150705 12:31:24.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (227, 112, '20150705 09:35:57.000', '20150705 09:35:57.000', '20150705 12:31:37.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (228, 113, '20150705 09:36:37.000', '20150705 09:36:37.000', '20150705 12:31:37.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (229, 114, '20150705 09:37:07.000', '20150705 09:37:07.000', '20150705 12:31:47.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (230, 115, '20150705 09:37:34.000', '20150705 09:37:34.000', '20150705 12:31:47.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (231, 116, '20150705 09:43:23.000', '20150705 09:43:24.000', '20150705 09:43:24.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (232, 117, '20150705 09:44:17.000', '20150705 09:44:17.000', '20150705 09:44:17.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (233, 118, '20150705 09:45:20.000', '20150705 09:45:20.000', '20150705 09:45:20.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (234, 119, '20150705 09:46:35.000', '20150705 09:46:35.000', '20150705 09:46:35.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (235, 120, '20150705 09:48:20.000', '20150705 09:48:21.000', '20150705 10:43:07.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (236, 103, '20150705 09:49:10.000', '20150705 09:49:10.000', '20150705 09:49:10.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (237, 103, '20150705 09:49:16.000', '20150705 09:49:16.000', '20150705 09:49:16.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (238, 105, '20150705 09:49:31.000', '20150705 09:49:31.000', '20150705 09:49:31.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (239, 121, '20150705 09:52:17.000', '20150705 09:52:17.000', '20150705 12:16:55.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (240, 122, '20150705 09:53:49.000', '20150705 09:53:49.000', '20150705 12:22:39.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (241, 123, '20150705 09:55:43.000', '20150705 09:55:43.000', '20150705 12:27:44.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (242, 124, '20150705 09:57:38.000', '20150705 09:57:39.000', '20150705 09:57:38.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (243, 100, '20150705 10:08:51.000', '20150705 10:08:58.000', '20150705 10:08:58.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (244, 100, '20150705 10:34:57.000', '20150705 10:35:09.000', '20150705 10:35:09.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (245, 125, '20150705 10:38:46.000', '20150705 10:38:47.000', '20150705 10:41:41.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (247, 127, '20150705 10:44:00.000', '20150705 10:44:00.000', '20150705 10:46:34.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (248, 128, '20150705 10:47:01.000', '20150705 12:51:46.000', '20150705 10:47:02.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (250, 130, '20150705 12:08:04.000', '20150705 12:08:04.000', '20150705 12:08:04.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (251, 130, '20150705 12:12:28.000', '20150705 12:12:28.000', '20150705 12:12:28.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (252, 130, '20150705 12:14:40.000', '20150705 12:14:40.000', '20150705 12:14:40.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (253, 130, '20150705 12:17:00.000', '20150705 12:17:00.000', '20150705 12:17:00.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (254, 131, '20150705 12:24:18.000', '20150705 12:24:18.000', '20150705 12:24:18.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (255, 132, '20150705 12:28:46.000', '20150705 12:28:46.000', '20150705 12:28:46.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (256, 132, '20150705 12:29:28.000', '20150705 12:29:28.000', '20150705 12:29:28.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (257, 132, '20150705 12:40:14.000', '20150705 12:40:14.000', '20150705 12:40:14.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (258, 133, '20150705 12:43:04.000', '20150705 12:43:04.000', '20150705 12:43:04.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (259, 134, '20150705 12:43:46.000', '20150705 12:43:46.000', '20150705 12:43:46.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (260, 135, '20150705 12:54:40.000', '20150705 14:17:22.000', '20150705 14:32:54.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (261, 136, '20150705 13:03:57.000', '20150705 13:03:57.000', '20150705 13:03:57.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (262, 137, '20150705 13:05:02.000', '20150705 13:05:02.000', '20150705 13:05:02.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (263, 138, '20150705 13:07:33.000', '20150705 13:07:33.000', '20150705 13:07:33.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (264, 62, '20150705 13:12:01.000', '20150705 13:12:01.000', '20150705 13:12:01.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (265, 62, '20150705 13:12:13.000', '20150705 13:12:13.000', '20150705 13:12:13.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (266, 139, '20150705 13:59:32.000', '20150705 13:59:33.000', '20150705 13:59:33.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (267, 140, '20150705 13:59:58.000', '20150705 13:59:58.000', '20150705 13:59:58.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (268, 141, '20150705 14:00:21.000', '20150705 14:00:21.000', '20150705 14:00:21.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (269, 142, '20150705 14:01:54.000', '20150705 14:01:54.000', '20150705 14:01:54.000')
INSERT INTO [dbo].[Common_CommonPartVersionRecord] ([Id], [ContentItemRecord_id], [CreatedUtc], [PublishedUtc], [ModifiedUtc]) VALUES (272, 144, '20150705 14:22:59.000', '20150705 14:22:59.000', '20150705 14:30:18.000')
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (99, 2, '20150705 09:12:17.000', '20150705 09:12:39.000', '20150705 09:12:39.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (100, 2, '20150705 09:17:06.000', '20150705 10:35:09.000', '20150705 10:35:09.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (101, 2, '20150705 09:22:27.000', '20150705 09:22:27.000', '20150705 09:22:27.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (102, 2, '20150705 09:25:56.000', '20150705 09:25:56.000', '20150705 09:26:43.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (103, 2, '20150705 09:27:19.000', '20150705 09:49:16.000', '20150705 09:49:16.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (104, 2, '20150705 09:27:55.000', '20150705 09:27:55.000', '20150705 09:27:55.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (105, 2, '20150705 09:29:20.000', '20150705 09:49:31.000', '20150705 09:49:31.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (106, 2, '20150705 09:30:06.000', '20150705 09:30:06.000', '20150705 09:30:06.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (107, 2, '20150705 09:31:20.000', '20150705 09:31:20.000', '20150705 09:31:20.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (108, 2, '20150705 09:32:08.000', '20150705 09:32:08.000', '20150705 09:32:08.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (109, 2, '20150705 09:33:32.000', '20150705 09:33:32.000', '20150705 12:31:24.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (110, 2, '20150705 09:34:00.000', '20150705 09:34:00.000', '20150705 09:34:15.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (111, 2, '20150705 09:34:54.000', '20150705 09:34:54.000', '20150705 12:31:24.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (112, 2, '20150705 09:35:57.000', '20150705 09:35:57.000', '20150705 12:31:37.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (113, 2, '20150705 09:36:37.000', '20150705 09:36:37.000', '20150705 12:31:37.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (114, 2, '20150705 09:37:07.000', '20150705 09:37:07.000', '20150705 12:31:47.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (115, 2, '20150705 09:37:34.000', '20150705 09:37:34.000', '20150705 12:31:47.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (116, 2, '20150705 09:43:23.000', '20150705 09:43:24.000', '20150705 09:43:24.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (117, 2, '20150705 09:44:17.000', '20150705 09:44:17.000', '20150705 09:44:17.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (118, 2, '20150705 09:45:20.000', '20150705 09:45:20.000', '20150705 09:45:20.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (119, 2, '20150705 09:46:35.000', '20150705 09:46:35.000', '20150705 09:46:35.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (120, 2, '20150705 09:48:20.000', '20150705 09:48:21.000', '20150705 10:43:07.000', 116)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (121, 2, '20150705 09:52:17.000', '20150705 09:52:17.000', '20150705 12:16:55.000', 117)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (122, 2, '20150705 09:53:49.000', '20150705 09:53:49.000', '20150705 12:22:39.000', 118)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (123, 2, '20150705 09:55:43.000', '20150705 09:55:43.000', '20150705 12:27:44.000', 119)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (124, 2, '20150705 09:57:38.000', '20150705 09:57:39.000', '20150705 09:57:38.000', 7)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (125, 2, '20150705 10:38:46.000', '20150705 10:38:47.000', '20150705 10:41:41.000', 116)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (127, 2, '20150705 10:44:00.000', '20150705 10:44:00.000', '20150705 10:46:34.000', 116)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (128, 2, '20150705 10:47:01.000', '20150705 12:51:46.000', '20150705 10:47:02.000', 116)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (130, 2, '20150705 12:08:04.000', '20150705 12:17:00.000', '20150705 12:17:00.000', 117)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (131, 2, '20150705 12:24:18.000', '20150705 12:24:18.000', '20150705 12:24:18.000', 118)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (132, 2, '20150705 12:28:46.000', '20150705 12:40:14.000', '20150705 12:40:14.000', 119)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (133, 2, '20150705 12:43:04.000', '20150705 12:43:04.000', '20150705 12:43:04.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (134, 2, '20150705 12:43:46.000', '20150705 12:43:46.000', '20150705 12:43:46.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (135, 2, '20150705 12:54:40.000', '20150705 14:17:22.000', '20150705 14:32:54.000', 134)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (136, 2, '20150705 13:03:57.000', '20150705 13:03:57.000', '20150705 13:03:57.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (137, 2, '20150705 13:05:02.000', '20150705 13:05:02.000', '20150705 13:05:02.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (138, 2, '20150705 13:07:33.000', '20150705 13:07:33.000', '20150705 13:07:33.000', 144)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (139, 2, '20150705 13:59:32.000', '20150705 13:59:33.000', '20150705 13:59:33.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (140, 2, '20150705 13:59:58.000', '20150705 13:59:58.000', '20150705 13:59:58.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (141, 2, '20150705 14:00:21.000', '20150705 14:00:21.000', '20150705 14:00:21.000', NULL)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (142, 2, '20150705 14:01:54.000', '20150705 14:01:54.000', '20150705 14:01:54.000', 4)
INSERT INTO [dbo].[Common_CommonPartRecord] ([Id], [OwnerId], [CreatedUtc], [PublishedUtc], [ModifiedUtc], [Container_id]) VALUES (144, 2, '20150705 14:22:59.000', '20150705 14:22:59.000', '20150705 14:30:18.000', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (245, 125, N'<div></div>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (250, 130, N'<p><button><a href="~/pricing">asd</a></button></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (251, 130, N'<p><button><a href="/pricing">asd</a></button></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (252, 130, N'<p><button class=".tb-button"><a href="/pricing">asd</a></button></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (253, 130, N'<p><a href="/pricing"><button class=".tb-button">NEXT PAGE</button></a></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (254, 131, N'<p><a href="/details"><button class=".tb-button">NEXT PAGE</button></a></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (255, 132, N'<p><a href="/"><button class=".tb-button">LOUNCH</button></a></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (256, 132, N'<p><a href="/"><button class=".tb-button">LOUNCH</button></a></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (257, 132, N'<p><a href="/"><button class=".tb-button">LAUNCH</button></a></p>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (260, 135, N'<div class="insideContentTop"></div>', NULL)
INSERT INTO [dbo].[Common_BodyPartRecord] ([Id], [ContentItemRecord_id], [Text], [Format]) VALUES (263, 138, N'<div class="insideContentTop"></div>', NULL)
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentTypeRecord] ON
INSERT INTO [dbo].[Orchard_Framework_ContentTypeRecord] ([Id], [Name]) VALUES (22, N'Layout')
INSERT INTO [dbo].[Orchard_Framework_ContentTypeRecord] ([Id], [Name]) VALUES (23, N'ProjectionPage')
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentTypeRecord] OFF
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentItemVersionRecord] ON
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (214, 1, 1, 1, N'<Data><AutoroutePart DisplayAlias="design" CustomPattern="" UseCustomPattern="false" /><TitlePart Title="GetStartedDesign" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Grid&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=4&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+INSTRUMENTS+FOR+DECORATIONG%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+INSTRUMENTS+FOR+DECORATIONG%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=4&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:1,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+TEE+PREVIEW%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+TEE+PREVIEW%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=4&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:2,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+STYLES+AND+BASE+COST%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+STYLES+AND+BASE+COST%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 99)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (215, 1, 0, 0, N'<Data><AutoroutePart DisplayAlias="details" /><TitlePart Title="GetStartedDetails" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Grid&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+CAMPAIGN+FIELDS%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+CAMPAIGN+FIELDS%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:1,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+BACK+AND+FRONT+TEE+IMAGE%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+BACK+AND+FRONT+TEE+IMAGE%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 100)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (216, 1, 1, 1, N'<Data><AutoroutePart DisplayAlias="pricing" CustomPattern="" UseCustomPattern="false" /><TitlePart Title="GetStartedPricing" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Grid&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+SALES+GOAL+AND+PROFIT%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+SALES+GOAL+AND+PROFIT%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:null,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:null}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:null,&quot;htmlClass&quot;:null,&quot;htmlStyle&quot;:null},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:1,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+FRONT+AND+BACK+TEE+IMAGE%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+FRONT+AND+BACK+TEE+IMAGE%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:null,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:null}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:null,&quot;htmlClass&quot;:null,&quot;htmlStyle&quot;:null}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:null,&quot;htmlClass&quot;:null,&quot;htmlStyle&quot;:null}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:null,&quot;htmlClass&quot;:null,&quot;htmlStyle&quot;:null}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 101)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (217, 1, 0, 0, NULL, 102)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (218, 1, 0, 0, N'<Data><TitlePart Title="GetStartedMenu" /></Data>', 103)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (219, 1, 1, 1, NULL, 104)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (220, 1, 0, 0, N'<Data><TitlePart Title="TeeWizardMenu" /></Data>', 105)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (221, 1, 1, 1, NULL, 106)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (222, 1, 1, 1, NULL, 107)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (223, 1, 1, 1, NULL, 108)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (224, 1, 0, 0, N'<Data><TitlePart Title="Wizard Design Next Button" /></Data>', 109)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (225, 1, 0, 0, NULL, 110)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (226, 1, 0, 0, NULL, 111)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (227, 1, 0, 0, N'<Data><TitlePart Title="Wizard Details Launch Button" /></Data>', 112)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (228, 1, 0, 0, NULL, 113)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (229, 1, 0, 0, N'<Data><TitlePart Title="Wizard Pricing Next Button" /></Data>', 114)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (230, 1, 0, 0, NULL, 115)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (231, 1, 1, 1, N'<Data />', 116)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (232, 1, 1, 1, N'<Data />', 117)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (233, 1, 1, 1, N'<Data />', 118)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (234, 1, 1, 1, N'<Data />', 119)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (235, 1, 0, 0, N'<Data />', 120)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (236, 2, 0, 0, N'<Data><TitlePart Title="Get Started Menu" /></Data>', 103)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (237, 3, 1, 1, N'<Data><TitlePart Title="Get Started Menu" /></Data>', 103)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (238, 2, 1, 1, N'<Data><TitlePart Title="Tee Wizard Menu" /></Data>', 105)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (239, 1, 0, 0, N'<Data />', 121)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (240, 1, 0, 0, N'<Data />', 122)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (241, 1, 0, 0, N'<Data />', 123)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (242, 1, 1, 1, N'<Data />', 124)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (243, 2, 0, 0, N'<Data><AutoroutePart DisplayAlias="details" /><TitlePart Title="GetStartedDetails" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Grid&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+CAMPAIGN+FIELDS%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+CAMPAIGN+FIELDS%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:1,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+BACK+AND+FRONT+TEE+IMAGE%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+BACK+AND+FRONT+TEE+IMAGE%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 100)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (244, 3, 1, 1, N'<Data><AutoroutePart DisplayAlias="details" CustomPattern="" UseCustomPattern="false" /><TitlePart Title="GetStartedDetails" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Grid&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Row&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+CAMPAIGN+FIELDS%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+CAMPAIGN+FIELDS%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;},{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Column&quot;,&quot;data&quot;:&quot;Width=6&amp;Offset=0&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:1,&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Html&quot;,&quot;data&quot;:&quot;TypeName=Orchard.Layouts.Elements.Html&amp;Text=%3ch2%3eTHIS+PART+CONTAINS+BACK+AND+FRONT+TEE+IMAGE%3c%2fh2%3e&amp;Content=%3ch2%3eTHIS+PART+CONTAINS+BACK+AND+FRONT+TEE+IMAGE%3c%2fh2%3e&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;wizard-text&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 100)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (245, 1, 0, 0, N'<Data><BodyPart Text="&lt;div&gt;&lt;/div&gt;" Format="" /></Data>', 125)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (247, 1, 0, 0, N'<Data />', 127)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (248, 1, 1, 1, N'<Data />', 128)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (250, 1, 0, 0, N'<Data><BodyPart Text="&lt;p&gt;&lt;button&gt;&lt;a href=&quot;~/pricing&quot;&gt;asd&lt;/a&gt;&lt;/button&gt;&lt;/p&gt;" Format="" /></Data>', 130)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (251, 2, 0, 0, N'<Data><BodyPart Text="&lt;p&gt;&lt;button&gt;&lt;a href=&quot;/pricing&quot;&gt;asd&lt;/a&gt;&lt;/button&gt;&lt;/p&gt;" Format="" /></Data>', 130)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (252, 3, 0, 0, N'<Data><BodyPart Text="&lt;p&gt;&lt;button class=&quot;.tb-button&quot;&gt;&lt;a href=&quot;/pricing&quot;&gt;asd&lt;/a&gt;&lt;/button&gt;&lt;/p&gt;" Format="" /></Data>', 130)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (253, 4, 1, 1, N'<Data><BodyPart Text="&lt;p&gt;&lt;a href=&quot;/pricing&quot;&gt;&lt;button class=&quot;.tb-button&quot;&gt;NEXT PAGE&lt;/button&gt;&lt;/a&gt;&lt;/p&gt;" Format="" /></Data>', 130)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (254, 1, 1, 1, N'<Data><BodyPart Text="&lt;p&gt;&lt;a href=&quot;/details&quot;&gt;&lt;button class=&quot;.tb-button&quot;&gt;NEXT PAGE&lt;/button&gt;&lt;/a&gt;&lt;/p&gt;" Format="" /></Data>', 131)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (255, 1, 0, 0, N'<Data><BodyPart Text="&lt;p&gt;&lt;a href=&quot;/&quot;&gt;&lt;button class=&quot;.tb-button&quot;&gt;LOUNCH&lt;/button&gt;&lt;/a&gt;&lt;/p&gt;" /></Data>', 132)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (256, 2, 0, 0, N'<Data><BodyPart Text="&lt;p&gt;&lt;a href=&quot;/&quot;&gt;&lt;button class=&quot;.tb-button&quot;&gt;LOUNCH&lt;/button&gt;&lt;/a&gt;&lt;/p&gt;" Format="" /></Data>', 132)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (257, 3, 1, 1, N'<Data><BodyPart Text="&lt;p&gt;&lt;a href=&quot;/&quot;&gt;&lt;button class=&quot;.tb-button&quot;&gt;LAUNCH&lt;/button&gt;&lt;/a&gt;&lt;/p&gt;" Format="" /></Data>', 132)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (258, 1, 1, 1, N'<Data><AutoroutePart DisplayAlias="LearnMore" CustomPattern="" UseCustomPattern="false" /><TitlePart Title="Learn More" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 133)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (259, 1, 1, 1, NULL, 134)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (260, 1, 0, 0, N'<Data><BodyPart Text="&lt;div class=&quot;insideContentTop&quot;&gt;&lt;/div&gt;" Format="" /></Data>', 135)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (261, 1, 1, 1, N'<Data><AutoroutePart DisplayAlias="TrackYourOrder" CustomPattern="" UseCustomPattern="false" /><TitlePart Title="Track Your Order" /><LayoutPart TemplateId="null" LayoutData="{&quot;elements&quot;:[{&quot;typeName&quot;:&quot;Orchard.Layouts.Elements.Canvas&quot;,&quot;data&quot;:&quot;&quot;,&quot;exportableData&quot;:&quot;&quot;,&quot;index&quot;:0,&quot;elements&quot;:[],&quot;isTemplated&quot;:false,&quot;htmlId&quot;:&quot;&quot;,&quot;htmlClass&quot;:&quot;&quot;,&quot;htmlStyle&quot;:&quot;&quot;}]}" /></Data>', 136)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (262, 1, 1, 1, NULL, 137)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (263, 1, 1, 1, N'<Data><BodyPart Text="&lt;div class=&quot;insideContentTop&quot;&gt;&lt;/div&gt;" Format="" /></Data>', 138)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (264, 2, 0, 0, NULL, 62)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (265, 3, 1, 1, NULL, 62)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (266, 1, 1, 1, N'<Data><TitlePart Title="Authenticated Menu" /></Data>', 139)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (267, 1, 1, 1, NULL, 140)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (268, 1, 1, 1, NULL, 141)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (269, 1, 1, 1, N'<Data />', 142)
INSERT INTO [dbo].[Orchard_Framework_ContentItemVersionRecord] ([Id], [Number], [Published], [Latest], [Data], [ContentItemRecord_id]) VALUES (272, 1, 1, 1, NULL, 144)
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentItemVersionRecord] OFF
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentItemRecord] ON
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (99, N'<Data><CommonPart CreatedUtc="2015-07-05T09:12:17.8134478Z" ModifiedUtc="2015-07-05T09:12:39.2967921Z" PublishedUtc="2015-07-05T09:12:39.3227933Z" /><LayoutPart SessionKey="b1e229c7-db39-482f-86c7-50327176b597" /><TagsPart CurrentTags="" /></Data>', 5)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (100, N'<Data><CommonPart CreatedUtc="2015-07-05T09:17:06.6566462Z" ModifiedUtc="2015-07-05T10:35:09.6209258Z" PublishedUtc="2015-07-05T10:35:09.6529261Z" /><LayoutPart SessionKey="0558f2eb-1c19-4c6b-8af0-071455c99dd5" /><TagsPart CurrentTags="" /></Data>', 5)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (101, N'<Data><CommonPart CreatedUtc="2015-07-05T09:22:27.4592841Z" ModifiedUtc="2015-07-05T09:22:27.5302877Z" PublishedUtc="2015-07-05T09:22:27.5413056Z" /><LayoutPart SessionKey="b8a1e92b-ae08-48eb-92eb-c78304591017" /><TagsPart CurrentTags="" /></Data>', 5)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (102, N'<Data><IdentityPart Identifier="ef10eab7accc43bb8d9309ca2e30a79e" /><CommonPart CreatedUtc="2015-07-05T09:25:56.1353755Z" ModifiedUtc="2015-07-05T09:26:43.8446495Z" PublishedUtc="2015-07-05T09:25:56.2653847Z" /><MenuItemPart Url="~/design" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (103, N'<Data><IdentityPart Identifier="f17f81082d7d49de9c7844278ace9f71" /><CommonPart CreatedUtc="2015-07-05T09:27:19.5427447Z" ModifiedUtc="2015-07-05T09:49:16.2742945Z" PublishedUtc="2015-07-05T09:49:16.291298Z" /></Data>', 1)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (104, N'<Data><IdentityPart Identifier="12e5d43b7c974907920b317ea63e9f19" /><CommonPart CreatedUtc="2015-07-05T09:27:55.5038228Z" ModifiedUtc="2015-07-05T09:27:55.5278242Z" PublishedUtc="2015-07-05T09:27:55.5448253Z" /><MenuItemPart Url="~/design" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (105, N'<Data><IdentityPart Identifier="d539ccec113445fdbe6444ca299debd8" /><CommonPart CreatedUtc="2015-07-05T09:29:20.9407346Z" ModifiedUtc="2015-07-05T09:49:31.5262102Z" PublishedUtc="2015-07-05T09:49:31.5372105Z" /></Data>', 1)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (106, N'<Data><IdentityPart Identifier="3dd7224ebf504a9a87ea7168d0213387" /><CommonPart CreatedUtc="2015-07-05T09:30:06.7799562Z" ModifiedUtc="2015-07-05T09:30:06.8199603Z" PublishedUtc="2015-07-05T09:30:06.8599635Z" /><MenuItemPart Url="~/design" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (107, N'<Data><IdentityPart Identifier="75bd45e197894a02a27e9c41295538ec" /><CommonPart CreatedUtc="2015-07-05T09:31:20.0291885Z" ModifiedUtc="2015-07-05T09:31:20.0491893Z" PublishedUtc="2015-07-05T09:31:20.0781911Z" /><MenuItemPart Url="~/pricing" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (108, N'<Data><IdentityPart Identifier="f706b2bdc67545ea8976d16435e9ff7e" /><CommonPart CreatedUtc="2015-07-05T09:32:08.3249784Z" ModifiedUtc="2015-07-05T09:32:08.3529801Z" PublishedUtc="2015-07-05T09:32:08.3769812Z" /><MenuItemPart Url="~/details" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (109, N'<Data><IdentityPart Identifier="e8fa2be34dd148daba1067fd1fd4d188" /><CommonPart CreatedUtc="2015-07-05T09:33:32.8883604Z" ModifiedUtc="2015-07-05T12:31:24.3051053Z" PublishedUtc="2015-07-05T09:33:32.9223625Z" /></Data>', 1)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (110, N'<Data><IdentityPart Identifier="5aebe0686c6a46ccafd4c50f3a52eb3d" /><CommonPart CreatedUtc="2015-07-05T09:34:00.6509651Z" ModifiedUtc="2015-07-05T09:34:15.9238653Z" PublishedUtc="2015-07-05T09:34:00.7000013Z" /><MenuItemPart Url="~/pricing" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (111, N'<Data><IdentityPart Identifier="c71a0e6545a248b0834c5014921942dd" /><CommonPart CreatedUtc="2015-07-05T09:34:54.7140876Z" ModifiedUtc="2015-07-05T12:31:24.3251065Z" PublishedUtc="2015-07-05T09:34:54.8120935Z" /><MenuItemPart Url="~/pricing" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (112, N'<Data><IdentityPart Identifier="1bdf41ffcbe7464fabe71bfe7cd35403" /><CommonPart CreatedUtc="2015-07-05T09:35:57.2017817Z" ModifiedUtc="2015-07-05T12:31:37.0508074Z" PublishedUtc="2015-07-05T09:35:57.241785Z" /></Data>', 1)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (113, N'<Data><IdentityPart Identifier="e984891477814ef1bec911bb9c23c67d" /><CommonPart CreatedUtc="2015-07-05T09:36:37.4491075Z" ModifiedUtc="2015-07-05T12:31:37.0638099Z" PublishedUtc="2015-07-05T09:36:37.4991097Z" /><MenuItemPart Url="~/" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (114, N'<Data><IdentityPart Identifier="b1116dfd60244895b8d22f9758ac239d" /><CommonPart CreatedUtc="2015-07-05T09:37:07.2098405Z" ModifiedUtc="2015-07-05T12:31:47.7914625Z" PublishedUtc="2015-07-05T09:37:07.2698413Z" /></Data>', 1)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (115, N'<Data><IdentityPart Identifier="cd2218af974e46f8bf19afbea4861d3e" /><CommonPart CreatedUtc="2015-07-05T09:37:34.4374001Z" ModifiedUtc="2015-07-05T12:31:47.8044629Z" PublishedUtc="2015-07-05T09:37:34.4954033Z" /><MenuItemPart Url="~/details" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (116, N'<Data><CommonPart CreatedUtc="2015-07-05T09:43:23.9962558Z" ModifiedUtc="2015-07-05T09:43:24.1072626Z" PublishedUtc="2015-07-05T09:43:24.0292582Z" /><LayerPart Name="Tee Wizard" Description="Tee Wizard Layer" LayerRule="url(&quot;~/design&quot;) or url(&quot;~/pricing&quot;) or url(&quot;~/details&quot;)" /></Data>', 4)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (117, N'<Data><CommonPart CreatedUtc="2015-07-05T09:44:17.3793412Z" ModifiedUtc="2015-07-05T09:44:17.4633451Z" PublishedUtc="2015-07-05T09:44:17.4093426Z" /><LayerPart Name="Design Page Layer" Description="Design Page Layer" LayerRule="url(&quot;~/design&quot;)" /></Data>', 4)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (118, N'<Data><CommonPart CreatedUtc="2015-07-05T09:45:20.9016809Z" ModifiedUtc="2015-07-05T09:45:20.9516852Z" PublishedUtc="2015-07-05T09:45:20.915683Z" /><LayerPart Name="Pricing Page Layer" Description="Pricing Page Layer" LayerRule="url(&quot;~/pricing&quot;)" /></Data>', 4)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (119, N'<Data><CommonPart CreatedUtc="2015-07-05T09:46:35.828976Z" ModifiedUtc="2015-07-05T09:46:35.8699961Z" PublishedUtc="2015-07-05T09:46:35.8449765Z" /><LayerPart Name="Details Page Layer" Description="Details Page Layer" LayerRule="url(&quot;~/details&quot;)" /></Data>', 4)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (120, N'<Data><IdentityPart Identifier="b045595134544b0da78054660c6c1b46" /><CommonPart CreatedUtc="2015-07-05T09:48:20.9080961Z" ModifiedUtc="2015-07-05T10:43:07.7056651Z" PublishedUtc="2015-07-05T09:48:21.2801525Z" /><WidgetPart RenderTitle="false" Title="Wizard Menu" Position="1" Zone="Navigation" Name="WizardMenu" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="105" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (121, N'<Data><IdentityPart Identifier="9a9a4f2559c941459d429d97ca6d40b0" /><CommonPart CreatedUtc="2015-07-05T09:52:17.4278275Z" ModifiedUtc="2015-07-05T12:16:55.811705Z" PublishedUtc="2015-07-05T09:52:17.6578408Z" /><WidgetPart RenderTitle="false" Title="Design Next Page Widget" Position="4" Zone="AfterContent" Name="designNetxPageWidget" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="109" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (122, N'<Data><IdentityPart Identifier="d740c1711f214f439df09d7f6353d839" /><CommonPart CreatedUtc="2015-07-05T09:53:49.5812234Z" ModifiedUtc="2015-07-05T12:22:39.4746516Z" PublishedUtc="2015-07-05T09:53:49.7462302Z" /><WidgetPart RenderTitle="false" Title="Pricing Next Page Widget" Position="3" Zone="AfterContent" Name="pricingNextPageWidget" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="114" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (123, N'<Data><IdentityPart Identifier="9c58a951f5444ba69df93ea95d64125e" /><CommonPart CreatedUtc="2015-07-05T09:55:43.5188076Z" ModifiedUtc="2015-07-05T12:27:44.2703133Z" PublishedUtc="2015-07-05T09:55:43.6988166Z" /><WidgetPart RenderTitle="false" Title="Details Launch Page Widget" Position="3" Zone="AfterContent" Name="detailsLaunchPageWidget" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="112" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (124, N'<Data><IdentityPart Identifier="2f6b0e26642e4c6a82a5f4fbf6623a10" /><CommonPart CreatedUtc="2015-07-05T09:57:38.7705148Z" ModifiedUtc="2015-07-05T09:57:38.9795284Z" PublishedUtc="2015-07-05T09:57:39.0195293Z" /><WidgetPart RenderTitle="false" Title="Get Started Menu" Position="6" Zone="Header" Name="getStartedMenu" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="103" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (125, N'<Data><IdentityPart Identifier="4fdde536035a442d8ac81af57e4bd862" /><CommonPart CreatedUtc="2015-07-05T10:38:46.789525Z" ModifiedUtc="2015-07-05T10:41:41.6456778Z" PublishedUtc="2015-07-05T10:38:47.0095372Z" /><WidgetPart RenderTitle="false" Title="Create Top Contet" Position="1" Zone="Featured" Name="createTopContent" /></Data>', 13)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (127, N'<Data><IdentityPart Identifier="3edaedb0bfe2444a87a91fa4b05e3a28" /><CommonPart CreatedUtc="2015-07-05T10:44:00.4847127Z" ModifiedUtc="2015-07-05T10:46:34.2256628Z" PublishedUtc="2015-07-05T10:44:00.9247388Z" /><WidgetPart RenderTitle="false" Title="Wizard Menu" Position="1" Zone="BeforeMain" Name="WizardMenu" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="105" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (128, N'<Data><IdentityPart Identifier="1bbcfe78985743448684dc20683e2ef9" /><CommonPart CreatedUtc="2015-07-05T10:47:01.9112981Z" ModifiedUtc="2015-07-05T10:47:02.1083093Z" PublishedUtc="2015-07-05T12:51:46.9997029Z" /><WidgetPart RenderTitle="false" Title="Wizard Menu" Position="3" Zone="Featured" Name="WizardMenu" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="105" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (130, N'<Data><IdentityPart Identifier="aa5a0cfb02154340b7fde9323c4f792f" /><CommonPart CreatedUtc="2015-07-05T12:08:04.4882479Z" ModifiedUtc="2015-07-05T12:17:00.3480021Z" PublishedUtc="2015-07-05T12:17:00.3760038Z" /><WidgetPart RenderTitle="false" Title="Design Next Page Widget" Position="1" Zone="AfterContent" Name="designNetxPageWidget" /></Data>', 13)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (131, N'<Data><IdentityPart Identifier="26c358b5151b428f84e91872b6fccab0" /><CommonPart CreatedUtc="2015-07-05T12:24:18.0443733Z" ModifiedUtc="2015-07-05T12:24:18.2653864Z" PublishedUtc="2015-07-05T12:24:18.3193893Z" /><WidgetPart RenderTitle="false" Title="Pricing Next Page Widget" Position="2" Zone="AfterContent" Name="pricingNextPageWidget" /></Data>', 13)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (132, N'<Data><IdentityPart Identifier="511ce46312b64e918b749c9d2cae7143" /><CommonPart CreatedUtc="2015-07-05T12:28:46.023915Z" ModifiedUtc="2015-07-05T12:40:14.7719886Z" PublishedUtc="2015-07-05T12:40:14.8259906Z" /><WidgetPart RenderTitle="false" Title="Details Launch Page Widget" Position="3" Zone="AfterContent" Name="detailsLaunchPageWidget" /></Data>', 13)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (133, N'<Data><CommonPart CreatedUtc="2015-07-05T12:43:04.3478317Z" ModifiedUtc="2015-07-05T12:43:04.4468374Z" PublishedUtc="2015-07-05T12:43:04.463838Z" /><LayoutPart SessionKey="cadd5359-00d2-4a17-a62d-3fa0e270e47d" /><TagsPart CurrentTags="" /></Data>', 5)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (134, N'<Data><CommonPart CreatedUtc="2015-07-05T12:43:46.8003198Z" ModifiedUtc="2015-07-05T12:43:46.8533222Z" PublishedUtc="2015-07-05T12:43:46.8163191Z" /><LayerPart Name="Learn More Page" Description="Learn More Page" LayerRule="url(&quot;~/LearnMore&quot;)" /></Data>', 4)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (135, N'<Data><IdentityPart Identifier="4ebf6cb78c124932b7a25de8e3438a0f" /><CommonPart CreatedUtc="2015-07-05T12:54:40.2337529Z" ModifiedUtc="2015-07-05T14:32:54.9625536Z" PublishedUtc="2015-07-05T14:17:22.7520241Z" /><WidgetPart RenderTitle="false" Title="Content Top" Position="2" Zone="Featured" Name="contentTop" /></Data>', 13)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (136, N'<Data><CommonPart CreatedUtc="2015-07-05T13:03:57.8101242Z" ModifiedUtc="2015-07-05T13:03:57.8951257Z" PublishedUtc="2015-07-05T13:03:57.9151261Z" /><LayoutPart SessionKey="26bd1793-574d-48ef-a5a6-33b2814e9393" /><TagsPart CurrentTags="" /></Data>', 5)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (137, N'<Data><CommonPart CreatedUtc="2015-07-05T13:05:02.0468552Z" ModifiedUtc="2015-07-05T13:05:02.0948573Z" PublishedUtc="2015-07-05T13:05:02.0618561Z" /><LayerPart Name="Track Your Order Page" Description="Track Your Order Page" LayerRule="url(&quot;~/TrackYourOrder&quot;)" /></Data>', 4)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (138, N'<Data><IdentityPart Identifier="f4947a38cd2c4563bfad8972fb08f88c" /><CommonPart CreatedUtc="2015-07-05T13:07:33.5416302Z" ModifiedUtc="2015-07-05T13:07:33.6746305Z" PublishedUtc="2015-07-05T13:07:33.7076329Z" /><WidgetPart RenderTitle="false" Title="Content Top in Track Your Order Page" Position="1" Zone="Featured" Name="contentTopInTrackYourOrderPage" /></Data>', 13)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (139, N'<Data><IdentityPart Identifier="7438c630c2884b3882bf05f6bdacd265" /><CommonPart CreatedUtc="2015-07-05T13:59:32.9620813Z" ModifiedUtc="2015-07-05T13:59:33.0660815Z" PublishedUtc="2015-07-05T13:59:33.0940816Z" /></Data>', 1)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (140, N'<Data><IdentityPart Identifier="450120e9ca4d4342a38ce12c631e89c3" /><CommonPart CreatedUtc="2015-07-05T13:59:58.1795386Z" ModifiedUtc="2015-07-05T13:59:58.2685436Z" PublishedUtc="2015-07-05T13:59:58.2965449Z" /><MenuItemPart Url="~/LearnMore" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (141, N'<Data><IdentityPart Identifier="8dd67e37ee3f4efd88ed672e9689c65c" /><CommonPart CreatedUtc="2015-07-05T14:00:21.5119221Z" ModifiedUtc="2015-07-05T14:00:21.5349395Z" PublishedUtc="2015-07-05T14:00:21.5659229Z" /><MenuItemPart Url="~/TrackYourOrder" /></Data>', 6)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (142, N'<Data><IdentityPart Identifier="234c59ee28654224a0227096ed98d198" /><CommonPart CreatedUtc="2015-07-05T14:01:54.4473059Z" ModifiedUtc="2015-07-05T14:01:54.7043134Z" PublishedUtc="2015-07-05T14:01:54.7643129Z" /><WidgetPart RenderTitle="false" Title="Authenticated Menu" Position="3" Zone="Header" Name="authenticatedMenu" /><MenuWidgetPart StartLevel="1" Levels="0" Breadcrumb="false" AddHomePage="false" AddCurrentPage="false" ShowFullMenu="false" MenuContentItemId="139" /></Data>', 7)
INSERT INTO [dbo].[Orchard_Framework_ContentItemRecord] ([Id], [Data], [ContentType_id]) VALUES (144, N'<Data><CommonPart CreatedUtc="2015-07-05T14:22:59.7670595Z" ModifiedUtc="2015-07-05T14:30:18.2720231Z" PublishedUtc="2015-07-05T14:22:59.7830613Z" /><LayerPart Name="Default page without home and get started page" Description="Default page without home and get started page" LayerRule="not (url(&quot;~/&quot;) or url(&quot;~/design&quot;) or url(&quot;~/pricing&quot;) or url(&quot;~/details&quot;))" /></Data>', 4)
SET IDENTITY_INSERT [dbo].[Orchard_Framework_ContentItemRecord] OFF
SET IDENTITY_INSERT [dbo].[Orchard_Indexing_IndexingTaskRecord] ON
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (210, 0, '20150705 09:12:39.000', 99)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (217, 0, '20150705 09:22:27.000', 101)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (218, 1, '20150705 09:25:29.000', 97)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (221, 1, '20150705 09:26:43.000', 102)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (226, 0, '20150705 09:27:55.000', 104)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (231, 0, '20150705 09:30:06.000', 106)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (233, 0, '20150705 09:31:20.000', 107)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (235, 0, '20150705 09:32:08.000', 108)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (241, 1, '20150705 09:34:15.000', 110)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (256, 0, '20150705 09:43:24.000', 116)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (259, 0, '20150705 09:44:17.000', 117)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (262, 0, '20150705 09:45:20.000', 118)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (265, 0, '20150705 09:46:35.000', 119)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (272, 0, '20150705 09:49:16.000', 103)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (274, 0, '20150705 09:49:31.000', 105)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (286, 0, '20150705 09:57:39.000', 124)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (292, 0, '20150705 10:35:09.000', 100)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (296, 1, '20150705 10:41:41.000', 125)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (300, 1, '20150705 10:43:07.000', 120)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (304, 1, '20150705 10:46:34.000', 127)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (318, 1, '20150705 12:16:55.000', 121)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (320, 0, '20150705 12:17:00.000', 130)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (321, 1, '20150705 12:22:39.000', 122)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (324, 0, '20150705 12:24:18.000', 131)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (325, 1, '20150705 12:27:44.000', 123)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (331, 1, '20150705 12:31:24.000', 111)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (332, 1, '20150705 12:31:24.000', 109)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (333, 1, '20150705 12:31:37.000', 113)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (334, 1, '20150705 12:31:37.000', 112)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (335, 1, '20150705 12:31:47.000', 115)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (336, 1, '20150705 12:31:47.000', 114)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (338, 0, '20150705 12:40:14.000', 132)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (341, 0, '20150705 12:43:04.000', 133)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (344, 0, '20150705 12:43:46.000', 134)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (346, 0, '20150705 12:51:47.000', 128)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (352, 0, '20150705 13:03:57.000', 136)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (355, 0, '20150705 13:05:02.000', 137)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (358, 0, '20150705 13:07:33.000', 138)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (362, 0, '20150705 13:12:13.000', 62)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (363, 0, '20150705 13:50:25.000', 7)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (366, 0, '20150705 13:59:33.000', 139)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (368, 0, '20150705 13:59:58.000', 140)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (370, 0, '20150705 14:00:21.000', 141)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (373, 0, '20150705 14:01:54.000', 142)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (388, 0, '20150705 14:30:18.000', 144)
INSERT INTO [dbo].[Orchard_Indexing_IndexingTaskRecord] ([Id], [Action], [CreatedUtc], [ContentItemRecord_id]) VALUES (389, 1, '20150705 14:32:54.000', 135)
SET IDENTITY_INSERT [dbo].[Orchard_Indexing_IndexingTaskRecord] OFF
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (120, N'Wizard Menu', N'1', N'Navigation', 0, N'WizardMenu')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (121, N'Design Next Page Widget', N'4', N'AfterContent', 0, N'designNetxPageWidget')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (122, N'Pricing Next Page Widget', N'3', N'AfterContent', 0, N'pricingNextPageWidget')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (123, N'Details Launch Page Widget', N'3', N'AfterContent', 0, N'detailsLaunchPageWidget')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (124, N'Get Started Menu', N'6', N'Header', 0, N'getStartedMenu')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (125, N'Create Top Contet', N'1', N'Featured', 0, N'createTopContent')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (127, N'Wizard Menu', N'1', N'BeforeMain', 0, N'WizardMenu')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (128, N'Wizard Menu', N'3', N'Featured', 0, N'WizardMenu')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (130, N'Design Next Page Widget', N'1', N'AfterContent', 0, N'designNetxPageWidget')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (131, N'Pricing Next Page Widget', N'2', N'AfterContent', 0, N'pricingNextPageWidget')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (132, N'Details Launch Page Widget', N'3', N'AfterContent', 0, N'detailsLaunchPageWidget')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (135, N'Content Top', N'2', N'Featured', 0, N'contentTop')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (138, N'Content Top in Track Your Order Page', N'1', N'Featured', 0, N'contentTopInTrackYourOrderPage')
INSERT INTO [dbo].[Orchard_Widgets_WidgetPartRecord] ([Id], [Title], [Position], [Zone], [RenderTitle], [Name]) VALUES (142, N'Authenticated Menu', N'3', N'Header', 0, N'authenticatedMenu')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (116, N'Tee Wizard', N'Tee Wizard Layer', N'url("~/design") or url("~/pricing") or url("~/details")')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (117, N'Design Page Layer', N'Design Page Layer', N'url("~/design")')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (118, N'Pricing Page Layer', N'Pricing Page Layer', N'url("~/pricing")')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (119, N'Details Page Layer', N'Details Page Layer', N'url("~/details")')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (134, N'Learn More Page', N'Learn More Page', N'url("~/LearnMore")')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (137, N'Track Your Order Page', N'Track Your Order Page', N'url("~/TrackYourOrder")')
INSERT INTO [dbo].[Orchard_Widgets_LayerPartRecord] ([Id], [Name], [Description], [LayerRule]) VALUES (144, N'Default page without home and get started page', N'Default page without home and get started page', N'not (url("~/") or url("~/design") or url("~/pricing") or url("~/details"))')
SET IDENTITY_INSERT [dbo].[Settings_ShellFeatureRecord] ON
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2015, N'Orchard.Framework', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2016, N'Common', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2017, N'Containers', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2018, N'Contents', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2019, N'Dashboard', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2020, N'Feeds', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2021, N'Navigation', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2022, N'Reports', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2023, N'Scheduling', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2024, N'Settings', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2025, N'Shapes', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2026, N'Title', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2027, N'Orchard.Pages', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2028, N'Orchard.ContentPicker', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2029, N'Orchard.Themes', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2030, N'Orchard.Users', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2031, N'Orchard.Roles', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2032, N'Orchard.Modules', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2033, N'PackagingServices', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2034, N'Orchard.Packaging', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2035, N'Gallery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2036, N'Orchard.Recipes', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2037, N'Orchard.Blogs', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2038, N'Orchard.Widgets', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2039, N'Orchard.jQuery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2040, N'Orchard.PublishLater', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2041, N'Orchard.Autoroute', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2042, N'Orchard.Alias', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2043, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2044, N'Orchard.Scripting', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2045, N'Orchard.Comments', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2046, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2047, N'Orchard.Tags', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2048, N'Orchard.jQuery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2049, N'Orchard.Alias', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2050, N'Orchard.Autoroute', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2051, N'Orchard.Alias', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2052, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2053, N'TinyMce', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2054, N'Orchard.MediaLibrary', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2055, N'Orchard.MediaProcessing', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2056, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2057, N'Orchard.Forms', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2058, N'Orchard.ContentPicker', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2059, N'Orchard.PublishLater', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2060, N'Orchard.jQuery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2061, N'Orchard.Widgets', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2062, N'Orchard.Scripting', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2063, N'Orchard.ContentTypes', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2064, N'Orchard.Scripting', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2065, N'Orchard.Scripting.Lightweight', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2066, N'Orchard.Scripting', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2067, N'PackagingServices', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2068, N'Orchard.Packaging', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2069, N'Orchard.Projections', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2070, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2071, N'Orchard.Forms', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2072, N'Orchard.Fields', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2073, N'Orchard.jQuery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2074, N'Orchard.OutputCache', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2075, N'Orchard.Taxonomies', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2076, N'Orchard.Autoroute', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2077, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2078, N'Orchard.Alias', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2079, N'Orchard.Workflows', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2080, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2081, N'Orchard.Forms', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2082, N'Orchard.jQuery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2083, N'Orchard.Layouts', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2084, N'Orchard.jQuery', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2085, N'Orchard.Forms', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2086, N'Orchard.Tokens', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2087, N'Orchard.MediaLibrary', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2088, N'TinyMce', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2089, N'Orchard.MediaProcessing', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2090, N'TheThemeMachine', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2091, N'Orchard.CodeGeneration', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2092, N'Teebay.Theme', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2093, N'TheThemeMachine', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2094, N'Teebay.Theme', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2095, N'TheThemeMachine', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2096, N'Teebay.Theme', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2097, N'Orchard.Search', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2098, N'Orchard.Indexing', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2099, N'Lucene', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2100, N'Teebay.Theme', 1)
INSERT INTO [dbo].[Settings_ShellFeatureRecord] ([Id], [Name], [ShellDescriptorRecord_id]) VALUES (2101, N'Orchard.DesignerTools', 1)
SET IDENTITY_INSERT [dbo].[Settings_ShellFeatureRecord] OFF
INSERT INTO [dbo].[Orchard_Tags_TagsPartRecord] ([Id]) VALUES (99)
INSERT INTO [dbo].[Orchard_Tags_TagsPartRecord] ([Id]) VALUES (100)
INSERT INTO [dbo].[Orchard_Tags_TagsPartRecord] ([Id]) VALUES (101)
INSERT INTO [dbo].[Orchard_Tags_TagsPartRecord] ([Id]) VALUES (133)
INSERT INTO [dbo].[Orchard_Tags_TagsPartRecord] ([Id]) VALUES (136)
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (214, 99, NULL, 0, N'design')
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (215, 100, NULL, 0, N'details')
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (216, 101, NULL, 0, N'pricing')
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (243, 100, NULL, 0, N'details')
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (244, 100, NULL, 0, N'details')
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (258, 133, NULL, 0, N'LearnMore')
INSERT INTO [dbo].[Orchard_Autoroute_AutoroutePartRecord] ([Id], [ContentItemRecord_id], [CustomPattern], [UseCustomPattern], [DisplayAlias]) VALUES (261, 136, NULL, 0, N'TrackYourOrder')
COMMIT TRANSACTION
