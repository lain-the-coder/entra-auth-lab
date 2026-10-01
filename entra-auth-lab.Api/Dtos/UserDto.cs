namespace entra_auth_lab.Api.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
    }
}