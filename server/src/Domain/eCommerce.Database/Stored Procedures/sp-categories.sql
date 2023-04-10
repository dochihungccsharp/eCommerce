USE eCommerce
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
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
@ParentId                       UNIQUEIDENTIFIER    =       NULL,
@ListId							VARCHAR(MAX)        =       NULL
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO Category (Id, [Name], [Description], ImageUrl, [Status], Created, IsDeleted, ParentId) 
	VALUES (@Id, @Name, @Description, @ImageUrl, 1, GETDATE(), 0, @ParentId)
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
		Modified = GETDATE(),
		ParentId = ISNULL(@ParentId, ParentId)
	WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE Category SET IsDeleted = 1 WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS'
BEGIN
	UPDATE Category SET [Status] = ~[Status] WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM Category (NOLOCK)
	WHERE [Name] = @Name AND (@Id IS NULL OR Id <> @Id) AND IsDeleted = 0
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
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.Created, c.Modified, c.ParentId, 
	(SELECT JSON_QUERY((SELECT TOP(1) Id, [Name], [Description], ImageUrl, [Status] FROM Category AS ct WHERE ct.Id = c.ParentId FOR JSON PATH), '$[0]')) AS _Category
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
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, C.[Status], c.Created, RecordCount.TotalRows as TotalRows
	FROM CategoriesTemp AS ct 
		CROSS JOIN 
		(
			SELECT COUNT(*) AS TotalRows
			FROM CategoriesTemp
		) as RecordCount
		INNER JOIN Category (NOLOCK) c ON c.Id = ct.Id
	ORDER BY c.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_ROOT'
BEGIN
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.Created, c.Modified, c.ParentId
	FROM Category AS c (NOLOCK)
	WHERE c.Id = @Id AND c.IsDeleted = 0 AND c.ParentId IS NULL
END
GO





