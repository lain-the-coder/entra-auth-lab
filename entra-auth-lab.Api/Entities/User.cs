namespace entra_auth_lab.Api.Entities
{
    public class User : IAuditable
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}