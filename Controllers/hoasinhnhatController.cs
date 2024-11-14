using do_an_web.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace do_an_web.Controllers
{
    public class hoasinhnhatController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();
        // GET: hoasinhnhat
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult tangban(int? page)
        {

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct  = db.Products
                    .Where(c => c.Category.category_name.ToLower() == "tặng bạn" &&
                                c.Category1.company_name.ToLower() == "hoa sinh nhật")
                    .ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
        public ActionResult tangnguoiyeu(int? page)
        {

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
                   .Where(c => c.Category.category_name.ToLower() == "tặng người yêu" &&
                               c.Category1.company_name.ToLower() == "hoa sinh nhật")
                   .ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
        public ActionResult sangtrong(int? page)
        {

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
                   .Where(c => c.Category.category_name.ToLower() == "sang trọng" &&
                               c.Category1.company_name.ToLower() == "hoa sinh nhật")
                   .ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
        public ActionResult tangme(int? page)
        {

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
                    .Where(c => c.Category.category_name.ToLower() == "tặng mẹ" &&
                                c.Category1.company_name.ToLower() == "hoa sinh nhật")
                    .ToList()
            };
            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }

        public ActionResult GoBack()
        {
            if (Session["PreviousUrl"] != null)
            {
                return Redirect(Session["PreviousUrl"].ToString());
            }

            // Chuyển hướng đến trang mặc định nếu không có referrer
            return RedirectToAction("Index", "Home");
        }

   
      
        //public ActionResult tangme()
        //{
        //    Homemol hm = new Homemol();
        //    hm.ListProduct = db.Products.ToList();
        //    return View(hm);
        //}
    }
}