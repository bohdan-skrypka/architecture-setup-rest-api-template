using AutoMapper;
using REST.API.Common.Settings;
using REST.Services.Contracts;
using REST.Services.Model;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace REST.Services
{
    public class UserService : IUserService
    {
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> settings, IMapper mapper)
        {
            _settings = settings?.Value;
            _mapper = mapper;
        }

        public async Task<User> CreateAsync(User user)
        {
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var t3 = Thread.CurrentThread.ManagedThreadId;

            // await Task.Delay(1000);

            var t4 = Thread.CurrentThread.ManagedThreadId;

            return false;
            // throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetAsync(string id)
        {
            return new User
            {
                Id = id,
                Firstname = "Firstname",
                Lastname = "Lastname",
                Address = new Address
                {
                    City = "City",
                    Country = "Country",
                    Street = "Street",
                    ZipCode = "ZipCode"
                }
            };
        }
    }
}
