/*DataAccess_improved Version 1
using System;
using System.Text;
using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using System.Collections;
namespace ShipJobPortal.Infrastructure.DataAccessLayer
{
    public interface IDataAccess_Improved
    {
        Task<DbResult<List<T>>> QueryAsync<T>(string spName, DynamicParameters parameters);
        Task<DbResult<T>> QuerySingleAsync<T>(string spName, DynamicParameters parameters);
        Task<DbResult<int>> ExecuteAsync(string spName, DynamicParameters parameters);
        Task<DbResult<object>> ExecuteScalarAsync(string spName, DynamicParameters parameters);
        Task<DbResult<List<IEnumerable<dynamic>>>> QueryMultipleAsync(string spName, DynamicParameters parameters);
        Task<DbResult<List<T>>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters);

        //Task<ReturnResult<T>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters); 

    }

    public class DataAccess_Improved: IDataAccess_Improved
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public DataAccess_Improved(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        private void AddOutputParams(DynamicParameters parameters)
        {
            if (!parameters.ParameterNames.Contains("@ReturnStatus"))
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            if (!parameters.ParameterNames.Contains("@ErrorCode"))
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        }

        private DbResult<T> MapResult<T>(T result, DynamicParameters parameters)
        {
            return new DbResult<T>
            {
                Data = result,
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }

        public async Task<DbResult<List<T>>> QueryAsync<T>(string spName, DynamicParameters parameters)
        {
            using var conn = _connectionFactory();
            try
            {
                AddOutputParams(parameters);
                var result = await conn.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
                return MapResult(result.ToList(), parameters);
            }
            catch (Exception ex)
            {
                return new DbResult<List<T>>
                {
                    Data = new List<T>(),
                    ReturnStatus = "error",
                    ErrorCode = ex.Message
                };
            }
        }

        public async Task<DbResult<T>> QuerySingleAsync<T>(string spName, DynamicParameters parameters)
        {
            using var conn = _connectionFactory();
            try
            {
                AddOutputParams(parameters);
                var result = await conn.QuerySingleOrDefaultAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
                return MapResult(result, parameters);
            }
            catch (Exception ex)
            {
                return new DbResult<T>
                {
                    Data = default!,
                    ReturnStatus = "error",
                    ErrorCode = ex.Message
                };
            }
        }

        public async Task<DbResult<int>> ExecuteAsync(string spName, DynamicParameters parameters)
        {
            using var conn = _connectionFactory();
            try
            {
                AddOutputParams(parameters);
                var result = await conn.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                return MapResult(result, parameters);
            }
            catch (Exception ex)
            {
                return new DbResult<int>
                {
                    Data = 0,
                    ReturnStatus = "error",
                    ErrorCode = ex.Message
                };
            }
        }

        public async Task<DbResult<object>> ExecuteScalarAsync(string spName, DynamicParameters parameters)
        {
            using var conn = _connectionFactory();
            try
            {
                AddOutputParams(parameters);
                var result = await conn.ExecuteScalarAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                return MapResult(result, parameters);
            }
            catch (Exception ex)
            {
                return new DbResult<object>
                {
                    Data = null,
                    ReturnStatus = "error",
                    ErrorCode = ex.Message
                };
            }
        }

        public async Task<DbResult<List<IEnumerable<dynamic>>>> QueryMultipleAsync(string spName, DynamicParameters parameters)
        {
            using var conn = _connectionFactory();
            try
            {
                AddOutputParams(parameters);
                using var grid = await conn.QueryMultipleAsync(spName, parameters, commandType: CommandType.StoredProcedure);

                var result = new List<IEnumerable<dynamic>>();
                while (!grid.IsConsumed)
                {
                    result.Add((await grid.ReadAsync()).ToList());
                }

                return MapResult(result, parameters);
            }
            catch (Exception ex)
            {
                return new DbResult<List<IEnumerable<dynamic>>>
                {
                    Data = new List<IEnumerable<dynamic>>(),
                    ReturnStatus = "error",
                    ErrorCode = ex.Message
                };
            }
        }

        public async Task<DbResult<List<T>>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters)
        {
            using var conn = _connectionFactory();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                AddOutputParams(parameters);
                var result = await conn.QueryAsync<T>(spName, parameters, tran, commandType: CommandType.StoredProcedure);

                if (parameters.Get<string>("@ReturnStatus") == "success")
                    tran.Commit();
                else
                    tran.Rollback();

                return MapResult(result.ToList(), parameters);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return new DbResult<List<T>>
                {
                    ReturnStatus = "error",
                    ErrorCode = ex.Message,
                    Data = new List<T>()
                };
            }
        }
    }

    public class DbResult<T>
    {
        public string ReturnStatus { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }
    }
}

// */

//public async Task<ReturnResult<T>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters)
//{
//    using var conn = _connectionFactory();
//    conn.Open();
//    using var tran = conn.BeginTransaction();

//    try
//    {
//        AddOutputParams(parameters);

