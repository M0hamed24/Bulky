using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace SD7501Bulky.Models.ViewModles
{
    public class ProductVM
    {
        public Product Product {  get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
