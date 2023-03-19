USE Auth
GO

-- CREATE STORED PROCEDURE INSERT USER
CREATE PROCEDURE [dbo].[sp_InsertUser]
(
	@Id UNIQUEIDENTIFIER                  =           NULL,
	@Username NVARCHAR(256)               =           NULL,
	@Fullname NVARCHAR(512)               =           NULL,
	@Email NVARCHAR(256)                  =           NULL,
	@EmailConfirmed BIT                   =           NULL,
	@PasswordHash NVARCHAR (MAX)          =           NULL,
	@PhoneNumber NVARCHAR (50)            =           NULL,
	@Avatar NVARCHAR (MAX)                =           NULL,
	@TotalAmountOwed DECIMAL              =           NULL,
	@UserAddressId UNIQUEIDENTIFIER       =           NULL,
	@Created DATETIME                     =           NULL,
	@IsDeleted BIT                        =           NULL

)
AS
BEGIN
	INSERT INTO [User]
	(
		[Id],
		[UserName], 
		[Fullname], 
		[Email],
		[EmailConfirmed],
		[PasswordHash],
		[PhoneNumber], 
		[Avatar], 
		[TotalAmountOwed],
		[UserAddressId],
		[Created],
		[IsDeleted]
	)
	VALUES
	(
		@Id,
		@Username,
		@Fullname,
		@Email,
		@EmailConfirmed,
		@PasswordHash,
		@PhoneNumber,
		@Avatar,
		@TotalAmountOwed,
		@UserAddressId,
		GETDATE(),
		@IsDeleted
	)
END
GO


-- CREATE STORED PROCEDURE sp_UpdateUser
CREATE PROCEDURE [dbo].[sp_UpdateUser]
(
	@Id UNIQUEIDENTIFIER                  =           NULL,
	@Username NVARCHAR(256)               =           NULL,
	@Fullname NVARCHAR(512)               =           NULL,
	@Email NVARCHAR(256)                  =           NULL,
	@EmailConfirmed BIT                   =           NULL,
	@PasswordHash NVARCHAR (MAX)          =           NULL,
	@PhoneNumber NVARCHAR (50)            =           NULL,
	@Avatar NVARCHAR (MAX)                =           NULL,
	@TotalAmountOwed DECIMAL              =           NULL,
	@UserAddressId UNIQUEIDENTIFIER       =           NULL,
	@IsDeleted BIT                        =           NULL
)
AS
BEGIN
	
UPDATE [User] SET
	[Username] = ISNULL(@Username, [Username]),
	[Fullname] = ISNULL(@Fullname, [Fullname]),
    [Email] = ISNULL(@Email, [Email]),
    [EmailConfirmed] = ISNULL(@EmailConfirmed, [EmailConfirmed]),
    [PasswordHash] = ISNULL(@PasswordHash, [PasswordHash]),
    [PhoneNumber] = ISNULL(@PhoneNumber, [PhoneNumber]),
    [Avatar] = ISNULL(@Avatar, [Avatar]),
    [TotalAmountOwed] = ISNULL(@TotalAmountOwed, [TotalAmountOwed]),
    [UserAddressId] = ISNULL(@UserAddressId, [UserAddressId]),
	[IsDeleted] = ISNULL(@IsDeleted, [IsDeleted])
WHERE Id = @Id

END
GO

-- CREATE STORED PROCEDURE sp_DeleteUser
CREATE PROCEDURE [dbo].[sp_DeleteUser]
	
@Id UNIQUEIDENTIFIER

AS
BEGIN
	UPDATE [User] SET [IsDeleted] = 1 WHERE [Id] = @Id
END
GO

-- CREATE STORED PROCEDURE sp_AddUserToRole
CREATE PROC [dbo].[sp_AddUserToRole]
@RoleId UNIQUEIDENTIFIER,
@UserId UNIQUEIDENTIFIER

AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM [UserRole]
		WHERE [UserId] = @UserId
		AND [RoleId] = @RoleId
	)
	INSERT INTO [UserRole] ([UserId], [RoleId])
	VALUES (@UserId, @RoleId)
END

GO
-- CREATE STORED PROCEDURE sp_CheckDuplicateUser
CREATE PROCEDURE [dbo].[sp_CheckDuplicateUser]
(
	@Id UNIQUEIDENTIFIER,
	@Username NVARCHAR(256),
	@Email NVARCHAR(256),
	@PhoneNumber NVARCHAR (50)
)
AS
BEGIN
	SELECT TOP 1 1
	FROM [User] (NOLOCK)
	WHERE  (@Username IS NULL OR [Username] = @Username) 
	AND  (@Email IS NULL OR [Email] = @Email) 
	AND  (@PhoneNumber IS NULL OR [PhoneNumber] = @PhoneNumber) 
	AND (@Id IS NULL OR Id <> @Id)
	AND [IsDeleted] = 0
END

GO
-- CREATE STORED PROCEDURE sp_FindByEmail
CREATE PROCEDURE [dbo].[sp_FindByEmail]
(
	@Email NVARCHAR(256)
)
AS
BEGIN
	
SELECT * FROM [dbo].[User]
WHERE [Email] = @Email
END

GO
-- CREATE STORED PROCEDURE sp_FindById
CREATE PROCEDURE [dbo].[sp_FindById]
(
	@Id UNIQUEIDENTIFIER
)
AS
BEGIN
	
SELECT * FROM [dbo].[User]
WHERE Id = @Id

END

GO
-- CREATE STORED PROCEDURE sp_FindByName
CREATE PROCEDURE [dbo].[sp_FindByName]
(
	@Username  nvarchar(256)
)
AS
BEGIN
	
SELECT * FROM [dbo].[User]
WHERE [Username] = @Username
END

