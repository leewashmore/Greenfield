
set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00023'
declare @CurrentScriptVersion as nvarchar(100) = '00024'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end




GO

IF OBJECT_ID ('[dbo].[Broker_Detail]') IS NOT NULL
	DROP PROCEDURE [dbo].[Broker_Detail]
GO

/****** Object:  StoredProcedure [dbo].[Broker_Detail]    Script Date: 08/23/2012 19:59:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--------------------------------------------------------------------------------------
-- Name:	Broker_Detail
--
-- Purpose:	Get the individual Broker detail for the Consensus gadgets.
--
-- Author:	David Muench
-- Date:	July 24, 2012
--------------------------------------------------------------------------------------
CREATE procedure [dbo].[Broker_Detail](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@ESTIMATE_TYPE		varchar(10)	= NULL				-- i.e. IBIT, EPS, NTP...
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
)
as

	select b.broker_name, de.EstimateType, de.fPeriodEnd, de.Amount, de.StartDate
		,  de.OriginalDate as Last_Update_Date, de.Currency as Reported_Currency
	  from Reuters.dbo.tblDetailedEstimate de
	 inner join Reuters.dbo.tblBrokers b on b.BROKER_ID = de.BROKERID
	 inner join (select distinct XREF from dbo.GF_SECURITY_BASEVIEW 
				  where (@ISSUER_ID is not null and @ISSUER_ID = ISSUER_ID)
				 ) sb on sb.XREF = de.XREF
	 -- Only show the most recent values.
	 inner join (select de.XREF, b.broker_name, de.EstimateType, de.fPeriodEnd, MAX(de.StartDate) as StartDate
				   from Reuters.dbo.tblDetailedEstimate de
				  inner join Reuters.dbo.tblBrokers b on b.BROKER_ID = de.BROKERID
				  inner join (select distinct XREF from dbo.GF_SECURITY_BASEVIEW 
							 ) sb on sb.XREF = de.XREF
				  where 1=1
				  group by de.XREF, b.broker_name, de.EstimateType, de.fPeriodEnd
				) z on z.broker_name = b.broker_name and z.EstimateType = de.EstimateType
					and z.fPeriodEnd = de.fPeriodEnd and z.StartDate = de.StartDate
					and z.XREF = de.XRef
	 where (@ESTIMATE_TYPE is null or de.EstimateType = @ESTIMATE_TYPE)
--	   and de.EstimateType in ('REVENUE', 'EBITDA', '', '', '', '')
	   and de.PeriodType = @PERIOD_TYPE
	   and (de.Currency is NULL or de.Currency = @CURRENCY)
	   and de.ExpirationDate is NULL
	 order by b.broker_name, EstimateType, de.fPeriodEnd, de.StartDate
	;

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00024'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


