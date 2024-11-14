using do_an_web.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace do_an_web.Controllers
{
    public class HomeController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();
        //public ActionResult Index()
        //{
        //    Homemol hm = new Homemol();
        //    hm.ListProduct = db.Products.ToList();
        //    return View(hm);
        //}

        public ActionResult huongdanmuahang()
        {
            return View();
        }
        public ActionResult chinhsachthanhtoan()
        {
            return View();
        }
        public ActionResult chinhsachvanchuyen()
        {
            return View();
        }
        public ActionResult chinhsachdoitrahang()
        {
            return View();
        }
        public ActionResult chinhsachbaomat()
        {
            return View();
        }
        public ActionResult trachnhiemdoiben()
        {
            return View();
        }
        public ActionResult dangky()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}