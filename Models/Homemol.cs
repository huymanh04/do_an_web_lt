using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace do_an_web.Models
{
    public class Homemol
    {
        public List<Product> ListProduct { get; set; }
        public IPagedList<Product> ListProduct1 { get; set; }
        public List<Account> listacc { get; set; }
        public List<Category1> Listthelao { get; set; }
        public List<Category> Listloai { get; set; }
    }
}