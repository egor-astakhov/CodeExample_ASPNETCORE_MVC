IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'SystemLog'))
BEGIN
	DROP TABLE [dbo].[SystemLog]	
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SystemLog] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime2] NOT NULL,
	[LogLevel] [nvarchar] (20) NOT NULL,
	[Logger] [nvarchar] (300) NULL,
	[Message] [nvarchar] (max) NOT NULL,
	[Username] [nvarchar] (256) NULL,
	[RequestMethod] [nvarchar] (10) NULL,
	[Url] [nvarchar] (2048) NULL,
	[QueryString] [nvarchar] (2048) NULL,
	[Body] [nvarchar] (max) NULL,
	[Exception] [nvarchar] (max) NULL,
CONSTRAINT [PK_SystemLog] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
