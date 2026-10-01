using entra_auth_lab.Api.Dtos;

namespace entra_auth_lab.Api.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> GetByExternalIdAsync(Guid externalId);
        Task<List<UserDto>> GetAsync();
        Task<UserDto> CreateAsync(CreateUserRequest user);
        Task UpdateAsync(CreateUserRequest user);
    }
}