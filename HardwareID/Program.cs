using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareID
{
    class Program
    {
        static void Main(string[] args)
        {
            string hwid = Hardware_ID.Get_HardWareID;
            Console.WriteLine(hwid);
            Console.ReadKey();
        }
    }
}
