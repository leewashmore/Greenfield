

alter table dbo.data_master add (MinValue decimal(18, 2))
alter table dbo.data_master add (MaxValue decimal(18, 2))	

alter table dbo.gf_security_baseview add(issuer_proxy varchar(20))
alter table dbo.gf_security_baseview add(UPDATE_BB_STATUS varchar(20))

update dbo.data_master set minvalue =0,maxvalue =0.9 where data_id =133
update dbo.data_master set minvalue =0,maxvalue =10 where data_id =164
update dbo.data_master set minvalue =0,maxvalue =60 where data_id =166
update dbo.data_master set minvalue =0,maxvalue =10 where data_id =188
update dbo.data_master set minvalue =0,maxvalue =60 where data_id =187
update dbo.data_master set minvalue =0,maxvalue =60 where data_id =193
update dbo.data_master set minvalue =0,maxvalue =0.3 where data_id =192
update dbo.data_master set minvalue =0 where data_id =185
