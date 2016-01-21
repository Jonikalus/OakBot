using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;

namespace OakBot
{
    public class Utils
    {
        //Extracts the OAuth token from an URL
        public static string getAuthTokenFromUrl(string absoluteUrl)
        {
            Match url = Regex.Match(absoluteUrl, "access_token=(?<token>[a-zA-Z0-9]+)&");
            string token = url.Groups["token"].Value.Trim();
            return token;
        }

        public static void clearIECache()
        {
            ClearFolder(new DirectoryInfo(Environment.GetFolderPath
            (Environment.SpecialFolder.InternetCache)));
        }

        public static void ClearFolder(DirectoryInfo folder)
        {
            try
            {
                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo subfolder in folder.GetDirectories())
                { ClearFolder(subfolder); }
            }
            catch (Exception ex)
            {
                
            }
        }

        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);


        public static unsafe void SuppressWininetBehavior()
        {
            /* SOURCE: http://msdn.microsoft.com/en-us/library/windows/desktop/aa385328%28v=vs.85%29.aspx
                * INTERNET_OPTION_SUPPRESS_BEHAVIOR (81):
                *      A general purpose option that is used to suppress behaviors on a process-wide basis. 
                *      The lpBuffer parameter of the function must be a pointer to a DWORD containing the specific behavior to suppress. 
                *      This option cannot be queried with InternetQueryOption. 
                *      
                * INTERNET_SUPPRESS_COOKIE_PERSIST (3):
                *      Suppresses the persistence of cookies, even if the server has specified them as persistent.
                *      Version:  Requires Internet Explorer 8.0 or later.
                */

            int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
            int* optionPtr = &option;

            bool success = InternetSetOption(IntPtr.Zero, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
            if (!success)
            {
                System.Windows.MessageBox.Show("Something went wrong !>?");
            }
        }
    }
}
