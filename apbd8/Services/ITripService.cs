using apbd8.Models;

namespace apbd8.Services;

public interface ITripService
{
    IEnumerable<TripDTO> GetAllTrips();
}