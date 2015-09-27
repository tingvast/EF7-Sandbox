USE [EF7]
GO

/****** Object: Table Valued Function [dbo].[SearchBlogs] Script Date: 2015-09-27 17:05:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP FUNCTION [dbo].[SearchBlogs];


GO
CREATE FUNCTION [dbo].[SearchBlogs]
(
	@param2 nvarchar(200)
)
RETURNS @returntable TABLE
(
	Id int,
	Author nvarchar(200),
	Name nvarchar(200),
	Description nvarchar(200)
)
AS
BEGIN
	INSERT @returntable
	SELECT blogs.Id, blogs.Author, blogs.Name, blogs.Description 
	from dbo.Blog blogs 
	where blogs.Name like @param2

	RETURN
END
