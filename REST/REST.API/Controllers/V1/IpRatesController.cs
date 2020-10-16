using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace REST.API.Controllers.V1
{
    /// <summary>
    /// https://codeburst.io/rate-limiting-api-endpoints-in-asp-net-core-926e31428017
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/iprates")]
    [ApiController]
    public class IpRatesController : ControllerBase
    {
        private readonly IpRateLimitOptions _options;
        private readonly IIpPolicyStore _ipPolicyStore;

        public IpRatesController(IOptions<IpRateLimitOptions> optionsAccessor, IIpPolicyStore ipPolicyStore)
        {
            _options = optionsAccessor.Value;
            _ipPolicyStore = ipPolicyStore;
        }

        [HttpGet]
        public async Task<IpRateLimitPolicies> Get()
        {
            return await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix);
        }

        [HttpPost]
        public async Task Post()
        {
            var pol = await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix);

            pol.IpRules.Add(new IpRateLimitPolicy
            {
                Ip = "8.8.4.4",
                Rules = new List<RateLimitRule>(new RateLimitRule[] {
                new RateLimitRule {
                    Endpoint = "*:/api/testupdate",
                    Limit = 100,
                    Period = "1d" }
            })
            });

            await _ipPolicyStore.SetAsync(_options.IpPolicyPrefix, pol);
        }
    }
}
