﻿ALTER TABLE [dbo].[KfsDocuments]
    ADD CONSTRAINT [FK_DocumentNumbers_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

