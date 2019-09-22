using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirportApi.Logic;
using Microsoft.AspNetCore.Http;
using CSharpFunctionalExtensions;

namespace AirportApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        ILocationFetcher _fetcher;

        public DistanceController(ILocationFetcher fetcher) => _fetcher = fetcher; 

        [HttpGet("{iataCode1}/{iataCode2}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DistanceResponse>> Get(string iataCode1, string iataCode2)
        {
            var iata1 = IATA.fromString(iataCode1);
            var iata2 = IATA.fromString(iataCode2);
            if(iata1.IsFailure || iata2.IsFailure) {
                return BadRequest();
            }
            
            Result<Location>[] locations = await Task.WhenAll(
                _fetcher.GetLocation(iata1.Value), 
                _fetcher.GetLocation(iata2.Value)
            );

            if(locations[0].IsFailure || locations[1].IsFailure) {
                return NotFound();
            }

            return Ok(DistanceResponse.FromLocations(
                locations[0].Value,
                locations[1].Value
            ));
        }

        public class DistanceResponse {
            public readonly double distance;

            private DistanceResponse(double distance) {
                this.distance = distance;
            }

            public static DistanceResponse FromLocations(Location loc1, Location loc2) {
                return new DistanceResponse(loc1.DistanceMiles(loc2));
            }
        }

    }
}
