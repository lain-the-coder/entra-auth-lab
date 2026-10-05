using entra_auth_lab.Api.Dtos;
using entra_auth_lab.Api.Entities;
using entra_auth_lab.Api.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            // in case client sends explicit all-zeros GUID
            if (user.ExternalId!.Value == Guid.Empty)
            {
                throw new ValidationException("ExternalId is required.");
            }
            var row = new User
            {
                ExternalId = user.ExternalId!.Value,
                Email = user.Email,
                DisplayName = user.DisplayName
            };
            uow.Users.Add(row);
            uow.Users.Add(row);
            try
            {
                await uow.SaveChangesAsync();
            }
            // concurrency safe for index issues
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new ConflictException($"A user with ExternalId {user.ExternalId} already exists.");
            }
            var createdUser = new UserDto
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Email = row.Email,
                DisplayName = row.DisplayName
            };
            return createdUser;
        }
        public async Task UpdateAsync(int id, UpdateUserRequest user)
        {
            var row = await uow.Users.GetByIdAsync(id);
            if (row == null)
            {
                throw new NotFoundException($"User with ID {id} was not found.");
            }
            row.Email = user.Email;
            row.DisplayName = user.DisplayName;
            await uow.SaveChangesAsync();
        }
    }
}