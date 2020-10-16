using System.Threading.Tasks;

namespace Repositories.DataContracts
{
    public interface IRepositoryWrapperAsync
    {
        IOwnerRepository Owner { get; }
        Task SaveAsync();
    }
}
