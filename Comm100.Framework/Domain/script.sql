USE [Comm100.File]
GO
/****** Object:  Table [dbo].[t_fileService_config]    Script Date: 2019/11/21 13:48:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_fileService_config](
	[Key] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[t_fileService_file]    Script Date: 2019/11/21 13:49:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_fileService_file](
	[FileKey] [char](172) NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[Checksum] [binary](32) NULL,
	[SiteId] [int] NOT NULL,
	[ExpireTime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[t_fileService_fileContent]    Script Date: 2019/11/21 13:49:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_fileService_fileContent](
	[Checksum] [binary](32) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Content] [varbinary](max) NOT NULL,
	[StorageType] [smallint] NOT NULL,
	[Link] [nvarchar](2048) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[t_fileService_fileLimit]    Script Date: 2019/11/21 13:49:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_fileService_fileLimit](
	[AppId] [nvarchar](1024) NOT NULL,
	[MaxSize] [int] NOT NULL
) ON [PRIMARY]
GO
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'S3Settings', N'{ "address": "mybucket.s3.amazonaws.com"
, "apiKey": ""
, "bucket": "mybucket"
}
')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'SharedSecret', N'')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'JWTPublicKeys', N'')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'JWTPrivateKeyPath', N'')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'FileBlackList', N'["*.exe", "*.com"]')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'RateLimiting', N'{ "getPerIP": {"duration": "00:00:60","limit": 50}
,"getPerFileKey": {
"duration":"00:00:60",
"limit":10}
,"savePerIP": {"duration":"00:00:00","limit":3
}
}
')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'IPWhiteList', N'[{"from": "192.168.1.1", to: "192.168.1.100"}]')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'IsMainServer', N'true')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'MainServiceUrl', N'https://example.main.fileservice/v1')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'DbToS3WorkerNum', N'10')
INSERT [dbo].[t_fileService_config] ([Key], [Value]) VALUES (N'StandbyToMainWorkerNum', N'10')
SET ANSI_PADDING ON
GO
/****** Object:  Index [PK_t_fileService_file_fileKey]    Script Date: 2019/11/21 13:50:21 ******/
ALTER TABLE [dbo].[t_fileService_file] ADD  CONSTRAINT [PK_t_fileService_file_fileKey] PRIMARY KEY NONCLUSTERED 
(
	[FileKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_t_fileService_file_checksum]    Script Date: 2019/11/21 13:50:21 ******/
CREATE NONCLUSTERED INDEX [IX_t_fileService_file_checksum] ON [dbo].[t_fileService_file]
(
	[Checksum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_t_fileService_file_creationTime]    Script Date: 2019/11/21 13:50:21 ******/
CREATE NONCLUSTERED INDEX [IX_t_fileService_file_creationTime] ON [dbo].[t_fileService_file]
(
	[CreationTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_t_fileService_file_expireTime]    Script Date: 2019/11/21 13:50:21 ******/
CREATE NONCLUSTERED INDEX [IX_t_fileService_file_expireTime] ON [dbo].[t_fileService_file]
(
	[ExpireTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_t_fileService_file_siteId]    Script Date: 2019/11/21 13:50:21 ******/
CREATE NONCLUSTERED INDEX [IX_t_fileService_file_siteId] ON [dbo].[t_fileService_file]
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [PK_t_fileServicce_fileContent_checksum]    Script Date: 2019/11/21 13:50:21 ******/
ALTER TABLE [dbo].[t_fileService_fileContent] ADD  CONSTRAINT [PK_t_fileServicce_fileContent_checksum] PRIMARY KEY NONCLUSTERED 
(
	[Checksum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_t_fileServicce_fileContent_storageType]    Script Date: 2019/11/21 13:50:21 ******/
CREATE NONCLUSTERED INDEX [IX_t_fileServicce_fileContent_storageType] ON [dbo].[t_fileService_fileContent]
(
	[StorageType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[t_fileService_config] ADD  CONSTRAINT [DF_t_fileService_config_Key]  DEFAULT ('') FOR [Key]
GO
ALTER TABLE [dbo].[t_fileService_config] ADD  CONSTRAINT [DF_t_fileService_config_Value]  DEFAULT ('') FOR [Value]
GO
ALTER TABLE [dbo].[t_fileService_file] ADD  CONSTRAINT [DF_t_fileService_file_CreationTime]  DEFAULT (getutcdate()) FOR [CreationTime]
GO
ALTER TABLE [dbo].[t_fileService_fileContent] ADD  CONSTRAINT [DF_t_fileServicce_fileContent_StorageType]  DEFAULT ((0)) FOR [StorageType]
GO
ALTER TABLE [dbo].[t_fileService_fileLimit] ADD  CONSTRAINT [DF_t_fileService_fileLimit_AppId]  DEFAULT (N'‘’') FOR [AppId]
GO
ALTER TABLE [dbo].[t_fileService_fileLimit] ADD  CONSTRAINT [DF_t_fileService_fileLimit_MaxSize]  DEFAULT ((0)) FOR [MaxSize]
GO
