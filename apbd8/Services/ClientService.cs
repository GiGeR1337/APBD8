using apbd8.Exceptions;
using apbd8.Models;
using Microsoft.Data.SqlClient;

namespace apbd8.Services;

public class ClientService : IClientService
{
    private readonly string _connectionString;

    public ClientService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public int CreateClient(ClientDTO clientDto)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var cmd = new SqlCommand("INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) " +
                                 "OUTPUT INSERTED.IdClient " +
                                 "VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)", conn);
        cmd.Parameters.AddWithValue("@FirstName", clientDto.FirstName);
        cmd.Parameters.AddWithValue("@LastName", clientDto.LastName);
        cmd.Parameters.AddWithValue("@Email", clientDto.Email);
        cmd.Parameters.AddWithValue("@Telephone", clientDto.Telephone);
        cmd.Parameters.AddWithValue("@Pesel", clientDto.Pesel);

        return (int)cmd.ExecuteScalar();
    }

    public IEnumerable<TripDTO> GetClientTrips(int clientId)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var cmd = new SqlCommand(
            @"SELECT T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, C.Name AS Country
              FROM Trip T
              JOIN Country_Trip CT ON T.IdTrip = CT.IdTrip
              JOIN Country C ON C.IdCountry = CT.IdCountry
              JOIN Client_Trip CLT ON CLT.IdTrip = T.IdTrip
              WHERE CLT.IdClient = @IdClient", conn);

        cmd.Parameters.AddWithValue("@IdClient", clientId);

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

        if (!trips.Any())
            throw new NotFoundException("Client has no trips or does not exist.");

        return trips;
    }

    public void RegisterClientToTrip(int clientId, int tripId)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var checkCmd = new SqlCommand(@"SELECT COUNT(*) FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", conn);
        checkCmd.Parameters.AddWithValue("@IdClient", clientId);
        checkCmd.Parameters.AddWithValue("@IdTrip", tripId);
        var exists = (int)checkCmd.ExecuteScalar();
        if (exists > 0)
            throw new ConflictException("Client already registered for this trip.");

        var insertCmd = new SqlCommand(@"INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
                                         VALUES (@IdClient, @IdTrip, @RegisteredAt, NULL)", conn);
        insertCmd.Parameters.AddWithValue("@IdClient", clientId);
        insertCmd.Parameters.AddWithValue("@IdTrip", tripId);
        insertCmd.Parameters.AddWithValue("@RegisteredAt", DateTime.Now);
        insertCmd.ExecuteNonQuery();
    }

    public void RemoveClientFromTrip(int clientId, int tripId)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var checkCmd = new SqlCommand(@"SELECT COUNT(*) FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", conn);
        checkCmd.Parameters.AddWithValue("@IdClient", clientId);
        checkCmd.Parameters.AddWithValue("@IdTrip", tripId);
        var exists = (int)checkCmd.ExecuteScalar();
        if (exists == 0)
            throw new NotFoundException("Registration does not exist.");

        var deleteCmd = new SqlCommand(@"DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", conn);
        deleteCmd.Parameters.AddWithValue("@IdClient", clientId);
        deleteCmd.Parameters.AddWithValue("@IdTrip", tripId);
        deleteCmd.ExecuteNonQuery();
    }
}
