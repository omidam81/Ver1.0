using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel() : this(null)
        {
        }

        public ProductViewModel(int? productId)
        {
            Id = productId;

            ProductColours = new List<ProductColourItemViewModel>();
            SelectedProductColours = new List<ProductColourItemViewModel>();

            ProductGroups = new List<ProductGroupItemViewModel>();
            SelectedProductGroups = new List<int>();
        }

        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<ProductColourItemViewModel> ProductColours { get; set; }
        public List<ProductColourItemViewModel> SelectedProductColours { get; set; }
        public IEnumerable<ProductGroupItemViewModel> ProductGroups { get; set; }
        public IEnumerable<int> SelectedProductGroups { get; set; }
        public IEnumerable<ProductHeadlineViewModel> ProductHeadlines { get; set; }
        public IEnumerable<ProductSizeItemViewModel> ProductSizes { get; set; }

        [Required]
        public int SelectedProductHeadline { get; set; }

        public HttpPostedFileBase ProductImageFront { get; set; }
        public HttpPostedFileBase ProductImageBack { get; set; }
        public string ProductImageFrontFilename { get; set; }
        public string ProductImageBackFilename { get; set; }
    }
}