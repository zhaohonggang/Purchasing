﻿ALTER TABLE [dbo].[vVendors]
    ADD CONSTRAINT [PK_vVendors] PRIMARY KEY CLUSTERED ([Id] ASC, [PartitionColumn] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);



