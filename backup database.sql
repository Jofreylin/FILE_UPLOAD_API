create DATABASE DOCU
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
