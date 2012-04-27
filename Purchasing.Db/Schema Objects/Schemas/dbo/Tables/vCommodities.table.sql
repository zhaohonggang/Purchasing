﻿CREATE TABLE [dbo].[vCommodities] (
    [Id]              VARCHAR (9)  NOT NULL,
    [Name]            VARCHAR (60) NOT NULL,
    [GroupCode]       VARCHAR (4)  NULL,
    [SubGroupCode]    VARCHAR (2)  NULL,
    [IsActive]        BIT          NOT NULL,
    [PartitionColumn] INT          NOT NULL
) ON [EvenOddPartitionScheme] ([PartitionColumn]);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Commodity_num/Commodity_code', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vCommodities', @level2type = N'COLUMN', @level2name = N'Id';
