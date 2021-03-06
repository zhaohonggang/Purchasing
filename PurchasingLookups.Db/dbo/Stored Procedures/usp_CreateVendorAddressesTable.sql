﻿-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the VendorAddresses table.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateVendorAddressesTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vVendorAddresses', --Default table name.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
	DECLARE @LoadTableName varchar(255) = 'vVendorAddresses'
	DECLARE @IsDebug bit = 0
*/
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;  --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF
	
	DECLARE @TSQL varchar(MAX) = ''
	
		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @LoadTableName + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @LoadTableName + '](
				[Id] [uniqueidentifier] NOT NULL,
				[VendorId] [char](10) NOT NULL,
				[TypeCode] [varchar](4) NOT NULL,
				[Name] [varchar](40)  NOT NULL,
				[Line1] [varchar](40) NOT NULL,
				[Line2] [varchar](40) NULL,
				[Line3] [varchar](40) NULL,
				[City] [varchar](40)  NOT NULL,
				[State] [char](2) NULL,
				[Zip] [varchar](11)  NULL,
				[CountryCode] [varchar](2) NULL,
				[PhoneNumber] [varchar](15) NULL,
				[FaxNumber] [varchar](15) NULL,
				[Email] [varchar](50) NULL,
				[Url] [varchar](128) NULL,
				[IsDefault] [bit] NULL, 
				CONSTRAINT [PK_' + @LoadTableName + '] PRIMARY KEY CLUSTERED
			(
				[VendorId] ASC,
				[TypeCode] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
) ON [PRIMARY]

			SET ANSI_PADDING ON

			ALTER TABLE [dbo].[' + @LoadTableName + '] ADD  CONSTRAINT [DF_' + @LoadTableName + '_Id]  DEFAULT (newid()) FOR [Id]
		'
		
		IF @IsDebug = 1 
			PRINT @TSQL 
		ELSE 
			EXEC(@TSQL)
END
GO

