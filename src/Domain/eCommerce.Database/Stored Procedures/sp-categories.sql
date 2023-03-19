USE eCommerce
GO

/****** Object:  StoredProcedure [dbo].[sp_Category]    Script Date: 2/21/2023 11:11:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[sp_Categories]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@Description					NVARCHAR(255)		=		NULL,
@ImageUrl                       NVARCHAR(MAX)       =       NULL,
@Status                         BIT                 =       NULL,
@CreatedTime                    DATETIME            =       NULL,
@CreatorId                      UNIQUEIDENTIFIER    =       NULL,
@ModifiedTime                   DATETIME            =       NULL,
@ModifierId                     UNIQUEIDENTIFIER    =       NULL,
@IsDeleted						BIT                 =       0,
@CategoryParentId               UNIQUEIDENTIFIER    =       NULL,
@ListId							VARCHAR(MAX)        =       NULL
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO Category (Id, [Name], [Description], ImageUrl, [Status], CreatedTime, CreatorId, IsDeleted, CategoryParentId) 
	VALUES (@Id, @Name, @Description, @ImageUrl, 1, GETDATE(), @CreatorId, 0, @CategoryParentId)
END

-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	UPDATE Category
	SET 
		[Name] = ISNULL(@Name, [Name]),
		[Description] = ISNULL(@Description, [Description]),
		ImageUrl = ISNULL(@ImageUrl, ImageUrl),
		[Status] = ISNULL(@Status, [Status]),
		ModifiedTime = GETDATE(),
		ModifierId = ISNULL(@ModifierId, ModifierId),
		CategoryParentId = ISNULL(@CategoryParentId, CategoryParentId)
	WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE Category SET IsDeleted = 1 WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS'
BEGIN
	UPDATE Category SET [Status] = ~[Status] WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM Category (NOLOCK)
	WHERE [Name] = @Name AND (@Id IS NULL OR Id <> @Id)
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT Id, [Name], [Description], ImageUrl, [Status]
	FROM Category AS Cate (NOLOCK)
	WHERE Cate.Id = @Id AND Cate.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.CreatedTime, c.CreatorId, c.ModifiedTime, c.ModifierId, c.CategoryParentId, 
	(SELECT JSON_QUERY((SELECT TOP(1) Id, [Name], [Description], ImageUrl, [Status] FROM Category AS ct WHERE ct.Id = c.CategoryParentId FOR JSON PATH), '$[0]')) AS ObjectCategoryParent
	FROM Category AS c (NOLOCK)
	WHERE c.Id = @Id AND c.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH CategoriesTemp AS (
		SELECT cate.Id
		FROM Category (NOLOCK) cate
		WHERE (@SearchString IS NULL OR @SearchString = '' OR cate.[Name] LIKE N'%'+@SearchString+'%' OR  cate.[Description] LIKE N'%'+@SearchString+'%') AND Cate.IsDeleted = 0
	)
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, C.[Status], RecordCount.TotalRows as TotalRows
	FROM CategoriesTemp AS ct 
		CROSS JOIN 
		(
			SELECT COUNT(*) AS TotalRows
			FROM CategoriesTemp
		) as RecordCount
		INNER JOIN Category (NOLOCK) c ON c.Id = ct.Id
	ORDER BY c.CreatedTime DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO




