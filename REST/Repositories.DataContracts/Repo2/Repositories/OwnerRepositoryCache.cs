using Common.Infrastructure.Caching;
using Common.Infrastructure.Enum;
using Database.Context.DataContracts.Entities;
using EFCoreProvider;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.DataContracts.Repo2.Repositories
{
    public class OwnerRepositoryCache : GenericRepository<Owner>, IOwnerRepositoryCache
    {
        private readonly DbSet<Owner> _owner;
        public OwnerRepositoryCache(DatabaseContext databaseContext, Func<CacheTech, ICacheService> cacheService) : base(databaseContext, cacheService)
        {
            _owner = databaseContext.Set<Owner>();
        }
    }
}
