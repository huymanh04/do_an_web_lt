using do_an_web.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace do_an_web.Controllers
{
    public class hoakhaitruongController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();
        // GET: hoakhaitruong
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult deban(int? page)
        {

            int pageSize = 4; 
            int pageNumber = (page ?? 1);


            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
           .Where(c => c.Category.category_name.ToLower() == "để bàn" &&
                       c.Category1.company_name.ToLower() == "hoa khai trương")
           .ToList()
            };


            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }
        public ActionResult langhoa(int? page)
        {

            int pageSize = 4; 
            int pageNumber = (page ?? 1);

            Homemol homemol = new Homemol
            {
                ListProduct = db.Products
         .Where(c => c.Category.category_name.ToLower() == "lãng hoa" &&
                     c.Category1.company_name.ToLower() == "hoa khai trương")
         .ToList()
            };

            homemol.ListProduct1 = homemol.ListProduct.ToPagedList(pageNumber, pageSize);

            return View(homemol);
        }

    }
}