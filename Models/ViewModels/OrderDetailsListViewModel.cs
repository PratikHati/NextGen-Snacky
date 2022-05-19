using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Models.ViewModels
{
    public class OrderDetailsListViewModel
    {
        public List<OrderDetails> OrderDetailsList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
