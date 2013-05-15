IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSecurityLevelPFDataForMarketing]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].GetSecurityLevelPFDataForMarketing
GO
GO
/****** Object:  StoredProcedure [dbo].[GetSecurityLevelPFDataForMarketing]    Script Date: 05/06/2013 10:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

------------------------------------------------------------------------
-- Purpose:	Retrieve list of Portfolios
--
-- Author:	Akhtar Nazirali
-- Date:	05-06-2013
------------------------------------------------------------------------
CREATE procedure [dbo].GetSecurityLevelPFDataForMarketing
@portfolio_id varchar(20)
,@effective_date datetime
,@DataId int
,@DataSource varchar(10)='PRIMARY'
,@Currency varchar(3)='USD'
,@period_type char(2)='C'
,@year int=0
,@fiscal_type varchar(8)= ''

As
begin

Declare  @Marketing_Temp table
(
issuer_id varchar(20) collate SQL_Latin1_General_CP1_CI_AS,
asec_Sec_Short_name varchar(20),
security_id varchar(20),
period_year int,
Data_Source varchar(10),
Currency varchar(3),
period_type char(2),
DataId int,
value decimal(32,6)
)

-- Get Market Cap from period financials.
insert into @marketing_temp(issuer_id,asec_Sec_Short_name,security_id,period_year,data_source,currency, period_type, DataId,Value)
select p.issuer_id,g.asec_sec_short_name,pfh.security_id,pfh.period_year,pfh.data_source,pfh.currency,pfh.period_type,pfh.Data_Id,pfh.amount
from dbo.period_financials_history pfh 
inner join dbo.GF_PORTFOLIO_LTHOLDINGS_HISTORY p on pfh.effective_Date = p.portfolio_Date 
inner join dbo.gf_Security_baseview_history g on p.portfolio_date = g.effective_date 
and p.asec_Sec_short_name = g.asec_Sec_short_name 
and pfh.security_id = g.security_id 
where  p.portfolio_DATE = @effective_date
		and pfh.DATA_ID = @DataId
		and pfh.DATA_SOURCE = @DataSource
		and pfh.CURRENCY = @Currency
		and pfh.PERIOD_TYPE = @period_type
		and pfh.PERIOD_YEAR = @year
		and pfh.FISCAL_TYPE = @fiscal_type
		and p.portfolio_id=@portfolio_id
		and p.SECURITYTHEMECODE = 'EQUITY'

select * from @Marketing_Temp

end

--exec GetMarketCapDataForMarketing 'KODAK', '04/30/2013' 
