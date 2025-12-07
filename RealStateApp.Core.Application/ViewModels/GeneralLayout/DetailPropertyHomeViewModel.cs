
namespace RealStateApp.Core.Application.ViewModels.GeneralLayout
{
    public class DetailPropertyHomeViewModel
    {



        public int Id { get; set; }
        public required string Code { get; set; }
        public required decimal Price { get; set; }
        public required double SizeInMeters { get; set; }
        public required int Bedrooms { get; set; }
        public required int Bathrooms { get; set; }
        public required string Description { get; set; }
        public List<string>? Images { get; set; } = new List<string>();
        public List<string>? Inproments { get; set; } = new List<string>();
        public required string PropertyType { get; set; }
        public required string SaleType { get; set; }


        //Data agent
        public required string AgentId { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string UrlImage { get; set; }

    }
}
