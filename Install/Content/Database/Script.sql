USE [DeskoTesting]
GO
/****** Object:  Table [dbo].[ActiveDlls]    Script Date: 30-8-2016 14:09:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActiveDlls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WidgetId] [int] NULL,
	[DllId] [int] NOT NULL,
	[PluginId] [int] NULL,
	[ThemeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdminBreadcrumbs]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdminBreadcrumbs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Controller] [varchar](50) NOT NULL,
	[Action] [varchar](50) NOT NULL,
	[Icon] [varchar](25) NOT NULL,
	[WidgetId] [int] NULL,
	[LocalizationBase] [varchar](255) NOT NULL,
	[LocalizedControllerFriendlyName] [varchar](255) NOT NULL,
	[LocalizedActionFriendlyName] [varchar](255) NOT NULL,
	[LocalizedTitle] [varchar](255) NOT NULL,
	[LocalizedDescription] [varchar](255) NOT NULL,
	[PluginId] [int] NULL,
	[ThemeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdminNavigation]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdminNavigation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Controller] [varchar](255) NOT NULL,
	[Action] [varchar](255) NOT NULL,
	[LocalizedName] [varchar](255) NOT NULL,
	[LocalizedBase] [varchar](255) NOT NULL,
	[Icon] [varchar](25) NOT NULL,
	[PluginId] [int] NULL,
	[WidgetId] [int] NULL,
	[ThemeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdminPages]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdminPages](
	[Id] [int] NOT NULL,
	[Controller] [varchar](50) NOT NULL,
	[Action] [varchar](50) NOT NULL,
	[Title] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Keywords] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Dlls]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Dlls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DynamicPages]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DynamicPages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](125) NOT NULL,
	[Description] [varchar](250) NOT NULL,
	[Keywords] [varchar](250) NULL,
	[Message] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [ntext] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmailAccounts]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailAccounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SMTPEmail] [varchar](50) NOT NULL,
	[SMTPPassword] [varchar](50) NOT NULL,
	[SMTPServer] [varchar](50) NOT NULL,
	[SMTPSecure] [bit] NOT NULL,
	[SMTPPort] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmailTemplates]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailTemplates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](1000) NOT NULL,
	[Subject] [varchar](75) NOT NULL,
	[Name] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Languages]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Languages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Culture] [varchar](10) NOT NULL,
	[Abbreviation] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Links]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Links](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](25) NOT NULL,
	[Link] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LoginFails]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LoginFails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ip] [varchar](50) NOT NULL,
	[Attempts] [tinyint] NOT NULL,
	[ExpireDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LoginTokens]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginTokens](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [nvarchar](72) NOT NULL,
	[Expires] [datetime] NOT NULL,
	[Selector] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NavigationPages]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NavigationPages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](25) NOT NULL,
	[Url] [varchar](125) NOT NULL,
	[OrderNumber] [int] NOT NULL,
	[ParentId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[News]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[News](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [varchar](25) NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[Information] [varchar](max) NOT NULL,
	[AuthorID] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[Date] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OpenUpdates]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OpenUpdates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DownloadPath] [varchar](255) NULL,
	[Version] [varchar](25) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDownloaded] [bit] NOT NULL,
	[DownloadUrl] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PluginOpenUpdates]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PluginOpenUpdates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PluginId] [int] NOT NULL,
	[DownloadPath] [varchar](255) NULL,
	[Version] [varchar](25) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDownloaded] [bit] NOT NULL,
	[DownloadUrl] [varchar](255) NOT NULL,
	[DeskoVersion] [varchar](25) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Plugins]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Plugins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](75) NOT NULL,
	[Version] [varchar](10) NOT NULL,
	[Author] [varchar](201) NOT NULL,
	[AuthorUrl] [varchar](75) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[UpdateUrl] [varchar](250) NULL,
	[Enabled] [bit] NOT NULL,
	[Area] [varchar](255) NOT NULL,
	[Namespace] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PluginUpdateSettings]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PluginUpdateSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LastCheckedDate] [datetime] NOT NULL,
	[IsChecking] [bit] NOT NULL,
	[IsInstalling] [bit] NOT NULL,
	[IsDownloading] [bit] NOT NULL,
	[IsUpdatingSuccess] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Poll]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Poll](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Question] [varchar](75) NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PollAnswers]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PollAnswers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Answer] [varchar](100) NOT NULL,
	[PollId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PollVotes]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PollVotes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[PollAnswerId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Reports]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Reports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [varchar](500) NOT NULL,
	[SenderIP] [varchar](45) NOT NULL,
	[WebsiteId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SeoPages]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SeoPages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PageController] [varchar](25) NOT NULL,
	[PageIndex] [varchar](25) NOT NULL,
	[ResourceTitleName] [varchar](255) NOT NULL,
	[ResourceDescriptionName] [varchar](255) NULL,
	[ResourceBaseName] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Settings](
	[ID] [int] NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[Description] [varchar](250) NOT NULL,
	[Keywords] [varchar](500) NOT NULL,
	[ShowAmountWebsites] [int] NOT NULL,
	[Maintenance] [bit] NOT NULL,
	[Version] [varchar](10) NOT NULL,
	[SiteTitle] [varchar](50) NOT NULL,
	[SiteSlogan] [varchar](50) NOT NULL,
	[IsEmailVerificationRequired] [bit] NOT NULL,
	[Url] [varchar](50) NOT NULL,
	[CronjobUserStatisticsEmail] [varchar](25) NOT NULL,
	[Footer] [varchar](1000) NULL,
	[IsPingEnabled] [bit] NOT NULL,
	[ThemeId] [int] NOT NULL,
	[CronjobInAndOut] [varchar](25) NOT NULL,
	[RecaptchaSecretKey] [varchar](50) NOT NULL,
	[RecaptchaSiteKey] [varchar](50) NOT NULL,
	[CronWebsiteThumbnail] [varchar](25) NOT NULL,
	[IsCreateThumbnailsEnabled] [bit] NOT NULL,
	[CronPingServer] [varchar](25) NOT NULL,
	[GoogleSiteId] [varchar](25) NULL,
	[CronUpdate] [varchar](25) NOT NULL,
	[UpdateWhenIncorrectVersion] [bit] NOT NULL,
	[AutoUpdate] [bit] NOT NULL,
	[IsEmailingUserStatisticsEnabled] [bit] NOT NULL,
	[IsResetInAndOutsEnabled] [bit] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Timezone] [varchar](50) NOT NULL,
	[IsAdminVerificationRequired] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SoftwareUpdateSettings]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SoftwareUpdateSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LastCheckedDate] [datetime] NOT NULL,
	[IsChecking] [bit] NOT NULL,
	[IsInstalling] [bit] NOT NULL,
	[IsDownloading] [bit] NOT NULL,
	[IsUpdatingSuccess] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ThemeOpenUpdates]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ThemeOpenUpdates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThemeId] [int] NOT NULL,
	[DownloadPath] [varchar](255) NULL,
	[Version] [varchar](25) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDownloaded] [bit] NOT NULL,
	[DownloadUrl] [varchar](255) NOT NULL,
	[DeskoVersion] [varchar](25) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Themes]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Themes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThemeName] [varchar](50) NOT NULL,
	[AuthorName] [varchar](50) NOT NULL,
	[AuthorUrl] [varchar](50) NOT NULL,
	[FolderName] [varchar](201) NOT NULL,
	[Version] [varchar](50) NOT NULL,
	[Image] [varchar](255) NULL,
	[Description] [varchar](500) NOT NULL,
	[UpdateUrl] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ThemeUpdateSettings]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThemeUpdateSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LastCheckedDate] [datetime] NOT NULL,
	[IsChecking] [bit] NOT NULL,
	[IsInstalling] [bit] NOT NULL,
	[IsDownloading] [bit] NOT NULL,
	[IsUpdatingSuccess] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](15) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Question] [int] NOT NULL,
	[Answer] [varchar](25) NOT NULL,
	[BannedStartDate] [datetime] NULL,
	[BannedEndDate] [datetime] NULL,
	[EmailVerificationCode] [varchar](30) NULL,
	[RegistrationDate] [datetime] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[IsAdminVerified] [bit] NOT NULL,
	[Password] [nvarchar](72) NOT NULL,
	[NewEmail] [varchar](50) NULL,
	[NewEmailVerificationCode] [varchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WebsiteBlackList]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WebsiteBlackList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Domain] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WebsiteIn]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WebsiteIn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WebsiteID] [int] NOT NULL,
	[IP] [varchar](45) NOT NULL,
	[Unique] [bit] NOT NULL,
	[Date] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WebsiteInDaily]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebsiteInDaily](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WebsiteId] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[UniqueIn] [int] NOT NULL,
	[TotalIn] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebsiteOut]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WebsiteOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WebsiteID] [int] NOT NULL,
	[IP] [varchar](50) NOT NULL,
	[Unique] [bit] NOT NULL,
	[Date] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WebsiteOutDaily]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebsiteOutDaily](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WebsiteId] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[UniqueOut] [int] NOT NULL,
	[TotalOut] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebsiteRating]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WebsiteRating](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WebsiteID] [int] NOT NULL,
	[Ip] [varchar](45) NOT NULL,
	[Rating] [int] NOT NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Websites]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Websites](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](75) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[Url] [varchar](150) NOT NULL,
	[BannerURL] [varchar](75) NULL,
	[UserID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Keywords] [varchar](150) NULL,
	[Enabled] [bit] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[Sponsored] [bit] NOT NULL,
	[ServerIP] [varchar](25) NULL,
	[ServerPort] [int] NULL,
	[MonitorCheckedDate] [datetime] NULL,
	[IsOnline] [bit] NOT NULL,
	[BannerFileName] [varchar](50) NULL,
	[Thumbnail] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WidgetOpenUpdates]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WidgetOpenUpdates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WidgetId] [int] NOT NULL,
	[DownloadPath] [varchar](255) NULL,
	[Version] [varchar](25) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDownloaded] [bit] NOT NULL,
	[DownloadUrl] [varchar](255) NOT NULL,
	[DeskoVersion] [varchar](25) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Widgets]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Widgets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](75) NOT NULL,
	[AreaName] [varchar](50) NOT NULL,
	[Version] [varchar](10) NOT NULL,
	[Author] [varchar](201) NOT NULL,
	[AuthorUrl] [varchar](75) NOT NULL,
	[Controller] [varchar](25) NOT NULL,
	[StartIndex] [varchar](25) NOT NULL,
	[Namespace] [varchar](50) NOT NULL,
	[Image] [varchar](255) NULL,
	[Description] [varchar](500) NOT NULL,
	[UpdateUrl] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WidgetsTheme]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WidgetsTheme](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WidgetId] [int] NOT NULL,
	[ThemeSectionId] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WidgetsThemeSection]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WidgetsThemeSection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThemeId] [int] NOT NULL,
	[FriendlyName] [varchar](50) NOT NULL,
	[CodeName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WidgetUpdateSettings]    Script Date: 30-8-2016 14:09:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WidgetUpdateSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LastCheckedDate] [datetime] NOT NULL,
	[IsChecking] [bit] NOT NULL,
	[IsInstalling] [bit] NOT NULL,
	[IsDownloading] [bit] NOT NULL,
	[IsUpdatingSuccess] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ActiveDlls]  WITH CHECK ADD  CONSTRAINT [FK_ActiveDlls_Dlls] FOREIGN KEY([DllId])
REFERENCES [dbo].[Dlls] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActiveDlls] CHECK CONSTRAINT [FK_ActiveDlls_Dlls]
GO
ALTER TABLE [dbo].[ActiveDlls]  WITH CHECK ADD  CONSTRAINT [FK_ActiveDlls_Plugins] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActiveDlls] CHECK CONSTRAINT [FK_ActiveDlls_Plugins]
GO
ALTER TABLE [dbo].[ActiveDlls]  WITH CHECK ADD  CONSTRAINT [FK_ActiveDlls_Themes] FOREIGN KEY([ThemeId])
REFERENCES [dbo].[Themes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActiveDlls] CHECK CONSTRAINT [FK_ActiveDlls_Themes]
GO
ALTER TABLE [dbo].[ActiveDlls]  WITH CHECK ADD  CONSTRAINT [FK_ActiveDlls_Widgets] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widgets] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActiveDlls] CHECK CONSTRAINT [FK_ActiveDlls_Widgets]
GO
ALTER TABLE [dbo].[AdminBreadcrumbs]  WITH CHECK ADD  CONSTRAINT [FK_AdminBreadcrumbs_Plugins] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminBreadcrumbs] CHECK CONSTRAINT [FK_AdminBreadcrumbs_Plugins]
GO
ALTER TABLE [dbo].[AdminBreadcrumbs]  WITH CHECK ADD  CONSTRAINT [FK_AdminBreadcrumbs_Themes] FOREIGN KEY([ThemeId])
REFERENCES [dbo].[Themes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminBreadcrumbs] CHECK CONSTRAINT [FK_AdminBreadcrumbs_Themes]
GO
ALTER TABLE [dbo].[AdminBreadcrumbs]  WITH CHECK ADD  CONSTRAINT [FK_AdminBreadcrumbs_Widgets] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widgets] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminBreadcrumbs] CHECK CONSTRAINT [FK_AdminBreadcrumbs_Widgets]
GO
ALTER TABLE [dbo].[AdminNavigation]  WITH CHECK ADD  CONSTRAINT [FK__AdminNavi__Theme__403A8C7D] FOREIGN KEY([ThemeId])
REFERENCES [dbo].[Themes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminNavigation] CHECK CONSTRAINT [FK__AdminNavi__Theme__403A8C7D]
GO
ALTER TABLE [dbo].[AdminNavigation]  WITH CHECK ADD  CONSTRAINT [FK_AdminNavigation_Plugins] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminNavigation] CHECK CONSTRAINT [FK_AdminNavigation_Plugins]
GO
ALTER TABLE [dbo].[AdminNavigation]  WITH CHECK ADD  CONSTRAINT [FK_AdminNavigation_Widgets] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widgets] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminNavigation] CHECK CONSTRAINT [FK_AdminNavigation_Widgets]
GO
ALTER TABLE [dbo].[LoginTokens]  WITH CHECK ADD  CONSTRAINT [FK_LoginTokens_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LoginTokens] CHECK CONSTRAINT [FK_LoginTokens_Users]
GO
ALTER TABLE [dbo].[NavigationPages]  WITH CHECK ADD  CONSTRAINT [FK_NavigationPages_NavigationPages] FOREIGN KEY([ParentId])
REFERENCES [dbo].[NavigationPages] ([Id])
GO
ALTER TABLE [dbo].[NavigationPages] CHECK CONSTRAINT [FK_NavigationPages_NavigationPages]
GO
ALTER TABLE [dbo].[News]  WITH CHECK ADD  CONSTRAINT [FK_News_Users] FOREIGN KEY([AuthorID])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_Users]
GO
ALTER TABLE [dbo].[PluginOpenUpdates]  WITH CHECK ADD  CONSTRAINT [FK_PluginOpenUpdates_Plugins] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PluginOpenUpdates] CHECK CONSTRAINT [FK_PluginOpenUpdates_Plugins]
GO
ALTER TABLE [dbo].[PollAnswers]  WITH CHECK ADD  CONSTRAINT [FK_PollAnswers_Poll] FOREIGN KEY([PollId])
REFERENCES [dbo].[Poll] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PollAnswers] CHECK CONSTRAINT [FK_PollAnswers_Poll]
GO
ALTER TABLE [dbo].[PollVotes]  WITH CHECK ADD  CONSTRAINT [FK_PollVotes_PollAnswers] FOREIGN KEY([PollAnswerId])
REFERENCES [dbo].[PollAnswers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PollVotes] CHECK CONSTRAINT [FK_PollVotes_PollAnswers]
GO
ALTER TABLE [dbo].[PollVotes]  WITH CHECK ADD  CONSTRAINT [FK_PollVotes_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PollVotes] CHECK CONSTRAINT [FK_PollVotes_Users]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Websites] FOREIGN KEY([WebsiteId])
REFERENCES [dbo].[Websites] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Websites]
GO
ALTER TABLE [dbo].[ThemeOpenUpdates]  WITH CHECK ADD  CONSTRAINT [FK_ThemeOpenUpdates_Themes] FOREIGN KEY([ThemeId])
REFERENCES [dbo].[Themes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ThemeOpenUpdates] CHECK CONSTRAINT [FK_ThemeOpenUpdates_Themes]
GO
ALTER TABLE [dbo].[WebsiteIn]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteIn_Websites] FOREIGN KEY([WebsiteID])
REFERENCES [dbo].[Websites] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebsiteIn] CHECK CONSTRAINT [FK_WebsiteIn_Websites]
GO
ALTER TABLE [dbo].[WebsiteInDaily]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteInDaily_Websites] FOREIGN KEY([WebsiteId])
REFERENCES [dbo].[Websites] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebsiteInDaily] CHECK CONSTRAINT [FK_WebsiteInDaily_Websites]
GO
ALTER TABLE [dbo].[WebsiteOut]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteOut_Websites] FOREIGN KEY([WebsiteID])
REFERENCES [dbo].[Websites] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebsiteOut] CHECK CONSTRAINT [FK_WebsiteOut_Websites]
GO
ALTER TABLE [dbo].[WebsiteOutDaily]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteOutDaily_Websites] FOREIGN KEY([WebsiteId])
REFERENCES [dbo].[Websites] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebsiteOutDaily] CHECK CONSTRAINT [FK_WebsiteOutDaily_Websites]
GO
ALTER TABLE [dbo].[WebsiteRating]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteRating_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[WebsiteRating] CHECK CONSTRAINT [FK_WebsiteRating_Users]
GO
ALTER TABLE [dbo].[WebsiteRating]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteRating_Websites] FOREIGN KEY([WebsiteID])
REFERENCES [dbo].[Websites] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebsiteRating] CHECK CONSTRAINT [FK_WebsiteRating_Websites]
GO
ALTER TABLE [dbo].[Websites]  WITH CHECK ADD  CONSTRAINT [FK_Websites_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Websites] CHECK CONSTRAINT [FK_Websites_Categories]
GO
ALTER TABLE [dbo].[Websites]  WITH CHECK ADD  CONSTRAINT [FK_Websites_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Websites] CHECK CONSTRAINT [FK_Websites_Users]
GO
ALTER TABLE [dbo].[WidgetOpenUpdates]  WITH CHECK ADD  CONSTRAINT [FK_WidgetOpenUpdates_Widgets] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widgets] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WidgetOpenUpdates] CHECK CONSTRAINT [FK_WidgetOpenUpdates_Widgets]
GO
ALTER TABLE [dbo].[WidgetsTheme]  WITH CHECK ADD  CONSTRAINT [FK_WidgetsTheme_Widgets] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widgets] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WidgetsTheme] CHECK CONSTRAINT [FK_WidgetsTheme_Widgets]
GO
ALTER TABLE [dbo].[WidgetsTheme]  WITH CHECK ADD  CONSTRAINT [FK_WidgetsTheme_WidgetsThemeSection] FOREIGN KEY([ThemeSectionId])
REFERENCES [dbo].[WidgetsThemeSection] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WidgetsTheme] CHECK CONSTRAINT [FK_WidgetsTheme_WidgetsThemeSection]
GO
