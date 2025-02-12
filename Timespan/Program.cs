using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timespan
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan timespan = TimeSpan.FromMilliseconds(-1);
            Console.WriteLine("Time span is {0}", timespan.ToString());
            Console.Read();
        }
    }
}
