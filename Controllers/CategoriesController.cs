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
    public class CategoriesController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();

        // GET: Categories
        public ActionResult Index()
        {
            
            Homemol hm = new Homemol();
            hm.Listloai = db.Categories.ToList();
            ViewBag.Listloai = hm;
             return View(hm.Listloai);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            ViewBag.category1_id = new SelectList(db.Category1, "category1_id", "company_name");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "category_id,category_name,category1_id")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.category1_id = new SelectList(db.Category1, "category1_id", "company_name", category.category1_id);
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.category1_id = new SelectList(db.Category1, "category1_id", "company_name", category.category1_id);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "category_id,category_name,category1_id")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category1_id = new SelectList(db.Category1, "category1_id", "company_name", category.category1_id);
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var productIds = db.Products
            .Where(p => p.category_id == id)
            .Select(p => p.product_id)
            .ToList();
            var cartItemsToDelete = db.CartItems
            .Where(ci => productIds.Contains((int)ci.product_id));
            db.CartItems.RemoveRange(cartItemsToDelete);
            productIds = db.Products
            .Where(p => p.category_id == id)
            .Select(p => p.product_id)
            .ToList();
            var orderItemsToDelete = db.OrderItems
            .Where(oi => productIds.Contains((int)oi.product_id));
            db.OrderItems.RemoveRange(orderItemsToDelete);
         var   productsToDelete = db.Products
            .Where(p => p.category_id == id);
            db.Products.RemoveRange(productsToDelete);
            var categoryToDelete = db.Categories
           .FirstOrDefault(c => c.category_id == id);
            db.Categories.Remove(categoryToDelete);
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
