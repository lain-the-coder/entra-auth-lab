using entra_auth_lab.Api.Data;
using entra_auth_lab.Api.Interfaces;

namespace entra_auth_lab.Api.Repositories
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        public IUserRepo Users { get; } = new UserRepo(db);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await db.SaveChangesAsync(cancellationToken);
    }
}