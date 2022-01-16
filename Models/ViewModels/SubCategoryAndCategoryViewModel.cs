using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel        //model for specific view
    {
        public IEnumerable<Category> CategoryList { get; set; }     //to show category list

        public SubCategory SubCategory{ get; set; }     //will show current sub category not list

        public List<string> SubCategoryList { get; set; }       //to show already existed sub-category of that category

        public string StatusMessage { get; set; }
    }
}
