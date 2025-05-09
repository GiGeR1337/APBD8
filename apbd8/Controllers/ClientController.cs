using apbd8.Models;
using apbd8.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd8.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost]
    public IActionResult CreateClient([FromBody] ClientDTO dto)
    {
        var id = _clientService.CreateClient(dto);
        return Created($"api/clients/{id}", new { Id = id });
    }

    [HttpGet("{id}/trips")]
    public IActionResult GetClientTrips(int id)
    {
        var trips = _clientService.GetClientTrips(id);
        return Ok(trips);
    }

    [HttpPut("{id}/trips/{tripId}")]
    public IActionResult RegisterClient(int id, int tripId)
    {
        _clientService.RegisterClientToTrip(id, tripId);
        return Ok("Registered");
    }

    [HttpDelete("{id}/trips/{tripId}")]
    public IActionResult RemoveClient(int id, int tripId)
    {
        _clientService.RemoveClientFromTrip(id, tripId);
        return Ok("Removed");
    }
}
