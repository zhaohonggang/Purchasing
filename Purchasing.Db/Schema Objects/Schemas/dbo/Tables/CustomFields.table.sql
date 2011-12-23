﻿CREATE TABLE [dbo].[CustomFields] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (MAX) NOT NULL,
    [OrganizationId] CHAR (4)      NOT NULL,
    [Order]          INT           NOT NULL,
    [IsActive]       BIT           NOT NULL,
    [IsRequired]     BIT           NOT NULL
);
