using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IISHealthCheckApi.Helpers;
using Microsoft.Web.Administration;

namespace IISHealthCheckApi.Controllers
{
    public class IISCheckController : ApiController
    {
        public const string DUMP_PATH = "C:\\Temp\\Dump_Files\\";
        [HttpGet]
        public string WebRequestCall(string appName)
        {
            string message = string.Empty;
            string url = "http://" + Environment.MachineName + "/" + appName;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    message = "Site not responding";
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    message = "Site is running";
                }
            }
            catch (Exception ex)
            {
                message = "Site not responding " + Environment.NewLine + ex.Message;
            }
            return message;
        }
        [HttpGet]
        public string RecycleSiteServer(string appName)
        {
            string message = string.Empty;
            try
            {
                ServerManager manager = new ServerManager();
                var applicationPool = GetApplicationPool(manager, appName);
                if (applicationPool != null)
                {
                    applicationPool.Recycle();
                    manager.CommitChanges();
                    message = "Application restarted successfully.";
                }
                else
                    message = "Failed to restart the application.";
            }
            catch (Exception ex)
            {
                message = "Failed to restart the application " + Environment.NewLine + ex.Message;
            }
            return message;
        }
        [HttpGet]
        public string CreateDump(string appName)
        {
            string message = string.Empty;
            try
            {
                ServerManager manager = new ServerManager();
                var applicationPool = GetApplicationPool(manager, appName);
                var pid = applicationPool.WorkerProcesses.Select(x => x.ProcessId).First();
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                //bool dumpStatus = MiniDump.TryDump(process, DUMP_PATH, MiniDumpType.Normal);
                try {
                    //System.Diagnostics.Process existingProcess = System.Diagnostics.Process.GetProcessById(process.Id);
                    //if (existingProcess != null)
                    //    existingProcess.Kill();
                }
                catch
                {

                }
                System.Diagnostics.Process clrDump = new System.Diagnostics.Process();
                clrDump.StartInfo.FileName = DUMP_PATH + "DumpSoftware\\clrdump.exe";
                clrDump.StartInfo.Arguments = process.Id + " " + DUMP_PATH + "Test_" + System.DateTime.Now.ToFileTimeUtc() + ".DMP";
                bool dumpStatus = clrDump.Start();
                clrDump.WaitForExit();

                if (dumpStatus)
                    message = "Dump created successfully";
                else
                    message = "Failed to create dump";
            }
            catch (Exception ex)
            {
                message = "Failed to create dump " + Environment.NewLine + ex.Message;
            }
            return message;
        }
        private ApplicationPool GetApplicationPool(ServerManager manager, string appName)
        {
            var site = manager.Sites.FirstOrDefault();
            var appPoolName = site.Applications.First(x=>x.Path.Contains(appName)).ApplicationPoolName;
            var applicationPool = manager.ApplicationPools.First(x => x.Name == appPoolName);
            return applicationPool;
        }
    }
}