//        var result = await conn.QueryAsync<T>(spName, parameters, tran, commandType: CommandType.StoredProcedure);

//        string returnStatus = parameters.Get<string>("@ReturnStatus") ?? "error";
//        string errorCode = parameters.Get<string>("@ErrorCode") ?? "ERR500";

//        if (returnStatus == "success")
//            tran.Commit();
//        else
//            tran.Rollback();

//        object output;

//        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
//        {
//            output = result.ToList();
//        }
//        else
//        {
//            output = result.FirstOrDefault();
//        }

//        return new ReturnResult<T>(returnStatus, errorCode, (T)output);
//    }
//    catch (Exception ex)
//    {
//        tran.Rollback();
//        return new ReturnResult<T>("error", ex.Message, default);
//    }
//}


//*DataAccess_improved Version 2

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using System.Collections;

namespace ShipJobPortal.Infrastructure.DataAccessLayer;

public interface IDataAccess_Improved
{
    Task<DbResult<List<T>>> QueryAsync<T>(string spName, DynamicParameters parameters, string dbKey);
    Task<DbResult<T>> QuerySingleAsync<T>(string spName, DynamicParameters parameters, string dbKey);
    Task<DbResult<int>> ExecuteAsync(string spName, DynamicParameters parameters, string dbKey);
    Task<DbResult<object>> ExecuteScalarAsync(string spName, DynamicParameters parameters, string dbKey);
    Task<DbResult<List<IEnumerable<dynamic>>>> QueryMultipleAsync(string spName, DynamicParameters parameters, string dbKey);
    Task<DbResult<List<T>>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters, string dbKey);
}

public class DataAccess_Improved : IDataAccess_Improved
{
    private readonly Func<string, IDbConnection> _connectionFactory;

    public DataAccess_Improved(Func<string, IDbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private void AddOutputParams(DynamicParameters parameters)
    {
        parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
    }

    private DbResult<T> MapResult<T>(T result, DynamicParameters parameters)
    {
        try
        {
            return new DbResult<T>
            {
                Data = result,
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            throw;

        }
    }

    public async Task<DbResult<List<T>>> QueryAsync<T>(string spName, DynamicParameters parameters, string dbKey)
    {
        try
        {
            using var conn = _connectionFactory(dbKey);
            AddOutputParams(parameters);
            var result = await conn.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
            return MapResult(result.ToList(), parameters);
        }
        catch (Exception ex)
        {
            throw;

        }
    }

    public async Task<DbResult<T>> QuerySingleAsync<T>(string spName, DynamicParameters parameters, string dbKey)
    {
        try
        {
            using var conn = _connectionFactory(dbKey);
            AddOutputParams(parameters);
            var result = await conn.QuerySingleOrDefaultAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
            return MapResult(result, parameters);
        }
        catch (Exception ex)
        {
            throw;

        }
    }

    public async Task<DbResult<int>> ExecuteAsync(string spName, DynamicParameters parameters, string dbKey)
    {
        try
        {
            using var conn = _connectionFactory(dbKey);
            AddOutputParams(parameters);
            var result = await conn.ExecuteAsync(spName, parameters, commandType: CommandType.Text);
            return MapResult(result, parameters);
        }
        catch (Exception ex)
        {
            throw;

        }
    }

    public async Task<DbResult<object>> ExecuteScalarAsync(string spName, DynamicParameters parameters, string dbKey)
    {
        try
        {
            using var conn = _connectionFactory(dbKey);
            AddOutputParams(parameters);
            var result = await conn.ExecuteScalarAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            return MapResult(result, parameters);
        }
        catch (Exception ex)
        {
            throw;

        }
    }

    public async Task<DbResult<List<IEnumerable<dynamic>>>> QueryMultipleAsync(string spName, DynamicParameters parameters, string dbKey)
    {
        try
        {
            using var conn = _connectionFactory(dbKey);
            AddOutputParams(parameters);
            using var grid = await conn.QueryMultipleAsync(spName, parameters, commandType: CommandType.StoredProcedure);

            var result = new List<IEnumerable<dynamic>>();
            while (!grid.IsConsumed)
            {
                result.Add((await grid.ReadAsync()).ToList());
            }

            return MapResult(result, parameters);
        }
        catch (Exception ex)
        {
            throw;

        }
    }

    public async Task<DbResult<List<T>>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters, string dbKey)
    {
        using var conn = _connectionFactory(dbKey);
        conn.Open();
        using var tran = conn.BeginTransaction();

        try
        {
            AddOutputParams(parameters);
            var result = await conn.QueryAsync<T>(spName, parameters, tran, commandType: CommandType.StoredProcedure);

            if (parameters.Get<string>("@ReturnStatus") == "success")
                tran.Commit();
            else
                tran.Rollback();

            return MapResult(result.ToList(), parameters);
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw;
        }
    }
}

public class DbResult<T>
{
    public string ReturnStatus { get; set; }
    public string ErrorCode { get; set; }
    public T Data { get; set; }
}

//*/