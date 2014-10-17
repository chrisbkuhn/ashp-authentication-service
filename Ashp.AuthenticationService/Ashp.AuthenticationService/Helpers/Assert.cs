using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ashp.AuthenticationService.Helpers
{
    public static class Assert
    {
        public static bool IsEmailAddress(string email)
        {
            var emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            var isEmail = Regex.IsMatch(email, emailRegex);

            return isEmail;
        }

        public static bool IsGuid(string guid)
        {
            Guid testGuid;
            return Guid.TryParse(guid, out testGuid);
        }
    }
}