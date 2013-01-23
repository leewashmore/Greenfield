CREATE TABLE [dbo].[TAXONOMY](
	[ID] [int] NOT NULL,
	[DEFINITION] [xml] NOT NULL,
 CONSTRAINT [PK_TAXONOMY] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
go

insert into TAXONOMY([ID], [DEFINITION]) values (0, '<taxonomy xmlns="urn:TopDown.Core">

	<region name="East Asia">
		<basket-country basketId="1" />
		<!--basket-country basketId="2" /-->
		<basket-country basketId="3" />
		<basket-country basketId="4" />
	</region>

	<region name="Latin America">
		<basket-country basketId="5" />
		<basket-country basketId="6" />
		<basket-country basketId="7" />
		<basket-country basketId="8" />
		<basket-country basketId="9" />
		<basket-country basketId="10" />
	</region>

	<region name="Europe">
		<basket-country basketId="11" />
		<basket-country basketId="12" />
		<basket-country basketId="13" />
		<basket-region basketId="19" />
	</region>

	<basket-region basketId="20" />
	
	<region name="Africa">
		<basket-country basketId="14" />
		<basket-region basketId="21" />
	</region>

	<basket-region basketId="22" />

</taxonomy>
')


insert into TAXONOMY([ID], [DEFINITION]) values (1, '<taxonomy xmlns="urn:TopDown.Core">
	<region name="Southeast Asia">
		<basket-country basketId="15" />
		<basket-country basketId="16" />
		<basket-country basketId="17" />
		<basket-country basketId="18" />
	</region>
</taxonomy>')
insert into TAXONOMY([ID], [DEFINITION]) values (2, '<taxonomy xmlns="urn:TopDown.Core">

	<region name="East Asia">
		<basket-region basketId="23" />
		<!--basket-country basketId="1" /-->
		<!--basket-country basketId="2" /-->
		<basket-country basketId="3" />
		<basket-country basketId="4" />
	</region>

	<region name="Latin America">
		<basket-country basketId="5" />
		<basket-country basketId="6" />
		<basket-country basketId="7" />
		<basket-country basketId="8" />
		<basket-country basketId="9" />
		<basket-country basketId="10" />
	</region>

	<region name="Europe">
		<basket-country basketId="11" />
		<basket-country basketId="12" />
		<basket-country basketId="13" />
		<basket-region basketId="19" />
	</region>

	<basket-region basketId="20" />

	<region name="Africa">
		<basket-country basketId="14" />
		<basket-region basketId="21" />
	</region>

	<region name="Southeast Asia">
		<basket-country basketId="15" />
		<basket-country basketId="16" />
		<basket-country basketId="17" />
		<basket-country basketId="18" />
		<basket-country basketId="25" />
	</region>
	
	<basket-region basketId="22" />

</taxonomy>
')
go