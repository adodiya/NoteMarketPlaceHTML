USE [master]
GO
/****** Object:  Database [NotesMarket]    Script Date: 14-04-2021 10:09:11 ******/
CREATE DATABASE [NotesMarket]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'NotesMarket', FILENAME = N'E:\SQL-2017\MSSQL14.MSSQLSERVER\MSSQL\DATA\NotesMarket.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'NotesMarket_log', FILENAME = N'E:\SQL-2017\MSSQL14.MSSQLSERVER\MSSQL\DATA\NotesMarket_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [NotesMarket] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NotesMarket].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NotesMarket] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NotesMarket] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NotesMarket] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NotesMarket] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NotesMarket] SET ARITHABORT OFF 
GO
ALTER DATABASE [NotesMarket] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NotesMarket] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NotesMarket] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NotesMarket] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NotesMarket] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NotesMarket] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NotesMarket] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NotesMarket] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NotesMarket] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NotesMarket] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NotesMarket] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NotesMarket] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NotesMarket] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NotesMarket] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NotesMarket] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NotesMarket] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NotesMarket] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NotesMarket] SET RECOVERY FULL 
GO
ALTER DATABASE [NotesMarket] SET  MULTI_USER 
GO
ALTER DATABASE [NotesMarket] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NotesMarket] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NotesMarket] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NotesMarket] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [NotesMarket] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'NotesMarket', N'ON'
GO
ALTER DATABASE [NotesMarket] SET QUERY_STORE = OFF
GO
USE [NotesMarket]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 14-04-2021 10:09:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Configurations]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configurations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Value] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Configurations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[CountryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[CountryCode] [varchar](100) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Downloads]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Downloads](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[Seller] [int] NOT NULL,
	[Downloader] [int] NOT NULL,
	[DownloadAllowed] [bit] NOT NULL,
	[AttachmentPath] [varchar](max) NULL,
	[DownloadDate] [datetime] NULL,
	[IsAttachmentDownloaded] [bit] NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[Price] [decimal](18, 0) NULL,
	[NoteTitle] [varchar](100) NOT NULL,
	[NoteCategory] [varchar](100) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
 CONSTRAINT [PK_Downloads] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notes]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SellerID] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ActionedBy] [int] NULL,
	[AdminRemarks] [varchar](max) NULL,
	[PublishedDate] [datetime] NULL,
	[Title] [varchar](100) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[DisplayPic] [varchar](500) NULL,
	[TypeID] [int] NULL,
	[NoOfPages] [int] NULL,
	[Description] [varchar](max) NOT NULL,
	[UniversityName] [varchar](200) NULL,
	[CountryID] [int] NULL,
	[Course] [varchar](100) NULL,
	[CourseCode] [varchar](100) NULL,
	[Professor] [varchar](100) NULL,
	[IsPaid] [bit] NOT NULL,
	[Price] [decimal](18, 0) NULL,
	[Preview] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Notes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotesAttachments]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotesAttachments](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[FileName] [varchar](100) NOT NULL,
	[FilePath] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NotesAttachments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotesReviews]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotesReviews](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[ReviewedBy] [int] NULL,
	[DownloadID] [int] NOT NULL,
	[Ratings] [decimal](18, 0) NOT NULL,
	[Comments] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NotesReviews] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReferenceData]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferenceData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[DataValue] [varchar](100) NOT NULL,
	[RefCategory] [varchar](100) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ReferenceData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportedIssues]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportedIssues](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[ReportedByID] [int] NOT NULL,
	[DownloadID] [int] NOT NULL,
	[Remarks] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
 CONSTRAINT [PK_ReportedIssues] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Types]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Types](
	[TypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Types] PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[DOB] [datetime] NULL,
	[Gender] [int] NOT NULL,
	[SecondaryEmailID] [varchar](100) NULL,
	[PhonenoCountryCode] [varchar](5) NOT NULL,
	[Phoneno] [varchar](20) NOT NULL,
	[ProfilePic] [varchar](500) NULL,
	[AddressL1] [varchar](100) NOT NULL,
	[AddressL2] [varchar](100) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](50) NOT NULL,
	[ZipCode] [varchar](50) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[University] [varchar](100) NULL,
	[College] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
 CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 14-04-2021 10:09:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[EmailID] [varchar](100) NOT NULL,
	[Password] [varchar](24) NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[ActivationCode] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'IT', N'Information Technology books', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Categories] ([CategoryID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (3, N'CA', N'..', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Categories] ([CategoryID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (4, N'MBA', N'..', NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Countries] ON 

INSERT [dbo].[Countries] ([CountryID], [Name], [CountryCode], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'India', N'91', NULL, 2, NULL, NULL, 1)
INSERT [dbo].[Countries] ([CountryID], [Name], [CountryCode], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'Australia', N'61', NULL, 2, NULL, NULL, 1)
INSERT [dbo].[Countries] ([CountryID], [Name], [CountryCode], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (3, N'Bangladesh', N'880', NULL, 2, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Countries] OFF
GO
SET IDENTITY_INSERT [dbo].[Downloads] ON 

INSERT [dbo].[Downloads] ([ID], [NoteID], [Seller], [Downloader], [DownloadAllowed], [AttachmentPath], [DownloadDate], [IsAttachmentDownloaded], [IsPaid], [Price], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (13, 44, 10, 9, 1, N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python21221924.pdf', NULL, 1, 0, CAST(0 AS Decimal(18, 0)), N'CA', N'CA', NULL, NULL, NULL, NULL)
INSERT [dbo].[Downloads] ([ID], [NoteID], [Seller], [Downloader], [DownloadAllowed], [AttachmentPath], [DownloadDate], [IsAttachmentDownloaded], [IsPaid], [Price], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (17, 48, 10, 9, 1, N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python21221924 (1)21021368.pdf', NULL, 1, 0, CAST(0 AS Decimal(18, 0)), N'heyy', N'CA', NULL, NULL, NULL, NULL)
INSERT [dbo].[Downloads] ([ID], [NoteID], [Seller], [Downloader], [DownloadAllowed], [AttachmentPath], [DownloadDate], [IsAttachmentDownloaded], [IsPaid], [Price], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (18, 55, 9, 10, 1, NULL, NULL, 0, 1, CAST(780 AS Decimal(18, 0)), N'Python', N'IT', CAST(N'2021-04-12T11:34:25.683' AS DateTime), 10, CAST(N'2021-04-12T12:09:24.690' AS DateTime), 10)
SET IDENTITY_INSERT [dbo].[Downloads] OFF
GO
SET IDENTITY_INSERT [dbo].[Notes] ON 

INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (38, 9, 6, 11, N'Inapproprite', NULL, N'Artificial Intelligence', 2, N'~/Uploaded_Images/image21390098.png', 3, 100, N'llll', N'VGEC', 2, N'IT', N'127568', N'DCP', 0, CAST(0 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p5_ai21390101.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (39, 9, 8, NULL, NULL, CAST(N'2021-04-14T07:38:56.847' AS DateTime), N'Robotics', 2, NULL, 2, 888, N'ggggg', N'VGEC', 2, N'IT', N'999', N'DCP', 0, CAST(0 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p5_ai21022557.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (40, 10, 5, NULL, NULL, NULL, N'CA', 3, N'~/Uploaded_Images/logo21460944.png', 2, 123, N'zx', N'VGEC', 2, N'CA', N'127568', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (41, 10, 8, NULL, NULL, CAST(N'2021-04-14T07:21:41.240' AS DateTime), N'IP', 3, N'~/Uploaded_Images/logo21555279.png', 4, 123, N'gfcv', N'Nirma', 2, N'MBA', N'127568', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (42, 10, 6, 9, N'Not proper', NULL, N'CA', 3, N'~/Uploaded_Images/user-121001209.jpg', 3, 123, N'qwe', N'Vgec', 2, N'MBA', N'543260', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (43, 10, 6, 11, N'njm', NULL, N'CA', 4, N'~/Uploaded_Images/image21050250.png', 2, 123, N'qwe', N'GEC', 1, N'CA', N'jnm', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (44, 10, 4, 9, N'not proper', NULL, N'DBMS', 3, N'~/Uploaded_Images/image21221704.png', 3, 123, N'asdf', N'', 2, N'CSE', N'543260', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (45, 10, 4, NULL, NULL, CAST(N'2021-04-14T07:21:03.597' AS DateTime), N'Big Data', 2, NULL, 3, 789, N'jnmkh', N'Vgec', 3, N'CSE', N'127568', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (46, 10, 9, 9, N'njmk', CAST(N'2021-04-02T10:41:02.260' AS DateTime), N'Science', 3, N'~/Uploaded_Images/image (1)21412398.png', 3, 123, N'hnj', NULL, 2, N'CSE', N'127568', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python2122192421412398.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (47, 10, 4, NULL, NULL, CAST(N'2021-04-02T10:48:03.137' AS DateTime), N'Maths', 2, N'~/Uploaded_Images/image21510616.png', 3, 123, N'asc', N'Vgec', 2, N'MBA', N'543260', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p5_python21510616.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (48, 10, 4, NULL, NULL, NULL, N'heyy', 3, N'~/Uploaded_Images/image21021129.png', 2, 789, N'cvfghb', N'LD', 1, N'CA', N'jnm', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python21221924 (2)21021130.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (49, 10, 4, NULL, NULL, CAST(N'2021-04-12T14:48:06.787' AS DateTime), N'Ninsd', 2, N'~/Uploaded_Images/image (1)21091940.png', 4, 123, N'zxc', N'LD', 1, N'IT', N'543267', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (50, 10, 6, 11, N'not proper', NULL, N'CA', 2, NULL, 2, 123, N'hbnjm', N'Vgec', 2, N'CA', N'6754', N'fgtc', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (54, 10, 4, NULL, NULL, NULL, N'CN', 3, NULL, 3, 789, N'hn', N'Nirma', 1, N'CA', N'543268', N'fgtc', 1, CAST(780 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013 SEM 8 FEE RECIEPT21312798.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (55, 9, 5, NULL, NULL, CAST(N'2021-04-12T10:51:50.997' AS DateTime), N'Python', 2, NULL, 3, 250, N'python ', N'GTU', 1, N'IT', N'543267', N'DSW', 1, CAST(780 AS Decimal(18, 0)), NULL, NULL, 10, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (56, 9, 4, NULL, NULL, CAST(N'2021-04-12T10:59:47.807' AS DateTime), N'IP', 2, NULL, 2, 250, N'ip ', N'GTU', 1, N'IT', N'543267', N'VDP', 1, CAST(780 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p3_python21594776.pdf', NULL, 10, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (57, 10, 4, NULL, NULL, CAST(N'2021-04-12T18:39:45.563' AS DateTime), N'Data analyticx', 2, N'~/Uploaded_Images/computer-science21394552.png', 3, 250, N'mklb', N'Vgec', 1, N'IT', N'543260', N'VDP', 1, CAST(780 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p1_ai21394549.pdf', NULL, 10, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (58, 10, 4, NULL, NULL, CAST(N'2021-04-14T07:48:35.317' AS DateTime), N'Cloud', 2, NULL, 2, 200, N'cloud computing', N'LD', 1, N'IT', N'543267', N'VDP', 1, CAST(780 AS Decimal(18, 0)), N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p1_ai2139491121483520.pdf', NULL, 10, NULL, NULL, 0)
INSERT [dbo].[Notes] ([ID], [SellerID], [Status], [ActionedBy], [AdminRemarks], [PublishedDate], [Title], [CategoryID], [DisplayPic], [TypeID], [NoOfPages], [Description], [UniversityName], [CountryID], [Course], [CourseCode], [Professor], [IsPaid], [Price], [Preview], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (59, 10, 4, NULL, NULL, CAST(N'2021-04-14T08:07:57.287' AS DateTime), N'WEB', 2, NULL, 3, 123, N'web', N'Vgec', 1, N'IT', N'543267', N'VDP', 0, CAST(0 AS Decimal(18, 0)), NULL, NULL, 10, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[Notes] OFF
GO
SET IDENTITY_INSERT [dbo].[NotesAttachments] ON 

INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, 44, N'170170116013_p4_python21221924.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python21221924.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, 44, N'170170116013_p3_python21221937.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p3_python21221937.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (5, 48, N'170170116013_p4_python21221924 (1)21021368.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python21221924 (1)21021368.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (6, 48, N'170170116013_p4_python2122192421021374.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python2122192421021374.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (8, 54, N'170170116013_p6_python21313410.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p6_python21313410.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (9, 56, N'170170116013_p4_python21221924 (1)2102136821595003.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p4_python21221924 (1)2102136821595003.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (10, 57, N'170170116013_p1_ai21394911.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p1_ai21394911.pdf', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (13, 55, N'170170116013_p1_ai21394911.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p1_ai21394911.pdf', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[NotesAttachments] ([ID], [NoteID], [FileName], [FilePath], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (15, 58, N'170170116013_p2_ai21483630.pdf', N'C:\Users\admin\source\repos\NotesMarketPlace\UploadedFiles\170170116013_p2_ai21483630.pdf', NULL, NULL, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[NotesAttachments] OFF
GO
SET IDENTITY_INSERT [dbo].[NotesReviews] ON 

INSERT [dbo].[NotesReviews] ([ID], [NoteID], [ReviewedBy], [DownloadID], [Ratings], [Comments], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (3, 48, 10, 17, CAST(4 AS Decimal(18, 0)), N'Good', CAST(N'2021-04-11T15:17:29.313' AS DateTime), 10, CAST(N'2021-04-13T16:15:15.563' AS DateTime), 10, 1)
SET IDENTITY_INSERT [dbo].[NotesReviews] OFF
GO
SET IDENTITY_INSERT [dbo].[ReferenceData] ON 

INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'Female', N'FEMALE', N'Gender', NULL, 2, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'Male', N'MALE', N'Gender', NULL, 2, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (3, N'Others', N'OTHERS', N'Gender', NULL, 2, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (4, N'Published', N'PUBLISHED', N'Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (5, N'Draft', N'DRAFT', N'Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (6, N'Rejected', N'REJECTED', N'Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (7, N'Submitted', N'SUBMITTED', N'Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (8, N'InReview', N'INREVIEW', N'Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (9, N'Removed', N'REMOVED', N'Status', NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[ReferenceData] OFF
GO
SET IDENTITY_INSERT [dbo].[ReportedIssues] ON 

INSERT [dbo].[ReportedIssues] ([ID], [NoteID], [ReportedByID], [DownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (2, 44, 10, 13, N'jnmkl', NULL, NULL, NULL, NULL)
INSERT [dbo].[ReportedIssues] ([ID], [NoteID], [ReportedByID], [DownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (3, 44, 10, 13, N'hbjuji', NULL, NULL, NULL, NULL)
INSERT [dbo].[ReportedIssues] ([ID], [NoteID], [ReportedByID], [DownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (4, 44, 10, 13, N'hnjmk', NULL, NULL, NULL, NULL)
INSERT [dbo].[ReportedIssues] ([ID], [NoteID], [ReportedByID], [DownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (5, 55, 10, 18, N'Inappropriate', CAST(N'2021-04-13T16:01:53.523' AS DateTime), 10, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ReportedIssues] OFF
GO
SET IDENTITY_INSERT [dbo].[Types] ON 

INSERT [dbo].[Types] ([TypeID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'Handwritten', N'.', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Types] ([TypeID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (3, N'University Notes', N'.', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Types] ([TypeID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (4, N'Story Books', N'.', NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Types] OFF
GO
SET IDENTITY_INSERT [dbo].[UserProfile] ON 

INSERT [dbo].[UserProfile] ([ID], [UserID], [DOB], [Gender], [SecondaryEmailID], [PhonenoCountryCode], [Phoneno], [ProfilePic], [AddressL1], [AddressL2], [City], [State], [ZipCode], [Country], [University], [College], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1, 9, CAST(N'2020-02-20T00:00:00.000' AS DateTime), 1, NULL, N'61', N'9090909090', NULL, N'Vikas Tenaments', N'Jivraj', N'Ahmedabad', N'Gujarat', N'367892', N'India', N'Nirma', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[UserProfile] ([ID], [UserID], [DOB], [Gender], [SecondaryEmailID], [PhonenoCountryCode], [Phoneno], [ProfilePic], [AddressL1], [AddressL2], [City], [State], [ZipCode], [Country], [University], [College], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (8, 10, NULL, 2, NULL, N'91', N'8978659840', N'~/ProfileImage/customer-121335357.png', N'Shilp Society', N'near prashant school', N'Gandhinagar', N'Gujarat', N'231456', N'Australia', N'GTU', N'VGEC', NULL, NULL, CAST(N'2021-04-13T20:48:53.200' AS DateTime), 10)
SET IDENTITY_INSERT [dbo].[UserProfile] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRoles] ON 

INSERT [dbo].[UserRoles] ([ID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [Modifiedby], [IsActive]) VALUES (1, N'Member', NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[UserRoles] ([ID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [Modifiedby], [IsActive]) VALUES (2, N'Admin', NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[UserRoles] ([ID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [Modifiedby], [IsActive]) VALUES (3, N'SuperAdmin', NULL, NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[UserRoles] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [ActivationCode]) VALUES (2, 3, N'Aadi', N'Dodiya', N'aadityaadodiya@gmail.com', N'A1812b@!', 1, NULL, NULL, NULL, NULL, 1, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [ActivationCode]) VALUES (9, 1, N'Niva', N'Dodiya', N'adityaniva18@gmail.com', N'Abc1010@', 1, NULL, NULL, NULL, NULL, 1, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [ActivationCode]) VALUES (10, 1, N'Hitu', N'Dodiya', N'ahdodiya99@gmail.com', N'Hitu12@', 1, NULL, NULL, NULL, NULL, 1, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [ActivationCode]) VALUES (11, 2, N'Juhi', N'Zanje', N'juhizanje@gmail.com', N'Juhi12@', 1, NULL, NULL, NULL, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [Email_Users]    Script Date: 14-04-2021 10:09:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Email_Users] ON [dbo].[Users]
(
	[EmailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Configurations] ADD  CONSTRAINT [DF_Configurations_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Countries] ADD  CONSTRAINT [DF_Countries_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Notes] ADD  CONSTRAINT [DF_Notes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NotesAttachments] ADD  CONSTRAINT [DF_NotesAttachments_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NotesReviews] ADD  CONSTRAINT [DF_NotesReviews_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ReferenceData] ADD  CONSTRAINT [DF_ReferenceData_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Types] ADD  CONSTRAINT [DF_Types_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [DF_UserRoles_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsEmailVerified]  DEFAULT ((0)) FOR [IsEmailVerified]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Downloads]  WITH CHECK ADD  CONSTRAINT [FK_Downloads_Notes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Notes] ([ID])
GO
ALTER TABLE [dbo].[Downloads] CHECK CONSTRAINT [FK_Downloads_Notes]
GO
ALTER TABLE [dbo].[Downloads]  WITH CHECK ADD  CONSTRAINT [FK_Downloads_Users] FOREIGN KEY([Seller])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Downloads] CHECK CONSTRAINT [FK_Downloads_Users]
GO
ALTER TABLE [dbo].[Downloads]  WITH CHECK ADD  CONSTRAINT [FK_Downloads_Users1] FOREIGN KEY([Downloader])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Downloads] CHECK CONSTRAINT [FK_Downloads_Users1]
GO
ALTER TABLE [dbo].[Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
GO
ALTER TABLE [dbo].[Notes] CHECK CONSTRAINT [FK_Notes_Categories]
GO
ALTER TABLE [dbo].[Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_Countries] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Countries] ([CountryID])
GO
ALTER TABLE [dbo].[Notes] CHECK CONSTRAINT [FK_Notes_Countries]
GO
ALTER TABLE [dbo].[Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_ReferenceData] FOREIGN KEY([Status])
REFERENCES [dbo].[ReferenceData] ([ID])
GO
ALTER TABLE [dbo].[Notes] CHECK CONSTRAINT [FK_Notes_ReferenceData]
GO
ALTER TABLE [dbo].[Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_Types] FOREIGN KEY([TypeID])
REFERENCES [dbo].[Types] ([TypeID])
GO
ALTER TABLE [dbo].[Notes] CHECK CONSTRAINT [FK_Notes_Types]
GO
ALTER TABLE [dbo].[Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_Users] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Notes] CHECK CONSTRAINT [FK_Notes_Users]
GO
ALTER TABLE [dbo].[Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_Users_ActionBy] FOREIGN KEY([ActionedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Notes] CHECK CONSTRAINT [FK_Notes_Users_ActionBy]
GO
ALTER TABLE [dbo].[NotesAttachments]  WITH CHECK ADD  CONSTRAINT [FK_NotesAttachments_Notes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Notes] ([ID])
GO
ALTER TABLE [dbo].[NotesAttachments] CHECK CONSTRAINT [FK_NotesAttachments_Notes]
GO
ALTER TABLE [dbo].[NotesReviews]  WITH CHECK ADD  CONSTRAINT [FK_NotesReviews_Downloads] FOREIGN KEY([DownloadID])
REFERENCES [dbo].[Downloads] ([ID])
GO
ALTER TABLE [dbo].[NotesReviews] CHECK CONSTRAINT [FK_NotesReviews_Downloads]
GO
ALTER TABLE [dbo].[NotesReviews]  WITH CHECK ADD  CONSTRAINT [FK_NotesReviews_Notes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Notes] ([ID])
GO
ALTER TABLE [dbo].[NotesReviews] CHECK CONSTRAINT [FK_NotesReviews_Notes]
GO
ALTER TABLE [dbo].[NotesReviews]  WITH CHECK ADD  CONSTRAINT [FK_NotesReviews_Users] FOREIGN KEY([ReviewedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[NotesReviews] CHECK CONSTRAINT [FK_NotesReviews_Users]
GO
ALTER TABLE [dbo].[ReportedIssues]  WITH CHECK ADD  CONSTRAINT [FK_ReportedIssues_Downloads] FOREIGN KEY([DownloadID])
REFERENCES [dbo].[Downloads] ([ID])
GO
ALTER TABLE [dbo].[ReportedIssues] CHECK CONSTRAINT [FK_ReportedIssues_Downloads]
GO
ALTER TABLE [dbo].[ReportedIssues]  WITH CHECK ADD  CONSTRAINT [FK_ReportedIssues_Notes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Notes] ([ID])
GO
ALTER TABLE [dbo].[ReportedIssues] CHECK CONSTRAINT [FK_ReportedIssues_Notes]
GO
ALTER TABLE [dbo].[ReportedIssues]  WITH CHECK ADD  CONSTRAINT [FK_ReportedIssues_Users] FOREIGN KEY([ReportedByID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[ReportedIssues] CHECK CONSTRAINT [FK_ReportedIssues_Users]
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD  CONSTRAINT [FK_UserProfile_ReferenceData] FOREIGN KEY([Gender])
REFERENCES [dbo].[ReferenceData] ([ID])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_ReferenceData]
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD  CONSTRAINT [FK_UserProfile_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserRoles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[UserRoles] ([ID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserRoles]
GO
USE [master]
GO
ALTER DATABASE [NotesMarket] SET  READ_WRITE 
GO
