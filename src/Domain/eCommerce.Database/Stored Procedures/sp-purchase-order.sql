USE eCommerce
GO

/****** Object:  StoredProcedure [dbo].[sp_Suppliers]  ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROC [dbo].[sp_PurchaseOrder]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
@FromTime                       DATETIME            =       NULL,
@ToTime                         DATETIME            =       NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@SupplierId				        UNIQUEIDENTIFIER	=		NULL,
@TotalMoney						DECIMAL     		=		NULL,
@Note       					NVARCHAR(MAX)		=		NULL,
@OrderStatus                    NVARCHAR(20)        =       NULL,
@PaymentStatus                  NVARCHAR(20)        =       NULL,
@TotalPaymentAmount             DECIMAL(18, 2)      =       NULL,
@CreatedTime                    DATETIME            =       NULL,
@CreatorId                      UNIQUEIDENTIFIER    =       NULL,
@ModifiedTime                   DATETIME            =       NULL,
@ModifierId                     UNIQUEIDENTIFIER    =       NULL,
@IsDeleted						BIT                 =       0,
@PurchaseOrderDetails           PurchaseOrderDetailsTableType READONLY
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY

		BEGIN
			INSERT INTO PurchaseOrder (Id, SupplierId, TotalMoney, Note, OrderStatus, PaymentStatus, TotalPaymentAmount, CreatedTime, CreatorId)
			VALUES (@Id, @SupplierId, @TotalMoney, @Note, @OrderStatus, @PaymentStatus, @TotalPaymentAmount, GETDATE(), NULL)

			DECLARE @ProductId UNIQUEIDENTIFIER;
			DECLARE @Quantity INT;
			DECLARE @Price DECIMAL(18,2);
			DECLARE @Index INT = 1;
			DECLARE @RowCount INT = (SELECT COUNT(*) FROM @PurchaseOrderDetails);
			IF @RowCount > 0
			BEGIN
				WHILE @Index <= @RowCount
				BEGIN
					SELECT @ProductId = ProductId, @Quantity = Quantity, @Price = Price
					FROM @PurchaseOrderDetails
					ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

					BEGIN TRANSACTION -- thêm BEGIN TRANSACTION ở đây

					-- create purchase order detail
					INSERT INTO PurchaseOrderDetails (PurchaseOrderId, ProductId, Quantity, Price) 
					VALUES (@Id, @ProductId, @Quantity, @Price)

					-- update product inventory
					DECLARE @InventoryId UNIQUEIDENTIFIER;
					IF(@OrderStatus = 'PURCHASE_INVOICE')
					BEGIN
						SELECT @InventoryId = p.InventoryId FROM Product AS p WHERE p.Id = @ProductId
						IF(@InventoryId IS NULL)
						BEGIN
							-- CREATE INVENTORY 
							SET @InventoryId = NEWID()
							INSERT INTO Inventory (Id, Quantity)
							VALUES (@InventoryId, @Quantity)

							-- SET INVENTORYID FOR PRODUCT
							UPDATE Product SET InventoryId = @InventoryId WHERE Id = @ProductId
							DECLARE @CurrentQuantity INT;
							DECLARE @CurrentOriginalPrice DECIMAL(18, 2);
		
							-- UPDATE OriginalPrice FOR PRODUCT
							SELECT @CurrentQuantity = COALESCE(i.Quantity, 0), @CurrentOriginalPrice = p.OriginalPrice
							FROM Product AS p
							LEFT JOIN Inventory i ON i.Id = p.InventoryId
							
							UPDATE Product 
							SET OriginalPrice = (@CurrentQuantity * @CurrentOriginalPrice + @Quantity * @Price)/(@CurrentQuantity + @Quantity) 
							WHERE Id = @ProductId

						END
						ELSE
							BEGIN
								UPDATE Inventory SET Quantity = Quantity + @Quantity WHERE Id = @InventoryId
							END
					END
					ELSE IF(@OrderStatus != 'DRAFT_INVOICE')
						BEGIN
							RAISERROR('Error: No details provided', 16, 1)
							ROLLBACK TRANSACTION
						END

					SET @Index = @Index + 1;
					COMMIT TRANSACTION -- thêm COMMIT TRANSACTION ở đây
				END;
			END
			ELSE
			BEGIN
				RAISERROR('Error: No details provided', 16, 1)
				ROLLBACK TRANSACTION
			END
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		RAISERROR('Error: No details provided', 16, 1)
		ROLLBACK TRANSACTION
	END CATCH
END

ELSE IF @Activity = 'DELETE'
BEGIN
BEGIN TRANSACTION
	BEGIN TRY
		-- GET PurchaseOrder BY ID
		IF EXISTS (SELECT * FROM PurchaseOrder WHERE Id = @Id)
			BEGIN
				SELECT @OrderStatus = OrderStatus FROM PurchaseOrder WHERE Id = @Id;
				IF(@OrderStatus = 'DRAFT_INVOICE')
					BEGIN
						DELETE FROM PurchaseOrder WHERE Id = @Id;
						COMMIT TRANSACTION
					END	
			END
		ELSE
			BEGIN
				ROLLBACK TRANSACTION
			END
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END

ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT  po.Id, po.TotalMoney, po.Note, po.OrderStatus, po.CreatedTime, s.[Name] as SupplierName, u.Fullname as CreatorName
	FROM PurchaseOrder AS po
	LEFT JOIN Supplier (NOLOCK) s ON s.Id = po.SupplierId
	LEFT JOIN [User] (NOLOCK) u ON u.Id = po.CreatorId
	WHERE @Id = po.Id AND po.IsDeleted = 0
END

ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT po.Id, po.SupplierId, po.TotalMoney, po.Note, po.OrderStatus, po.CreatedTime, po.CreatorId, po.ModifiedTime, po.ModifierId,
	(SELECT JSON_QUERY((SELECT TOP(1) u.Id, u.Username, u.Email, u.Fullname, u.PhoneNumber FROM [User] AS u WHERE u.Id = po.CreatorId FOR JSON PATH), '$[0]')) AS ObjectUser,
	(SELECT JSON_QUERY((SELECT TOP(1) s.Id, s.[Name], s.[Description], s.[Address], s.[Phone], s.Email, s.ContactPerson FROM Supplier AS s WHERE s.Id = po.SupplierId FOR JSON PATH), '$[0]')) AS ObjectSupplier,
	
	(
		SELECT pod.PurchaseOrderId, pod.ProductId, pod.Price, pod.Quantity, p.[Name] AS ProductName
		FROM PurchaseOrderDetails AS pod 
		LEFT JOIN Product AS p ON p.Id = pod.ProductId
		WHERE pod.PurchaseOrderId = po.Id FOR JSON PATH
	) AS ListObjectPurchaseOrderDetails

	FROM PurchaseOrder AS po (NOLOCK)
	WHERE po.Id = @Id AND po.IsDeleted = 0
END

ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH PurchaseOrderTemp AS
	(
		SELECT po.Id
		FROM PurchaseOrder (NOLOCK) po
		LEFT JOIN [User] AS u ON u.Id = po.CreatorId
		LEFT JOIN [Supplier] AS s ON s.Id = po.SupplierId
 		WHERE (@SearchString IS NULL OR po.Note LIKE N'%'+@SearchString+'%' OR  s.[Name] LIKE N'%'+@SearchString+'%' OR  u.[Fullname] LIKE N'%'+@SearchString+'%')
		AND (@SupplierId IS NULL OR po.SupplierId = @SupplierId)
		AND (@CreatorId IS NULL OR po.CreatorId = @CreatorId)
		AND (@OrderStatus IS NULL OR po.OrderStatus = @OrderStatus)
		AND ((@FromTime IS NULL OR @ToTime IS NULL) OR (po.CreatedTime >= @FromTime AND po.CreatedTime <= @ToTime))
		AND po.IsDeleted = 0
	)

	SELECT po.Id, po.TotalMoney, po.Note, po.OrderStatus, po.CreatedTime, s.[Name] as SupplierName, u.Fullname as CreatorName,
	RecordCount.TotalRows as TotalRows
	FROM PurchaseOrderTemp AS pot
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM PurchaseOrderTemp
	) as RecordCount
	LEFT JOIN PurchaseOrder (NOLOCK) po ON po.Id = pot.Id
	LEFT JOIN Supplier (NOLOCK) s ON s.Id = po.SupplierId
	LEFT JOIN [User] (NOLOCK) u ON u.Id = po.CreatorId
	ORDER BY po.CreatedTime DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END