GO
-- CREATE STORED PROCEDURE sp_GetUsersInRoleByRoleName
CREATE PROCEDURE [dbo].[sp_GetUsersInRoleByRoleName]
(
	@RoleName NVARCHAR(256)
)

AS
BEGIN

SELECT u.* FROM [User] u
INNER JOIN [UserRole] ur ON ur.[UserId] = u.[Id] 
INNER JOIN [Role] r ON r.[Id] = ur.[RoleId] 
WHERE r.[Name] = @RoleName

END

GO
-- CREATE STORED PROCEDURE sp_GetUserRoleByUserId
CREATE PROCEDURE [dbo].[sp_GetUserRolesByUserId]
(
	@UserId UNIQUEIDENTIFIER
)
AS
BEGIN

SELECT r.[Id] ,r.[Name], r.[Description], r.Created FROM [Role] r 
INNER JOIN [UserRole] ur ON ur.[RoleId] = r.Id 
WHERE ur.UserId = @UserId

END

GO

-- CREATE STORED PROCEDURE sp_GetAllUsers
CREATE PROCEDURE [dbo].[sp_GetDetailsUser]
@Id UNIQUEIDENTIFIER
AS
BEGIN
	SELECT u.Id, u.Username, u.Fullname, u.Email, u.PhoneNumber, u.Avatar, u.TotalAmountOwed, u.UserAddressId, u.Created, u.IsDeleted,
	(SELECT r.Id, r.[Name], r.[Description] FROM [Role]  AS r 
	INNER JOIN [UserRole] AS ur ON ur.RoleId = r.Id
	WHERE ur.UserId = u.Id
	FOR JSON PATH
	) AS ListObjectRole
	FROM [User] AS u
	WHERE u.Id = @Id
END


GO
-- CREATE STORED PROCEDURE sp_GetAllUsers
CREATE PROCEDURE [dbo].[sp_GetAllUsers]
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL
AS
BEGIN
	;WITH UserTemp AS(
		SELECT * 
		FROM [User](NOLOCK) u
		WHERE (@SearchString IS NULL OR u.[Fullname] LIKE N'%'+@SearchString+'%' 
		OR u.[Username] LIKE N'%'+@SearchString+'%' 
		OR  u.[Email] LIKE N'%'+@SearchString+'%'
		OR  u.[PhoneNumber] LIKE N'%'+@SearchString+'%') 
	)

	SELECT * 
	FROM UserTemp AS ut
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM UserTemp
	) as RecordCount
	INNER JOIN [User] (NOLOCK) u ON u.Id = ut.Id
	ORDER BY u.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY

END
GO

-- CREATE STORED PROCEDURE sp_RemoveUserFromRole
CREATE PROCEDURE [dbo].[sp_RemoveUserFromRole]
	
@RoleId UNIQUEIDENTIFIER,
@UserId UNIQUEIDENTIFIER

AS
BEGIN

DELETE FROM [UserRole]
WHERE [UserId] = @UserId
AND [RoleId] = @RoleId

END
GO

-- CREATE STORED PROCEDURE sp_CheckDuplicateRole
CREATE PROCEDURE [dbo].[sp_CheckDuplicateRole]
(
	@Id UNIQUEIDENTIFIER,
	@Name NVARCHAR(256)

)
AS
BEGIN
	SELECT TOP 1 1
	FROM [Role] (NOLOCK)
	WHERE  (@Name IS NULL OR [Name] = @Name) 
	AND (@Id IS NULL OR Id <> @Id)
END
GO
-- CREATE STORED PROCEDURE sp_InsertRole

CREATE PROCEDURE [dbo].[sp_InsertRole]
(
	@Name NVARCHAR(256),
	@Description NVARCHAR(256),
	@Created DATETIME
)

AS
BEGIN
	INSERT INTO [Role]
	(
		[Name],
		[Description],
		[Created]
	)
	VALUES
	(
		@Name,
		@Description,
		@Created
	)
	SELECT CAST(SCOPE_IDENTITY() as int)
END
GO
-- CREATE STORED PROCEDURE sp_UpdateRole
CREATE PROCEDURE [dbo].[sp_UpdateRole]
(
	@Id UNIQUEIDENTIFIER,
	@Name NVARCHAR(256),
	@Description NVARCHAR(256)
)

AS
BEGIN
	
UPDATE [Role] SET

    [Name] = @Name,
    [Description] = @Description

WHERE Id = @Id

END
GO
-- CREATE STORED PROCEDURE sp_DeleteRole
CREATE PROCEDURE [dbo].[sp_DeleteRole]
(
	@Id UNIQUEIDENTIFIER
)
AS
BEGIN
	DELETE FROM [UserRole]
	WHERE [RoleId] = @Id

	DELETE FROM [Role]
	WHERE [Id] = @Id
END
GO
-- CREATE STORED PROCEDURE sp_FindRoleById
CREATE PROCEDURE [dbo].[sp_FindRoleById]
(
	@Id UNIQUEIDENTIFIER
)
AS
BEGIN
	
SELECT TOP 1 * FROM [dbo].[Role]
WHERE [Id] = @Id

END
GO
-- CREATE STORED PROCEDURE sp_FindRoleByName
CREATE PROCEDURE [dbo].[sp_FindRoleByName]
(
	@RoleName NVARCHAR(256)
)
AS
BEGIN
	
SELECT TOP 1 * FROM [dbo].[Role]
WHERE [Name] = @RoleName
END
GO

-- CREATE STORED PROCEDURE sp_FindRoleByName
CREATE PROCEDURE [dbo].[sp_GetAllRole]

AS
BEGIN
	
SELECT * FROM [dbo].[Role]

END
GO

