using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Coupon> Coupon { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<MenuItem> MenuItem { get; set; }
    }
}
