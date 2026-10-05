using System.ComponentModel.DataAnnotations;

namespace entra_auth_lab.Api.Dtos
{
    public class UpdateUserRequest
    {
        [MaxLength(256)]
        public string? Email { get; set; }
        [MaxLength(200)]
        public string? DisplayName { get; set; }
    }
}