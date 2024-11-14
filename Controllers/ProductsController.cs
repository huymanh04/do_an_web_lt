using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using do_an_web.Models;
using PagedList;

namespace do_an_web.Controllers
{
    public class ProductsController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();

        // GET: Products
        public ActionResult ProductList(int? category, int? page, string sortOrder, string SearchString, double min = double.MinValue, double max = double.MaxValue)
        {
            var products = db.Products.Include(p => p.Category);

            // Tìm kiếm chuỗi truy vấn theo category
            if (category == null)
            {
                products = db.Products.OrderByDescending(x => x.product_name);
            }
            else
            {
                products = db.Products.OrderByDescending(x => x.category_id).Where(x => x.category_id == category);
            }
            // Tìm kiếm chuỗi truy vấn theo NamePro (SearchString)
            if (!String.IsNullOrEmpty(SearchString))
            {
                products = db.Products.OrderByDescending(x => x.category_id).Where(s => s.product_name.Contains(SearchString.Trim()));
            }
            // Tìm kiếm chuỗi truy vấn theo đơn giá
            if (min >= 0 && max > 0)
            {
                products = db.Products.OrderByDescending(x => x.price).Where(p => (double)p.price >= min && (double)p.price <= max);
            }


            int pageSize = 4; // Số bản ghi trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1
            Homemol homemol = new Homemol
            {
                ListProduct = products.ToList() // Giả sử lấy dữ liệu từ database
            };

            // Phân trang cho ListProduct
            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
        public ActionResult Index1(  int? page)
        {

            int pageSize = 4; // Số bản ghi trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1

            // Khởi tạo đối tượng Homemol và lấy dữ liệu từ ListProduct
            Homemol homemol = new Homemol
            {
                ListProduct = db.Products.ToList() // Giả sử lấy dữ liệu từ database
            };

            // Phân trang cho ListProduct
            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }

        public JsonResult GetCategories(int companyId)
        {
            var categories = db.Categories
                .Where(c => c.category_id == companyId)
                .Select(c => new { Id = c.category_id, Name = c.category_name })
                .ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index(string searchType = null, string searchCompany = null, int? page=1)
        {
            ViewBag.use = Session["username"];
            ViewBag.check = Session["check"];
            ViewBag.admin = Session["admin"];
            if (ViewBag.admin == null)
            {
                return RedirectToAction("Index1", "Products");
            }
            var products = db.Products.Include(p => p.Category).Include(p => p.Category1).AsQueryable();

            // Tìm kiếm theo loại
            if (!string.IsNullOrEmpty(searchType))
            {
                products = products.Where(p => p.Category.category_name.Contains(searchType));
            }

            // Tìm kiếm theo công ty
            if (!string.IsNullOrEmpty(searchCompany))
            {
                products = products.Where(p => p.Category1.company_name.Contains(searchCompany));
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
             Homemol homemol = new Homemol
            {
                ListProduct = db.Products.ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);
           

            return View(homemol);
        }
        public ActionResult GetImage(int id)
        {
            // Tìm sản phẩm theo ID
            var product = db.Products.Find(id);
            if (product == null || product.images == null)
            {
                return HttpNotFound();
            }

            // Trả về hình ảnh dưới dạng file
            return File(product.images, "image/jpeg"); // Thay "image/jpeg" nếu sử dụng định dạng khác
        }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            Product product = db.Products.Find(id);
            try { 
            ViewBag.admin = Session["admin"];
            Session["PreviousUrl"] = Request.UrlReferrer.ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (product == null)
            {
                return HttpNotFound();
            }
            }
            catch { }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            
            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name");
            ViewBag.company_id = new SelectList(db.Category1, "category1_id", "company_name");
            ViewBag.use = Session["username"];

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "product_id,product_name,price,stock,description,company_id,category_id")] Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Chuyển đổi hình ảnh thành mảng byte để lưu vào cơ sở dữ liệu
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        product.images = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }
                else
                {
                    // Nếu không có ảnh nào được tải lên, có thể đặt giá trị mặc định hoặc bỏ qua bước này
                    ModelState.AddModelError("image", "Ảnh sản phẩm là bắt buộc.");
                    ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
                    ViewBag.company_id = new SelectList(db.Category1, "company_id", "company_name", product.company_id);
                    return View(product);
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
            ViewBag.company_id = new SelectList(db.Category1, "company_id", "company_name", product.company_id);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
            ViewBag.company_id = new SelectList(db.Category1, "category1_id", "company_name", product.company_id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "product_id,product_name,price,stock,description,company_id,category_id,images")] Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu có tệp hình ảnh được tải lên
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Đọc tệp hình ảnh vào byte[]
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(memoryStream);
                        product.images = memoryStream.ToArray(); // Gán mảng byte vào thuộc tính image
                    }
                }

                // Cập nhật thông tin sản phẩm
                db.Entry(product).State = EntityState.Modified;
                try {
                db.SaveChanges();
                return RedirectToAction("Index");
                }
                catch { return RedirectToAction("Index"); }
            }
            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
            ViewBag.company_id = new SelectList(db.Category1, "category1_id", "company_name", product.company_id);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
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
