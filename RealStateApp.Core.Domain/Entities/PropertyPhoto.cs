namespace RealStateApp.Core.Domain.Entities
{
    public class PropertyPhoto
    {
        public int Id { get; set; }
        public required string ImageUrl { get; set; }

        public int PropertyId { get; set; }

        public Property Property { get; set; } = null!;
    }
}
