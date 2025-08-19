using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;
using ShipJobPortal.Infrastructure.DataHelpers;

namespace ShipJobPortal.Infrastructure.Repositories;

public class AdvertismentRepository: IAdvertismentRepository
{

    private readonly IDataAccess_Improved _dbHelper;
    private readonly ILogger<AdvertismentRepository> _logger;
    private readonly IMapper _mapper;
    private readonly string _dbKey;
    private readonly IConfiguration _configuration;


    public AdvertismentRepository(IConfiguration configuration, IDataAccess_Improved dbHelper, ILogger<AdvertismentRepository> logger, IMapper mapper)
    {
        _dbHelper = dbHelper;
        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;

        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
            ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
            : "DefaultConnection";
    }


}
