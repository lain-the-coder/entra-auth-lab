using System.ComponentModel.DataAnnotations;

namespace entra_auth_lab.Api.Dtos
{
    public class CreateUserRequest
    {
        [Required]
        public Guid ExternalId { get; set; }
        [MaxLength(256)]
        public string? Email { get; set; }
        [MaxLength(200)]
        public string? DisplayName { get; set; }
    }
}