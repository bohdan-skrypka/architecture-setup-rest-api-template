using EFCoreProvider;
using Repositories.DataAccess.Repos;
using Repositories.DataContracts;
using System.Threading.Tasks;

namespace DataAccess.Async
{
    public class AsyncVersionRepositoryWrapper : IRepositoryWrapperAsync
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

        public AsyncVersionRepositoryWrapper(DatabaseContext databaseContext)
        {
            _repoContext = databaseContext;
        }

        public async Task SaveAsync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
