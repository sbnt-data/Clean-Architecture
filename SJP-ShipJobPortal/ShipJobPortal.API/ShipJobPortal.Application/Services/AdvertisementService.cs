using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class AdvertisementService: IAdvertisementService
{
    private readonly IAdvertismentRepository _adRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdvertisementService> _logger;

    public AdvertisementService(IAdvertismentRepository adRepository, IMapper mapper, ILogger<AdvertisementService> logger)
    {
        _adRepository = adRepository;
        _mapper = mapper;
        _logger = logger;

    }
}
