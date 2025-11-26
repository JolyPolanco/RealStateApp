namespace RealStateApp.Core.Domain.Entities
{
    public class FavoriteProperty
    {
        public int Id { get; set; }
        public required string ClientId { get; set; }
        public int PropertyId { get; set; }

        public Property Property { get; set; } = null!;
    }
}
