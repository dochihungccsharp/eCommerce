USE eCommerce
GO


ALTER PROC [dbo].[sp_UserAddresses]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId						    UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@DeliveryAddress                NVARCHAR(MAX)       =       NULL,
@Telephone   					NVARCHAR(20)		=		NULL,
@Active                         BIT                  =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'GET_USER_ADDRESSES_BY_USER_ID'
BEGIN
	SELECT * FROM UserAddress WHERE UserId = @UserId
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_USER_ADDRESS_BY_USER_ID'
BEGIN
	SELECT TOP(1) * FROM UserAddress WHERE UserId = @UserId AND Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CREATE_USER_ADDRESS'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF NOT EXISTS (SELECT TOP 1 1 FROM UserAddress AS ua (NOLOCK) WHERE ua.[Name] = @Name AND ua.Telephone = @Telephone AND ua.DeliveryAddress = @DeliveryAddress AND (@Id IS NULL OR Id <> @Id))
		THROW 400, 'Brand with the same name already exits.',  1

	IF(@Active = 1)
		BEGIN
			UPDATE UserAddress SET Active = 1 WHERE 1 = 1 AND UserId = @UserId
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END
	INSERT INTO UserAddress (Id, UserId, [Name], DeliveryAddress, Telephone, Active, Created)
	VALUES (@Id, @UserId, @Name, @DeliveryAddress, @Telephone, @Active, GETDATE())
	
	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	THROW 99001, 'Create user address fail', 1
	ROLLBACK TRANSACTION
END CATCH
END 
-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE_USER_ADDRESS'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF(@Active = 1)
		BEGIN
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END
	UPDATE UserAddress
	SET 
		UserId = ISNULL(@UserId, UserId),
		[Name] = ISNULL(@Name, [Name]),
		DeliveryAddress = ISNULL(@DeliveryAddress, DeliveryAddress),
		Telephone = ISNULL(@Telephone, Telephone),
		Active = ISNULL(@Active, Active),
		Modified = GETDATE()
	WHERE Id = @Id

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
	THROW 99001, 'update user address fail', 1
	ROLLBACK TRANSACTION
END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE_USER_ADDRESS'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	DELETE UserAddress WHERE Id = @Id AND UserId = @UserId
	
	SELECT TOP 1 @Id = Id FROM UserAddress 
	IF(@Id IS NULL)
		BEGIN
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END
	ELSE
		BEGIN
			UPDATE UserAddress SET Active = 1 WHERE Id = @Id
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END

	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	THROW 99001, 'delete user address fail', 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'SET_DEFAULT_ADDRESS_FOR_USER'
BEGIN
BEGIN TRANSACTION
BEGIN TRY	
	BEGIN
		UPDATE UserAddress SET Active = 1 WHERE Id = @Id
		UPDATE [User] SET UserAddressId = @Id WHERE Id = @UserId
		COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
	THROW 99001, 'set default user address for user fail', 1
	ROLLBACK TRANSACTION
END CATCH
END


	




