CREATE TABLE [dbo].[GameServer] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (255) DEFAULT ('PP2 Server') NOT NULL,
    [Dns]               VARCHAR (500) DEFAULT ('server dns') NOT NULL,
    [Region]            VARCHAR (50)  DEFAULT ('westeurope') NOT NULL,
    [Port]              INT           DEFAULT ((55551)) NOT NULL,
    [Max_Players_Limit] INT           DEFAULT ((0)) NOT NULL,
    [Players_Count]     INT           DEFAULT ((0)) NOT NULL,
    [Status]            VARCHAR (50)  DEFAULT ('None') NOT NULL,
    [Created_At]        DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

