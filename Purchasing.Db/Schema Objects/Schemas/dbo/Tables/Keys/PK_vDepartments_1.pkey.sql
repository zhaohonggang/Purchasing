﻿ALTER TABLE [dbo].[vDepartments]
    ADD CONSTRAINT [PK_vDepartments_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

