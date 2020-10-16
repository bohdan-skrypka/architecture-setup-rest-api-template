using Database.Context.DataContracts.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.DataContracts
{
    public interface IOwnerRepository : IRepositoryBase<Owner>
    {
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<Owner> GetOwnerByIdAsync(Guid ownerId);
        Task<Owner> GetOwnerWithDetailsAsync(Guid ownerId);
        void CreateOwner(Owner owner);
        void UpdateOwner(Owner owner);
        void DeleteOwner(Owner owner);
    }
}
