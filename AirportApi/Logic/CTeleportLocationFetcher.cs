using CSharpFunctionalExtensions;
using System.Net;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AirportApi.Logic {

    public class CTeleportLocationFetcher: ILocationFetcher {
        private readonly HttpClient _client;
        private readonly ICachedLocationFetcher _cachedFetcher;

        public CTeleportLocationFetcher(HttpClient client, ICachedLocationFetcher cachedFetcher) {
            client.BaseAddress = new Uri("https://places-dev.cteleport.com/airports/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
            _cachedFetcher = cachedFetcher;
        }

        public static void AddHttpClient(IServiceCollection services) {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    });
            var circuitBreaker = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(15)
                );
            services.AddHttpClient<ILocationFetcher, CTeleportLocationFetcher>()
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreaker);
        }

        public async Task<Result<Location>> GetLocation(IATA iata) {
            var cacheResult = await _cachedFetcher.GetLocation(iata);
            if(cacheResult.IsSuccess) {
                return cacheResult;
            }
            var teleportResult = await GetLocationFromCTeleport(iata);
            if(teleportResult.IsSuccess) {
                await _cachedFetcher.SetLocation(iata, teleportResult.Value);
            }
            return teleportResult;
        }

        private async Task<Result<Location>> GetLocationFromCTeleport(IATA iata) {
            try {
                var response = await _client.GetAsync(iata.Code);
                if(response.StatusCode != HttpStatusCode.OK) {
                    return Result.Fail<Location>(response.StatusCode.ToString());
                }
                var data = await response.Content.ReadAsStringAsync();
                var ctResponse = JsonConvert.DeserializeObject<CTResponse>(data);
                return ctResponse.location.ToLocation();
            } catch(Exception e) {
                return Result.Fail<Location>(e.Message);
            }
        }

        private class CTResponseLocation {
            public double lon {get; set;} 
            public double lat {get; set;}

            public Result<Location> ToLocation() {
                return Location.FromDoubles(this.lat, this.lon);
            }
        }
        private class CTResponse {
            public CTResponseLocation location {get; set;}
        }

    }

}