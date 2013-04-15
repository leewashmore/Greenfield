USE [AIMS_Main_Dev]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


------------------------------------------------------------------------
-- Purpose:	This procedure gets data for the investment context
--
--			
--
-- Author:	Syedeen Nazirali
-- Date:	April 2, 2013
------------------------------------------------------------------------
alter procedure [dbo].[getInvestmentContext](
	@issuer_id			varchar(20) = NULL,			-- The company identifier		
	@ViewReportBy    varchar(20) = 'Country'	-- ViewReportBy can take value Country or Industry

)
as
-- If @ViewReportBy = Country then get iso_country_code 
begin
Declare @iso_country_code   nvarchar(255)='';
Declare @gics_industry		nvarchar(255)='';
Declare @marketcap			decimal(32,6)=0;
declare @curryear int = 0;
declare @nextyear int = 0;

Declare  @IC_Temp table
(
issuer_id varchar(20) collate SQL_Latin1_General_CP1_CI_AS,
issuer_name nvarchar(255),
SecurityId varchar(20),
iso_country_code nvarchar(255),
gics_sector nvarchar(255),
gics_sector_name nvarchar(255),
gics_industry nvarchar(255),
gics_industry_name nvarchar(255),
period_year int,
DataId int,
value decimal(32,6)
);

set @curryear = year(getdate());
set @nextyear = @curryear+1

if @ViewReportBy = 'Country'
	select @iso_country_code = max(iso_country_code) from dbo.gf_security_baseview where issuer_id = @issuer_id;
else if @ViewReportBy = 'Industry'
	select @gics_industry = max(gics_industry) from dbo.gf_security_baseview where issuer_id = @issuer_id;
	
	
--print @iso_country_code
--print @gics_industry
-- Get the market cap for the issuer id passed.

select @marketcap = amount from period_financials where security_id = (select max(issuer_proxy) from dbo.gf_security_baseview where issuer_id = @issuer_id)
and data_id = 185 and period_type = 'C' and currency ='USD' and data_source='PRIMARY' 
print @marketcap


--print @marketcap
if @ViewReportBy = 'Country'
begin
-- select all the issuers  for the selected country.
-- insert market cap
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount  from dbo.gf_security_baseview gsb
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where  pf.data_id = 185 and pf.period_type = 'C' and pf.currency ='USD' and pf.data_source='PRIMARY' 
	and gsb.iso_country_code = @iso_country_code 
	group by pf.amount
		
	-- insert forward pe
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 187 and pf.period_type = 'C' and pf.currency ='USD' and pf.data_source='PRIMARY' 
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount
	
	-- insert forward pb
	
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 188 and pf.period_type = 'C' and pf.currency ='USD' and pf.data_source='PRIMARY' 
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount	
		
	-- insert pe current year	
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 166 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR' 
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount	
	
	-- insert pe next year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 166 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @nextyear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount		

	--insert pb current year
	
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 164 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount		
	
	--insert pb NEXT year
	
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 164 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @nextyear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount		
	
	--- insert ev/ebitda current year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 193 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount		
	
	--- insert ev/ebitda next year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 193 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @nextyear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount	
	
	--insert  DY
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 192 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount	

	--insert  ROE
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 133 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.iso_country_code = @iso_country_code 
	group by  pf.amount	
		
		
		
end
else if @ViewReportBy = 'Industry'
begin 
-- -- select all the issuers  for the selected industry.
	-- insert market cap
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where  pf.data_id = 185 and pf.period_type = 'C' and pf.currency ='USD' and pf.data_source='PRIMARY' 
	and gsb.gics_industry = @gics_industry 
		group by pf.amount

	-- insert forward pe

	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 187 and pf.period_type = 'C' and pf.currency ='USD' and pf.data_source='PRIMARY' 
	and gsb.gics_industry = @gics_industry 
	group by  pf.amount

	-- insert forward pb
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 188 and pf.period_type = 'C' and pf.currency ='USD' and pf.data_source='PRIMARY' 
	and gsb.gics_industry = @gics_industry 
	group by  pf.amount	

	-- insert pe current year	
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 166 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR' 
	and gsb.gics_industry = @gics_industry 
	group by  pf.amount		
	
		-- insert pe next year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 166 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @nextyear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry 
	group by  pf.amount		
	
	--insert pb CURRENT year
	
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 164 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry 
	group by  pf.amount	
	
	--insert pb NEXT year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 164 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @nextyear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry  
	group by  pf.amount		
	
	--- insert ev/ebitda current year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 193 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry   
	group by  pf.amount	
	
	--insert ev/ebitda next year
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 193 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @nextyear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry   
	group by  pf.amount	
	
	--insert  DY
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 192 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry    
	group by  pf.amount	
	
	--insert  ROE
	insert into @IC_Temp (issuer_id,issuer_name,iso_country_code,gics_sector,gics_sector_name,gics_industry,gics_industry_name,securityid,DataId,period_year,value)
	select max(gsb.issuer_id),max(gsb.issuer_name),max(gsb.iso_country_code),max(gsb.gics_sector),max(gsb.gics_sector_name),max(gsb.gics_industry),max(gsb.gics_industry_name), max(gsb.issuer_proxy),max(pf.Data_id),max(pf.period_year),pf.amount from dbo.gf_security_baseview gsb	
	inner join dbo.period_financials pf on pf.security_id = gsb.issuer_proxy
	where pf.data_id = 133 and pf.period_type = 'A' and pf.currency ='USD' and pf.data_source='PRIMARY'  
	and period_year = @curryear and fiscal_type = 'CALENDAR'
	and gsb.gics_industry = @gics_industry   
	group by  pf.amount	
	
end





select * from @IC_Temp order by issuer_name 

	end
GO

-- exec [dbo].[getInvestmentContext] '117929','Country'
-- exec [dbo].[getInvestmentContext] '10047328','Industry'
