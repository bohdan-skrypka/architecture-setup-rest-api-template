using Database.Context.DataContracts.Entities;
using EFCoreProvider;
using Microsoft.EntityFrameworkCore;
using Repositories.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading;
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

        public async Task<Owner> GetOwnerByIdAsync(int ownerId)
        {
            var t1 = Thread.CurrentThread.ManagedThreadId;

            await Task.Delay(1000).ConfigureAwait(false);

            var t2 = Thread.CurrentThread.ManagedThreadId;

            var owner = await FindByCondition(owner => owner.Id.Equals(ownerId))
                        .FirstOrDefaultAsync().ConfigureAwait(false);

            var t3 = Thread.CurrentThread.ManagedThreadId;

            return owner;
        }

        public async Task<Owner> GetOwnerWithDetailsAsync(int ownerId)
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
