using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;

namespace OneCache
{
    public class ProcessInfo
    {
        public static bool NoRunningPlatformProcesses()
        {
            string domain_user = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
            string[] names = new string[] { "1cv8", "1cv8c", "1cv8s" };
            
            foreach (var name in names)
            {
                var processes = Process.GetProcessesByName(name).Where(p => GetProcessOwner(p.Id) == domain_user).ToList();
                if (processes.Count > 0)
                {
                    return false;
                }
            }
           
            return true;
        }

        public static string GetProcessOwner(int processId)
        {
            var query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectCollection processList;

            using (var searcher = new ManagementObjectSearcher(query))
            {
                processList = searcher.Get();
            }

            foreach (var mo in processList.OfType<ManagementObject>())
            {
                object[] argList = { string.Empty, string.Empty };
                var returnVal = Convert.ToInt32(mo.InvokeMethod("GetOwner", argList));

                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return new StringBuilder().Append(argList[1])
                        .Append("\\")
                        .Append(argList[0])
                        .ToString();
                }
            }

            return "NO OWNER";
        }
    }
}
