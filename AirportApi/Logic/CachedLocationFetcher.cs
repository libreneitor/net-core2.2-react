using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace AirportApi.Logic {

    public class CachedLocationFetcher: ICachedLocationFetcher {
        private readonly IDistributedCache _distributedCache;

        public CachedLocationFetcher(IDistributedCache distributedCache) =>
            _distributedCache = distributedCache;

        public async Task<Result<Location>> GetLocation(IATA iata) {
            var strValue = await _distributedCache.GetStringAsync(iata.Code);
            return Location.FromString(strValue);
        }

        public async Task SetLocation(IATA iata, Location location) {
            await _distributedCache.SetStringAsync(iata.Code, location.ToString());
        }

    }

}