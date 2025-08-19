using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.Validators
{
    public class PasswordPolicyValidator
    {
        public static (bool IsValid, string ErrorMessage) Validate(string password, string username, string? previousPassword = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                    return (false, "Password cannot be empty.");

                if (password.Length < 8)
                    return (false, "Password must be at least 8 characters long.");

                if (!password.Any(char.IsDigit))
                    return (false, "Password must contain at least one numeric digit.");

                if (!password.Any(char.IsLetter))
                    return (false, "Password must contain at least one alphabetic character.");

                if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                    return (false, "Password must contain at least one special character.");

                if (password.Equals(username, StringComparison.OrdinalIgnoreCase))
                    return (false, "Password cannot be the same as the username.");

                if (previousPassword != null)
                {
                    int differences = password.Zip(previousPassword, (c1, c2) => c1 != c2 ? 1 : 0).Sum();
                    differences += Math.Abs(password.Length - previousPassword.Length);

                    if (differences < 3)
                        return (false, "Password must differ from the previous password by at least 3 characters.");
                }

                return (true, string.Empty);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
