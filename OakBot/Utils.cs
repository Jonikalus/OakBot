using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Controls;
using System.Reflection;
using System.Windows;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OakBot
{
    public class Utils
    {
        // Extracts the OAuth token from an Twitch URL
        public static string GetTwitchAuthToken(string absoluteUrl)
        {
            Match url = Regex.Match(absoluteUrl, "access_token=(?<token>[a-zA-Z0-9]+)");
            return url.Groups["token"].Value.Trim();
        }

        // Extracts the OAuth token from a TwitchAlerts URL
        public static string GetTwitchAlertsAuthToken(string absoluteUrl)
        {
            Match url = Regex.Match(absoluteUrl, "code=(?<token>[a-zA-Z0-9]+)");
            return url.Groups["token"].Value.Trim();
        }

        public static SimpleHTTPServer botHttp;

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
                {
                    ClearFolder(subfolder);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ClearFolder Exception: " + ex.ToString());
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
                MessageBox.Show("Something went wrong !>?");
            }
        }

        public static void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        /// <summary>
        /// Add the viewer as TwitchViewer to the viewers collection.
        /// Creates a new TwitchViewer and adds it to the database if needed.
        /// </summary>
        /// <param name="viewerName">Viewers Twitch username to add</param>
        public static void AddToViewersCol(string viewerName)
        {
            // First check if viewer is not already in the viewers list
            var isInViewList = MainWindow.colViewers.FirstOrDefault(x => x.UserName == viewerName);
            if (isInViewList == null)
            {
                // Check if viewer exists in database to refer to
                var isInDatabase = MainWindow.colDatabase.FirstOrDefault(x => x.UserName == viewerName);
                if (isInDatabase != null)
                { // is in database
                    MainWindow.colViewers.Add(isInDatabase);
                }
                else
                { // is not in database
                    TwitchViewer newViewer = new TwitchViewer(viewerName);
                    MainWindow.colDatabase.Add(newViewer);
                    MainWindow.colViewers.Add(newViewer);
                }
            }
        }

        /// <summary>
        /// Removes the viewer from the viewers collection.
        /// </summary>
        /// <param name="viewerName">Viewers Twitch username to remove</param>
        public static void RemoveFromViewersCol(string viewerName)
        {
            // Check if PARTing viewer is in the viewers list
            var toRemove = MainWindow.colViewers.FirstOrDefault(x => x.UserName == viewerName);
            if (toRemove != null)
            {
                MainWindow.colViewers.Remove(toRemove);
            }

            // other method (itteration)
            //MainWindow.colViewers.Where(x => x.username == ircMessage.author).ToList().ForEach(
            //    e => MainWindow.colViewers.Remove(e));
        }

        public static void StartWebserver()
        {
            botHttp = new SimpleHTTPServer(Config.AppDataPath + "\\Webserver", 8080);
        }
        
    }
}
