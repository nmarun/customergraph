USE [master]
GO
/****** Object:  Database [CustomerGraph]    Script Date: 8/14/2019 9:45:11 PM ******/
CREATE DATABASE [CustomerGraph]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CustomerGraph', FILENAME = N'C:\Users\arun_mahendrakar\CustomerGraph.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CustomerGraph_log', FILENAME = N'C:\Users\arun_mahendrakar\CustomerGraph_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [CustomerGraph] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CustomerGraph].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CustomerGraph] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CustomerGraph] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CustomerGraph] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CustomerGraph] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CustomerGraph] SET ARITHABORT OFF 
GO
ALTER DATABASE [CustomerGraph] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CustomerGraph] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CustomerGraph] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CustomerGraph] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CustomerGraph] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CustomerGraph] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CustomerGraph] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CustomerGraph] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CustomerGraph] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CustomerGraph] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CustomerGraph] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CustomerGraph] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CustomerGraph] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CustomerGraph] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CustomerGraph] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CustomerGraph] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CustomerGraph] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CustomerGraph] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CustomerGraph] SET  MULTI_USER 
GO
ALTER DATABASE [CustomerGraph] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CustomerGraph] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CustomerGraph] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CustomerGraph] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CustomerGraph] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CustomerGraph] SET QUERY_STORE = OFF
GO
USE [CustomerGraph]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [CustomerGraph]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 8/14/2019 9:45:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[AddressId] [int] NOT NULL,
	[AddressLineOne] [nvarchar](100) NOT NULL,
	[AddressLineTwo] [nvarchar](30) NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](3) NOT NULL,
	[ZipCode] [nvarchar](10) NOT NULL,
	[Country] [nvarchar](5) NOT NULL,
	[CustomerNumber] [int] NOT NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contact]    Script Date: 8/14/2019 9:45:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[ContactId] [int] NOT NULL,
	[AddressId] [int] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](5) NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContactMethod]    Script Date: 8/14/2019 9:45:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContactMethod](
	[ContactMethodId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[Type] [nvarchar](30) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[AreaCode] [int] NOT NULL,
	[Number] [int] NOT NULL,
 CONSTRAINT [PK_ContactMethod] PRIMARY KEY CLUSTERED 
(
	[ContactMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 8/14/2019 9:45:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerNumber] [int] NOT NULL,
	[BusinessUnitId] [int] NOT NULL,
	[SalesChannel] [nvarchar](10) NOT NULL,
	[Currency] [nvarchar](5) NOT NULL,
	[Status] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Customer] FOREIGN KEY([CustomerNumber])
REFERENCES [dbo].[Customer] ([CustomerNumber])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Customer]
GO
ALTER TABLE [dbo].[Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([AddressId])
GO
ALTER TABLE [dbo].[Contact] CHECK CONSTRAINT [FK_Contact_Address]
GO
ALTER TABLE [dbo].[ContactMethod]  WITH CHECK ADD  CONSTRAINT [FK_ContactMethod_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[ContactMethod] CHECK CONSTRAINT [FK_ContactMethod_Contact]
GO
USE [master]
GO
ALTER DATABASE [CustomerGraph] SET  READ_WRITE 
GO