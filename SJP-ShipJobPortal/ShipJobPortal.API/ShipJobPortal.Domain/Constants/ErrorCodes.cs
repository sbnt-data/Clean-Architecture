using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Constants
{
    public static class ErrorCodes
    {
        public const string Success = "ERR200";
        public const string BadRequest = "ERR400";
        public const string InvalidCredentials = "ERR401";
        public const string InvalidPassword = "ERR402";
        public const string NotFound = "ERR404";
        public const string MissingPassword = "ERR405";
        public const string InvalidEmailFormat = "ERR406";
        public const string Conflict = "ERR409";
        public const string ValidationError = "ERR422";
        public const string InternalServerError = "ERR500";
        public const string NoAnswersFound = "ERR_NO_ANSWERS";
    }
}
