using entra_auth_lab.Api.Dtos;
using entra_auth_lab.Api.Entities;
using entra_auth_lab.Api.Interfaces;
using MiniShop.Api.Services;

namespace entra_auth_lab.Api.Services
{
    public class UserService(IUnitOfWork uow) : IUserService
    {
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var row = await uow.Users.GetByIdAsync(id);
            if (row == null)
            {
                throw new NotFoundException($"User with ID {id} was not found.");
            }
            var user = new UserDto
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Email = row.Email,
                DisplayName = row.DisplayName
            };
            return user;
        }
        public async Task<UserDto> GetByExternalIdAsync(Guid externalId)
        {
            var row = await uow.Users.GetByExternalIdAsync(externalId);
            if (row == null)
            {
                throw new NotFoundException($"User with External ID {externalId} was not found.");
            }
            var user = new UserDto
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Email = row.Email,
                DisplayName = row.DisplayName
            };
            return user;
        }
        public async Task<List<UserDto>> GetAsync()
        {
            var users = await uow.Users.GetAllAsync();
            var result = new List<UserDto>();
            foreach (var user in users)
            {
                result.Add(new UserDto
                {
                    Id = user.Id,
                    ExternalId = user.ExternalId,
                    Email = user.Email,
                    DisplayName = user.DisplayName
                });
            }
            return result;
        }
        public async Task<UserDto> CreateAsync(CreateUserRequest user)
        {
            var row = new User
            {
                ExternalId = user.ExternalId,
                Email = user.Email,
                DisplayName = user.DisplayName
            };
            uow.Users.Add(row);
            await uow.SaveChangesAsync();
            var createdUser = new UserDto
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Email = row.Email,
                DisplayName = row.DisplayName
            };
            return createdUser;
        }
        public async Task UpdateAsync(int id, CreateUserRequest user)
        {
            var row = await uow.Users.GetByIdAsync(id);
            if (row == null)
            {
                throw new NotFoundException($"User with ID {id} was not found.");
            }
            row.ExternalId = user.ExternalId;
            row.Email = user.Email;
            row.DisplayName = user.DisplayName;
            await uow.SaveChangesAsync();
        }
    }
}