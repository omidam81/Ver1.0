using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.ViewModels
{
    public class WizardProductsViewModel
    {
        public ProductViewModel[] products { get; set; }
        public ColorViewModel[] product_colors { get; set; }
        public ProductImageViewModel[] product_images { get; set; }
        public ProductGroupViewModel[] product_groups { get; set; }

    }

    public class ProductViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string headline { get; set; }
        public int[] colors_available { get; set; }
        public string list_of_sizes { get; set; }
        public ProductPriceViewModel[] prices { get; set; }
    }

    public class ColorViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public int? importance { get; set; }
    }

    public class ProductImageViewModel
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int ppi { get; set; }
        public int printable_front_left { get; set; }
        public int printable_front_top { get; set; }
        public int printable_front_width { get; set; }
        public int printable_front_height { get; set; }
        public int chest_line_front { get; set; }
        public int printable_back_left { get; set; }
        public int printable_back_top { get; set; }
        public int printable_back_width { get; set; }
        public int printable_back_height { get; set; }
        public int chest_line_back { get; set; }
        public string gender { get; set; }
    }

    public class ProductGroupViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string singular { get; set; }
        public int[] products { get; set; }
    }

    public class ProductPriceViewModel
    {
        public int color_id { get; set; }
        public double price { get; set; }
    }
}