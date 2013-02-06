declare @CurrentScriptVersion as nvarchar(100) = '00000'


if object_id('ChangeScripts', 'U') is null 
begin

	CREATE TABLE [dbo].[ChangeScripts](
		[ScriptVersion] [nvarchar](100) NOT NULL,
		[DateExecuted] [datetime] NOT NULL,
	CONSTRAINT [PK_ChangeScripts] PRIMARY KEY CLUSTERED 
	(
		[ScriptVersion] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
end



