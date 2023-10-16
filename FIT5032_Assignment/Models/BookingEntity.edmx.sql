
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/11/2023 01:01:36
-- Generated from EDMX file: D:\5032\FIT5032_Assignment\FIT5032_Assignment\Models\BookingEntity.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [aspnet-FIT5032_Assignment-20230914095023];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO


ALTER TABLE [dbo].[BookingSet]
ALTER COLUMN [BookingDate] Date NOT NULL;
