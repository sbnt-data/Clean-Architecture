using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

// Generic version
public class ReturnResult<T>
{
    public string ReturnStatus { get; set; }
    public string ErrorCode { get; set; }
    public T? Data { get; set; }

    public ReturnResult(string returnStatus = "error", string errorCode = "ERR500", T? data = default)
    {
        ReturnStatus = returnStatus;
        ErrorCode = errorCode;
        Data = data;
    }
}

// Non-generic version
public class ReturnResult : ReturnResult<object>
{
    public ReturnResult() : base() { }

    public ReturnResult(string returnStatus, string errorCode)
        : base(returnStatus, errorCode, null) { }
}

