use AIMS_Main
go

/****** Object:  StoredProcedure [dbo].[updateICTable]    Script Date: 08/06/2014 15:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


alter procedure [dbo].[updateICTable](
	@issuerID varchar(20) = ''
	,@tmpICAction varchar(50) = ''
	,@icMeasure int = ''
	,@datePresented datetime = ''
	,@Buy decimal(32,6) = ''
	,@Sell decimal(32,6) = ''
	,@updateDate datetime = ''
	)
	as

declare @presenter varchar(50)
declare @icAction varchar(50) 

select @presenter = sb.ASHMOREEMM_PRIMARY_ANALYST from GF_SECURITY_BASEVIEW sb 
where sb.ISSUER_ID = @issuerID
and sb.issuer_proxy = sb.SECURITY_ID

set @icAction = CASE WHEN @tmpICAction = '' 
	THEN (
		select ic.IC_ACTION 
		from IC_TABLE ic 
		where ic.ISSUER_ID = @issuerID 
			and ic.DATE_PRESENTED = (
				select MIN(ic.DATE_PRESENTED) from IC_TABLE ic where ic.ISSUER_ID = @issuerID
				)
			)
		ELSE @tmpICAction
	END

insert into IC_TABLE (
	ISSUER_ID
	,DATE_PRESENTED
	,PRESENTER
	,IC_ACTION
	,IC_MEASURE
	,IC_BUY
	,IC_SELL
	,update_date
	)
values (
	@issuerID
	,@datePresented
	,@presenter
	,@icAction
	,@icMeasure
	,@Buy
	,@Sell
	,@updateDate
	)

--select * from IC_TABLE ic where ic.ISSUER_ID = @issuerID
