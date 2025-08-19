using System;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;
using ShipJobPortal.Infrastructure.DataHelpers;

namespace ShipJobPortal.Infrastructure.DataHelpers;


public class DbExceptionLogger : IDbExceptionLogger
{
    private readonly IDataAccess_Improved _dbHelper;
    private readonly string _dbKey;
    private readonly IConfiguration _configuration;


    public DbExceptionLogger(IConfiguration configuration,IDataAccess_Improved dbHelper)
    {            
        _dbHelper = dbHelper;
        _configuration = configuration;

        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
            ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
            : "DefaultConnection";
    }


    public async Task LogExceptionAsync(string source, string message, string stackTrace)
    {
        try
        {
            // Step 1: Log to DB using ExecuteAsync (recommended for INSERT)
            var parameters = new DynamicParameters();
            parameters.Add("@Source", source);
            parameters.Add("@Message", message);
            parameters.Add("@StackTrace", stackTrace);

            var result = await _dbHelper.ExecuteScalarAsync("usp_LogException", parameters,_dbKey);

            // Optional: Check DB result and act if needed
            if (result.ReturnStatus != "success")
            {
                Console.WriteLine($"Failed to log to DB: {result.ErrorCode}");
            }

            // Step 2: Also log to file
            string basePath = AppContext.BaseDirectory;
            string logDir = Path.Combine(basePath, "Log");

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string logPath = Path.Combine(logDir, "ErrorLog.txt");

            var logContent = $"""
                --------------------------------------------------
                Exception occurred in {source}
                --------------------------------------------------
                Timestamp : {DateTime.Now}
                Source    : {source}
                Message   : {message}
                Stack     : {stackTrace}
                --------------------------------------------------


                """;

            await File.AppendAllTextAsync(logPath, logContent + Environment.NewLine);
        }
        catch (Exception ex)
        {
            // Silent fallback to avoid recursion
            Console.WriteLine("Failed to log exception: " + ex.Message);
        }
    }
}