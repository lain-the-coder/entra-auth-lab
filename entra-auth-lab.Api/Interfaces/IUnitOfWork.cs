namespace entra_auth_lab.Api.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepo Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}