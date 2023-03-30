USE eCommerce
GO

CREATE TYPE CategoryProductExclusion AS TABLE
(
	CategoryId UNIQUEIDENTIFIER,
	ProductId UNIQUEIDENTIFIER
)
GO

CREATE PROC [dbo].[sp_CategoryDiscount]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId						    UNIQUEIDENTIFIER	=		NULL,
@CategoryId						UNIQUEIDENTIFIER	=		NULL,
@Code							NVARCHAR(150)		=		NULL,
@DiscountType		     		NVARCHAR(150)		=		NULL,
@DiscountValue					DECIMAL     		=		NULL,
@StartDate                      DATETIME            =       NULL,
@EndDate                        DATETIME            =       NULL,
@IsActive                       BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
-----------------------------------------------------------------
@CategoryProductExclusions CategoryProductExclusion READONLY,
-----------------------------------------------------------------
@RowCount						INT                 =       0,
@Index  						INT                 =       0,
@CateId						    UNIQUEIDENTIFIER	=		NULL,
@ProductId						UNIQUEIDENTIFIER	=		NULL,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =       NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------