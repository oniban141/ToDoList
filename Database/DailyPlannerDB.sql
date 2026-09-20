-- DailyPlanner Database Script for SQL Server 2019
-- This script creates all necessary tables and initial data

USE [master]
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DailyPlannerDB')
BEGIN
    CREATE DATABASE [DailyPlannerDB]
    GO
    ALTER DATABASE [DailyPlannerDB] SET COMPATIBILITY_LEVEL = 150
    GO
    ALTER DATABASE [DailyPlannerDB] SET ANSI_NULL_DEFAULT ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET ANSI_NULLS ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET ANSI_PADDING ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET ANSI_WARNINGS ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET ARITHABORT ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET AUTO_CLOSE OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET AUTO_CREATE_STATISTICS ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET AUTO_SHRINK OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET AUTO_UPDATE_STATISTICS ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET CURSOR_CLOSE_ON_COMMIT OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET CURSOR_DEFAULT  GLOBAL
    GO
    ALTER DATABASE [DailyPlannerDB] SET CONCAT_NULL_YIELDS_NULL ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET NUMERIC_ROUNDABORT OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET QUOTED_IDENTIFIER ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET RECURSIVE_TRIGGERS ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET  DISABLE_BROKER
    GO
    ALTER DATABASE [DailyPlannerDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET DATE_CORRELATION_OPTIMIZATION ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET TRUSTWORTHY OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET ALLOW_SNAPSHOT_ISOLATION ON
    GO
    ALTER DATABASE [DailyPlannerDB] SET PARAMETERIZATION SIMPLE
    GO
    ALTER DATABASE [DailyPlannerDB] SET READ_COMMITTED_SNAPSHOT OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET HONOR_BROKER_PRIORITY OFF
    GO
    ALTER DATABASE [DailyPlannerDB] SET RECOVERY FULL
    GO
    ALTER DATABASE [DailyPlannerDB] SET  MULTI_USER
    GO
    ALTER DATABASE [DailyPlannerDB] SET PAGE_VERIFY CHECKSUM
    GO
    ALTER DATABASE [DailyPlannerDB] SET DB_CHAINING OFF
    GO
END
GO

USE [DailyPlannerDB]
GO

-- Drop tables if they exist (for clean reinstallation)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskTags]') AND type in (N'U'))
    DROP TABLE [dbo].[TaskTags]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reminders]') AND type in (N'U'))
    DROP TABLE [dbo].[Reminders]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tags]') AND type in (N'U'))
    DROP TABLE [dbo].[Tags]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notes]') AND type in (N'U'))
    DROP TABLE [dbo].[Notes]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Events]') AND type in (N'U'))
    DROP TABLE [dbo].[Events]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND type in (N'U'))
    DROP TABLE [dbo].[Tasks]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
    DROP TABLE [dbo].[Users]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Genders]') AND type in (N'U'))
    DROP TABLE [dbo].[Genders]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
    DROP TABLE [dbo].[Roles]
GO

-- Create Roles table
CREATE TABLE [dbo].[Roles] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Insert Roles data
INSERT INTO [dbo].[Roles] ([Name]) VALUES (N'Admin')
INSERT INTO [dbo].[Roles] ([Name]) VALUES (N'User')
GO

-- Create Genders table
CREATE TABLE [dbo].[Genders] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_Genders] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Insert Genders data
INSERT INTO [dbo].[Genders] ([Name]) VALUES (N'Male')
INSERT INTO [dbo].[Genders] ([Name]) VALUES (N'Female')
INSERT INTO [dbo].[Genders] ([Name]) VALUES (N'Other')
GO

-- Create Users table
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NULL,
    [RoleId] INT NOT NULL,
    [GenderId] INT NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Users_Username] UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [FK_Users_Genders] FOREIGN KEY ([GenderId]) REFERENCES [dbo].[Genders] ([Id])
)
GO

-- Create Tasks table
CREATE TABLE [dbo].[Tasks] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [DueDate] DATETIME NULL,
    [Priority] NVARCHAR(20) NULL,
    [Status] NVARCHAR(20) NULL,
    [UserId] INT NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tasks_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [CHK_Tasks_Priority] CHECK ([Priority] IN (N'Low', N'Medium', N'High')),
    CONSTRAINT [CHK_Tasks_Status] CHECK ([Status] IN (N'Todo', N'InProgress', N'Completed'))
)
GO

-- Create Events table
CREATE TABLE [dbo].[Events] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [Location] NVARCHAR(255) NULL,
    [UserId] INT NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Events_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
)
GO

-- Create Notes table
CREATE TABLE [dbo].[Notes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NULL,
    [Content] NVARCHAR(MAX) NULL,
    [UserId] INT NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL,
    CONSTRAINT [PK_Notes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Notes_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
)
GO

-- Create Tags table
CREATE TABLE [dbo].[Tags] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NULL,
    [Color] NVARCHAR(50) NULL,
    [UserId] INT NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tags_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
)
GO

-- Create TaskTags table (junction table for many-to-many relationship)
CREATE TABLE [dbo].[TaskTags] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [TaskId] INT NOT NULL,
    [TagId] INT NOT NULL,
    CONSTRAINT [PK_TaskTags] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TaskTags_Tasks] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([Id]),
    CONSTRAINT [FK_TaskTags_Tags] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tags] ([Id])
)
GO

-- Create Reminders table
CREATE TABLE [dbo].[Reminders] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [TaskId] INT NOT NULL,
    [ReminderTime] DATETIME NOT NULL,
    [Message] NVARCHAR(MAX) NULL,
    [IsActive] BIT DEFAULT 1,
    [UserId] INT NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [PK_Reminders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Reminders_Tasks] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([Id]),
    CONSTRAINT [FK_Reminders_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
)
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Tasks_UserId] ON [dbo].[Tasks] ([UserId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Tasks_Status] ON [dbo].[Tasks] ([Status] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Tasks_Priority] ON [dbo].[Tasks] ([Priority] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Tasks_DueDate] ON [dbo].[Tasks] ([DueDate] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Events_UserId] ON [dbo].[Events] ([UserId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Notes_UserId] ON [dbo].[Notes] ([UserId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Tags_UserId] ON [dbo].[Tags] ([UserId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Reminders_UserId] ON [dbo].[Reminders] ([UserId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Reminders_TaskId] ON [dbo].[Reminders] ([TaskId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Reminders_ReminderTime] ON [dbo].[Reminders] ([ReminderTime] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Reminders_IsActive] ON [dbo].[Reminders] ([IsActive] ASC)
GO

-- Insert sample data (optional)
-- Uncomment below to insert sample admin user
-- INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [Email], [RoleId], [GenderId]) 
-- VALUES (N'admin', N'5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', N'admin@example.com', 1, 1)
GO

PRINT 'DailyPlannerDB database created successfully!'
GO
