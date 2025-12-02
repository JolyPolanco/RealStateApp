

using RealStateApp.Core.Application.ViewModels.FavoriteProperty;

namespace RealStateApp.Core.Application.ViewModels.Properties
{
    public class DataPropertyListViewModel
    {


        public List<DataPropertyViewModel> ListProperty { get; set; } = new List<DataPropertyViewModel>();

        public CreateFavoriteViewModel CreateFavoriteViewModel { get; set; } = new();



        //Data para los filtros
        public string? SearchCode { get; set; }
        public string? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }

    }
}
