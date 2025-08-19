using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;

namespace ShipJobPortal.Infrastructure.Repositories;

public class ShiphireRepository: IShiphireRepository
{
    private readonly IDataAccess_Improved _dbHelper;
    private readonly IConfiguration _configuration;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<ShiphireRepository> _logger;
    private readonly string _dbKey;

    public ShiphireRepository(IConfiguration configuration, IDataAccess_Improved dbHelper, IEncryptionService encryptionService, ILogger<ShiphireRepository> logger)
    {
        _dbHelper = dbHelper;
        _encryptionService = encryptionService;
        _logger = logger;
        _configuration = configuration;

        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:ShipHireDB"])
            ? throw new Exception("ShipHireDB is missing in ConnectionStrings.")
            : "ShipHireDB";
    }

    public async Task<ReturnResult> SentcandidatetoShiphire(ShiphireApplicantModel model)
    {
        try
        {
            return new ReturnResult();
        }
        catch (Exception ex) 
        {
            throw;
        }
    }
}
