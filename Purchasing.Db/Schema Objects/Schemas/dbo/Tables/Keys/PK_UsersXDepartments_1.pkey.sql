﻿ALTER TABLE [dbo].[UsersXOrganizations]
    ADD CONSTRAINT [PK_UsersXDepartments] PRIMARY KEY CLUSTERED ([UserId] ASC, [OrganizationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

