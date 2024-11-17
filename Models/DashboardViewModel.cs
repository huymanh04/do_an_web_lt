using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace do_an_web.Models
{
    public class DashboardViewModel
    {

        public List<decimal> MonthlyRevenue { get; set; } // Doanh thu từng tháng
        public List<int> MonthlyOrders { get; set; } // Tổng đơn hàng từng tháng
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCategories1 { get; set; }
        public int Totaldanggiao { get; set; }
        public int Totaldahuy { get; set; }
        public int Totaldagiao { get; set; }
        public int Totalacp { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Product> Products { get; set; } // Danh sách sản phẩm
        public List<Category> Categories { get; set; } // Danh sách danh mục
        public List<Category1> Categories1 { get; set; } // Danh sách danh mục
    }
}