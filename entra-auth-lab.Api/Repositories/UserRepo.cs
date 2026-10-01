using entra_auth_lab.Api.Data;
using entra_auth_lab.Api.Entities;
using entra_auth_lab.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace entra_auth_lab.Api.Repositories
{
    public class UserRepo(AppDbContext db) : IUserRepo
    {
        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await db.Users.FindAsync(id, cancellationToken);
        }
        public async Task<User?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken = default)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);
        }
        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await db.Users.AsNoTracking().ToListAsync(cancellationToken);
        }
        public void Add(User user)
        {
            db.Users.Add(user);
        }
        public void Remove(User user)
        {
            db.Users.Remove(user);
        }
    }
}