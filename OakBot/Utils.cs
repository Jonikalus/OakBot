using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OakBot
{
    class Utils
    {
        public static string getAuthTokenFromUrl(string absoluteUrl)
        {
            Match url = Regex.Match(absoluteUrl, "access_token=(?<token>[a-zA-Z0-9]+)&");
            string token = url.Groups["token"].Value.Trim();
            return token;
        }
    }
}
