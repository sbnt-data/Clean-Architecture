using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdvertisementController : ControllerBase
{
    private readonly IAdvertisementService _adService;
    private readonly ILogger<AdvertisementController> _logger;
    private readonly IDbExceptionLogger _dbExceptionLogger;
    public AdvertisementController(IAdvertisementService adService, ILogger<AdvertisementController> logger, IDbExceptionLogger dbExceptionLogger)
    {
        _adService = adService;
        _logger = logger;
        _dbExceptionLogger = dbExceptionLogger;
    }


}
