using do_an_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace do_an_web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private DB_shopbanhoaEntities db = new DB_shopbanhoaEntities();
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
                return View("ShowCart");
            Cartt _cart = Session["Cart"] as Cartt;
            return View(_cart);
        }
        public ActionResult CheckOut_Success()
        {
            return View("CheckOut_Success");
        }
    

        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                Cartt cart = Session["Cart"] as Cartt;
                Order _order = new Order();
                OrderItem a_order = new OrderItem();
              
                _order.order_date = DateTime.Now;
                _order.account_id = int.Parse(form["CodeCustomer"]);
                db.Orders.Add(_order);
                foreach (var item in cart.Items)
                {
                    // lưu dòng sản phẩm vào chi tiết hóa đơn
                    OrderItem _order_detail = new OrderItem();
                    _order_detail.order_id = _order.order_id;
                    _order_detail.product_id = item._product.product_id;
                    _order_detail.price = (decimal)item._product.price;
                    _order_detail.quantity = item._quantity;
                    db.OrderItems.Add(_order_detail);
                }
                db.SaveChanges();
                cart.ClearCart();
                return RedirectToAction("CheckOut_Success", "ShoppingCart");
            }
            catch
            {
                return Content("Có sai sót! Xin kiểm tra lại thông tin"); ;
            }
        }

        public PartialViewResult BagCart()
        {
            decimal total_money_item = 0;
            Cartt cart = Session["Cart"] as Cartt;
            if (cart != null)
                total_money_item = cart.Total_money();
            ViewBag.TotalCart = total_money_item;
            return PartialView("BagCart");
        }

        // Tạo mới giỏ hàng, nguồn được lấy từ Session
        public Cartt GetCart()
        {
            Cartt cart = Session["Cart"] as Cartt;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cartt();
                Session["Cart"] = cart;
            }
            return cart;
        }
        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            var _pro = db.Products.SingleOrDefault(s => s.product_id == id);
            if (_pro != null)
            {
                GetCart().Add_Product_Cart(_pro);
            }
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        // Cập nhật số lượng và tính lại tổng tiền
        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            Cartt cart = Session["Cart"] as Cartt;
            int id_pro = int.Parse(Request.Form["idPro"]);
            int _quantity = int.Parse(Request.Form["carQuantity"]);
            cart.Update_quantity(id_pro, _quantity);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        // Xóa dòng sản phẩm trong giỏ hàng
        public ActionResult RemoveCart(int id)
        {
            Cartt cart = Session["Cart"] as Cartt;
            cart.Remove_CartItem(id);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }
    }
}