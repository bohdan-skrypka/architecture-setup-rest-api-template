using Database.Context.DataContracts.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.DataContracts
{
    public interface IOwnerRepository : IRepositoryBase<Owner>
    {
        Task<IReadOnlyList<Owner>> GetAllOwnersAsync();
        Task<Owner> GetOwnerByIdAsync(int ownerId);
        Task<Owner> GetOwnerWithDetailsAsync(int ownerId);
        void CreateOwner(Owner owner);
        void UpdateOwner(Owner owner);
        void DeleteOwner(Owner owner);
    }
}
