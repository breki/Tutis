using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
using LibroLib;
using LibroLib.ConsoleShells;

namespace SelfSignedHttpsListener
{
    public class RunWebServerCommand : StandardConsoleCommandBase
    {
        public RunWebServerCommand()
        {
            AddSetting("port", "port number the web server should run on. The default is {0}".Fmt(DefaultPort))
                .IntValue((portToUse, env) => port = portToUse);
            AddSwitch("https", "tells the web server to use HTTPS protocol", (x, env) => useHttps = x);
        }

        public override string CommandId
        {
            get { return "serve"; }
        }

        public override object Description
        {
            get { return "Runs a test web server"; }
        }

        public override int Execute(IConsoleEnvironment env)
        {
            StartWebServer ();

            WaitForUserClosing ();

            StopWebServer ();

            return 0;
        }

        private void StartWebServer ()
        {
            serviceUrl = string.Format (
                CultureInfo.InvariantCulture, "{0}://127.0.0.1:{1}/", useHttps ? "https" : "http", port);

            listener = new HttpListener ();
            listener.Prefixes.Add (serviceUrl);
            listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
            listener.Start ();
            listener.BeginGetContext (WebRequestCallback, listener);
        }

        private static void WaitForUserClosing ()
        {
            Console.CancelKeyPress += SignalServerToStop;

            Console.WriteLine (
                "Test web server is now running on {0}. Press Ctrl+Break to stop it.",
                serviceUrl);
            consoleStopSignal.WaitOne ();
        }

        private static void StopWebServer ()
        {
            if (listener != null && listener.IsListening)
            {
                Console.Out.WriteLine ("Stopping web server...");

                listener.Stop ();
                listener.Close ();
                listener = null;
            }
        }

        private static void WebRequestCallback (IAsyncResult result)
        {
            if (listener == null || false == listener.IsListening)
                return;

            HttpListenerContext context = listener.EndGetContext (result);

            Stream stream = context.Response.OutputStream;
            using (StreamWriter writer = new StreamWriter(stream)) do
            {
                try
                {
                    Output (writer, "Received request...");

                    listener.BeginGetContext (WebRequestCallback, listener);
                    if (context.User == null)
                    {
                        Output (writer, "context.User == null");
                        Console.Out.WriteLine ("context.User == null");
                        break;
                    }

                    WindowsPrincipal windowsPrincipal = context.User as WindowsPrincipal;
                    if (windowsPrincipal == null)
                    {
                        Output (writer, "windowsPrincipal == null");
                        break;
                    }

                    WindowsIdentity windowsIdentity = windowsPrincipal.Identity as WindowsIdentity;
                    if (windowsIdentity == null)
                    {
                        Output (writer, "windowsIdentity == null");
                        break;
                    }

                    Output (writer, "IsAuthenticated: {0}", windowsPrincipal.Identity.IsAuthenticated);

                    Output (writer, string.Empty);
                    Output (writer, "REQUEST HEADERS:");
                    Output (writer, "{0}", context.Request.Headers);

                    OutputUsersGroups (writer, windowsIdentity);
                    
                    Output (writer, "--------------------");

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Console.WriteLine (ex);
                }
            }
            while (false);

            context.Response.Close ();
        }

        private static void OutputUsersGroups (TextWriter writer, WindowsIdentity windowsIdentity)
        {
            Output (writer, string.Empty);
            Output (writer, "USER'S GROUPS:");

            List<KeyValuePair<string, string>> groupsSorted = new List<KeyValuePair<string, string>> ();

            IdentityReferenceCollection groups = windowsIdentity.Groups;
            if (groups == null)
                return;

            foreach (IdentityReference identityReference in groups)
            {
                string sid = identityReference.Value;
                string account = new SecurityIdentifier (sid).Translate (typeof(NTAccount)).ToString ();
                groupsSorted.Add (new KeyValuePair<string, string> (sid, account));
            }

            groupsSorted.Sort ((a, b) => string.Compare (a.Value, b.Value, StringComparison.Ordinal));

            foreach (KeyValuePair<string, string> group in groupsSorted)
                Output (writer, "{0} ({1})", @group.Value, @group.Key);
        }

        private static void Output(TextWriter writer, string line)
        {
            Console.Out.WriteLine(line);
            writer.WriteLine(line);
        }

        private static void Output(TextWriter writer, string lineFormat, params object[] args)
        {
            string line = string.Format(CultureInfo.InvariantCulture, lineFormat, args);
            Output(writer, line);
        }

        private static void SignalServerToStop (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            consoleStopSignal.Set ();
        }

        private const int DefaultPort = 13522;
        private static readonly ManualResetEvent consoleStopSignal = new ManualResetEvent (false);
        private static HttpListener listener;
        private static string serviceUrl;
        private int port = DefaultPort;
        private bool useHttps;
    }
}