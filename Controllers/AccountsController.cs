using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using do_an_web.Models;

namespace do_an_web.Controllers
{
    public class AccountsController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();
        public class RevenueByDate
        {
            public DateTime Date { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        public class RevenueByWeek
        {
            public int WeekNumber { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        public class RevenueByMonth
        {
            public int Month { get; set; }
            public decimal TotalRevenue { get; set; }
        }
        // GET: Accounts
  
        public ActionResult Index()
        {
            ViewBag.admin = Session["admin"];
            ViewBag.use = Session["username"];
            Homemol hm=new Homemol();
            hm.listacc=db.Accounts.ToList();
            return View(hm);
        }
  

        public ActionResult lichsudonhang(int pageNumber = 1, int pageSize = 8)
        {

            int accountId = int.Parse(Session["id"].ToString());
            // Lấy tổng số đơn hàng
            var totalRecords = db.Orders
                .Where(o => o.account_id == accountId)
                .Join(db.OrderItems, o => o.order_id, oi => oi.order_id, (o, oi) => new { o, oi })
                .Join(db.Products, o => o.oi.product_id, p => p.product_id, (o, p) => new { o.o, o.oi, p })
                .Count();

            // Lấy các đơn hàng theo phân trang
            var purchaseHistory = db.Orders
                .Where(o => o.account_id == accountId)  // Lọc theo accountId
                .Join(db.OrderItems, o => o.order_id, oi => oi.order_id, (o, oi) => new { o, oi })
                .Join(db.Products, o => o.oi.product_id, p => p.product_id, (o, p) => new PurchaseHistoryViewModel
                {
                    OrderId = o.o.order_id,
                    OrderDate = o.o.order_date ?? DateTime.MinValue,  // Kiểm tra nullable DateTime
                    TotalAmount = o.o.total_amount,
                    OrderStatus = o.o.status,
                    ProductId = p.product_id,
                    ProductName = p.product_name,
                    Quantity = o.oi.quantity,
                    ProductPrice = o.oi.price,
                    TotalPricePerProduct = o.oi.quantity * o.oi.price  // Tính tổng tiền cho sản phẩm
                })
                .OrderByDescending(p => p.OrderDate)  // Sắp xếp theo ngày đặt hàng mới nhất
                .Skip((pageNumber - 1) * pageSize)  // Bỏ qua các bản ghi trước trang hiện tại
                .Take(pageSize)  // Lấy số lượng bản ghi theo pageSize
                .ToList();

            var model = new PurchaseHistoryViewModel
            {
                PurchaseHistory = purchaseHistory,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };

            return View(model);
        }

        public ActionResult dashboard()
        {
            ViewBag.admin = Session["admin"];
            ViewBag.use = Session["username"];
            if (Session["admin"] != null)
            {
                var soucess = db.Orders.Where(c => c.status == "Completed").ToList();
                var custumer = db.Accounts.Where(c => c.idtype == 1).ToList();
                var chogiao = db.Orders.Where(c => c.status == "Shipped").ToList();
                var acp = db.Orders.Where(c => c.status == "Pending").ToList();
                var fall = db.Orders.Where(c => c.status == "Cancelled").ToList();
                var dashboardData = new DashboardViewModel
                {
                    Totaldagiao = soucess.Count(),
                    Totaldahuy = fall.Count(),
                    Totaldanggiao = chogiao.Count(),
                    TotalCategories1 = db.Category1.Count(),
                    TotalProducts = db.Products.Count(),
                    TotalCategories = db.Categories.Count(),
                    TotalCustomers = custumer.Count(),
                    Totalacp = acp.Count(),
                    TotalOrders = db.Orders.Count(),
                    TotalRevenue = db.Orders.Sum(o => o.total_amount), // Trực tiếp tính tổng nếu total_amount không nullable
                    Products = db.Products.ToList(),
                    Categories = db.Categories.ToList()
                };


                return View(dashboardData);
            }
            return RedirectToAction("Index","Products");
        }
        public JsonResult DailyRevenueAndOrders()
        {
            var data = db.Orders
                .Where(o => o.order_date.HasValue)  // Kiểm tra order_date có giá trị hay không
                .GroupBy(o => DbFunctions.TruncateTime(o.order_date.Value))  // Loại bỏ phần giờ, chỉ nhóm theo ngày
                .Select(g => new
                {
                    Date = g.Key,  // Trả về giá trị DateTime gốc (sẽ chuyển đổi sau khi truy vấn)
                    TotalRevenue = g.Sum(o => o.total_amount),  // Tổng doanh thu
                    TotalOrders = g.Count()  // Số lượng đơn hàng
                })
                .ToList()  // Thực thi truy vấn và lấy dữ liệu về bộ nhớ
                .Select(item => new
                {
                    Date = item.Date.HasValue ? item.Date.Value.ToString("yyyy-MM-dd") : null,  // Kiểm tra giá trị null trước khi gọi ToString
                    item.TotalRevenue,
                    item.TotalOrders
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DailyRevenue()
        {
            var dailyRevenue = db.Orders
                .GroupBy(o => DbFunctions.TruncateTime(o.order_date)) // TruncateTime để chỉ lấy ngày
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.OrderItems.Sum(oi => oi.quantity * oi.price))
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Json(dailyRevenue, JsonRequestBehavior.AllowGet);
        }

        // Doanh thu theo tháng
        public ActionResult MonthlyRevenue()
        {
            var monthlyRevenue = db.Orders
         .Where(o => o.order_date.HasValue)  // Lọc chỉ các đơn hàng có OrderDate
         .GroupBy(o => new { o.order_date.Value.Year, o.order_date.Value.Month })  // Truy cập Year và Month qua .Value
         .Select(g => new
         {
             Year = g.Key.Year,
             Month = g.Key.Month,
             TotalRevenue = g.Sum(o => o.OrderItems.Sum(oi => oi.quantity * oi.price))
         })
         .OrderBy(x => x.Year).ThenBy(x => x.Month)
         .ToList();

            return Json(monthlyRevenue, JsonRequestBehavior.AllowGet);
        }

        // Doanh thu theo tuần
        public ActionResult WeeklyRevenue()
        {
            var weeklyRevenue = db.Orders
                .GroupBy(o => System.Data.Entity.SqlServer.SqlFunctions.DatePart("week", o.order_date))
                .Select(g => new
                {
                    Week = g.Key,
                    TotalRevenue = g.Sum(o => o.OrderItems.Sum(oi => oi.quantity * oi.price))
                })
                .OrderBy(x => x.Week)
                .ToList();

            return Json(weeklyRevenue, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Quanlythanhvien(int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.use = Session["username"];
            var totalRecords = db.Accounts.Count();
            var accounts = db.Accounts
                                   .OrderBy(a => a.account_id)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            var model = new AccountListViewModel
            {
                ListAcc = accounts,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(model);
        }
       
        public ActionResult view()
        {
            ViewBag.admin = Session["admin"];
            ViewBag.use = Session["username"];
            ViewBag.account_id = Session["username"];
            Homemol hm = new Homemol();
            hm.listacc = db.Accounts.ToList();
            return View(hm);
        }
        public ActionResult GetUserByUsername(string username)
        {
            var user = db.Accounts.ToList().FirstOrDefault(u => u.username == username);

            if (user != null)
            {
                return View(user); // Truyền user vào view
            }

            return HttpNotFound(); // Nếu không tìm thấy
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["check"] != null)
            {
                if (Session["check"].ToString() == "True")
                {
                    return RedirectToAction("Index", "Accounts");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult dangky()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {


            if (ModelState.IsValid)
            {
        
                {


                    {
                        var data = db.Accounts.FirstOrDefault(u => u.username == email && u.password == password);
                        if (data!=null)
                       {
                           
                            {
                                if (data.idtype == 0)
                                {
                                    Session["admin"] = "ok";
                                    ViewBag.admin = Session["admin"];
                                }
                                else
                                {
                                    ViewBag.admin = null; Session["admin"] = null;
                                }
                            }
                            Session["username"] = email;
                            ViewBag.use = email;
                            Session["id"] = data.account_id;
                            Session["check"] = true;
                            return RedirectToAction("dashboard", "Accounts");
                        }
                        else
                        {
                            Session["check"] = false;
                            Session["ErrorMessage"] = "Thông tin đăng nhập không hợp lệ.";
                            return RedirectToAction("Login");
                        }
                    }
                }
            }
            return View();
        }


        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.admin = Session["admin"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.use = Session["username"];
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "account_id,username,password,full_name,email,phone,address,idtype")] Account account)
        {
            // Kiểm tra xem tất cả các trường có null không
            if (string.IsNullOrEmpty(account.full_name))
            {
                ModelState.AddModelError("full_name", "Tên khách hàng không được để trống.");
            }

            if (string.IsNullOrEmpty(account.username))
            {
                ModelState.AddModelError("username", "Username không được để trống.");
            }

            if (string.IsNullOrEmpty(account.password))
            {
                ModelState.AddModelError("password", "Password không được để trống.");
            }

            if (string.IsNullOrEmpty(account.email))
            {
                ModelState.AddModelError("email", "Email không được để trống.");
            }

            if (string.IsNullOrEmpty(account.phone))
            {
                ModelState.AddModelError("phone", "Số điện thoại không được để trống.");
            }

            if (string.IsNullOrEmpty(account.address))
            {
                ModelState.AddModelError("address", "Địa chỉ không được để trống.");
            }
            if (ModelState.IsValid)
            {
                if (account.idtype == null)
                {
                    account.idtype = 1;

                }
                db.Accounts.Add(account);
                db.SaveChanges();
                Session["username"] = account.username;
                ViewBag.use = account.username;
                Session["id"] = account.account_id;
                Session["check"] = true;
                if (Session["admin"] == null) { return RedirectToAction("index", "Accounts"); }
                else
                {
                    return RedirectToAction("Quanlythanhvien", "Accounts");
                }
            }
            if (Session["check"] == null)
            {
                return RedirectToAction("dangky", "Accounts");
            }
            return RedirectToAction("Quanlythanhvien", "Accounts");
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.admin= Session["admin"];
            ViewBag.admina = Session["admin"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
           
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "account_id,username,password,full_name,email,phone,address,idtype")] Account account)
        {
            
         
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Quanlythanhvien","Accounts");
            }
            return View(account);
        }
        public ActionResult Dangxuat()
        {
            Session["ErrorMessage"] = null;
            Session["username"] = null ;
            Session["check"] = false;
            return RedirectToAction("Index1", "Products");
        }
        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            if (account != null)
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
            }
            return RedirectToAction("Quanlythanhvien");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
