USE [PrePurchasing]
GO
/****** Object:  StoredProcedure [dbo].[usp_DownloadAccountsPartitionTable]    Script Date: 02/14/2012 08:36:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download account data and ultimately load into the vAccountsPartitionTable
-- =============================================
ALTER PROCEDURE [dbo].[usp_DownloadAccountsPartitionTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vAccountsPartitionTable', --Table name of load table being loaded 
	@ReferentialTableName varchar(244) = 'vOrganizationsPartitionTable', --Name of Organizations table being referenced 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.
	@PartitionColumn char(1) = 0, --Number to use for partition column
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TableName varchar(255) = SUBSTRING(@LoadTableName, 0, CHARINDEX('_Load',@LoadTableName ))
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
	TRUNCATE TABLE [PrePurchasing].[dbo].[' + @LoadTableName + ']
	
	INSERT INTO ' + @LoadTableName + ' SELECT 
       [Id]
      ,[Name]
      ,  (CASE WHEN (ExpirationDate IS NOT NULL AND ExpirationDate <= GETDATE()) THEN 0
		  ELSE 1 END) AS [IsActive]
      ,[AccountManager]
      ,[AccountManagerId]
      ,[PrincipalInvestigator]
      ,[PrincipalInvestigatorId]
      ,[OrganizationId]
      , ' + Convert(char(1), @PartitionColumn) + ' AS [PartitionColumn]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT 
			A.CHART_NUM || ''''-'''' || A.ACCT_NUM AS Id,
			A.ACCT_NAME AS Name,
			A.acct_expiration_date AS ExpirationDate,
			A.ACCT_MGR_NAME AS AccountManager,
			A.ACCT_MGR_ID AS AccountManagerId,
			A.PRINCIPAL_INVESTIGATOR_NAME AS PrincipalInvestigator,
			A.PRINCIPAL_INVESTIGATOR_ID AS PrincipalInvestigatorId,
			A.CHART_NUM || ''''-'''' || A.ORG_ID AS OrganizationId
		FROM FINANCE.ORGANIZATION_ACCOUNT A
		WHERE A.FISCAL_YEAR = 9999 AND A.FISCAL_PERIOD = ''''--'''' 
		AND A.ORG_ID IS NOT NULL
	'')
	
	-- This is where the issue is: --------------------------
	-- Before we can switch today''s load table partition with today''s main table partition, we have to handle
	-- the load table''s FK reference/constraint against the Organizations table.
	-- Normally this would be against the Organizations load table; however, this is not possible since the Organizations load
	-- table does not exist; therefore, we create the FK against the main organizations table instead, and this technique seems
	-- to work and allow us to swap partitions:
	
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @LoadTableName + '_' + @ReferentialTableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']''))
		ALTER TABLE [dbo].[' + @LoadTableName + '] DROP CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + ']
		
	ALTER TABLE [dbo].[' + @LoadTableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
		REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id], [PartitionColumn])

	ALTER TABLE [dbo].[' + @LoadTableName + '] CHECK CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] 
	
	-- Do the same thing for the main accounts and organizations tables:
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @TableName + '_' + @ReferentialTableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']''))
		ALTER TABLE [dbo].[' + @TableName + '] DROP CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + ']
		
	ALTER TABLE [dbo].[' + @TableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
		REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id], [PartitionColumn])

	ALTER TABLE [dbo].[' + @TableName + '] CHECK CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + '] 
	'
	
	-------------------------------------------------------------------------
	if @IsDebug = 1
		BEGIN
			--used for testing
			PRINT @TSQL	
		END
	else
		BEGIN
			--Execute the command:
			EXEC(@TSQL)
		END 
END
