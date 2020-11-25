using Database.Context.DataContracts.Entities;
using MediatR;
using Repositories.DataContracts;
using REST.API.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using MediatR;

namespace REST.API.CQRS.Queries
{
    public class FindAllOwnersQuery : IRequest<List<Owner>>
    {
    }

    public class FindOwnersHandler : IRequestHandler<FindAllOwnersQuery, List<Owner>>
    {
        private IRepositoryWrapperAsync _repositoryWrapper;

        public FindOwnersHandler(IRepositoryWrapperAsync repositoryWrapperAsync)
        {
            _repositoryWrapper = repositoryWrapperAsync;
        }

        public Task<List<Owner>> Handle(FindAllOwnersQuery request, CancellationToken cancellationToken)
        {
            var t = _repositoryWrapper.Owner.FindAll().ToList();

            return Task.FromResult(t);
        }
    }
}
