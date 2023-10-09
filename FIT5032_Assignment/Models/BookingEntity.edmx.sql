
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/07/2023 16:54:13
-- Generated from EDMX file: D:\5032\FIT5032_Assignment\FIT5032_Assignment\Models\BookingEntity.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [aspnet-FIT5032_Assignment-20230914095023];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_BookingAspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BookingSet] DROP CONSTRAINT [FK_BookingAspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetRolesAspNetUsers_AspNetRoles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetRolesAspNetUsers] DROP CONSTRAINT [FK_AspNetRolesAspNetUsers_AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetRolesAspNetUsers_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetRolesAspNetUsers] DROP CONSTRAINT [FK_AspNetRolesAspNetUsers_AspNetUsers];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[BookingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BookingSet];
GO
IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetRolesAspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRolesAspNetUsers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'BookingSet'
CREATE TABLE [dbo].[BookingSet] (
    [BookingId] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [BookingDate] nvarchar(max)  NOT NULL,
    [AspNetUsersId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'RatingSet'
CREATE TABLE [dbo].[RatingSet] (
    [RatingId] int IDENTITY(1,1) NOT NULL,
    [Score] nvarchar(max)  NOT NULL,
    [Comment] nvarchar(max)  NOT NULL,
    [AspNetUsersIdDoctor] nvarchar(128)  NOT NULL,
    [AspNetUsersIdPatient] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetRolesAspNetUsers'
CREATE TABLE [dbo].[AspNetRolesAspNetUsers] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [BookingId] in table 'BookingSet'
ALTER TABLE [dbo].[BookingSet]
ADD CONSTRAINT [PK_BookingSet]
    PRIMARY KEY CLUSTERED ([BookingId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [RatingId] in table 'RatingSet'
ALTER TABLE [dbo].[RatingSet]
ADD CONSTRAINT [PK_RatingSet]
    PRIMARY KEY CLUSTERED ([RatingId] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetRolesAspNetUsers'
ALTER TABLE [dbo].[AspNetRolesAspNetUsers]
ADD CONSTRAINT [PK_AspNetRolesAspNetUsers]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AspNetUsersId] in table 'BookingSet'
ALTER TABLE [dbo].[BookingSet]
ADD CONSTRAINT [FK_BookingAspNetUsers]
    FOREIGN KEY ([AspNetUsersId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BookingAspNetUsers'
CREATE INDEX [IX_FK_BookingAspNetUsers]
ON [dbo].[BookingSet]
    ([AspNetUsersId]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetRolesAspNetUsers'
ALTER TABLE [dbo].[AspNetRolesAspNetUsers]
ADD CONSTRAINT [FK_AspNetRolesAspNetUsers_AspNetRoles]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetRolesAspNetUsers'
ALTER TABLE [dbo].[AspNetRolesAspNetUsers]
ADD CONSTRAINT [FK_AspNetRolesAspNetUsers_AspNetUsers]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetRolesAspNetUsers_AspNetUsers'
CREATE INDEX [IX_FK_AspNetRolesAspNetUsers_AspNetUsers]
ON [dbo].[AspNetRolesAspNetUsers]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [AspNetUsersIdDoctor] in table 'RatingSet'
ALTER TABLE [dbo].[RatingSet]
ADD CONSTRAINT [FK_RatingAspNetUsers]
    FOREIGN KEY ([AspNetUsersIdDoctor])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RatingAspNetUsers'
CREATE INDEX [IX_FK_RatingAspNetUsers]
ON [dbo].[RatingSet]
    ([AspNetUsersIdDoctor]);
GO

-- Creating foreign key on [AspNetUsersIdPatient] in table 'RatingSet'
ALTER TABLE [dbo].[RatingSet]
ADD CONSTRAINT [FK_RatingAspNetUsers1]
    FOREIGN KEY ([AspNetUsersIdPatient])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RatingAspNetUsers1'
CREATE INDEX [IX_FK_RatingAspNetUsers1]
ON [dbo].[RatingSet]
    ([AspNetUsersIdPatient]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------