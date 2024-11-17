using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using do_an_web.Models;

namespace do_an_web.Controllers
{
    public class Orders1Controller : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();

        // GET: Orders1
        public ActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.use = Session["username"];
            var totalRecords = db.Orders.Count();
            var accounts = db.Orders.OrderBy(a => a.order_id).Skip((pageNumber - 1)*pageSize).Take(pageSize).ToList();
            var model = new AccountListViewModel
            {
                orders = accounts,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {

            var order = db.Orders.Find(orderId);

            if (order == null)
            {

                return Json(new { success = false, message = "Order not found" });
            }


            order.status = status;


            try
            {

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }
        // GET: Orders1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders1/Create
        public ActionResult Create()
        {
            ViewBag.account_id = new SelectList(db.Accounts, "account_id", "username");
            return View();
        }

        // POST: Orders1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_id,account_id,order_date,total_amount,status,phuongthuc")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.account_id = new SelectList(db.Accounts, "account_id", "username", order.account_id);
            return View(order);
        }

        // GET: Orders1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.account_id = new SelectList(db.Accounts, "account_id", "username", order.account_id);
            return View(order);
        }

        // POST: Orders1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_id,account_id,order_date,total_amount,status,phuongthuc")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.account_id = new SelectList(db.Accounts, "account_id", "username", order.account_id);
            return View(order);
        }

        // GET: Orders1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
