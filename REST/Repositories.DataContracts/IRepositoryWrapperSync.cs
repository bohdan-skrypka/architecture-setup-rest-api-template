namespace Repositories.DataContracts
{
    public interface IRepositoryWrapperSync
    {
        IOwnerRepository Owner { get; }
        void Save();
    }
}
