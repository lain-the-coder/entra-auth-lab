using entra_auth_lab.Api.Dtos;
using entra_auth_lab.Api.Interfaces;

namespace entra_auth_lab.Api.Services
{
    public class UserService(IUnitOfWork unitOfWork) : IUserService
    {
        public Task<UserDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<UserDto> GetByExternalIdAsync(Guid externalId)
        {
            throw new NotImplementedException();
        }
        public Task<List<UserDto>> GetAsync()
        {
            throw new NotImplementedException();
        }
        public Task<UserDto> CreateAsync(CreateUserRequest user)
        {
            throw new NotImplementedException();
        }
        public Task UpdateAsync(CreateUserRequest user)
        {
            throw new NotImplementedException();
        }
    }
}