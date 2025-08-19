using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Interfaces;

public interface IDbExceptionLogger
{
    Task LogExceptionAsync(string methodName, string message, string stackTrace);
}
