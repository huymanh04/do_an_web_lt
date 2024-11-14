using do_an_web.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace do_an_web.Controllers
{
    public class thietkeController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();
        // GET: thietke
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult bohoa(int? page)
        {

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
                   .Where(c => c.Category.category_name.ToLower() == "bó hoa" &&
                               c.Category1.company_name.ToLower() == "thiết kế")
                   .ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
        public ActionResult giohoa(int? page)
        {

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
                     .Where(c => c.Category.category_name.ToLower() == "giỏ hoa" &&
                                 c.Category1.company_name.ToLower() == "thiết kế")
                     .ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
    
    }
}