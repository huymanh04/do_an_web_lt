using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace do_an_web.Models
{
    public class PurchaseHistoryViewModel
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPricePerProduct { get; set; }
        public List<PurchaseHistoryViewModel> PurchaseHistory { get; set; }
    }
}