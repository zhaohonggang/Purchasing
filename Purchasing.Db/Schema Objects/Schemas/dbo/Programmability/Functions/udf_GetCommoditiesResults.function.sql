﻿-- =============================================
-- Author:		Ken Taylor
-- Create date: February 29, 2012
-- Description:	Given a ContainsSearchCondition search string, 
-- return the records matching the search string provided.
--
-- Usage:
-- USE [PrePurchasing]
-- GO
-- 
-- DECLARE @ContainsSearchCondition varchar(255) = 'thread tap' 
-- 
-- SELECT * from udf_GetCommoditiesResults(@ContainsSearchCondition)
-- 
-- results:
-- Id	Name
-- 13114	THREAD SEALING TAPE, INCL TEFLON TAPE
-- 14208	CARBON STEEL, THREADED ROD
-- 18421	THREAD
-- 20224	SCREW, SELF-TAPPING THREAD
-- 20600	SCREW, SPECIFIED THREAD, NOT SELF-TAPPING
-- 20860	NUT/INSERT, SPECIFIED THREAD
-- 21712	SCREW, UNSPECIFIED THREAD
-- 21726	NUT, UNSPECIFIED THREAD
-- 21727	NUT, UNSPECIFIED THREAD, NOT KINDORF/SUPERSTRUT/UNISTRUT
-- 21730	INSERT OR SCREW/WASHER ASSEMBLY, UNSPECIFIED THREAD
-- 21XXX	FASTENERS, THREADED, NUTS/CREW/WASHER/INSERT,  MISCELLANEOUS
-- 27005	PLUG VALVE/COCK, NOT RATED, TWO WAY, THREADED, BRASS/BRONZE
-- 28102	PIPE FITTING, ED, IRON
-- 28110	PIPE FITTING, THREADED, IRON, UNCOATED, NOT RATED, NOT UNION
-- 28122	PIPE FITTING, THREADED, IRON, COATED INCL GALVANIZED, UNION
-- 28130	PIPE FITTING, THREADED, IRON, COATED, NOT RATED, NOT UNION
-- 28145	PIPE FITTING, THREADED, IRON, DRAINAGE, ELBOW/COUPLING
-- 28146	PIPE FITTING, THREAD, IRON, DRAINAGE, NOT ELBOW/COUPLING
-- 28160	PIPE FITTING, THREADED, CARBON STEEL, UNCOATED, UNION
-- 28180	PIPE FITTING,THREADED, CARBON STEEL, COATED INCL GALV, UNION
-- 28192	PIPE FITTING, THREADED , CARBON STEEL
-- 28200	PIPE FITTING, THREADED, ALLOY OR STAINLESS STEEL
-- 29125	PIPE FITTING, BRASS/BRONZE/COPPER, >2 THREADED ENDS,INCL TEE
-- 29404	PIPE FIT,NONFERR,WROUGHT,FEMALE THREAD, ADAPTER/UNION
-- 29405	PIPE FIT,NONFERR,WROUGHT,FEMALE THREAD, ELBOW
-- 29406	PIPE FIT,NONFERR,WROUGHT,FEMALE THREAD,TEE,NOT 29404-29405
-- 29407	PIPE FIT,NONFERR,WROUGHT,MALE THREAD, ADAPTER/UNION
-- 29408	PIPE FIT,NONFERR,WROUGHT,MALE THREAD, ELBOW
-- 29409	PIPE FIT,NONFERR,WROUGHT,MALE THREAD, TEE, NOT 29407-29408
-- 29414	PIPE FIT,NONFERR,CAST,FEMALE THREAD, ADAPTER/UNION
-- 29415	PIPE FIT,NONFERR,CAST,FEMALE THREAD, ELBOW
-- 29416	PIPE FIT,NONFERR,CAST,FEMALE THREAD, TEE, NOT 29404-29405
-- 29417	PIPE FIT,NONFERR,CAST,MALE THREAD, ADAPTER/UNION
-- 29418	PIPE FIT,NONFERR,CAST,MALE THREAD, ELBOW
-- 29419	PIPE FIT,NONFERR,CAST,MALE THREAD, TEE, NOT 29417-29418
-- 29740	PIPE FITTING, THREADED JOINTS ONLY, POLYVINYL CHLORIDE (PVC)
-- 29750	PIPE FIT, THREADED JOINTS ONLY, NOT POLYVINYL CHLORIDE (PVC)
-- 34557	ELECTRIC FITTING, SERVICE ENTRANCE, THREADED
-- 34587	ELECTRIC FITTING, SERVICE ENTRANCE, NOT THREADED
-- 34611	ELECT FIT, STRAIGHT, RIGID PIPE, THREADED, NIPPLE/ELBOW/CONN
-- 34613	ELECT FIT, STRAIGHT, RIGID PIPE, NOT THREADED, NIPPLE/ELBOW/
-- 34621	ELECT FIT, ANGLED, RIGID PIPE, THREADED, NIPPLE/ELBOW/CONN
-- 34623	ELECT FIT, ANGLED, RIGID PIPE, NOT THREADED, NIPPLE/ELBOW/CO
-- 34630	ELECT FIT, RIGID PIPE, THREADED, NOT 34611 OR 34621
-- 34636	ELECT FIT, RIGID PIPE, NOT THREADED, NOT 34613 OR 34623
-- 38410	THREAD FORMER, MACHINERY
-- 40022-55	TAPS 100G
-- 40022-56	TAPS 25G
-- 40022-57	TAPS 500G
-- 40122-70	TAPS 99% 25GR 29915-38-6
-- 55410	PUNCH/TAP/AWL/BROACH/GOUGE/REAMER, MEDICAL TOOL
-- 55810	SUTURE/THREAD, NON-STERILE, ON SPOOL, MEDICAL
--
-- Modifications:
--	2012-03-02 by kjt: Revised to include filter by IsActive, TOP 20, and rank as per Scott Kirkland.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetCommoditiesResults]
(
	@ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
	 Id varchar(9) not null
	,Name varchar(60) not null
)
AS
BEGIN
	INSERT INTO @returntable
	SELECT TOP 20 
		   [Id]
		  ,[Name]
	FROM [PrePurchasing].[dbo].[vCommodities] FT_TBL INNER JOIN
	FREETEXTTABLE([vCommodities], ([Id], [Name]), @ContainsSearchCondition) KEY_TBL on FT_TBL.Id = KEY_TBL.[KEY]
	WHERE [IsActive] = 1
	ORDER BY KEY_TBL.[RANK] DESC

RETURN
END