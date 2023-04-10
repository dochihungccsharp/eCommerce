USE eCommerce
GO



CREATE TYPE CartItemsTableType AS TABLE
(
	Id UNIQUEIDENTIFIER
);
GO



ALTER PROC sp_Carts
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id					    		UNIQUEIDENTIFIER	=		NULL,
@UserId							UNIQUEIDENTIFIER	=		NULL,
@ProductId						UNIQUEIDENTIFIER	=		NULL,
@Quantity                       INT                 =       NULL,
-----------------------------------------------------------------
@CartItems                      CartItemsTableType      READONLY,
-----------------------------------------------------------------
@RowCount                       INT                 =       NULL,
@Index                          INT                 =       NULL,
@Price                          DECIMAL             =       NULL,
@Total                          DECIMAL             =       NULL


AS
-----------------------------------------------------------------
IF @Activity = 'ADD_OR_UPDATE_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		-- CHECK THE PRODUCT EXISTENCE
		IF NOT EXISTS (SELECT TOP 1 1 FROM Product WHERE Id = @ProductId)
			THROW 400000, 'Product in cart does not exist', 1


		-- ADD OR UPDATE CART ITEM
		IF EXISTS (SELECT TOP 1 1 FROM CartItem WHERE UserId = @UserId AND ProductId = @ProductId)
			BEGIN
				UPDATE CartItem SET Quantity = @Quantity, Modified = GETDATE() WHERE UserId = @UserId AND ProductId = @ProductId
			END
		ELSE
			BEGIN
				INSERT INTO CartItem (Id, UserId, ProductId, Quantity, Created)
				VALUES (NEWID(), @UserId, @ProductId, @Quantity, GETDATE())
			END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		;THROW 400000, 'Add or update cart fail', 1;
		ROLLBACK TRANSACTION
	END CATCH
END
--------------------------------------------
ELSE IF @Activity = 'REMOVE_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE Id = @Id OR (UserId = @UserId AND ProductId = @ProductId))
			BEGIN
				;THROW 400000, 'Product in cart does not exist', 1;
			END

		-- Remove Cart Item
		DELETE CartItem WHERE Id = @Id OR (UserId = @UserId AND ProductId = @ProductId)
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		;THROW 400000, 'Remove cart item fail', 1;
		ROLLBACK TRANSACTION
	END CATCH
END

--------------------------------------------
ELSE IF @Activity = 'REMOVE_CART_ITEMS'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		SET @RowCount = (SELECT COUNT(1) FROM @CartItems);
		SET @Index = 1;
		WHILE @Index <= @RowCount
			BEGIN
				
				SELECT @Id = Id
				FROM @CartItems
				ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

				EXEC sp_Carts @Activity = 'REMOVE_CART_ITEM', @Id = @Id

				SET @Index = @Index + 1;
			END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		;THROW 400000, 'Remove list cart item fail', 1;
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_CART_ITEM_BY_ID'
BEGIN
	SELECT * FROM CartItem WHERE Id = @Id;
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_CART_ITEMS_BY_USER_ID'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		BEGIN
			IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE UserId = @UserId)
				THROW 400000, 'User is cart does not exist', 1;

			SET @RowCount = (SELECT COUNT(1) FROM @CartItems);
			SET @Index = 1;

			WHILE @Index <= @RowCount
				BEGIN

					print @RowCount;
					print @Index;
					-- GET @Id OF @CartItems
					SELECT @Id = Id
					FROM @CartItems
					ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

					IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE Id = @Id)
						THROW 400000, 'Cart item does not exist', 1;
					
					-- GET @ProductId OF CartItem
					SELECT @ProductId = ProductId, @Quantity = Quantity FROM CartItem WHERE Id = @Id

					-- GET Price Product
					 SELECT @Price = Price FROM Product WHERE Id = @ProductId

					-- SET Total
					 SET @Total = COALESCE(@Total, 0) + @Quantity * @Price;

					 SET @Index = @Index + 1;
				END


			SELECT  @Total as Total,
			(SELECT *, (SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS s WHERE s.Id = c.ProductId FOR JSON PATH), '$[0]')) AS _Product
			FROM CartItem AS c WHERE UserId = @UserId AND Id IN (SELECT Id FROM @CartItems) FOR JSON PATH) AS _CartItems
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		DECLARE @Message NVARCHAR(MAX) =  ERROR_MESSAGE();
		;THROW 400000, @Message, 1;
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_CART_BY_USER_ID'
BEGIN
	IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE UserId = @UserId)
	BEGIN
		;THROW 99001, 'User is cart does not exist', 1;
	END

	SET @RowCount = (SELECT COUNT(1) FROM CartItem WHERE UserId = @UserId);
	SET @Index = 1;

	WHILE @Index <= @RowCount
		BEGIN
			-- GET @ProductId, @Quantity 
			SELECT @ProductId = ProductId, @Quantity = Quantity
			FROM CartItem
			ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
			
			-- GET Price Product
			SELECT @Price = Price FROM Product WHERE Id = @ProductId

			-- SET Total
			SET @Total = COALESCE(@Total, 0) + @Quantity * @Price;

			SET @Index = @Index + 1;
		END


	SELECT  @Total as Total,
	(SELECT *, (SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS s WHERE s.Id = c.ProductId FOR JSON PATH), '$[0]')) AS _Product
	FROM CartItem AS c WHERE UserId = @UserId FOR JSON PATH) AS _CartItems
END
GO


