
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter table dbo.data_master add MinValue decimal(18, 2);
GO
alter table dbo.data_master add MaxValue decimal(18, 2);	
GO
alter table dbo.gf_security_baseview add issuer_proxy varchar(20);
GO
alter table dbo.gf_security_baseview add UPDATE_BB_STATUS varchar(20);
GO
update dbo.data_master set MinValue =0,MaxValue =0.9 where data_id =133;
update dbo.data_master set MinValue =0,MaxValue =10 where data_id =164;
update dbo.data_master set MinValue =0,MaxValue =60 where data_id =166;
update dbo.data_master set MinValue =0,MaxValue =10 where data_id =188;
update dbo.data_master set MinValue =0,MaxValue =60 where data_id =187;
update dbo.data_master set MinValue =0,MaxValue =60 where data_id =193;
update dbo.data_master set MinValue =0,MaxValue =0.3 where data_id =192;
update dbo.data_master set MinValue =0 where data_id =185;
GO

