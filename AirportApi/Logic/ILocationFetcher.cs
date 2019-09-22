using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace AirportApi.Logic {

    public interface ILocationFetcher {
        Task<Result<Location>> GetLocation(IATA iata);
    }

    public interface ICachedLocationFetcher : ILocationFetcher {
        Task SetLocation(IATA iata, Location location);
    }

}