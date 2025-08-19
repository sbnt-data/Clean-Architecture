using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.Validators
{
    public static class DateConverter
    {
        public static DateTime? ConvertToNullableDate(string? input)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(input) || input.Trim().ToLower() == "unlimited")
                    return DateTime.MaxValue.Date;

                // Accept multiple common formats
                string[] formats = { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" };

                if (DateTime.TryParseExact(input.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None, out DateTime parsed))
                {
                    return parsed.Date; // ✅ Strip time
                }

                // Fallback for general parsing
                if (DateTime.TryParse(input, out parsed))
                    return parsed.Date;

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
