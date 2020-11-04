using Database.Context.DataContracts.Entities;
using EFCoreProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Repositories.DataContracts.Repo2;
using Repositories.DataContracts.Repo2.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REST.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/owners")]
    [ApiController]
    public class OwnerCacheTestController : ControllerBase
    {
        private readonly IOwnerRepositoryCache _repository;

        private readonly IMemoryCache memoryCache;
        private readonly DatabaseContext context;
        private readonly IDistributedCache distributedCache;

        public OwnerCacheTestController(IOwnerRepositoryCache repository, IMemoryCache memoryCache,
            DatabaseContext context, IDistributedCache distributedCache)
        {
            _repository = repository;
            this.memoryCache = memoryCache;
            this.context = context;
            this.distributedCache = distributedCache;
        }

        #region InMemory cache
        [HttpGet]
        public async Task<IReadOnlyList<Owner>> Get()
        {
            return await _repository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> Get([FromQuery] int id)
        {
            var owner = await _repository.GetByIdAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return owner;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Owner owner)
        {
            if (id != owner.Id)
            {
                return BadRequest();
            }
            await _repository.UpdateAsync(owner);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Owner>> Post(Owner customer)
        {
            await _repository.AddAsync(customer);

            return CreatedAtAction("Get", new { id = customer.Id, version = $"1" }, $"{customer.Id} : 1");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Owner>> Delete(int id)
        {
            var owner = await _repository.GetByIdAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            await _repository.DeleteAsync(owner);
            return owner;
        }
        #endregion

        [HttpGet("redis")]
        public async Task<IActionResult> GetAllCustomersUsingRedisCache()
        {
            var cacheKey = "customerList";
            string serializedCustomerList;
            var customerList = new List<Owner>();
            var redisCustomerList = await distributedCache.GetAsync(cacheKey);
            if (redisCustomerList != null)
            {
                serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
                customerList = JsonConvert.DeserializeObject<List<Owner>>(serializedCustomerList,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            else
            {
                customerList = await context.Owners.ToListAsync();
                serializedCustomerList = JsonConvert.SerializeObject(customerList);
                redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await distributedCache.SetAsync(cacheKey, redisCustomerList, options);
            }
            return Ok(customerList);
        }
    }
}
