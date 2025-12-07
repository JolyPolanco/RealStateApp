

namespace RealStateApp.Core.Application.ViewModels.GeneralLayout
{
    public class DataPropertyHomeListViewModel
    {



        public List<DataPropertyHomeViewModel> ListDataHomeProperty = new();



        //Data para los filtros
        public string? SearchCode { get; set; }
        public string? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }

    }
}
