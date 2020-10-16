using Database.Context.DataContracts.Entities;
using EFCoreProvider;
using Microsoft.EntityFrameworkCore;
using Repositories.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.DataAccess.Repos
{
    public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
    {
        public OwnerRepository(DatabaseContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Owner>> GetAllOwnersAsync()
        {
            return await FindAll()
               //.OrderBy(ow => ow.Name)
               .ToListAsync();
        }

        public async Task<Owner> GetOwnerByIdAsync(Guid ownerId)
        {
            return await FindByCondition(owner => owner.Id.Equals(ownerId))
                .FirstOrDefaultAsync();
        }

        public async Task<Owner> GetOwnerWithDetailsAsync(Guid ownerId)
        {
            return await FindByCondition(owner => owner.Id.Equals(ownerId))
                //.Include(ac => ac.Accounts)
                .FirstOrDefaultAsync();
        }

        public void CreateOwner(Owner owner)
        {
            Create(owner);
        }

        public void UpdateOwner(Owner owner)
        {
            Update(owner);
        }

        public void DeleteOwner(Owner owner)
        {
            Delete(owner);
        }
    }
}
