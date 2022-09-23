DROP TABLE IF EXISTS [dbo].[Product]
GO

DROP TABLE IF EXISTS [dbo].[Category]
GO

CREATE TABLE [dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Description] [varchar](500) NULL,
 CONSTRAINT [PK_Category_CategoryId] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[QuantityPerUnit] [int] NOT NULL,
	[UnitPrice] [decimal](18, 4) NULL,
	[UnitsInStock] [decimal](18, 4) NULL,
	[UnitsOnOrder] [decimal](18, 4) NULL,
	[ReorderLevel] [decimal](18, 4) NULL,
	[Discontinued] [bit] NOT NULL,
	[CategoryId] [int] NULL,
	[ProductReason] [varchar](250) NULL,
 CONSTRAINT [PK_ProductId] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [Discontinued]
GO

ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
