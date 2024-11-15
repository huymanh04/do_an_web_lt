﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace do_an_web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    //public class CartItem
    //{
    //    public Models.Product Product { get; set; }
    //    public int Quantity { get; set; }
    //}

    public partial class Cart
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cart()
        {

            this.CartItems = new HashSet<CartItem>();
        }

        public int cart_id { get; set; }
        public Nullable<int> account_id { get; set; }

        public virtual Account Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartItem> CartItems { get; set; }

        internal void AddProductCart(Product pro)
        {
            throw new NotImplementedException();
            // Kiểm tra xem sản phẩm có trong giỏ hàng chưa
            var cartItem = CartItems.FirstOrDefault(item => item.product_id == pro.product_id);

            if (cartItem != null)
            {
                // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng lên 1
                cartItem.quantity += 1;
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, tạo mới một CartItem và thêm vào giỏ hàng
                CartItems.Add(new CartItem
                {
                    product_id = pro.product_id,
                    quantity = 1,  // Thêm 1 sản phẩm vào giỏ hàng
                    Product = pro   // Gán sản phẩm cho CartItem
                });
            }
        }

        internal int Total_quantity()
        {
            throw new NotImplementedException();
            // Trả về tổng số lượng sản phẩm trong giỏ hàng
            return CartItems.Sum(item => item.quantity);
        }

        internal void Update_Quantity(int id_pro, int quantity)
        {
            throw new NotImplementedException();

            // Kiểm tra nếu quantity không hợp lệ
            if (quantity <= 0)
            {
                throw new ArgumentException("Số lượng phải lớn hơn 0.", nameof(quantity));
            }

            // Tìm kiếm CartItem có product_id tương ứng với id_pro
            var cartItem = CartItems.FirstOrDefault(item => item.product_id == id_pro);

            if (cartItem != null)
            {
                // Nếu tìm thấy CartItem, cập nhật số lượng sản phẩm
                cartItem.quantity = quantity;
            }
            else
            {
                // Nếu không tìm thấy sản phẩm trong giỏ hàng, có thể ném ngoại lệ hoặc xử lý lỗi
                throw new InvalidOperationException("Sản phẩm không tồn tại trong giỏ hàng.");
            }


        }


    }
}
