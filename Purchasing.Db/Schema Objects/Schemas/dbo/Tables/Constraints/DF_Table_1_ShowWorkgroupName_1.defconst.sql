﻿ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_Table_1_ShowWorkgroupName] DEFAULT ((0)) FOR [ShowWorkgroup];
