SET IDENTITY_INSERT [dbo].[Category] ON 
INSERT [dbo].[Category] ([CategoryId], [Name], [Description]) VALUES (1, N'Phone', N'Phone Category')
INSERT [dbo].[Category] ([CategoryId], [Name], [Description]) VALUES (2, N'TV', N'TV Category')
SET IDENTITY_INSERT [dbo].[Category] OFF
GO

SET IDENTITY_INSERT [dbo].[Product] ON 
INSERT [dbo].[Product] ([ProductId], [Name], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued], [CategoryId], [ProductReason]) VALUES (1, N'iPhone', 1, CAST(1000.0000 AS Decimal(18, 4)), CAST(10.0000 AS Decimal(18, 4)), CAST(5.0000 AS Decimal(18, 4)), CAST(3.0000 AS Decimal(18, 4)), 0, 1, NULL)
INSERT [dbo].[Product] ([ProductId], [Name], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued], [CategoryId], [ProductReason]) VALUES (2, N'Samsung QLED TV', 1, CAST(1500.0000 AS Decimal(18, 4)), CAST(20.0000 AS Decimal(18, 4)), CAST(10.0000 AS Decimal(18, 4)), CAST(3.0000 AS Decimal(18, 4)), 0, 2, NULL)
INSERT [dbo].[Product] ([ProductId], [Name], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued], [CategoryId], [ProductReason]) VALUES (3, N'LG QLED TV', 1, CAST(1400.0000 AS Decimal(18, 4)), CAST(15.0000 AS Decimal(18, 4)), CAST(8.0000 AS Decimal(18, 4)), CAST(3.0000 AS Decimal(18, 4)), 0, 2, NULL)
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
