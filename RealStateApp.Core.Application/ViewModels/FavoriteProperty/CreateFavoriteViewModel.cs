

namespace RealStateApp.Core.Application.ViewModels.FavoriteProperty
{
    public class CreateFavoriteViewModel
    {


        public int Id { get; set; }
        public  string? ClientId { get; set; }
        public int PropertyId { get; set; }
        public bool IsFavorite { get; set; }
        public int FavoriteId { get; set; }
        public string? ReturnUrl {  get; set; }
    }
}
