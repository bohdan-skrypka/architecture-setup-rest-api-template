using AutoMapper;
using REST.API.DataContracts;
using REST.API.DataContracts.Requests;
using REST.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using S = REST.Services.Model;
using REST.IoC.Configuration.ThrottleAttribute;
using System.Threading;
using Repositories.DataContracts;

namespace REST.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private IRepositoryWrapperAsync _repositoryWrapper;

#pragma warning disable CS1591
        public UserController(IUserService service, IMapper mapper, IRepositoryWrapperAsync repositoryWrapper)
        {
            _service = service;
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
#pragma warning restore CS1591

        #region GET
        /// <summary>
        /// Comments and descriptions can be added to every endpoint using XML comments.
        /// </summary>
        /// <remarks>
        /// XML comments included in controllers will be extracted and injected in Swagger/OpenAPI file.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Throttle(Name = "ThrottleGetUserId", Seconds = 1)]
        public async Task<User> Get(string id)
        {
            var data = await _service.GetAsync(id);

            if (data != null)
                return _mapper.Map<User>(data);
            else
                return null;
        }
        #endregion

        #region POST
        [HttpPost]
        public async Task<User> CreateUser([FromBody] UserCreationRequest value)
        {

            //TODO: include exception management policy according to technical specifications
            if (value == null)
                throw new ArgumentNullException("value");

            if (value == null)
                throw new ArgumentNullException("value.User");


            var data = await _service.CreateAsync(_mapper.Map<S.User>(value.User));

            if (data != null)
                return _mapper.Map<User>(data);
            else
                return null;

        }
        #endregion

        #region PUT
        [HttpPut()]
        public async Task<bool> UpdateUser(User parameter)
        {
            var t1 = 0;
            var t2 = 0;

            //for (int i = 0; i < 100; i++)
            //{
            //    _repositoryWrapper.Owner.Create(new Database.Context.DataContracts.Entities.Owner { });
            //}

            //await _repositoryWrapper.SaveAsync();

            for (int i = 0; i < 100; i++)
            {
                t1 = Thread.CurrentThread.ManagedThreadId;

                await _repositoryWrapper.Owner.GetOwnerByIdAsync(50).ConfigureAwait(false);

                t2 = Thread.CurrentThread.ManagedThreadId;
                
                // await _service.UpdateAsync(_mapper.Map<S.User>(parameter)).ConfigureAwait(false);

                // await Task.Delay(100).ConfigureAwait(false);


                if (t1 != t2)
                {

                }
                else
                {

                }
            }


            if (parameter == null)
                throw new ArgumentNullException("parameter");

            await _service.UpdateAsync(_mapper.Map<S.User>(parameter)).ConfigureAwait(false);

            var t5 = Thread.CurrentThread.ManagedThreadId;

            return false;
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        public async Task<bool> DeleteDevice(string id)
        {
            return await _service.DeleteAsync(id);
        }
        #endregion
    }
}
