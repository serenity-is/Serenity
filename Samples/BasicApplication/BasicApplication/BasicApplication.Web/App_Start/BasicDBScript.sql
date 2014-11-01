CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[Source] [nvarchar](4) NOT NULL,
	[PasswordHash] [nvarchar](86) NOT NULL,
	[PasswordSalt] [nvarchar](10) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[InsertUserId] [int] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUserId] [int] NULL,
	[IsActive] [smallint] NOT NULL CONSTRAINT [DF_UserIsActive]  DEFAULT ((1)),
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[Users] (Username, DisplayName, Email, Source, PasswordHash, PasswordSalt, InsertDate, InsertUserId, IsActive) VALUES
('admin', 'admin', 'admin@dummy.com', 'site', 'rfqpSPYs0ekFlPyvIRTXsdhE/qrTHFF+kKsAUla7pFkXL4BgLGlTe89GDX5DBysenMDj8AqbIZPybqvusyCjwQ',	'hJf_F', '2014-01-01', 1, 1)

GO

CREATE TABLE [dbo].[Languages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LanguageId] [nvarchar](10) NOT NULL,
	[LanguageName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Languages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[Languages] ([LanguageId] ,[LanguageName]) VALUES ('en', 'English');
INSERT INTO [dbo].[Languages] ([LanguageId] ,[LanguageName]) VALUES ('ru', 'Russian');
INSERT INTO [dbo].[Languages] ([LanguageId] ,[LanguageName]) VALUES ('es', 'Spanish');
INSERT INTO [dbo].[Languages] ([LanguageId] ,[LanguageName]) VALUES ('tr', 'Turkish');