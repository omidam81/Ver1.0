
CREATE TABLE [dbo].[Orchard_Indexing_IndexingTaskRecord] (
    [Id]                   INT      IDENTITY (1, 1) NOT NULL,
    [Action]               INT      NULL,
    [CreatedUtc]           DATETIME NULL,
    [ContentItemRecord_id] INT      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Orchard_Media_MediaSettingsPartRecord]...';


GO
CREATE TABLE [dbo].[Orchard_Media_MediaSettingsPartRecord] (
    [Id]                             INT            NOT NULL,
    [UploadAllowedFileTypeWhitelist] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating unnamed constraint on [dbo].[Orchard_Media_MediaSettingsPartRecord]...';


GO
ALTER TABLE [dbo].[Orchard_Media_MediaSettingsPartRecord]
    ADD DEFAULT ('jpg jpeg gif png txt doc docx xls xlsx pdf ppt pptx pps ppsx odt ods odp') FOR [UploadAllowedFileTypeWhitelist];


GO
PRINT N'Update complete.';


GO