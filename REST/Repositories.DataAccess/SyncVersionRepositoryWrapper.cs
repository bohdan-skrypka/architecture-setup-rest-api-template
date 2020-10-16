using EFCoreProvider;
using Repositories.DataAccess.Repos;
using Repositories.DataContracts;

namespace DataAccess.Sync
{
    public class SyncVersionRepositoryWrapper : IRepositoryWrapperSync
    {
        private DatabaseContext _repoContext;
        private IOwnerRepository _owner;
        public IOwnerRepository Owner
        {
            get
            {
                if (_owner == null)
                {
                    _owner = new OwnerRepository(_repoContext);
                }
                return _owner;
            }
        }

        public SyncVersionRepositoryWrapper(DatabaseContext databaseContext)
        {
            _repoContext = databaseContext;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
