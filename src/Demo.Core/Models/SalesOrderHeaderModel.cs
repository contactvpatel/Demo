using Demo.Core.Entities.Base;
using Demo.Util.FIQL;
using System.Text.Json.Serialization;

namespace Demo.Core.Models
{
    public partial class SalesOrderHeaderModel
    {
        public int SalesOrderId { get; set; }
        public byte RevisionNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public byte Status { get; set; }
        public bool? OnlineOrderFlag { get; set; }
        public string SalesOrderNumber { get; set; } = null!;
        public string PurchaseOrderNumber { get; set; }
        public string AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public int? ShipToAddressId { get; set; }
        public int? BillToAddressId { get; set; }
        public string ShipMethod { get; set; } = null!;
        public string CreditCardApprovalCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal Freight { get; set; }
        public decimal TotalDue { get; set; }
        public string Comment { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        [JsonIgnore]
        public dynamic SalesOrderDetails { get; set; }
    }

    public class SalesOrderFilterModel
    {
        [FilterMapping("A.CustomerID")]
        public int CustomerId { get; set; }

        [FilterMapping("A.SalesOrderID")]
        public int SalesOrderId { get; set; }

        [FilterMapping("A.RevisionNumber")]
        public byte RevisionNumber { get; set; } // tinyint maps to byte

        [FilterMapping("A.OrderDate")]
        public DateTime OrderDate { get; set; }

        [FilterMapping("A.DueDate")]
        public DateTime DueDate { get; set; }

        [FilterMapping("A.ShipDate")]
        public DateTime? ShipDate { get; set; } // Nullable for potential nulls

        [FilterMapping("A.Status")]
        public byte Status { get; set; } // tinyint maps to byte

        [FilterMapping("A.OnlineOrderFlag")]
        public bool OnlineOrderFlag { get; set; } // Flag maps to bool

        [FilterMapping("A.SalesOrderNumber")]
        public string SalesOrderNumber { get; set; } = null!; // nvarchar

        [FilterMapping("A.PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; } = null!; // nvarchar

        [FilterMapping("A.AccountNumber")]
        public string AccountNumber { get; set; } = null!; // nvarchar

        [FilterMapping("A.ShipToAddressID")]
        public int ShipToAddressId { get; set; }

        [FilterMapping("A.BillToAddressID")]
        public int BillToAddressId { get; set; }

        [FilterMapping("A.ShipMethod")]
        public string ShipMethod { get; set; } = null!; // nvarchar

        [FilterMapping("A.CreditCardApprovalCode")]
        public string CreditCardApprovalCode { get; set; } = null!; // varchar

        [FilterMapping("A.SubTotal")]
        public decimal SubTotal { get; set; } // money maps to decimal

        [FilterMapping("A.TaxAmt")]
        public decimal TaxAmt { get; set; } // money maps to decimal

        [FilterMapping("A.Freight")]
        public decimal Freight { get; set; } // money maps to decimal

        [FilterMapping("A.TotalDue")]
        public decimal TotalDue { get; set; } // money maps to decimal

        [FilterMapping("A.Comment")]
        public string Comment { get; set; } = null!; // nvarchar

        // SalesOrderDetail properties
        [FilterMapping("B.SalesOrderDetailID")]
        public int SalesOrderDetailId { get; set; }

        [FilterMapping("B.OrderQty")]
        public short OrderQty { get; set; } // smallint maps to short

        [FilterMapping("B.ProductID")]
        public int ProductId { get; set; }

        [FilterMapping("B.UnitPrice")]
        public decimal UnitPrice { get; set; } // money maps to decimal

        [FilterMapping("B.UnitPriceDiscount")]
        public decimal UnitPriceDiscount { get; set; } // money maps to decimal

        [FilterMapping("B.LineTotal")]
        public decimal LineTotal { get; set; } // numeric maps to decimal

        // Product properties
        [FilterMapping("C.Name")]
        public string ProductName { get; set; } = null!; // nvarchar

        [FilterMapping("C.ProductNumber")]
        public string ProductNumber { get; set; } = null!; // nvarchar

        [FilterMapping("C.Color")]
        public string Color { get; set; } = null!; // nvarchar

        [FilterMapping("C.StandardCost")]
        public decimal StandardCost { get; set; } // money maps to decimal

        [FilterMapping("C.ListPrice")]
        public decimal ListPrice { get; set; } // money maps to decimal

        [FilterMapping("C.Size")]
        public string Size { get; set; } = null!; // nvarchar

        [FilterMapping("C.Weight")]
        public decimal Weight { get; set; } // decimal

        [FilterMapping("C.ProductCategoryID")]
        public int ProductCategoryId { get; set; }

        [FilterMapping("C.ProductModelID")]
        public int ProductModelId { get; set; }

        [FilterMapping("C.SellStartDate")]
        public DateTime? SellStartDate { get; set; } // Nullable for potential nulls

        [FilterMapping("C.SellEndDate")]
        public DateTime? SellEndDate { get; set; } // Nullable for potential nulls

        [FilterMapping("C.DiscontinuedDate")]
        public DateTime? DiscontinuedDate { get; set; } // Nullable for potential nulls

        // Category and Model properties
        [FilterMapping("D.ParentProductCategoryID")]
        public int? ParentProductCategoryId { get; set; } // Nullable for potential nulls

        [FilterMapping("D.Name")]
        public string ProductCategory { get; set; } = null!; // nvarchar

        [FilterMapping("E.Name")]
        public string ProductModel { get; set; } = null!; // nvarchar
    }

}
