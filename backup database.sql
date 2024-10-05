CREATE DATABASE DOCU
GO
USE DOCU
GO
CREATE SCHEMA [XDMS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [XDMS].[SavedFiles](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [uniqueidentifier] NOT NULL,
	[OriginalFileName] [varchar](200) NULL,
	[OriginalFileExtension] [varchar](10) NULL,
	[DocumentCategoryId] [int] NULL,
	[RouteSaved] [varchar](max) NULL,
	[StorageTypeId] [int] NULL,
	[Status] [bit] NOT NULL,
	[CompanyId] [int] NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreateUserId] [int] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyUserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[FileName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [XDMS].[DocumentCategories]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [XDMS].[DocumentCategories](
	[CategoryId] [int] NOT NULL,
	[CategoryName] [varchar](200) NOT NULL,
	[SectionId] [int] NOT NULL,
	[FolderName] [varchar](50) NOT NULL,
	[DisplayOrder] [int] NULL,
	[CompanyId] [int] NULL,
	[Status] [bit] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreateUserId] [int] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyUserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [XDMS].[View_GetSavedFiles]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [XDMS].[View_GetSavedFiles] AS(
	SELECT
		F.FileId
		,F.FileName
		,F.OriginalFileName
		,F.OriginalFileExtension
		,F.DocumentCategoryId
		,DC.CategoryName
		,DC.SectionId
		,F.RouteSaved
		,F.StorageTypeId
		,F.Status
		,F.CompanyId
		,F.CreationDate
		,F.CreateUserId
		,F.ModifyDate
		,F.ModifyUserId
	FROM XDMS.SavedFiles AS F
	LEFT JOIN XDMS.DocumentCategories AS DC
		ON DC.CategoryId = F.DocumentCategoryId
)
GO
/****** Object:  Table [XDMS].[StorageTypes]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [XDMS].[StorageTypes](
	[TypeId] [int] NOT NULL,
	[TypeName] [varchar](200) NOT NULL,
	[Status] [bit] NOT NULL,
	[CompanyId] [int] NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreateUserId] [int] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyUserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [XDMS].[View_GetStorageTypes]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [XDMS].[View_GetStorageTypes] AS(
	SELECT
		ST.*
	FROM XDMS.StorageTypes AS ST
);
GO
/****** Object:  Table [XDMS].[DocumentSections]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [XDMS].[DocumentSections](
	[SectionId] [int] NOT NULL,
	[SectionName] [varchar](200) NOT NULL,
	[SectionKeyword] [varchar](100) NULL,
	[FolderName] [varchar](50) NOT NULL,
	[CompanyId] [int] NULL,
	[Status] [bit] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreateUserId] [int] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyUserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [XDMS].[View_GetDocumentSections]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [XDMS].[View_GetDocumentSections] AS(
	SELECT 
		DS.SectionId
		,DS.SectionName
		,DS.SectionKeyword
		,TRIM(DS.FolderName) AS FolderName
		,DS.CompanyId
		,DS.[Status]
		,DS.CreationDate
		,DS.CreateUserId
		,DS.ModifyDate
		,DS.ModifyUserId
	FROM XDMS.DocumentSections AS DS
);
GO
/****** Object:  View [XDMS].[View_GetDocumentCategories]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [XDMS].[View_GetDocumentCategories] AS(
	SELECT 
		DC.CategoryId
		,DC.CategoryName
		,DC.SectionId
		,DS.SectionName
		,DS.SectionKeyword
		,TRIM(DC.FolderName) AS FolderName
		,IIF(TRIM(DS.FolderName) IS NOT NULL AND TRIM(DC.FolderName) IS NOT NULL, (TRIM(DS.FolderName) + '/' + TRIM(DC.FolderName)), NULL) AS Directory
		,DC.DisplayOrder
		,DC.CompanyId
		,DC.[Status]
		,DC.CreationDate
		,DC.CreateUserId
		,DC.ModifyDate
		,DC.ModifyUserId
	FROM XDMS.DocumentCategories AS DC
	INNER JOIN XDMS.DocumentSections AS DS
		ON DS.SectionId = DC.SectionId
);
GO
INSERT [XDMS].[DocumentCategories] ([CategoryId], [CategoryName], [SectionId], [FolderName], [DisplayOrder], [CompanyId], [Status], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (1, N'Viejos', 1, N'Viejos', NULL, NULL, 1, CAST(N'2023-10-31T15:49:49.177' AS DateTime), NULL, NULL, NULL)
GO
INSERT [XDMS].[DocumentSections] ([SectionId], [SectionName], [SectionKeyword], [FolderName], [CompanyId], [Status], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (1, N'Generales', N'GENERAL', N'Generales', NULL, 1, CAST(N'2023-10-31T15:49:49.173' AS DateTime), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [XDMS].[SavedFiles] ON 
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (1, N'0d037806-6f25-4369-9704-be30d1a1699a', N'Gabriel Antonio CERT.pdf', N'.pdf', 1, N'Generales/Viejos\0d037806-6f25-4369-9704-be30d1a1699a.pdf', 2, 1, 1, CAST(N'2023-11-03T10:53:27.127' AS DateTime), NULL, CAST(N'2023-11-03T11:33:20.393' AS DateTime), 2)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (2, N'88fffde0-f07b-4986-8da4-516b2c283e00', N'Proposito de la capacitacion.pdf', N'.pdf', 1, N'Generales/Viejos/88fffde0-f07b-4986-8da4-516b2c283e00.pdf', 2, 1, 1, CAST(N'2023-11-03T11:01:40.100' AS DateTime), NULL, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (3, N'3f06cb04-a1ea-4ae5-b8e9-fbf610c0b1f0', N'announcement - Prerequisites Preparation Resources.pdf', N'.pdf', 1, N'Generales/Viejos/3f06cb04-a1ea-4ae5-b8e9-fbf610c0b1f0.pdf', 2, 0, 1, CAST(N'2023-11-03T11:13:07.580' AS DateTime), 2, CAST(N'2023-12-18T12:54:56.060' AS DateTime), 452)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (4, N'c8066734-021d-482e-9b43-2a9ff74294d2', N'Base64File-11/3/2023 1:01:58 PM.pdf', N'.pdf', 1, N'Generales/Viejos/c8066734-021d-482e-9b43-2a9ff74294d2.pdf', 2, 1, 1, CAST(N'2023-11-03T13:02:02.510' AS DateTime), 2, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (5, N'5e99a20b-e9a9-4305-9266-b17cb4131a4d', N'Base64File-05:05:56.pdf', N'.pdf', 1, N'Generales/Viejos/5e99a20b-e9a9-4305-9266-b17cb4131a4d.pdf', 2, 1, 1, CAST(N'2023-11-03T13:05:58.250' AS DateTime), 2, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (6, N'42b88539-e72b-41ab-a6de-df8239b4ff6f', N'Gabriel Antonio CERT.pdf', N'.pdf', 1, N'Generales/Viejos/42b88539-e72b-41ab-a6de-df8239b4ff6f.pdf', 1, 1, 1, CAST(N'2023-11-03T15:23:36.047' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (7, N'8dee037d-a647-4cfe-b968-3b05dd7b0264', N'Gabriel Antonio CERT.pdf', N'.pdf', 1, N'SIGEI-DOCUMENTS/Generales/Viejos/8dee037d-a647-4cfe-b968-3b05dd7b0264.pdf', 1, 1, 1, CAST(N'2023-11-03T15:29:18.600' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (8, N'058f6b21-36d9-418c-9724-bfb4713d54df', N'certificadoPDF.pdf', N'.pdf', 1, N'C:\Users\jperezv\Desktop\Repositories\SIGEI_DOCUMENTS/SIGEI-DOCUMENTS/Generales/Viejos/058f6b21-36d9-418c-9724-bfb4713d54df.pdf', 1, 1, 1, CAST(N'2023-11-03T15:34:15.733' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (9, N'1c38ee89-e391-4dd8-a7a1-979129f0fedf', N'Gabriel Antonio CERT.jpg', N'.jpg', 1, N'SIGEI-DOCUMENTS/Generales/Viejos/1c38ee89-e391-4dd8-a7a1-979129f0fedf.jpg', 1, 1, 1, CAST(N'2023-11-03T15:47:43.750' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (10, N'337278e9-343f-4e77-96e5-7b11f21f26d5', N'Gabriel Antonio CERT.jpg', N'.jpg', 1, N'~/SIGEI-DOCUMENTS/1/Generales/Viejos/337278e9-343f-4e77-96e5-7b11f21f26d5.jpg', 1, 1, 1, CAST(N'2023-11-07T09:21:09.987' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (11, N'f295c804-f4ae-43f0-85fa-bb7d88dc17e4', N'Elon Musk.jpg', N'.jpg', 1, N'/SIGEI-DOCUMENTS/1/Generales/Viejos/f295c804-f4ae-43f0-85fa-bb7d88dc17e4.jpg', 1, 0, 1, CAST(N'2023-11-07T09:40:33.093' AS DateTime), 1, CAST(N'2023-11-07T15:28:27.167' AS DateTime), 2)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (12, N'a3ab0c93-0184-4935-8726-87fea1ee4517', N'Sigei.png', N'.png', 1, N'1/Generales/Viejos/a3ab0c93-0184-4935-8726-87fea1ee4517.png', 2, 1, 1, CAST(N'2023-11-07T14:41:44.373' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (13, N'9792805f-0203-4e26-bff9-d70d949ce4d2', N'bg-itla.jpg', N'.jpg', 1, N'1/Generales/Viejos/9792805f-0203-4e26-bff9-d70d949ce4d2.jpg', 4, 1, 1, CAST(N'2023-12-18T11:40:40.123' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (14, N'530831be-b7ee-4ca4-b68a-ead139f3f397', N'bg-itla.jpg', N'.jpg', 1, N'1/Generales/Viejos/530831be-b7ee-4ca4-b68a-ead139f3f397.jpg', 4, 1, 1, CAST(N'2023-12-18T11:45:50.180' AS DateTime), 1, NULL, NULL)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (15, N'0dd5b03c-8649-4f4c-ae73-2a73629910e6', N'COVID_Breaker_red.png', N'.png', 1, N'1/Generales/Viejos/0dd5b03c-8649-4f4c-ae73-2a73629910e6.png', 4, 0, 1, CAST(N'2023-12-18T12:42:12.213' AS DateTime), 1, CAST(N'2023-12-18T12:48:42.163' AS DateTime), 452)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (16, N'b9ef5543-cd3b-4d17-8cf1-9b91a5fd52d1', N'Elon Musk.jpg', N'.jpg', 1, N'1/Generales/Viejos/b9ef5543-cd3b-4d17-8cf1-9b91a5fd52d1.jpg', 4, 0, 1, CAST(N'2023-12-18T12:57:18.863' AS DateTime), 1, CAST(N'2023-12-18T12:58:37.537' AS DateTime), 452)
GO
INSERT [XDMS].[SavedFiles] ([FileId], [FileName], [OriginalFileName], [OriginalFileExtension], [DocumentCategoryId], [RouteSaved], [StorageTypeId], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (17, N'4ccd1a35-99bf-4bf5-b709-3c8ce9e9f867', N'Sigei.png', N'.png', 1, N'1/Generales/Viejos/4ccd1a35-99bf-4bf5-b709-3c8ce9e9f867.png', 4, 1, 1, CAST(N'2023-12-18T14:56:07.323' AS DateTime), 1, NULL, NULL)
GO
SET IDENTITY_INSERT [XDMS].[SavedFiles] OFF
GO
INSERT [XDMS].[StorageTypes] ([TypeId], [TypeName], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (1, N'Local', 1, NULL, CAST(N'2023-11-02T11:13:19.483' AS DateTime), NULL, NULL, NULL)
GO
INSERT [XDMS].[StorageTypes] ([TypeId], [TypeName], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (2, N'AWS S3', 1, NULL, CAST(N'2023-11-02T11:13:19.483' AS DateTime), NULL, NULL, NULL)
GO
INSERT [XDMS].[StorageTypes] ([TypeId], [TypeName], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (3, N'Azure Vault', 1, NULL, CAST(N'2023-11-02T11:13:19.483' AS DateTime), NULL, NULL, NULL)
GO
INSERT [XDMS].[StorageTypes] ([TypeId], [TypeName], [Status], [CompanyId], [CreationDate], [CreateUserId], [ModifyDate], [ModifyUserId]) VALUES (4, N'FTP', 1, NULL, CAST(N'2023-12-14T15:00:43.750' AS DateTime), NULL, NULL, NULL)
GO
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [XDMS].[ServicesLog](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[MethodName] [varchar](500) NOT NULL,
	[SentParameters] [varchar](max) NULL,
	[LogMessages] [varchar](max) NULL,
	[CreateUserId] [bigint] NULL,
 CONSTRAINT [PK__Services__5E548648018C8685] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TABLE [XDMS].[ServicesLog] ADD  CONSTRAINT [DF__ServicesL__Creat__5E8AD4CA]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [XDMS].[DocumentCategories] ADD  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [XDMS].[DocumentSections] ADD  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [XDMS].[SavedFiles] ADD  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [XDMS].[StorageTypes] ADD  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [XDMS].[DocumentCategories]  WITH CHECK ADD FOREIGN KEY([SectionId])
REFERENCES [XDMS].[DocumentSections] ([SectionId])
GO
ALTER TABLE [XDMS].[SavedFiles]  WITH CHECK ADD FOREIGN KEY([DocumentCategoryId])
REFERENCES [XDMS].[DocumentCategories] ([CategoryId])
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [XDMS].[SP_SetLogs]
	@MethodName VARCHAR(MAX),
	@SentParameters VARCHAR(MAX),
	@LogMessages VARCHAR(MAX),
	@CreateUserId BIGINT = NULL,
	@Output Int Output
AS
BEGIN
	INSERT INTO [XDMS].ServicesLog
	(
		MethodName
		,SentParameters
		,LogMessages
		,CreateUserId
	)
	VALUES
	(
		@MethodName
		,@SentParameters
		,@LogMessages
		,@CreateUserId
	)
	SELECT @Output = @@IDENTITY
END
GO
/****** Object:  StoredProcedure [XDMS].[Sp_GetDocumentCategories]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [XDMS].[Sp_GetDocumentCategories]
	@CategoryId INT,
	@SectionId INT,
	@SectionKeyword VARCHAR(100),
	@CompanyId INT,
	@Type INT = 0
AS
BEGIN
	
	IF ISNULL(@Type,0) = 0
		BEGIN
			
			SELECT * FROM XDMS.View_GetDocumentCategories
			ORDER BY DisplayOrder ASC

		END
	ELSE IF @Type = 1
		BEGIN

			SELECT * FROM XDMS.View_GetDocumentCategories
			WHERE CategoryId = @CategoryId
			ORDER BY DisplayOrder ASC

		END

END
GO
/****** Object:  StoredProcedure [XDMS].[Sp_GetDocumentSections]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [XDMS].[Sp_GetDocumentSections]
	@SectionId INT,
	@SectionKeyword VARCHAR(100),
	@CompanyId INT,
	@Type INT = 0
AS
BEGIN
	
	IF ISNULL(@Type,0) = 0
		BEGIN
			
			SELECT * FROM XDMS.View_GetDocumentSections
			ORDER BY CreationDate DESC

		END

END
GO
/****** Object:  StoredProcedure [XDMS].[Sp_GetSavedFiles]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [XDMS].[Sp_GetSavedFiles]
	@FileId INT,
	@FileName UNIQUEIDENTIFIER,
	@CategoryId INT,
	@SectionId INT,
	@StorageTypeId INT,
	@CompanyId INT,
	@Type INT = 0
AS
BEGIN
	
	IF ISNULL(@Type,0) = 0
		BEGIN
			
			SELECT * FROM XDMS.View_GetSavedFiles
			WHERE FileName = @FileName AND Status = 1
			ORDER BY CreationDate DESC

		END
	ELSE IF @Type = 1
		BEGIN

			SELECT * FROM XDMS.View_GetSavedFiles
			WHERE Status = 1
			ORDER BY CreationDate DESC

		END
	ELSE IF @Type = 2
		BEGIN

			SELECT * FROM XDMS.View_GetSavedFiles
			WHERE Status = 1 AND CompanyId = @CompanyId
			ORDER BY CreationDate DESC

		END
	ELSE IF @Type = 3
		BEGIN

			SELECT * FROM XDMS.View_GetSavedFiles
			WHERE DocumentCategoryId = @CategoryId AND CompanyId = @CompanyId
			ORDER BY CreationDate DESC

		END


END
GO
/****** Object:  StoredProcedure [XDMS].[Sp_GetStorageTypes]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [XDMS].[Sp_GetStorageTypes]
	@TypeId INT,
	@CategoryId INT,
	@FileId INT,
	@FileName UNIQUEIDENTIFIER,
	@CompanyId INT,
	@Type INT = 0
AS
BEGIN
	
	IF ISNULL(@Type,0) = 0
		BEGIN
			
			SELECT * FROM XDMS.View_GetStorageTypes
			ORDER BY CreationDate DESC

		END

END
GO
/****** Object:  StoredProcedure [XDMS].[Sp_SetSavedFiles]    Script Date: 10/4/2024 11:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [XDMS].[Sp_SetSavedFiles]
	@FileId INT 
	,@FileName UNIQUEIDENTIFIER 
	,@OriginalFileName VARCHAR(200)
	,@OriginalFileExtension VARCHAR(10)
	,@DocumentCategoryId INT 
	,@RouteSaved VARCHAR(MAX)
	,@StorageTypeId INT
	,@CompanyId INT
	,@UserId INT
	,@Operation INT
	,@Output INT OUT
AS
BEGIN
	
	IF @Operation = 1
		BEGIN
			
			INSERT INTO XDMS.SavedFiles
			(
				FileName
				,OriginalFileName
				,OriginalFileExtension
				,DocumentCategoryId
				,RouteSaved
				,StorageTypeId
				,CompanyId
				,CreateUserId
				,CreationDate
				,Status
			)
			VALUES
			(
				@FileName
				,@OriginalFileName
				,@OriginalFileExtension
				,@DocumentCategoryId
				,@RouteSaved
				,@StorageTypeId
				,@CompanyId
				,@UserId
				,GETDATE()
				, 1
			);

			SELECT @Output = @@identity;

		END
	ELSE IF @Operation = 3
		BEGIN

			UPDATE XDMS.SavedFiles
			SET
				Status = 0
				,ModifyDate = GETDATE()
				,ModifyUserId = @UserId
			WHERE FileId = @FileId;

			SELECT @Output = @FileId;

		END


END
GO
