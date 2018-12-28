using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Desktop_Notification.Models
{
    public class Hardware_ID
    {
        public static string Get_HardWareID => ReturnHardwareId().Result;
        private static async Task<string> ReturnHardwareId()
        {
            byte[] bytes;
            byte[] hashedbytes;
            StringBuilder stringBuilder = new StringBuilder();
            Task task = Task.Run(() =>
            {
                ManagementObjectSearcher cpu = new ManagementObjectSearcher("select * from Win32_Processor");
                ManagementObjectCollection cpu_collection = cpu.Get();
                foreach (ManagementObject obj in cpu_collection)
                {
                    stringBuilder.Append(obj["processorId"].ToString().Substring(0, 4));
                    break;
                }

                ManagementObjectSearcher hdd = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                ManagementObjectCollection hdd_collection = hdd.Get();
                foreach (ManagementObject obj in hdd_collection)
                {
                    stringBuilder.Append(obj["Signature"].ToString().Substring(0, 4));
                    break;
                }

                ManagementObjectSearcher bios = new ManagementObjectSearcher("select * from Win32_BIOS");
                ManagementObjectCollection bios_collection = bios.Get();
                foreach (ManagementObject obj in bios_collection)
                {
                    stringBuilder.Append(obj["Version"].ToString().Substring(0, 4));
                    break;
                }
            });
            Task.WaitAll(task);
            bytes = System.Text.Encoding.UTF8.GetBytes(stringBuilder.ToString());
            hashedbytes = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);
            return await Task.FromResult(Convert.ToBase64String(hashedbytes).Substring(25));
        }
    }
}