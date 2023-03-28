USE eCommerce
GO

--CREATE TYPE CartItemsTableType AS TABLE
--(
--   ProductId UNIQUEIDENTIFIER,
--   Quantity INT
--);
--GO


alter PROC sp_Shoppings
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId							UNIQUEIDENTIFIER	=		NULL,
@Total							DECIMAL(18, 2)		=		NULL,
-----------------------------------------------------------------
@ShoppingId						UNIQUEIDENTIFIER	=		NULL,
@ProductId						UNIQUEIDENTIFIER	=		NULL,
@Quantity                       INT                 =       NULL,
-----------------------------------------------------------------
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
-----------------------------------------------------------------
@CartItems                      CartItemsTableType READONLY,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =        NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'CREATE_SHOPPING'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		BEGIN
			-- CREATE SHOPPING
			INSERT INTO Shopping (Id, [UserId], Total, Created)
			VALUES (@Id, @UserId, 0, GETDATE());

			-- LOOP CART ITEM
			DECLARE @RowCount INT = (SELECT COUNT(1) FROM @CartItems);
			IF @RowCount > 0
				BEGIN
					DECLARE @Index INT = 1;
					WHILE @Index <= @RowCount
						BEGIN
							DECLARE @Price DECIMAL(18, 2);

							-- GET @ProductId, @Quantity
							SELECT @ProductId = ProductId, @Quantity = Quantity
							FROM @CartItems
							ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
							
							-- CREATE CartItem
							INSERT INTO CartItem (Id, ShoppingId, ProductId, Quantity, Created)
							VALUES (NEWID(), @Id, @ProductId, @Quantity, GETDATE())
							SET @Index = @Index + 1;
						END
				END
			COMMIT TRANSACTION
		END
	END TRY
	BEGIN CATCH
		SELECT 
			@ErrorMessage =  ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
IF @Activity = 'ADD_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY

		IF EXISTS (SELECT * FROM CartItem WHERE ShoppingId = @Id AND ProductId = @ProductId)
			BEGIN
				-- Update Cart Item
				UPDATE CartItem SET Quantity = @Quantity WHERE ShoppingId = @Id AND ProductId = @ProductId
			END
		ELSE
			BEGIN
				INSERT INTO CartItem (Id, ShoppingId, ProductId, Quantity, Created)
				VALUES (NEWID(), @Id, @ProductId, @Quantity, GETDATE())
			END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT 
			@ErrorMessage =  ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		-- Update Cart Item
		UPDATE CartItem SET Quantity = @Quantity WHERE ShoppingId = @ShoppingId AND ProductId = @ProductId AND Id = @Id;
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT 
			@ErrorMessage =  ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		-- Update Cart Item
		DELETE CartItem WHERE  Id = @Id
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT 
			@ErrorMessage =  ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_CART_ITEM_BY_ID'
BEGIN
	SELECT * FROM CartItem WHERE Id = @Id;
END

-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_SHOPPING_BY_USER_ID'
BEGIN
	SELECT * FROM Shopping WHERE UserId = @UserId;
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_SHOPPING_DETAILS_BY_USER_ID'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		BEGIN
			IF NOT EXISTS (SELECT * FROM Shopping WHERE UserId = @UserId)
			BEGIN
				SELECT 
					@ErrorMessage = 'User is cart does not exist', -- Sản phẩm trong purchase order không tồn tại
					@ErrorSeverity = ERROR_SEVERITY(),
					@ErrorState = ERROR_STATE();
				RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
			END

			DECLARE @CountCartItem INT = (SELECT COUNT(1) FROM CartItem);
			DECLARE @Index_ INT = 1;
			DECLARE @Price_ DECIMAL(18, 2);

			WHILE @Index_ <= @CountCartItem
				BEGIN
					-- GET @ProductId, @Quantity 
					SELECT @ProductId = ProductId, @Quantity = Quantity
					FROM CartItem
					ORDER BY (SELECT NULL) OFFSET @Index_-1 ROWS FETCH NEXT 1 ROWS ONLY;
			
					-- GET Price Product
					SELECT @Price_ = Price FROM Product WHERE Id = @ProductId

					-- SET Total
					SET @Total = COALESCE(@Total, 0) + @Quantity * @Price_;

					SET @Index_ = @Index_ + 1;
				END


			SELECT s.Id, s.UserId, @Total as Total, Created, Modified,
			(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = s.UserId FOR JSON PATH), '$[0]')) AS _User,
			(SELECT *, (SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS s WHERE s.Id = c.ProductId FOR JSON PATH), '$[0]')) AS _Product
			FROM CartItem AS c WHERE c.ShoppingId = s.Id FOR JSON PATH) AS _CartItems
			FROM Shopping (NOLOCK) AS s
			WHERE UserId = @UserId
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT 
			@ErrorMessage =  ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END

