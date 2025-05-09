using System.Data.SqlClient;
using apbd8.Models;

namespace apbd8.Services;

public class TripService : ITripService
{
    private readonly string _connectionString;

    public TripService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public IEnumerable<TripDTO> GetAllTrips()
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var cmd = new SqlCommand(@"SELECT T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, C.Name AS Country
                                   FROM Trip T
                                   JOIN Country_Trip CT ON CT.IdTrip = T.IdTrip
                                   JOIN Country C ON C.IdCountry = CT.IdCountry", conn);
        using var reader = cmd.ExecuteReader();
        var trips = new List<TripDTO>();
        while (reader.Read())
        {
            trips.Add(new TripDTO
            {
                IdTrip = (int)reader["IdTrip"],
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                DateFrom = (DateTime)reader["DateFrom"],
                DateTo = (DateTime)reader["DateTo"],
                MaxPeople = (int)reader["MaxPeople"],
                Country = reader["Country"].ToString()
            });
        }
        return trips;
    }
}
