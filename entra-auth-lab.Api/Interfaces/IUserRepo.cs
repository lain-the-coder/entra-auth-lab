using entra_auth_lab.Api.Entities;

namespace entra_auth_lab.Api.Interfaces
{
    public interface IUserRepo
    {
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
        void Add(User user);
        void Remove(User user);
    }
}