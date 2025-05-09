using apbd8.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd8.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    public IActionResult GetAllTrips()
    {
        var trips = _tripService.GetAllTrips();
        return Ok(trips);
    }
}
