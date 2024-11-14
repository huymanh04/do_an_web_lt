using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

        // GET: Accounts
        public ActionResult Index()
        {
            ViewBag.admin = Session["admin"];
            ViewBag.use = Session["username"];
            Homemol hm=new Homemol();
            hm.listacc=db.Accounts.ToList();
            return View(hm);
        }
        public ActionResult Quanlythanhvien(int pageNumber = 1, int pageSize = 6)
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
                            Session["check"] = true;
                            return RedirectToAction("Index", "Products");
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
                
                db.Accounts.Add(account);
                db.SaveChanges(); 
                Session["username"] = account.username;
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
