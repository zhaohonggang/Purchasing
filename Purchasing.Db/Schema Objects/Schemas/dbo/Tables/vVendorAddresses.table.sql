﻿CREATE TABLE [dbo].[vVendorAddresses] (
    [VendorId]    CHAR (10)    NOT NULL,
    [Name]        VARCHAR (40) NOT NULL,
    [Line1]       VARCHAR (40) NOT NULL,
    [Line2]       VARCHAR (40) NULL,
    [Line3]       VARCHAR (40) NULL,
    [City]        VARCHAR (40) NOT NULL,
    [State]       CHAR (2)     NOT NULL,
    [Zip]         VARCHAR (11) NOT NULL,
    [CountryCode] VARCHAR (2)  NULL
);

