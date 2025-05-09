using apbd8.Models;

namespace apbd8.Services;

public interface IClientService
{
    int CreateClient(ClientDTO clientDto);
    IEnumerable<TripDTO> GetClientTrips(int clientId);
    void RegisterClientToTrip(int clientId, int tripId);
    void RemoveClientFromTrip(int clientId, int tripId);
}