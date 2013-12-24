

/****** Object:  StoredProcedure [dbo].[expAimsMktCap]    Script Date: 12/24/2013 09:28:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expAimsMktCap]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expAimsMktCap]
GO


/****** Object:  StoredProcedure [dbo].[expAimsMktCap]    Script Date: 12/24/2013 09:28:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE procedure [dbo].[expAimsMktCap]
as

select convert(varchar(10),pf.Root_source_Date,101) as Root_Source_Date, g.ASEC_SEC_SHORT_NAME,pf.amount from period_financials pf inner join gf_Security_baseview g on g.security_id = pf.security_id
where pf.data_id = 185  and pf.period_type = 'C' and pf.data_source = 'PRIMARY'
and pf.Currency = 'USD'
 





GO


